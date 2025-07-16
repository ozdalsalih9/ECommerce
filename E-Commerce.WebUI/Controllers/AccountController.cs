using E_Commerse.Core.Entities;
using E_Commerce.Data;
using E_Commerce.WebUI.Utils; // IEmailSender veya ICustomEmailSender burada olmalı
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using E_Commerce.WebUI.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly ICustomEmailSender _emailSender;
        private readonly PasswordHasher<AppUser> _passwordHasher;
        public AccountController(DatabaseContext context, ICustomEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
            _passwordHasher = new PasswordHasher<AppUser>();
        }

        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _context.AppUsers.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("", "Bu e-posta zaten kayıtlı.");
                return View(model);
            }

            var user = new AppUser
            {
                Name = model.Name,
                Surname = model.Surname,
                Email = model.Email,
                Phone = model.Phone,
                IsActive = false,
                CreateDate = DateTime.Now,
                UserGuid = Guid.NewGuid()
            };

            // Şifreyi hashle
            user.Password = _passwordHasher.HashPassword(user, model.Password);

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            var confirmLink = Url.Action("ConfirmEmail", "Account", new { email = user.Email, code = user.UserGuid }, Request.Scheme);

            string body = $@"
    <div style='font-family:Segoe UI, sans-serif; max-width:600px; margin:auto; padding:20px; border:1px solid #e0e0e0; border-radius:10px; background:#f9f5fc;'>
        <h2 style='color:#5c3d74;'>Selam Tekstil - E-Posta Doğrulama</h2>
        <p>Merhaba <strong>{user.Name} {user.Surname}</strong>,</p>
        <p>Hesabınızı aktif hale getirmek için aşağıdaki butona tıklayın:</p>
        <div style='text-align:center; margin:20px 0;'>
            <a href='{confirmLink}' style='padding:12px 20px; background-color:#8e44ad; color:#fff; text-decoration:none; border-radius:8px; font-weight:bold;'>E-Postamı Doğrula</a>
        </div>
        <p>Bu bağlantı 24 saat boyunca geçerlidir. Eğer bu işlemi siz yapmadıysanız, bu e-postayı dikkate almayabilirsiniz.</p>
        <hr style='margin-top:30px;' />
        <p style='font-size:0.9em; color:#999;'>Selam Tekstil | © {DateTime.Now.Year}</p>
    </div>";

            await _emailSender.SendEmailAsync(user.Email, "E-Posta Doğrulama", body);

            TempData["Message"] = "Kayıt başarılı! Lütfen e-posta adresinizi doğrulayın.";
            return RedirectToAction("SignIn");
        }

        public async Task<IActionResult> ConfirmEmail(string email, Guid code)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == email && u.UserGuid == code);

            if (user == null)
            {
                TempData["Message"] = "❌ E-posta doğrulama başarısız oldu. Kod geçersiz veya kullanıcı bulunamadı.";
                return RedirectToAction("SignIn");
            }

            user.IsActive = true;
            await _context.SaveChangesAsync();

            TempData["Message"] = "✅ E-posta adresiniz başarıyla doğrulandı. Giriş yapabilirsiniz.";
            return RedirectToAction("SignIn");
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError("", "Geçersiz giriş bilgileri veya e-posta doğrulanmamış.");
                return View();
            }

            // ➕ LoginAttempt kontrolü
            var attempt = await _context.LoginAttempts.FirstOrDefaultAsync(a => a.AppUserId == user.Id);

            if (attempt != null && attempt.LockedUntil.HasValue && attempt.LockedUntil > DateTime.Now)
            {
                var kalan = attempt.LockedUntil.Value - DateTime.Now;
                ModelState.AddModelError("", $"Hesabınız kilitlendi. Lütfen {kalan.Minutes} dakika {kalan.Seconds} saniye sonra tekrar deneyin.");
                return View();
            }

            // ✅ Şifre doğrulama (hashli)
            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            bool success = verifyResult == PasswordVerificationResult.Success;

            if (!success)
            {
                if (attempt == null)
                {
                    attempt = new LoginAttempt
                    {
                        AppUserId = user.Id,
                        FailedCount = 1
                    };
                    _context.LoginAttempts.Add(attempt);
                }
                else
                {
                    attempt.FailedCount++;

                    if (attempt.FailedCount >= 5)
                    {
                        attempt.LockedUntil = DateTime.Now.AddMinutes(5);
                        ModelState.AddModelError("", "5 kez başarısız giriş yaptınız. Hesabınız 5 dakika kilitlendi.");
                    }
                }

                await _context.SaveChangesAsync();
                ModelState.AddModelError("", "Geçersiz giriş bilgileri.");
                return View();
            }

            // Başarılı giriş: LoginAttempt temizle
            if (attempt != null)
            {
                _context.LoginAttempts.Remove(attempt);
                await _context.SaveChangesAsync();
            }

            // Giriş için Claims oluştur
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, $"{user.Name} {user.Surname}"),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (user.IsAdmin)
                return RedirectToAction("Index", "Main", new { area = "Admin" });

            return RedirectToAction("Index", "Home");
        }


        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "Bu e-posta ile eşleşen kullanıcı bulunamadı.");
                return View();
            }

            var resetLink = Url.Action("ResetPassword", "Account", new { email = user.Email, code = user.UserGuid }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Şifre Sıfırlama",
                $"Şifrenizi sıfırlamak için <a href='{resetLink}'>tıklayın</a>.");

            TempData["SuccessMessage"] = "Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.";
            return RedirectToAction("SignIn");
        }

        public IActionResult ResetPassword(string email, Guid code)
        {
            ViewBag.Email = email;
            ViewBag.Code = code;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string email, Guid code, string newPassword)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == email && u.UserGuid == code);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            // Yeni şifreyi hashle ve kaydet
            user.Password = PasswordHelper.HashPassword(user, newPassword);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Şifreniz başarıyla güncellendi.";
            return RedirectToAction("SignIn");
        }
        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("SignIn");
        }
    }
}
