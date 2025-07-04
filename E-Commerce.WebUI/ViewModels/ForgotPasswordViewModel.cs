using System.ComponentModel.DataAnnotations;
namespace E_Commerce.WebUI.ViewModels
{

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [Display(Name = "Mail Adresi")]
        public string Email { get; set; }
    }

}
