using System;
using System.Collections.Generic;

namespace VGSShop.Models
{
    public class ProductViewModel
    {
        public int ID { set; get; }
        public string Images { set; get; }
        public string Name { set; get; }
        public decimal? Price { set; get; }
        public string CateName { set; get; }
        public string CateMetaTitle { set; get; }
        public string MetaTitle { set; get; }
        public DateTime? CreatedDate { set; get; }      
    }
}