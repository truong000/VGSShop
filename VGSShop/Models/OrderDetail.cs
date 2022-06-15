using System;
using System.Collections.Generic;

#nullable disable

namespace VGSShop.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? OrderNumber { get; set; }
        public int? Quantity { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Total { get; set; }
        public DateTime? ShipDate { get; set; }
        public decimal? TotalMoney { get; set; }
        public DateTime? CreateDate { get; set; }
        public decimal Price { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
