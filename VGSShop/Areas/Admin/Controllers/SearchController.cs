using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Models;

namespace VGSShop.Areas.Admin.Controllers
{
    public class SearchController : Controller
    {
        private readonly VGSShopContext _context;

        public SearchController(VGSShopContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult FindNews(string keyword)
        {
            List<Product> ls = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListNewsSearchPartial", null);
            }
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.ProductName.Contains(keyword))
                .OrderByDescending(x => x.ProductName)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListNewsSearchPartial", null);
            }
            else
            {
                return PartialView("ListNewsSearchPartial", ls);
            }
        }
    }
}
