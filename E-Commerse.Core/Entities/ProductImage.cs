using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerse.Core.Entities
{
    public class ProductImage : IEntity
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
