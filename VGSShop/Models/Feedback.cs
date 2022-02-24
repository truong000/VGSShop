using System;
using System.Collections.Generic;

#nullable disable

namespace VGSShop.Models
{
    public partial class Feedback
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? Status { get; set; }
    }
}
