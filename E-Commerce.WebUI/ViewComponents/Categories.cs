using E_Commerce.Data;
using E_Commerse.Core.Entities;
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
            var categories = await _context.Categories
                .Where(c => c.IsActive) // Sadece aktif kategoriler
                .OrderBy(c => c.OrderNo) // Sıra numarasına göre sırala
                .ToListAsync();

            return View(categories);
        }
    }
}