using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VGSShop.ModelsView
{
    public class RegisterVM 
    {
        [Key]
        public int CustomerId { get; set; }
        [Display(Name = "Họ Và Tên")]
        [Required(ErrorMessage = "Vui lòng nhập Họ Tên")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [MaxLength(20)]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "ValidateEmail", controller: "Accounts")]
        public string Email { get; set; }
        [MaxLength(11)]
        [MinLength(5, ErrorMessage = "Bạn cần đặt số điện tối thiểu 5 ký tự")]
        [Required(ErrorMessage = "Vui lòng nhập Số điện thoại")]
        [Display(Name = "Điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Accounts")]
        public string Phone { get; set; }
        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        public string Password { get; set; }
        [MaxLength(15, ErrorMessage = "Mật khẩu tối đã chỉ 15 ký tự")]
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Vui lòng nhập mật khẩu giống nhau")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
    }
}
