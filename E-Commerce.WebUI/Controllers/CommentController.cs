using E_Commerce.Service.Abstract;
using E_Commerse.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerse.WebUI.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly IService<Comment> _commentService;

        public CommentController(IService<Comment> commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int productId, string content, int? rating)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Yorum içeriği boş olamaz.";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var comment = new Comment
            {
                Content = content,
                Rating = rating,
                ProductId = productId,
                AppUserId = int.Parse(userId),
                CreateDate = DateTime.Now,
                IsApproved = false // Admin onayı bekleniyor
            };

            await _commentService.AddAsync(comment);
            await _commentService.saveChangesAsync();

            TempData["SuccessMessage"] = "Yorumunuz başarıyla gönderildi. Onaylandıktan sonra görünecektir.";
            return RedirectToAction("Details", "Product", new { id = productId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentService.FindAsync(id);
            if (comment == null)
                return NotFound();

            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (comment.AppUserId != int.Parse(userId) && !isAdmin)
                return Forbid();

            _commentService.Delete(comment);
            await _commentService.saveChangesAsync();

            TempData["SuccessMessage"] = "Yorum başarıyla silindi.";
            return RedirectToAction("Details", "Product", new { id = comment.ProductId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveComment(int id)
        {
            var comment = await _commentService.FindAsync(id);
            if (comment == null)
                return NotFound();

            comment.IsApproved = true;
            _commentService.Update(comment);
            await _commentService.saveChangesAsync();

            TempData["SuccessMessage"] = "Yorum onaylandı.";
            return RedirectToAction("Details", "Product", new { id = comment.ProductId });
        }
    }
}
