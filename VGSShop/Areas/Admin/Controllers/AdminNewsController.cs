using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using VGSShop.Helpers;
using VGSShop.Models;

namespace VGSShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize()]
    public class AdminNewsController : Controller
    {
        private readonly VGSShopContext _context;
        public INotyfService _notyfService { get; }
        public AdminNewsController(VGSShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminNews
        public async Task<IActionResult> Index(int? page)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap-admin.html");
            var taiKhoanID = HttpContext.Session.GetString("AccountId");
            if (taiKhoanID == null) return RedirectToAction("Login", "Account", new { Area = "Admin" });
            var admin = _context.Accounts.AsNoTracking()
                    .SingleOrDefault(x => x.AccountId == Convert.ToInt32(taiKhoanID));

            List<News> lsNews = new List<News>();

            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = Utilities.PAGE_SIZE; //20

            if (admin.RoleId == 4)
            {
                lsNews = _context.News
                .AsNoTracking()
                .OrderByDescending(x => x.PostId).ToList();
            }
            else
            {
                lsNews = _context.News
                .AsNoTracking()
                .Where(x =>x.AccountId == admin.AccountId )
                .OrderByDescending(x => x.PostId).ToList();
            }
            PagedList<News> models = new PagedList<News>(lsNews.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/AdminNews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // GET: Admin/AdminNews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminNews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,CreatedDate,Author,AccountId,Tags,CatId,IsHot,IsNewfeed,MetaKey,MetaDesc,Views")] News news, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap-admin.html");
            var taiKhoanID = HttpContext.Session.GetString("AccountId");
            if (taiKhoanID == null) return RedirectToAction("Login", "Account", new { Area = "Admin" });
            var admin = _context.Accounts.AsNoTracking()
                    .SingleOrDefault(x => x.AccountId == Convert.ToInt32(taiKhoanID));

            if (ModelState.IsValid)
            {
                //Xử lý hình ảnh thumb
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.ToUnsignString(news.Title) + extension;
                    news.Thumb = await Utilities.UploadFile(fThumb, @"news", image.ToLower());
                }
                if (string.IsNullOrEmpty(news.Thumb)) news.Thumb = "default.jpg";
                news.Alias = Utilities.ToUnsignString(news.Title);
                news.CreatedDate = DateTime.Now;
                news.Author = admin.FullName;
                news.AccountId = admin.AccountId;
                _context.Add(news);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm mới thành công");
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        // GET: Admin/AdminNews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: Admin/AdminNews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,CreatedDate,Author,AccountId,Tags,CatId,IsHot,IsNewfeed,MetaKey,MetaDesc,Views")] News news, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap-admin.html");
            var taiKhoanID = HttpContext.Session.GetString("AccountId");
            if (taiKhoanID == null) return RedirectToAction("Login", "Account", new { Area = "Admin" });
            var admin = _context.Accounts.AsNoTracking()
                    .SingleOrDefault(x => x.AccountId == Convert.ToInt32(taiKhoanID));
            if (id != news.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                { //Xử lý hình ảnh thumb
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.ToUnsignString(news.Title) + extension;
                        news.Thumb = await Utilities.UploadFile(fThumb, @"news", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(news.Thumb)) news.Thumb = "default.jpg";
                    news.Alias = Utilities.ToUnsignString(news.Title);
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Chỉnh sửa thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.PostId))
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
            return View(news);
        }

        // GET: Admin/AdminNews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: Admin/AdminNews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.FindAsync(id);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.PostId == id);
        }
    }
}
