using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Dao;
using VGSShop.Models;
using VGSShop.ModelsView;

namespace VGSShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly VGSShopContext _context;
        public HomeController(ILogger<HomeController> logger, VGSShopContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewVM model = new HomeViewVM();

            var lsProduct = _context.Products.AsNoTracking()
                .Where(x => x.Active == true && x.HomeFlag == true)
                .OrderByDescending(x => x.DateCreated)
                .Take(8)
                .ToList();
            var lsProductHot = _context.Products.AsNoTracking()
                .Where(x => x.Active == true && x.HomeFlag == true && x.BestSellers)
                .OrderByDescending(x => x.DateCreated)
                .Take(8)
                .ToList();
            List<ProductHomeVM> lsProductsViews = new List<ProductHomeVM>();

            var lsCats = _context.Categories
                .AsNoTracking()
                .Where(x => x.Published == true)
                .OrderByDescending(x => x.Ordering)
                .ToList();

            foreach (var item in lsCats)
            {
                ProductHomeVM productHome = new ProductHomeVM();
                productHome.category = item;
                productHome.lsProduct = lsProduct
                    .Where(x => x.CatId == item.CatId)
                    .ToList();
                lsProductsViews.Add(productHome);
            }
            var TinTuc = _context.News
                .AsNoTracking()
                .Where(x => x.Published == true && x.IsNewfeed == true)
                .OrderByDescending(x => x.CreatedDate)
                .Take(3)
                .ToList();
            var Banner = _context.Banners
                .AsNoTracking()
                .Where(x => x.Status == true)
                .OrderByDescending(x => x.DisplayOrder)
                .ToList();
            model.Banners = Banner;
            model.Products = lsProductsViews;
            model.News = TinTuc;
            ViewBag.AllProduct = lsProduct;
            ViewBag.AllProductHot = lsProductHot;
            return View(model);
        }

        [Route("gioi-thieu.html", Name = "About")]
        public IActionResult About()
        {
            return View();
        }
        //[Route("lien-he.html", Name = "Contact")]
        //public IActionResult Contact()
        //{
        //    return View();
        //}
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
