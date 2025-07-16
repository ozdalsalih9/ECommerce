using System.Diagnostics;
using E_Commerce.Data;
using E_Commerce.WebUI.Models;
using E_Commerce.WebUI.ViewModels;
using E_Commerse.Core.Entities;
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

            var popularProducts = await _context.Products
    .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
    .Include(p => p.Brand)
    .Include(p => p.Category)
    .Include(p => p.Favorites)
    .Where(p => p.IsActive && p.Favorites.Any())
    .OrderByDescending(p => p.Favorites.Count)
    .Take(8)
    .ToListAsync();


            ViewData["SearchQuery"] = q; // Arama terimini view'a gönderiyoruz

            var model = new HomePageViewModel
            {
                Sliders = await _context.Sliders.ToListAsync(),
                News = await _context.News
         .Where(n => n.IsActive)
         .OrderByDescending(n => n.CreateTime)
         .ToListAsync(),
                Products = await productsQuery.OrderBy(p => p.OrderNo).ToListAsync(),
                PopularProducts = popularProducts
            };


            if (!string.IsNullOrWhiteSpace(q))
            {
                productsQuery = productsQuery
                    .Where(p => p.Name.Contains(q) || p.Description.Contains(q));
            }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactUs(ContactViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return Json(new
                {
                    success = false,
                    message = "Formda eksik ya da hatalý alanlar var.",
                    errors
                });
            }

            var contact = new Contact
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                Phone = model.Phone,
                Message = model.Message
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Mesajýnýz baþarýyla gönderildi. En kýsa sürede dönüþ yapýlacaktýr."
            });
        }



    }
}
