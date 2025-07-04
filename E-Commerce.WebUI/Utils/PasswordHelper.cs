using E_Commerse.Core.Entities;
using Microsoft.AspNetCore.Identity;
namespace E_Commerce.WebUI.Utils
{


    public class PasswordHelper
    {
        private static PasswordHasher<AppUser> hasher = new PasswordHasher<AppUser>();

        public static string HashPassword(AppUser user, string password)
        {
            return hasher.HashPassword(user, password);
        }

        public static PasswordVerificationResult VerifyHashedPassword(AppUser user, string hashedPassword, string providedPassword)
        {
            return hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }
    }

}
