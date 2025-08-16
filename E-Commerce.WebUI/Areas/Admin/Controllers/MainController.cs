using E_Commerce.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using E_Commerce.Core.Entities;

namespace E_Commerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MainController : Controller
    {
        private readonly DatabaseContext _ctx;
        public MainController(DatabaseContext ctx) { _ctx = ctx; }

        // Basit dashboard (son 7 gün)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var since = DateTime.UtcNow.AddDays(-7);
            var model = await _ctx.PageViews
                .Where(v => v.At >= since)
                .GroupBy(v => v.Path)
                .Select(g => new PathCount { Path = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            ViewBag.Total = await _ctx.PageViews.CountAsync(v => v.At >= since);
            return View(model);
        }

        // İstemciden çağrılan kayıt endpoint'i (çerez onayı varsa kaydeder)
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> Track([FromQuery] string? p = null)
        {
            var canTrack = HttpContext.Features.Get<ITrackingConsentFeature>()?.CanTrack ?? false;
            if (!canTrack) return NoContent();

            var path = string.IsNullOrWhiteSpace(p) ? HttpContext.Request.Path.ToString() : p;
            var referer = Request.Headers["Referer"].ToString();
            var ua = Request.Headers["User-Agent"].ToString();

            _ctx.PageViews.Add(new PageView
            {
                Path = path.Length > 256 ? path[..256] : path,
                Referrer = referer.Length > 512 ? referer[..512] : referer,
                At = DateTime.UtcNow,
                UserAgent = ua.Length > 256 ? ua[..256] : ua
            });
            await _ctx.SaveChangesAsync();
            return NoContent();
        }

        public class PathCount { public string Path { get; set; } = ""; public int Count { get; set; } }
    }
}
