using System.ComponentModel.DataAnnotations;

namespace E_Commerce.WebUI.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string Surname { get; set; }

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Telefon numarası zorunludur.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Telefon numarası sadece 10 rakamdan oluşmalıdır.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Mesaj alanı boş olamaz.")]
        public string Message { get; set; }
    }
}
