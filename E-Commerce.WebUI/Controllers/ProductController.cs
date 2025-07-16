using Microsoft.AspNetCore.Mvc;
using E_Commerce.Service.Abstract;
using E_Commerse.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace E_Commerce.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IService<Product> _productService;
        private readonly IService<Category> _categoryService;
        private readonly IService<Favorite> _favoriteService;
        private readonly IService<ProductSize> _productSizeService;

        public ProductController(
            IService<Product> productService,
            IService<Category> categoryService,
            IService<Favorite> favoriteService,
            IService<ProductSize> productSizeService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _favoriteService = favoriteService;
            _productSizeService = productSizeService;
        }

        // GET: Product/Index (Product Listing)
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreateTime)
                .ToListAsync();

            return View(products);
        }

        // GET: Product/Details/5 (Product Detail Page)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _productService.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.Color)
                .Include(p => p.ProductColors).ThenInclude(pc => pc.ProductColorImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            var relatedProducts = await _productService.GetQueryable()
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                .Take(4)
                .ToListAsync();

            if (User.Identity.IsAuthenticated)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ViewBag.IsFavorite = await _favoriteService.GetQueryable()
                    .AnyAsync(f => f.AppUserId == userId && f.ProductId == id);
            }

            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }

        // GET: Product/Category/5 (Products by Category)
        public async Task<IActionResult> Category(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _categoryService.FindAsync(id.Value);
            if (category == null)
                return NotFound();

            ViewBag.CategoryName = category.Name;

            var products = await _productService.GetQueryable()
                .Include(p => p.ProductImages)
                .Where(p => p.CategoryId == id && p.IsActive)
                .ToListAsync();

            return View(products);
        }

        // AJAX: Check Product Stock
        [HttpGet]
        public async Task<IActionResult> CheckStock(int productId, int sizeId)
        {
            var stock = await _productSizeService.GetQueryable()
                .Where(ps => ps.ProductId == productId && ps.SizeId == sizeId)
                .Select(ps => ps.Stock)
                .FirstOrDefaultAsync();

            return Json(new { stock });
        }
    }
}
