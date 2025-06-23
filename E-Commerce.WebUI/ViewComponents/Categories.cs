using E_Commerce.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.WebUI.ViewComponents
{
    public class Categories : ViewComponent
    {
        private readonly DatabaseContext _context;

        public Categories(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _context.Categories.ToListAsync());
        }
    }
}
