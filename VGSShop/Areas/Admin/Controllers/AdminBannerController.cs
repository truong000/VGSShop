using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Helpers;
using VGSShop.Models;

namespace VGSShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminBannerController : Controller
    {
        private readonly VGSShopContext _context;
        public INotyfService _notyfService { get; }
        public AdminBannerController(VGSShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminPages
        public async Task<IActionResult> Index(int? page)
        {
            var lsPages = _context.Banners
                .AsNoTracking()
                .OrderByDescending(x => x.Id);
            return View();
        }

        // GET: Admin/AdminPages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Banners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // GET: Admin/AdminPages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminPages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Image,DisplayOrder,Description,Status")] Banner page, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {//Xử lý hình ảnh thumb
                if (fThumb != null)
                {
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.ToUnsignString(page.Description) + extension;
                    page.Image = await Utilities.UploadFile(fThumb, @"banner", image.ToLower());
                }
                if (string.IsNullOrEmpty(page.Image)) page.Image = "default.jpg";
                page.Description = Utilities.ToUnsignString(page.Description);
                _context.Add(page);
                await _context.SaveChangesAsync();
                _notyfService.Success("Thêm sản PAGES thành công");
                return RedirectToAction(nameof(Index));
            }
            return View(page);
        }

        // GET: Admin/AdminPages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Banners.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        // POST: Admin/AdminPages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Image,DisplayOrder,Description,Status")] Banner page, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != page.Id)
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
                        string image = Utilities.ToUnsignString(page.Description) + extension;
                        page.Image = await Utilities.UploadFile(fThumb, @"banner", image.ToLower());
                    }
                    if (string.IsNullOrEmpty(page.Image)) page.Image = "default.jpg";
                    page.Description = Utilities.ToUnsignString(page.Description);
                    _context.Update(page);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Chỉnh sửa Banner thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageExists(page.Id))
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
            return View(page);
        }

        // GET: Admin/AdminPages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var page = await _context.Banners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // POST: Admin/AdminPages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var page = await _context.Banners.FindAsync(id);
            _context.Banners.Remove(page);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa PAGES thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool PageExists(int id)
        {
            return _context.Banners.Any(e => e.Id == id);
        }
    }
}
