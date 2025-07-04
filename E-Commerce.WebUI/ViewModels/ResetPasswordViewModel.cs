using System;
using System.ComponentModel.DataAnnotations;
namespace E_Commerce.WebUI.ViewModels
{


    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public Guid Code { get; set; }

        [Required(ErrorMessage = "Yeni şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        [Compare("NewPassword", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string ConfirmPassword { get; set; }
    }

}
