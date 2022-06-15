using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using VGSShop.Extension;
using VGSShop.Helpers;
using VGSShop.Models;
using VGSShop.ModelsView;

namespace VGSShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly VGSShopContext _context;
        public INotyfService _notyfService { get; }

        public CheckoutController(VGSShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        //Cart
        public List<CartItem> Giohang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItem>>("Giohang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }
                return gh;
            }
        }

        //GET: checkout/Index
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            //Lấy Cart ra để xử lý
            var cart = HttpContext.Session.Get<List<CartItem>>("Giohang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuahangVM model = new MuahangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Adress;
            }
           
            ViewBag.Giohang = cart;
            return View(model);
        }

        [HttpPost]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(MuahangVM muahang)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("Giohang");
            var taikhoanID = HttpContext.Session.GetString("CustomerId");
            MuahangVM model = new MuahangVM();
            if (taikhoanID != null)
            {
                var khachhang = _context.Customers.AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                model.CustomerId = khachhang.CustomerId;
                model.FullName = khachhang.FullName;
                model.Email = khachhang.Email;
                model.Phone = khachhang.Phone;
                model.Address = khachhang.Adress;
                khachhang.Adress = muahang.Address;
                _context.Update(khachhang);
                _context.SaveChanges();
            }
            try
            {
                if (ModelState.IsValid)
                {
                    //Khởi tạo đơn hàng
                    Order donhang = new Order();
                    donhang.CustomerId = model.CustomerId;
                    donhang.Address = model.Address;
                    donhang.OrderDate = DateTime.Now;
                    donhang.TransactStatusId = 1; // Đơn hàng mới
                    donhang.Deleted = false;
                    donhang.Paid = false;
                    donhang.Note = Utilities.StripHTML(model.Note);
                    donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney));
                    _context.Add(donhang);
                    _context.SaveChanges();

                    //Tạo danh sách đơn hàng
                    foreach (var item in cart)
                    {
                        OrderDetail orderDetails = new OrderDetail();
                        orderDetails.OrderId = donhang.OrderId;
                        orderDetails.ProductId = item.product.ProductId;
                        orderDetails.Quantity = item.amount;
                        orderDetails.TotalMoney = donhang.TotalMoney;
                        orderDetails.Price = item.product.Price;
                        orderDetails.CreateDate = DateTime.Now;
                        _context.Add(orderDetails);
                    }
                    _context.SaveChanges();

                    // Cập nhật lại số lượng hàng trong kho

                    foreach (var item in cart)
                    {
                        var found = _context.Products.AsNoTracking().SingleOrDefault(x => x.ProductId == item.product.ProductId);
                        if (found != null)
                        {
                            found.UnitslnStock = found.UnitslnStock - item.amount;
                            _context.Update(found);
                            _context.SaveChanges();
                        }
                    }



                    //Xóa giỏ hàng
                    HttpContext.Session.Remove("Giohang");
                    //Thông báo
                    _notyfService.Success("Đặt hàng thành công");

                    return RedirectToAction("Dashboard", "Accounts");
                }
            }
            catch (Exception)
            {
                ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
                ViewBag.Giohang = cart;
                return View(model);
            }
            ViewData["lsTinhThanh"] = new SelectList(_context.Locations.Where(x => x.Levels == 1).OrderBy(x => x.Type).ToList(), "LocationId", "Name");
            ViewBag.Giohang = cart;
            return View(model);
        }

        [Route("dat-hang-thanh-cong.html", Name = "Success")]
        public IActionResult Success()
        {
            try
            {
                var taikhoanID = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong.html" });
                }
                var khachhang = _context.Customers.AsNoTracking()
                    .SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
                var donhang = _context.Orders.Where(x => x.CustomerId == Convert.ToInt32(taikhoanID))
                    .OrderByDescending(x => x.OrderDate);
                MuaHangSuccessVM successVM = new MuaHangSuccessVM();
                successVM.FullName = khachhang.FullName;
                successVM.Phone = khachhang.Phone;
                successVM.Address = khachhang.Adress;
                return View(successVM);
            }
            catch
            {
                return View();
            }
        }

    }
}