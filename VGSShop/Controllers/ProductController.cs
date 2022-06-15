using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Dao;
using VGSShop.Models;
using VGSShop.ModelsView;

namespace VGSShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly VGSShopContext _context;
        public ProductController(VGSShopContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Route("shop.html", Name = "ShopProduct")]
        public IActionResult Index(int? page)
        {
            try
            {
                HomeViewVM model = new HomeViewVM();
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 12;
                var lsProduct = _context.Products
                    .AsNoTracking()
                    .OrderByDescending(x => x.DateCreated);
                PagedList<Product> models = new PagedList<Product>(lsProduct, pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            var product = (from Product in _context.Products
                             where Product.ProductName.StartsWith(prefix)
                             select new
                             {
                                 label = Product.ProductName,
                                 val = Product.CatId
                             }).ToList();

            return Json(product);
        }
        [Route("/{Alias}", Name = "ListProduct")]
        public IActionResult List(string Alias, int page = 1, string sort = "")
        {
            try
            {

                var pageSize = 10; //20
                var category = _context.Categories.AsNoTracking().SingleOrDefault(x=>x.Alias== Alias);
                var lsProduct = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == category.CatId);

                switch (sort)
                {
                    case "discount":
                        lsProduct = lsProduct.OrderByDescending(x => x.Discount.HasValue);
                        break;
                    case "price":
                        lsProduct = lsProduct.OrderBy(x => x.Price);
                        break;
                    default:
                        lsProduct = lsProduct.OrderByDescending(x => x.DateCreated);
                        break;
                }
                PagedList<Product> models = new PagedList<Product>(lsProduct, page, pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = category;
                //ViewBag.ListProduct = lsProduct;
                return View(models);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult Filtter(string sort = "")
        {
            var url = $"/Product/List?sort={sort}";
            if (sort == null)
            {
                url = $"/Product/List?";
            }
            return Json(new { status = "success", redirectUrl = url });
        }
        [Route("/{Alias}-{id}.html", Name = "ProductDetails")]
        public IActionResult Detail(int id)
        {
            try
            {
                var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {
                    return RedirectToAction("Index");
                }
                var lsProduct = _context.Products.AsNoTracking()
                    .Where(x => x.CatId == product.CatId && x.ProductId != id && x.Active == true)
                    .OrderByDescending(x => x.DateCreated)
                    .Take(4)
                    .ToList();
                ViewBag.SanPham = lsProduct;
                return View(product);
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public JsonResult ListName(string q)
        {
            var data = new ProductDao().ListName(q);
            return Json(new
            {
                data = data,
                status = true
            });
        }
        public ActionResult Search(string keyword, int page = 1, int pageSize = 1)
        {
            int totalRecord = 0;
            var model = new ProductDao().Search(keyword, ref totalRecord, page, pageSize);

            ViewBag.Total = totalRecord;
            ViewBag.Page = page;
            ViewBag.Keyword = keyword;
            int maxPage = 5;
            int totalPage = 0;

            totalPage = (int)Math.Ceiling((double)(totalRecord / pageSize));
            ViewBag.TotalPage = totalPage;
            ViewBag.MaxPage = maxPage;
            ViewBag.First = 1;
            ViewBag.Last = totalPage;
            ViewBag.Next = page + 1;
            ViewBag.Prev = page - 1;

            return View(model);
        }

    }
}
