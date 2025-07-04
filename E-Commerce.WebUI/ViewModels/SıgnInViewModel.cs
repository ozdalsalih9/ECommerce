
using System.ComponentModel.DataAnnotations;
namespace E_Commerce.WebUI.ViewModels
{
public class SignInViewModel
{
    [Required(ErrorMessage = "E-posta adresi zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [Display(Name = "Mail Adresi")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; }
}
}

