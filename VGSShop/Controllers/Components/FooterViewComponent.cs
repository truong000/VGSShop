using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using VGSShop.Enums;
using VGSShop.Models;

namespace VGSShop.Controllers.Components
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly VGSShopContext _context;
        private IMemoryCache _memoryCache;
        public FooterViewComponent(VGSShopContext context, IMemoryCache memory)
        {
            _context = context;
            _memoryCache = memory;
        }
        public IViewComponentResult Invoke()
        {
            var Footer = _memoryCache.GetOrCreate(CacheKeys.Footer, entry =>
            {
                entry.SlidingExpiration = TimeSpan.MaxValue;
                return GetlsCategory();
            });
            return View(Footer);
        }
        public List<Contact> GetlsCategory()
        {
            List<Contact> lsCate = new List<Contact>();
            lsCate = _context.Contacts
                .ToList();
            return lsCate;
        }
    }
}
