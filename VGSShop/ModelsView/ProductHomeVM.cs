using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Models;

namespace VGSShop.ModelsView
{
    public class ProductHomeVM
    {
        public Category category { get; set; }
        public List<Product> lsProduct { get; set; }
    }
}
