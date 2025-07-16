using E_Commerse.Core.Entities;
using E_Commerce.Service.Abstract;
using E_Commerce.WebUI.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using E_Commerce.WebUI.ViewModels;

namespace E_Commerce.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IService<AppUser> _userService;
        private readonly IService<LoginAttempt> _attemptService; // Eğer LoginAttempt için de generic kullanacaksan
        private readonly ICustomEmailSender _emailSender;
        private readonly PasswordHasher<AppUser> _passwordHasher;

        public AccountController(IService<AppUser> userService, IService<LoginAttempt> attemptService, ICustomEmailSender emailSender)
        {
            _userService = userService;
            _attemptService = attemptService;
            _emailSender = emailSender;
            _passwordHasher = new PasswordHasher<AppUser>();
        }

        public IActionResult SignIn() => View();

        public IActionResult SignUp() => View();

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if ((await _userService.GetAsync(u => u.Email == model.Email)) != null)
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

            user.Password = _passwordHasher.HashPassword(user, model.Password);

            await _userService.AddAsync(user);
            await _userService.saveChangesAsync();

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
            var user = await _userService.GetAsync(u => u.Email == email && u.UserGuid == code);
            if (user == null)
            {
                TempData["Message"] = "❌ E-posta doğrulama başarısız oldu.";
                return RedirectToAction("SignIn");
            }

            user.IsActive = true;
            _userService.Update(user);
            await _userService.saveChangesAsync();

            TempData["Message"] = "✅ E-posta adresiniz başarıyla doğrulandı.";
            return RedirectToAction("SignIn");
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(string email, string password)
        {
            var user = await _userService.GetAsync(u => u.Email == email);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError("", "Geçersiz giriş bilgileri veya e-posta doğrulanmamış.");
                return View();
            }

            var attempt = await _attemptService.GetAsync(a => a.AppUserId == user.Id);

            if (attempt != null && attempt.LockedUntil.HasValue && attempt.LockedUntil > DateTime.Now)
            {
                var kalan = attempt.LockedUntil.Value - DateTime.Now;
                ModelState.AddModelError("", $"Hesabınız kilitlendi. {kalan.Minutes} dk {kalan.Seconds} sn sonra tekrar deneyin.");
                return View();
            }

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
                    await _attemptService.AddAsync(attempt);
                }
                else
                {
                    attempt.FailedCount++;
                    if (attempt.FailedCount >= 5)
                    {
                        attempt.LockedUntil = DateTime.Now.AddMinutes(5);
                        ModelState.AddModelError("", "5 kez hatalı giriş yaptınız. Hesabınız 5 dk kilitlendi.");
                    }
                }

                await _attemptService.saveChangesAsync();
                ModelState.AddModelError("", "Geçersiz giriş bilgileri.");
                return View();
            }

            if (attempt != null)
            {
                _attemptService.Delete(attempt);
                await _attemptService.saveChangesAsync();
            }

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

        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userService.GetAsync(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "Bu e-posta ile eşleşen kullanıcı bulunamadı.");
                return View();
            }

            var resetLink = Url.Action("ResetPassword", "Account", new { email = user.Email, code = user.UserGuid }, Request.Scheme);
            await _emailSender.SendEmailAsync(user.Email, "Şifre Sıfırlama",
                $"Şifrenizi sıfırlamak için <a href='{resetLink}'>tıklayın</a>.");

            TempData["SuccessMessage"] = "Şifre sıfırlama bağlantısı gönderildi.";
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
            var user = await _userService.GetAsync(u => u.Email == email && u.UserGuid == code);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            user.Password = _passwordHasher.HashPassword(user, newPassword);
            _userService.Update(user);
            await _userService.saveChangesAsync();

            TempData["SuccessMessage"] = "Şifreniz güncellendi.";
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
