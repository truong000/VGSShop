using System;
using System.Collections.Generic;

#nullable disable

namespace VGSShop.Models
{
    public partial class Banner
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public int? DisplayOrder { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool Status { get; set; }
    }
}
