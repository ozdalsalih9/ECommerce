using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerse.Core.Entities
{
    public class ProductColorImage : IEntity
    {
        public int Id { get; set; }
        public int ProductColorId { get; set; }
        public string? ImageUrl { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        [ForeignKey("ProductColorId")]
        public ProductColor ProductColor { get; set; }
    }
}