using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Helpers;
using VGSShop.Models;

namespace VGSShop.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class SearchController : Controller
    {
        private readonly VGSShopContext _context;

        public SearchController(VGSShopContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            List<Product> ls = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListProductSearchPartialView", null);
            }
            ls = _context.Products
                .AsNoTracking()
                .Include(a => a.Cat)
                .Where(x => x.ProductName.Contains(keyword))
                .OrderByDescending(x => x.ProductName)
                .ToList();
            if (ls == null)
            {
                return PartialView("ListProductSearchPartialView", null);
            }
            else
            {
                return PartialView("ListProductSearchPartialView", ls);
            }
        }
        [HttpPost]
        public IActionResult FindOrder(string keyword)
        {
            List<Order> ls = new List<Order>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListOrderSearchPartialView", null);
            }
            if (Utilities.IsInteger(keyword))
            {
                int id = int.Parse(keyword.Trim());
                ls = _context.Orders
                    .AsNoTracking()
                    .Include(a => a.Customer)
                    .Include(s => s.TransactStatus)
                    .Where(x => x.Customer.Phone.Contains(keyword))
                    .OrderByDescending(x => x.OrderDate)
                    .ToList();
            }
            else
            {
                ls = _context.Orders
                                    .AsNoTracking()
                                    .Include(a => a.Customer)
                                    .Include(s => s.TransactStatus)
                                    .Where(x => x.Customer.FullName.Contains(keyword) || x.Customer.Email.Contains(keyword))
                                    .OrderByDescending(x => x.OrderDate)
                                    .ToList();
            }
    
            if (ls == null)
            {
                return PartialView("ListOrderSearchPartialView", null);
            }
            else
            {
                return PartialView("ListOrderSearchPartialView", ls);
            }
        }
        [HttpPost]
        public IActionResult FindCustomer(string keyword)
        {
            List<Customer> ls = new List<Customer>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListCustomerSearchPartialView", null);
            }
            if (Utilities.IsInteger(keyword))
            {
                ls = _context.Customers
                    .AsNoTracking()
                    .Where(x => x.Phone.Contains(keyword))
                    .OrderByDescending(x => x.CustomerId)
                    .ToList();
            }
            else
            {
                ls = _context.Customers
                                    .AsNoTracking()
                                    .Where(x => x.FullName.Contains(keyword) || x.Email.Contains(keyword))
                                    .OrderByDescending(x => x.CustomerId)
                                    .ToList();
            }

            if (ls == null)
            {
                return PartialView("ListCustomerSearchPartialView", null);
            }
            else
            {
                return PartialView("ListCustomerSearchPartialView", ls);
            }
        }
    }
}
