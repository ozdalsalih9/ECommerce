using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerse.Core.Entities
{
    public class Size : IEntity
    {
        public int Id { get; set; }

        [Display(Name = "Beden Adı")]
        public string? Name { get; set; }

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Display(Name = "Sıra No")]
        public int OrderNo { get; set; }

        public ICollection<ProductSize> ProductSizes { get; set; }
    }
}