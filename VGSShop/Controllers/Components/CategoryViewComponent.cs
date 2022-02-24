using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Enums;
using VGSShop.Models;

namespace VGSShop.Controllers.Components
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly VGSShopContext _context;
        private IMemoryCache _memoryCache;
        public CategoryViewComponent(VGSShopContext context, IMemoryCache memory)
        {
            _context = context;
            _memoryCache = memory;
        }
        public IViewComponentResult Invoke()
        {
            var _lsDanhMuc = _memoryCache.GetOrCreate(CacheKeys.Categories, entry =>
            {
                entry.SlidingExpiration = TimeSpan.MaxValue;
                return GetlsCategory();
            });
            return View(_lsDanhMuc);
        }
        public List<Category> GetlsCategory()
        {
            List<Category> lsCate = new List<Category>();
            lsCate = _context.Categories
                .Where(x => x.Published == true)
                .OrderBy(x => x.Ordering)
                .ToList();
            return lsCate;
        }
    }
}
