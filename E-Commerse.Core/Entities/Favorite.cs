using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerse.Core.Entities
{
    public class Favorite : IEntity
    {
        public int Id { get; set; }

        // Kullanıcıya ait
        public int AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

        // Favori ürüne ait
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.Now;
    }
}
