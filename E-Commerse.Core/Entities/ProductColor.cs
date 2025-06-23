using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerse.Core.Entities
{
    public class ProductColor : IEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ColorId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [ForeignKey("ColorId")]
        public Color Color { get; set; }

        public ICollection<ProductColorImage> ProductColorImages { get; set; } = new List<ProductColorImage>();
    }
}