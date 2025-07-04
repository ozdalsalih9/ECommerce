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

        public async Task<IActionResult> Index(string? q)
        {
            var productsQuery = _context.Products
                .Include(p => p.ProductSizes)
                    .ThenInclude(ps => ps.Size)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.IsActive && p.IsHome);

            if (!string.IsNullOrWhiteSpace(q))
            {
                productsQuery = productsQuery
                    .Where(p => p.Name.Contains(q) || p.Description.Contains(q));
            }

            ViewData["SearchQuery"] = q; // Arama terimini view'a gönderiyoruz

            var model = new HomePageViewModel
            {
                Sliders = await _context.Sliders.ToListAsync(),
                News = await _context.News
    .Where(n => n.IsActive)
    .OrderByDescending(n => n.CreateTime)
    .ToListAsync(),
                Products = await productsQuery
                    .OrderBy(p => p.OrderNo)
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
