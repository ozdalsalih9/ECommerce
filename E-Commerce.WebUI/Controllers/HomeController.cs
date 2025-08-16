using System.Diagnostics;
using E_Commerce.Service.Abstract;
using E_Commerce.WebUI.Models;
using E_Commerce.WebUI.ViewModels;
using E_Commerse.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IService<Product> _productService;
        private readonly IService<Slider> _sliderService;
        private readonly IService<News> _newsService;
        private readonly IService<Contact> _contactService;

        public HomeController(
            IService<Product> productService,
            IService<Slider> sliderService,
            IService<News> newsService,
            IService<Contact> contactService)
        {
            _productService = productService;
            _sliderService = sliderService;
            _newsService = newsService;
            _contactService = contactService;
        }

        public async Task<IActionResult> Index(string? q)
        {
            var productsQuery = _productService.GetQueryable()
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.IsActive && p.IsHome);

            if (!string.IsNullOrWhiteSpace(q))
            {
                productsQuery = productsQuery
                    .Where(p => p.Name.Contains(q) || p.Description.Contains(q));
            }

            var products = await productsQuery
                .OrderBy(p => p.OrderNo)
                .ToListAsync();

            // Description'ı tek satır görünüm için kısalt (örneğin 40 karakter)
            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.Description) && product.Description.Length > 40)
                {
                    product.Description = product.Description.Substring(0, 40) + "...";
                }
            }

            var popularProducts = await _productService.GetQueryable()
                .Include(p => p.ProductSizes).ThenInclude(ps => ps.Size)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Favorites)
                .Where(p => p.IsActive && p.Favorites.Any())
                .OrderByDescending(p => p.Favorites.Count)
                .Take(8)
                .ToListAsync();

            // Description ve Name kısaltma
            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.Name) && product.Name.Length > 30)
                {
                    product.Name = product.Name.Substring(0, 30) + "...";
                }

                if (!string.IsNullOrEmpty(product.Description) && product.Description.Length > 35)
                {
                    product.Description = product.Description.Substring(0, 35) + "...";
                }
            }

            foreach (var product in popularProducts)
            {
                if (!string.IsNullOrEmpty(product.Name) && product.Name.Length > 30)
                {
                    product.Name = product.Name.Substring(0, 30) + "...";
                }

                if (!string.IsNullOrEmpty(product.Description) && product.Description.Length > 35)
                {
                    product.Description = product.Description.Substring(0, 35) + "...";
                }
            }

            var sliders = await _sliderService.GetAllAsync();

            var news = await _newsService.GetQueryable()
                .Where(n => n.IsActive)
                .OrderByDescending(n => n.CreateTime)
                .ToListAsync();

            ViewBag.NewsList = news;
            ViewData["SearchQuery"] = q;

            var model = new HomePageViewModel
            {
                Sliders = sliders,
                News = news,
                Products = products,
                PopularProducts = popularProducts
            };

            return View(model);
        }


        public IActionResult CookiePolicy()
        {
            return View();
        }

        public IActionResult About()
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
                    message = "Formda eksik ya da hatalı alanlar var.",
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

            await _contactService.AddAsync(contact);
            await _contactService.saveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Mesajınız başarıyla gönderildi. En kısa sürede dönüş yapılacaktır."
            });
        }
    }
}
