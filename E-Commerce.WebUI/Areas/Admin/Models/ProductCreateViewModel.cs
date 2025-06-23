using E_Commerse.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace E_Commerce.WebUI.Areas.Admin.Models
{
    public class ProductCreateViewModel
    {
        public Product Product { get; set; } = new();
        public IFormFile? Image { get; set; }

        public List<string>? Sizes { get; set; }
        public List<int>? Stocks { get; set; }
    }
}
