using E_Commerce.Data;
using E_Commerse.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Commerce.WebUI.Controllers
{
    [Authorize]
    public class FavoriteController : Controller
    {
        private readonly DatabaseContext _context;

        public FavoriteController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorites = await _context.Favorites
                .Include(f => f.Product)
                .Where(f => f.AppUserId == userId)
                .OrderByDescending(f => f.AddedDate)
                .ToListAsync();

            return View(favorites);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı." });
            }

            var existingFavorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.AppUserId == userId && f.ProductId == productId);

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

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Ürün favorilere eklendi.",
                favoriteId = favorite.Id // Yeni eklenen favori ID'sini dönüyoruz
            });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int favoriteId) // Parametre adını değiştirdik
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.Id == favoriteId && f.AppUserId == userId);

            if (favorite == null)
            {
                return Json(new { success = false, message = "Favori bulunamadı." });
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Ürün favorilerden kaldırıldı.",
                productId = favorite.ProductId // Favori sayacı güncelleme için
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoriteCount()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var count = await _context.Favorites
                .Where(f => f.AppUserId == userId)
                .CountAsync();

            return Json(new { count });
        }

        [HttpGet]
        public async Task<IActionResult> CheckFavoriteStatus(int productId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.AppUserId == userId && f.ProductId == productId);

            return Json(new
            {
                isFavorite = favorite != null,
                favoriteId = favorite?.Id
            });
        }
    }
}