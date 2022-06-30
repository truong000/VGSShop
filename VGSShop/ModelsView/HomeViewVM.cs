using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Models;

namespace VGSShop.ModelsView
{
    public class HomeViewVM
    {
        public List<ProductHomeVM> Products { get; set; }

        public List<News> News { get; set; }
        public List<Banner> Banners { get; set; }
    }

}
