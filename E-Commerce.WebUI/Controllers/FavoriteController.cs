using E_Commerce.Service.Abstract;
using E_Commerse.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.WebUI.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly IService<Favorite> _favoriteService;
        private readonly IService<Product> _productService;

        public FavoriteController(IService<Favorite> favoriteService, IService<Product> productService)
        {
            _favoriteService = favoriteService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var favorites = await _favoriteService.GetQueryable()
                .Where(f => f.AppUserId == userId)
                .Include(f => f.Product)
                .OrderByDescending(f => f.AddedDate)
                .ToListAsync();

            return View(favorites);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var product = await _productService.FindAsync(productId);

            if (product == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı." });
            }

            var existingFavorite = await _favoriteService.GetAsync(f => f.AppUserId == userId && f.ProductId == productId);

            if (existingFavorite != null)
            {
                return Json(new { success = false, message = "Bu ürün zaten favorilerinizde." });
            }

            var favorite = new Favorite
            {
                AppUserId = userId,
                ProductId = productId,
                AddedDate = DateTime.Now
            };

            await _favoriteService.AddAsync(favorite);
            await _favoriteService.saveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Ürün favorilere eklendi.",
                favoriteId = favorite.Id
            });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int favoriteId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var favorite = await _favoriteService.GetAsync(f => f.Id == favoriteId && f.AppUserId == userId);

            if (favorite == null)
            {
                return Json(new { success = false, message = "Favori bulunamadı." });
            }

            _favoriteService.Delete(favorite);
            await _favoriteService.saveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Ürün favorilerden kaldırıldı.",
                productId = favorite.ProductId
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoriteCount()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var count = (await _favoriteService.GetAllAsync(f => f.AppUserId == userId)).Count;

            return Json(new { count });
        }

        [HttpGet]
        public async Task<IActionResult> CheckFavoriteStatus(int productId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorite = await _favoriteService.GetAsync(f => f.AppUserId == userId && f.ProductId == productId);

            return Json(new
            {
                isFavorite = favorite != null,
                favoriteId = favorite?.Id
            });
        }
    }
}
