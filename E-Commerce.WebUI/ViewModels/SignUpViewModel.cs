using System.ComponentModel.DataAnnotations;
namespace E_Commerce.WebUI.ViewModels
{

    public class SignUpViewModel
    {
        [Required]
        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Soyisim")]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-Posta")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre (Tekrar)")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }

        [Phone]
        [Display(Name = "Telefon")]
        public string? Phone { get; set; }
    }

}

