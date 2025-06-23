using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerse.Core.Entities
{
    public class ProductSize : IEntity
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int SizeId { get; set; }
        public Size Size { get; set; }

        [Display(Name = "Stok Adedi")]
        public int Stock { get; set; }
    }
}