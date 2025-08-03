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
        public async Task<IActionResult> Add([FromBody] AddFavoriteRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });

            var userId = int.Parse(userIdClaim);

            Favorite exists = null;

            if (request.FavoriteId > 0)
            {
                exists = await _favoriteService.GetAsync(f => f.AppUserId == userId && f.Id == request.FavoriteId);
            }
            else if (request.ProductId > 0)
            {
                exists = await _favoriteService.GetAsync(f => f.AppUserId == userId && f.ProductId == request.ProductId);
            }

            if (exists != null)
                return Json(new { success = false, message = "Ürün zaten favorilerde.", favoriteId = exists.Id });

            var fav = new Favorite
            {
                AppUserId = userId,
                ProductId = request.ProductId,
                AddedDate = DateTime.Now
            };

            await _favoriteService.AddAsync(fav);
            await _favoriteService.saveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Ürün favorilere eklendi.",
                favoriteId = fav.Id
            });
        }

        public class AddFavoriteRequest
        {
            public int ProductId { get; set; }
            public int FavoriteId { get; set; } // yeni eklendi
        }




        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] RemoveFavoriteRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });

            var userId = int.Parse(userIdClaim);

            Favorite favorite = null;

            if (request.FavoriteId > 0)
            {
                favorite = await _favoriteService.GetAsync(f => f.AppUserId == userId && f.Id == request.FavoriteId);
            }
            else if (request.ProductId > 0)
            {
                favorite = await _favoriteService.GetAsync(f => f.AppUserId == userId && f.ProductId == request.ProductId);
            }

            if (favorite == null)
            {
                return Json(new { success = false, message = $"Favori bulunamadı. userId={userId}, favoriteId={request.FavoriteId}, productId={request.ProductId}" });
            }

            _favoriteService.Delete(favorite);
            await _favoriteService.saveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Ürün favorilerden kaldırıldı.",
                favoriteId = request.FavoriteId,
                productId = request.ProductId
            });
        }

        public class RemoveFavoriteRequest
        {
            public int FavoriteId { get; set; }
            public int ProductId { get; set; }
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
