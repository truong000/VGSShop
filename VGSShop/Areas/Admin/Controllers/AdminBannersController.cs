using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VGSShop.Helpers;
using VGSShop.Models;

namespace VGSShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBannersController : Controller
    {
        private readonly VGSShopContext _context;
        public INotyfService _notyfService { get; }

        public AdminBannersController(VGSShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminBanners
        public async Task<IActionResult> Index()
        {
            return View(await _context.Banners.ToListAsync());
        }

        // GET: Admin/AdminBanners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (banner == null)
            {
                return NotFound();
            }

            return View(banner);
        }

        // GET: Admin/AdminBanners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminBanners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image,DisplayOrder,Link,Description,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy,Status")] Banner banner, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                //Xử lý hình ảnh thumb
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.ToUnsignString(banner.Description) + extension;
                    banner.Image = await Utilities.UploadFile(fThumb, @"banner", image.ToLower());
                }
                if (string.IsNullOrEmpty(banner.Image)) banner.Image = "default.jpg";
                _context.Add(banner);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm thành công");
                return RedirectToAction(nameof(Index));

            }
            return View(banner);
        }

        // GET: Admin/AdminBanners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound();
            }
            return View(banner);
        }

        // POST: Admin/AdminBanners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Image,DisplayOrder,Link,Description,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy,Status")] Banner banner, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != banner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Xử lý hình ảnh thumb
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.ToUnsignString(banner.Description) + extension;
                        banner.Image = await Utilities.UploadFile(fThumb, @"banner", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(banner.Image)) banner.Image = "default.jpg";
                    _context.Update(banner);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BannerExists(banner.Id))
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
            return View(banner);
        }

        // GET: Admin/AdminBanners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (banner == null)
            {
                return NotFound();
            }

            return View(banner);
        }

        // POST: Admin/AdminBanners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool BannerExists(int id)
        {
            return _context.Banners.Any(e => e.Id == id);
        }
    }
}
