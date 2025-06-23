using System.Diagnostics;
using E_Commerce.Data;
using E_Commerce.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _context;
        public HomeController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomePageViewModel
            {
                Sliders = await _context.Sliders.ToListAsync(),
                News = await _context.News.ToListAsync(),
                Products = await _context.Products
                    .Include(p => p.ProductSizes)
                        .ThenInclude(ps => ps.Size) // Beden bilgilerini include ediyoruz
                    .Include(p => p.Category)       // Kategori bilgisi
                    .Include(p => p.Brand)          // Marka bilgisi
                    .Where(p => p.IsActive && p.IsHome) // Sadece aktif ve anasayfa ürünleri
                    .OrderBy(p => p.OrderNo)       // Sýralama
                    .ToListAsync()
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
