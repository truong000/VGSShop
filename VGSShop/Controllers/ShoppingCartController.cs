using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Extension;
using VGSShop.Models;
using VGSShop.ModelsView;

namespace VGSShop.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly VGSShopContext _context;
        public INotyfService _notyfService { get; }
        public ShoppingCartController(VGSShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
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
        //Thêm mới sản phẩm vào Cart
        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            List<CartItem> gioHang = Giohang;
            try
            {
                //Thêm sản phẩm vào Cart
                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
                //Cart -- đã có sản sản phẩm ở trong
                if (item != null)
                {
                    item.amount = item.amount + amount.Value;
                    //Lưu lại SS
                    HttpContext.Session.Set<List<CartItem>>("Giohang", gioHang);
                }
                else
                {
                    Product hanghoa = _context.Products.AsNoTracking().SingleOrDefault(p => p.ProductId == productID);
                    if (amount > hanghoa.UnitslnStock) amount = hanghoa.UnitslnStock.Value;
                    item = new CartItem
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = hanghoa

                    };
                    gioHang.Add(item);// Thêm sp vào cart
                }
                HttpContext.Session.Set<List<CartItem>>("Giohang", gioHang);
                _notyfService.Success("Sản phẩm đã được thêm");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }

        }
        
        //Cập nhật Cart
        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            //Lấy giỏ hàng ra để xử lý
            var cart = HttpContext.Session.Get<List<CartItem>>("Giohang");
            try
            {
                if(cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                    if (item != null && amount.HasValue) // Đã có sp => cập nhật lại số lượng
                    {
                        item.amount = amount.Value;
                    }
                    //Lưu lại Session
                    HttpContext.Session.Set<List<CartItem>>("Giohang", cart);
                    _notyfService.Success("Sản phẩm đã được thêm");
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }

        }
        //Xóa sp ra khỏi Cart
        [HttpPost]
        [Route("api/cart/remove")]
        public ActionResult Remove(int productID)
        {
            try
            {
                List<CartItem> gioHang = Giohang;
                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    gioHang.Remove(item);
                    //_notyfService.Success("Sản phẩm đã bỏ ra giỏ hàng");
                }
                //Lưu lại session
                HttpContext.Session.Set<List<CartItem>>("Giohang", gioHang);
                _notyfService.Success("Sản phẩm đã được loại ra");
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            var lsGioHang = Giohang;
            return View(Giohang);
        }
    }
}
