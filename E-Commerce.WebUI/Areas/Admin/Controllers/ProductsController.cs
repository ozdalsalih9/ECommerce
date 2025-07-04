using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using E_Commerce.Data;
using E_Commerse.Core.Entities;
using E_Commerce.WebUI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace E_Commerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly DatabaseContext _context;

        public ProductsController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .ToListAsync();
            return View(products);
        }

        // GET: Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    Product product,
    List<IFormFile> Images,
    string[] sizeNames,
    int[] stocks,
    List<int> selectedColorIds)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Ürünü kaydet
                    _context.Add(product);
                    await _context.SaveChangesAsync(); // product.Id oluşur

                    // 2. Ana resim
                    if (product.ImageFile != null)
                    {
                        product.Image = await FileHelper.FileLoaderAsync(product.ImageFile);
                        _context.Update(product); // güncelleme yapılmalı
                        await _context.SaveChangesAsync();
                    }

                    // 3. Ek resimler
                    if (Images != null && Images.Count > 0)
                    {
                        foreach (var imagePath in await FileHelper.MultipleFileLoaderAsync(Images))
                        {
                            _context.ProductImages.Add(new ProductImage
                            {
                                ProductId = product.Id,
                                ImagePath = imagePath
                            });
                        }
                    }

                    // 4. Beden ve stok işle
                    ProcessSizes(product, sizeNames, stocks);

                    // 5. Renk ve renk resimleri
                    if (selectedColorIds != null && selectedColorIds.Count > 0)
                    {
                        foreach (int colorId in selectedColorIds)
                        {
                            var productColor = new ProductColor
                            {
                                ProductId = product.Id,
                                ColorId = colorId
                            };

                            _context.ProductColors.Add(productColor);
                            await _context.SaveChangesAsync(); // ID oluşsun

                            // colorImages[3] gibi adla gelenleri yakala
                            var colorFiles = Request.Form.Files
                                .Where(f => f.Name == $"colorImages[{colorId}]")
                                .ToList();

                            var savedPaths = await FileHelper.SaveColorImagesAsync(colorFiles, product.Id, colorId);

                            foreach (var path in savedPaths)
                            {
                                _context.ProductColorImages.Add(new ProductColorImage
                                {
                                    ProductColorId = productColor.Id,
                                    ImageUrl = path
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Ürün oluşturulurken hata: {ex.Message}");
                }
            }

            await PopulateDropdowns(product);
            return View(product);
        }


        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.ProductSizes)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.ProductColorImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            product.SelectedColorIds = product.ProductColors.Select(pc => pc.ColorId).ToList();
            await PopulateDropdowns(product);

            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
     int id,
     Product product,
     IFormFile ImageFile,
     bool cbResmiSil = false,
     List<IFormFile> Images = null,
     int[] sizeIds = null,
     int[] stocks = null,
     List<int> selectedColorIds = null)
        {
            if (id != product.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(product);
                return View(product);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingProduct = await _context.Products
                        .Include(p => p.ProductSizes)
                        .Include(p => p.ProductImages)
                        .Include(p => p.ProductColors)
                            .ThenInclude(pc => pc.ProductColorImages)
                        .FirstOrDefaultAsync(p => p.Id == id);

                    if (existingProduct == null)
                        return NotFound();

                    // 1. Ana resmi güncelle (sadece yeni resim yüklendiyse veya silme seçeneği işaretlendiyse)
                    if (ImageFile != null || cbResmiSil)
                    {
                        await ProcessMainImage(existingProduct, product, ImageFile, cbResmiSil);
                    }

                    // 2. Ek resimleri ekle (zorunlu değil)
                    if (Images != null && Images.Count > 0)
                    {
                        var paths = await FileHelper.MultipleFileLoaderAsync(Images);
                        foreach (var path in paths)
                        {
                            existingProduct.ProductImages.Add(new ProductImage
                            {
                                ProductId = existingProduct.Id,
                                ImagePath = path
                            });
                        }
                    }

                    // 3. Beden bilgilerini güncelle
                    _context.ProductSizes.RemoveRange(existingProduct.ProductSizes);
                    if (sizeIds != null && stocks != null && sizeIds.Length == stocks.Length)
                    {
                        for (int i = 0; i < sizeIds.Length; i++)
                        {
                            existingProduct.ProductSizes.Add(new ProductSize
                            {
                                ProductId = existingProduct.Id,
                                SizeId = sizeIds[i],
                                Stock = stocks[i]
                            });
                        }
                    }

                    // 4. Renkleri ve renk resimlerini güncelle
                    await UpdateProductColors(existingProduct, selectedColorIds);

                    // 5. Diğer alanları güncelle
                    UpdateProductProperties(existingProduct, product);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError("", $"Ürün güncellenirken bir hata oluştu: {ex.Message}");
                    await PopulateDropdowns(product);
                    return View(product);
                }
            }
        }
        private async Task UpdateProductColors(Product existingProduct, List<int> selectedColorIds)
        {
            // Önce mevcut renkleri ve resimleri temizle
            foreach (var productColor in existingProduct.ProductColors.ToList())
            {
                foreach (var image in productColor.ProductColorImages.ToList())
                {
                    FileHelper.FileRemover(image.ImageUrl, $"/img/Products/ColorImages/P-{existingProduct.Id}/C-{productColor.ColorId}/");
                    _context.ProductColorImages.Remove(image);
                }
                FileHelper.RemoveColorFolder(existingProduct.Id, productColor.ColorId);
                _context.ProductColors.Remove(productColor);
            }

            // Yeni renkleri ekle
            if (selectedColorIds != null && selectedColorIds.Count > 0)
            {
                var formFiles = Request.Form.Files;
                existingProduct.ProductColors = new List<ProductColor>();

                foreach (var colorId in selectedColorIds)
                {
                    var productColor = new ProductColor
                    {
                        ProductId = existingProduct.Id,
                        ColorId = colorId,
                        ProductColorImages = new List<ProductColorImage>()
                    };

                    var colorFiles = formFiles.Where(f => f.Name == $"colorImages[{colorId}]").ToList();
                    if (colorFiles.Any())
                    {
                        var savedPaths = await FileHelper.SaveColorImagesAsync(colorFiles, existingProduct.Id, colorId);
                        foreach (var path in savedPaths)
                        {
                            productColor.ProductColorImages.Add(new ProductColorImage
                            {
                                ImageUrl = path
                            });
                        }
                    }

                    existingProduct.ProductColors.Add(productColor);
                }
            }
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.Color)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColors)
                    .ThenInclude(pc => pc.ProductColorImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            try
            {
                // Delete main image
                if (!string.IsNullOrEmpty(product.Image))
                {
                    FileHelper.FileRemover(product.Image);
                }

                // Delete additional images
                foreach (var image in product.ProductImages)
                {
                    FileHelper.FileRemover(image.ImagePath);
                }

                // Delete color images and folders
                foreach (var productColor in product.ProductColors)
                {
                    foreach (var image in productColor.ProductColorImages)
                    {
                        FileHelper.FileRemover(image.ImageUrl, $"/img/Products/ColorImages/P-{product.Id}/C-{productColor.ColorId}/");
                    }
                    // Remove the entire color folder
                    FileHelper.RemoveColorFolder(product.Id, productColor.ColorId);
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Delete error: " + ex.Message);
                return View("Delete", product);
            }
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        #region Helper Methods

        private async Task PopulateDropdowns(Product product = null)
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product?.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product?.CategoryId);
            ViewData["Sizes"] = new SelectList(_context.Sizes, "Id", "Name");
            ViewData["Colors"] = new SelectList(await _context.Colors.ToListAsync(), "Id", "Name");

            var colors = await _context.Colors
                .Select(c => new { value = c.Id, text = c.Name })
                .ToListAsync();

            ViewBag.ColorListJson = System.Text.Json.JsonSerializer.Serialize(colors);
            ViewBag.Colors = new SelectList(colors, "value", "text", product?.SelectedColorIds);
        }

        private void ProcessSizes(Product product, string[] sizeNames, int[] stocks)
        {
            if (sizeNames != null && stocks != null && sizeNames.Length == stocks.Length)
            {
                product.ProductSizes = new List<ProductSize>();
                for (int i = 0; i < sizeNames.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(sizeNames[i]))
                    {
                        var size = _context.Sizes.FirstOrDefault(s => s.Name.ToLower() == sizeNames[i].ToLower())
                            ?? new Size { Name = sizeNames[i].Trim() };

                        if (size.Id == 0)
                        {
                            _context.Sizes.Add(size);
                            _context.SaveChanges();
                        }

                        product.ProductSizes.Add(new ProductSize
                        {
                            SizeId = size.Id,
                            Stock = stocks[i]
                        });
                    }
                }
            }
        }
        private async Task ProcessMainImage(Product existingProduct, Product newProduct, IFormFile imageFile, bool deleteImage)
        {
            if (deleteImage)
            {
                // Resmi sil
                if (!string.IsNullOrEmpty(existingProduct.Image))
                {
                    FileHelper.FileRemover(existingProduct.Image, "/img/Products/MainImage/");
                    existingProduct.Image = null;
                }
            }
            else if (imageFile != null && imageFile.Length > 0)
            {
                // Yeni resim yükle
                if (!string.IsNullOrEmpty(existingProduct.Image))
                {
                    FileHelper.FileRemover(existingProduct.Image, "/img/Products/MainImage/");
                }

                var imagePath = await FileHelper.FileLoaderAsync(imageFile);
                existingProduct.Image = imagePath;
            }
            // Hiçbir şey yapılmazsa mevcut resim korunur
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductImage([FromBody] int id)
        {
            var image = await _context.ProductImages.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            try
            {
                FileHelper.FileRemover(image.ImagePath, "/img/Products/OtherImages/");
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Resim silinirken hata oluştu: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteColorImage(int id)
        {
            var image = await _context.ProductColorImages.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            try
            {
                // Resmi fiziksel olarak sil
                FileHelper.FileRemover(image.ImageUrl);

                _context.ProductColorImages.Remove(image);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Resim silinirken hata oluştu: {ex.Message}");
            }
        }
        private void UpdateProductProperties(Product existingProduct, Product newProduct)
        {
            existingProduct.Name = newProduct.Name;
            existingProduct.Description = newProduct.Description;
            existingProduct.Price = newProduct.Price;
            existingProduct.ProductCode = newProduct.ProductCode;
            existingProduct.IsActive = newProduct.IsActive;
            existingProduct.IsHome = newProduct.IsHome;
            existingProduct.CategoryId = newProduct.CategoryId;
            existingProduct.BrandId = newProduct.BrandId;
            existingProduct.OrderNo = newProduct.OrderNo;
        }

        #endregion
    }
}