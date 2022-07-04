using System;
using System.Collections.Generic;

#nullable disable

namespace VGSShop.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; }
        public int? CatId { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public string Thumb { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool BestSellers { get; set; }
        public bool HomeFlag { get; set; }
        public bool Active { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public int? UnitslnStock { get; set; }
        public DateTime? DateOfManufacture { get; set; }

        public virtual Category Cat { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
