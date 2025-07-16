using E_Commerce.Service.Abstract;
using E_Commerse.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace E_Commerce.WebUI.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IService<Category> _categoryService;

        public CategoriesController(IService<Category> categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> IndexAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var category = await _categoryService
                .GetQueryable()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            return View(category);
        }
    }
}
