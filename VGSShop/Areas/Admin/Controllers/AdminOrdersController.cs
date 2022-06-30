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
using OfficeOpenXml;
using PagedList.Core;
using VGSShop.Models;

namespace VGSShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize()]
    public class AdminOrdersController : Controller
    {
        private readonly VGSShopContext _context;
        public INotyfService _notyfService { get; }
        public AdminOrdersController(VGSShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Admin/AdminOrders
        public IActionResult Index(int? page, int TransactStatusID = 0)
        {
            if (!User.Identity.IsAuthenticated) Response.Redirect("/dang-nhap-admin.html");
            var taiKhoanID = HttpContext.Session.GetString("AccountId");
            if (taiKhoanID == null) return RedirectToAction("Login", "Account", new { Area = "Admin" });
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20; //20
            List<Order> lsNews = new List<Order>();
            if (TransactStatusID != 0)
            {
                lsNews = _context.Orders
                .AsNoTracking()
                .Where(x => x.TransactStatusId == TransactStatusID)
                .Include(x => x.Customer)
                .Include(o => o.TransactStatus)
                .OrderByDescending(x => x.OrderDate).ToList();

            }
            else
            {
                lsNews = _context.Orders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.TransactStatus)
                .OrderByDescending(x => x.OrderDate).ToList();

            }
            PagedList<Order> models = new PagedList<Order>(lsNews.AsQueryable(), pageNumber, pageSize);         
            ViewBag.TransactStatusID = TransactStatusID;
            ViewBag.CurrentPage = pageNumber;
            ViewData["TrangThaiDonHang"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", TransactStatusID);
            return View(models);
        }
        public IActionResult Filtter(int TransactStatusID = 0)
        {
            var url = $"/Admin/AdminOrders?TransactStatusID={TransactStatusID}";
            if (TransactStatusID == 0)
            {
                url = $"/Admin/AdminOrders";
            }
            return Json(new { status = "success", redirectUrl = url });
        }
        public async Task<IActionResult> ChangeStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.AsNoTracking().Include(x => x.Customer).FirstOrDefaultAsync(x => x.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["TrangThai"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return PartialView("ChangeStatus", order);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, [Bind("OrderId,CustomerId,OrderDate,ShipDate,TransactStatusId,Deleted,Paid,PaymentDate,PaymentId,Note,TotalMoney,Address,LocationId,District,Ward")] Order order)
        {
            if(id != order.OrderId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var donhang = await _context.Orders.AsNoTracking().Include(x => x.Customer).FirstOrDefaultAsync(x => x.OrderId == id);
                    if(donhang != null)
                    {
                        donhang.Paid = order.Paid;
                        donhang.Deleted = order.Deleted;
                        donhang.TransactStatusId = order.TransactStatusId;
                        if(donhang.Paid == true)
                        {
                            donhang.PaymentDate = DateTime.Now;
                        }
                        if (donhang.TransactStatusId == 5) donhang.Deleted = true;
                        if (donhang.TransactStatusId == 3) donhang.ShipDate = DateTime.Now;
                    }
                    _context.Update(donhang);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật trạng thấy thành công thành công");
                }
                catch(DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["TrangThai"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return PartialView("ChangeStatus", order);
        }

        // GET: Admin/AdminOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            var chitietdonhang = _context.OrderDetails
                .Include(x => x.Product)
                .AsNoTracking()
                .Where(x => x.OrderId == order.OrderId)
                .OrderBy(x => x.OrderDetailId)
                .ToList();
            ViewBag.ChiTiet = chitietdonhang;
            return View(order);
        }

        // GET: Admin/AdminOrders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "TransactStatusId");
            return View();
        }

        // POST: Admin/AdminOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,OrderDate,ShipDate,TransactStatusId,Deleted,Paid,PaymentDate,PaymentId,Note,TotalMoney,Address,LocationId,District,Ward")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "TransactStatusId", order.TransactStatusId);
            return View(order);
        }

        // GET: Admin/AdminOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["TrangThai"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }

        // POST: Admin/AdminOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,OrderDate,ShipDate,TransactStatusId,Deleted,Paid,PaymentDate,PaymentId,Note,TotalMoney,Address,LocationId,District,Ward")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var donhang = await _context.Orders.AsNoTracking().Include(x => x.Customer).FirstOrDefaultAsync(x => x.OrderId == id);
                    if (donhang != null)
                    {
                        donhang.Paid = order.Paid;
                        donhang.Deleted = order.Deleted;
                        donhang.TransactStatusId = order.TransactStatusId;
                        if (donhang.Paid == true)
                        {
                            donhang.PaymentDate = DateTime.Now;
                        }
                        if (donhang.TransactStatusId == 5) donhang.Deleted = true;
                        if (donhang.TransactStatusId == 3) donhang.ShipDate = DateTime.Now;
                    }
                    _context.Update(donhang);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Cập nhật trạng thấy thành công thành công");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["TrangThai"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }

        // GET: Admin/AdminOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/AdminOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            order.Deleted = true;
            _context.Update(order);
            await _context.SaveChangesAsync();
            _notyfService.Success("Đơn hàng đã xóa");
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
        public IActionResult ExportToExcel()
        {
            List<Order> lsNews = new List<Order>();
            lsNews = _context.Orders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.TransactStatus)
                .Include(x => x.OrderDetails)
                .OrderByDescending(x => x.OrderDate).ToList();

            byte[] fileContents;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("ThongTinHoaDon");
            Sheet.Cells["A1"].Value = "Mã hóa đơn";
            Sheet.Cells["B1"].Value = "Họ và tên";
            Sheet.Cells["C1"].Value = "Ngày tạo";
            Sheet.Cells["D1"].Value = "Tổng hóa đơn";
            Sheet.Cells["E1"].Value = "Địa chỉ";
            Sheet.Cells["F1"].Value = "Số điện thoại";
            Sheet.Cells["G1"].Value = "Trạng thái đơn hàng";

            int row = 2;
            foreach (var item in lsNews)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.OrderId;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Customer.FullName;
                Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = "yyyy-mm-dd";
                Sheet.Cells[string.Format("C{0}", row)].Value = item.OrderDate;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.TotalMoney;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.Address;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Customer.Phone;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.TransactStatus.Status;
                row++;
            }


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            fileContents = Ep.GetAsByteArray();

            if (fileContents == null || fileContents.Length == 0)
            {
                return NotFound();
            }

            return File(
                fileContents: fileContents,
                contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileDownloadName: "DanhSachDonHang.xlsx"
            );
        }
    }
}
