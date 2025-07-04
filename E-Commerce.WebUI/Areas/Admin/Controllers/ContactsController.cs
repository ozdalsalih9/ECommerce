using E_Commerce.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ContactsController : Controller
    {
        private readonly DatabaseContext _context;

        public ContactsController(DatabaseContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Contacts);
        }
    }
}
