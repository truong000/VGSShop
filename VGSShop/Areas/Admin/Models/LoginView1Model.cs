using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VGSShop.Areas.Admin.Models
{
    public class LoginView1Model
    {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [Display(Name = " Địa chỉ Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Vui lòng nhập Email")]
        public string Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu ít nhất 5 ký tự")]
        public string Password { get; set; }
    }
    
}
