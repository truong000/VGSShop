using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Helpers;
using VGSShop.Models;

namespace VGSShop.Controllers
{
    public class BlogController : Controller
    {
        private readonly VGSShopContext _context;
        public INotyfService _notyfService { get; }
        public BlogController(VGSShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        // GET: Blog/Index
        [Route("blogs.html", Name = "Blog")]
        public async Task<IActionResult> Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 10; //20
            var lsNews = _context.News
                .AsNoTracking()
                .OrderByDescending(x => x.PostId);
            PagedList<News> models = new PagedList<News>(lsNews, pageNumber, pageSize);
            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }
        // GET: Blog/Details
        [Route("/tin-tuc/{Alias}-{id}.html", Name = "NewsDetails")]
        public IActionResult Details(int id)
        {
            var news = _context.News.AsNoTracking().SingleOrDefault(x => x.PostId == id);
            if (news == null)
            {
                return RedirectToAction("Index");
            }
            var lsNews = _context.News
                    .AsNoTracking()
                    .Where(x => x.Published == true && x.PostId != id )
                    .Take(3)
                    .OrderByDescending(x => x.CreatedDate)                   
                    .ToList();
            ViewBag.lsNews = lsNews;
            return View(news);
        }
    }
}
