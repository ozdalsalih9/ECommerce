using E_Commerce.Service.Abstract;
using E_Commerse.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.WebUI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IService<Category> _categoryService;
        private readonly IService<Product> _productService;

        public CategoriesController(IService<Category> categoryService, IService<Product> productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(int? id, string search, string sort, int page = 1)
        {
            if (id == null)
                return NotFound();

            const int pageSize = 9;

            var category = await _categoryService
                .GetQueryable()
                .Include(c => c.Products)
                .ThenInclude(p => p.ProductSizes)
                .ThenInclude(ps => ps.Size)
        .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            var productsQuery = _productService
                .GetQueryable()
                .Where(p => p.CategoryId == id);

            // Arama filtresi
            if (!string.IsNullOrEmpty(search))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(search) ||
                    p.Description.Contains(search));
            }

            // Sıralama
            switch (sort)
            {
                case "name_asc":
                    productsQuery = productsQuery.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Name);
                    break;
                case "newest":
                    productsQuery = productsQuery.OrderByDescending(p => p.CreateTime);
                    break;
                default:
                    productsQuery = productsQuery.OrderBy(p => p.Name);
                    break;
            }

            // Sayfalama
            var totalProducts = await productsQuery.CountAsync();
            var products = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            category.Products = products;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);
            ViewBag.SearchQuery = search;
            ViewBag.SortBy = sort;

            return View(category);
        }
    }
}