using E_Commerce.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CommentsController : Controller
    {
        private readonly DatabaseContext _context;

        public CommentsController(DatabaseContext context)
        {
            _context = context;
        }

        // Bekleyen yorumları listele
        public async Task<IActionResult> Index()
        {
            var pendingComments = await _context.Comments
                .Include(c => c.Product)
                .Include(c => c.AppUser)
                .Where(c => !c.IsApproved)
                .OrderByDescending(c => c.CreateDate)
                .ToListAsync();

            return View(pendingComments);
        }

        // Onayla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            comment.IsApproved = true;
            _context.Update(comment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Yorum onaylandı.";
            return RedirectToAction(nameof(Index));
        }

        // İsterse reddet/sil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Yorum silindi.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Product)
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }
    }
}