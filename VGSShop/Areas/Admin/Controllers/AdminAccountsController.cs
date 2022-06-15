using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VGSShop.Areas.Admin.Models;
using VGSShop.Extension;
using VGSShop.Helpers;
using VGSShop.Models;

namespace VGSShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize()]
    public class AdminAccountsController : Controller
    {
        private readonly VGSShopContext _context;

        public INotyfService _notyfService { get; }
        public AdminAccountsController(VGSShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminAccounts
        public async Task<IActionResult> Index()
        {
            //Xác thực tài khoản
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap-admin.html");
            var taiKhoanID = HttpContext.Session.GetString("AccountId");
            if (taiKhoanID == null) return RedirectToAction("Login", "Account", new { Area = "Admin" });
            var admin = _context.Accounts.AsNoTracking()
                    .SingleOrDefault(x => x.AccountId == Convert.ToInt32(taiKhoanID));
            if(admin.RoleId != 4)
            {
                _notyfService.Warning("Chỉ quản trị viên mới được phép sửa dụng chức năng này");
                return RedirectToAction("Index", "AdminHome", new { Area = "Admin" });
            }
            else
            {                
                ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "Description");
                List<SelectListItem> lsTrangThai = new List<SelectListItem>();
                lsTrangThai.Add(new SelectListItem() { Text = "Kích hoạt", Value = "1" });
                lsTrangThai.Add(new SelectListItem() { Text = "Khóa", Value = "0" });
                ViewData["lsTrangThai"] = lsTrangThai;
                var vGSShopContext = _context.Accounts.Include(a => a.Role);
                return View(await vGSShopContext.ToListAsync());
            }
           
        }

        // GET: Admin/AdminAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/AdminAccounts/Create
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap-admin.html");
            var taiKhoanID = HttpContext.Session.GetString("AccountId");
            if (taiKhoanID == null) return RedirectToAction("Login", "Account", new { Area = "Admin" });
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Admin/AdminAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,Phone,Email,Password,Salt,Active,FullName,RoleId,LastLogin,CreateDate")] Account account)
        {         
            if (ModelState.IsValid)
            {
                string salt = Utilities.GetRandomKey();
                account.Salt = salt;

                //Tạo random mật khẩu 
                account.Password = (account.Password + salt.Trim()).ToMD5();
                account.CreateDate = DateTime.Now;

                _context.Add(account);
                await _context.SaveChangesAsync();
                _notyfService.Success("Tạo mới tài quản quản trị thành công");
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }
        public IActionResult ChangePassword()
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap-admin.html");
            var taiKhoanID = HttpContext.Session.GetString("AccountId");
            if (taiKhoanID == null) return RedirectToAction("Login", "Account", new { Area = "Admin" });
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var taikhoan = _context.Accounts.AsNoTracking().SingleOrDefault(x => x.Email == model.Email);
                if (taikhoan == null) return RedirectToAction("Login", "Accounts");

                var pass = (model.PasswordNow.Trim() + taikhoan.Salt.Trim()).ToMD5();
                if (pass == taikhoan.Password)
                {
                    string newpass = (model.Password.Trim() + taikhoan.Salt.Trim()).ToMD5();
                    taikhoan.Password = newpass;
                    taikhoan.LastLogin = DateTime.Now;
                    _context.Update(taikhoan);
                    _context.SaveChanges();
                    _notyfService.Success("Đổi mật khẩu thành công");
                    return RedirectToAction("Login", "Accounts" , new { Area = "Admin"});
                }
            }
            return View();
        }

        // GET: Admin/AdminAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // POST: Admin/AdminAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,Phone,Email,Password,Salt,Active,FullName,RoleId,LastLogin,CreateDate")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuyenTruyCap"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // GET: Admin/AdminAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/AdminAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
