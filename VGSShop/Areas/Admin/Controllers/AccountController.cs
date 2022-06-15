using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VGSShop.Areas.Admin.Models;
using VGSShop.Extension;
using VGSShop.Models;

namespace VGSShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly VGSShopContext _context;

        public INotyfService _notyfService { get; }
        public AccountController(VGSShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap-admin.html", Name = "LoginAdmin")]

        public IActionResult Login(string returnUrl = null)
        {
            var taiKhoanID = HttpContext.Session.GetString("AccountId");
            if (taiKhoanID != null) return RedirectToAction("Index", "Home", new { Area = "Admin" });
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap-admin.html", Name = "LoginAdmin")]
        public async Task<IActionResult> Login(LoginView1Model model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Account kh = _context.Accounts
                        .Include(p => p.Role)
                        .SingleOrDefault(p => p.Email.ToLower() == model.Email.ToLower().Trim());

                    if (kh == null)
                    {
                        ViewBag.Error = "Thông tin đăng nhập không chính xác";
                        return View(model);
                    }

                    string pass = (model.Password.Trim() + kh.Salt.Trim()).ToMD5();
                    if (kh.Password.Trim() != pass)
                    {
                        ViewBag.Error = "Thông tin đăng nhập không chính xác";
                        return View(model);
                    }
                    //Đăng nhập thành công
                    //Ghi nhận thời gian đăng nhập
                    kh.LastLogin = DateTime.Now;
                    _context.Update(kh);
                    await _context.SaveChangesAsync();

                    var taiKhoanID = HttpContext.Session.GetString("AccountId");
                    //Identity
                    //Lưu Session MaKh
                    HttpContext.Session.SetString("AccountId", kh.AccountId.ToString());

                    //Identity
                    var userClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, kh.FullName),
                        new Claim(ClaimTypes.Email, kh.Email),
                        new Claim("AccountId", kh.AccountId.ToString()),
                        new Claim("RoleID", kh.RoleId.ToString()),
                        new Claim(ClaimTypes.Role, kh.Role.RoleName)
                    };

                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                    await HttpContext.SignInAsync(userPrincipal);
                    _notyfService.Success("Đăng nhập thành công");
                    return RedirectToAction("Index", "AdminHome", new { Area ="Admin" });
                }
            }
            catch
            {
                return RedirectToAction("Login", "Account", new { Area = "Admin" });
            }
            return RedirectToAction("Login", "Account", new { Area = "Admin" });
        }
        [HttpGet]
        [Route("dang-xuat-admin.html", Name = "LogoutAdmin")]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.SignOutAsync();
                HttpContext.Session.Remove("AccountId");
                return RedirectToAction("Login", "Account", new { Area = "Admin" });
            }
            catch
            {
                return RedirectToAction("Login", "Account", new { Area = "Admin" });
            }

        }
    }
}
