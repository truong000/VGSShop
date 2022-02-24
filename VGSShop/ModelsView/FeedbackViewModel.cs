using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VGSShop.ModelsView
{
    public class FeedbackViewModel
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Họ Và Tên")]
        [Required(ErrorMessage = "Vui lòng nhập Họ Tên")]
        public string Name { get; set; }
        [MaxLength(11)]
        [MinLength(5, ErrorMessage = "Bạn cần đặt số điện tối thiểu 5 ký tự")]
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
        [Display(Name = "Điện thoại")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [MaxLength(20)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Hỗ trợ")]
        public string Content { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
