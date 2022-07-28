using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Dao;
using VGSShop.Models;

namespace VGSShop.Controllers
{
    public class ContacController : Controller
    {
        private readonly VGSShopContext _context;
        public ContacController(VGSShopContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Send(string Name, string Mobile, string Address, string Email, string Content)
        {
            var feedback = new Feedback();
            feedback.Name = Name;
            feedback.Email = Email;
            feedback.CreatedDate = DateTime.Now;
            feedback.Phone = Mobile;
            feedback.Content = Content;
            feedback.Address = Address;

            var id = new ContenDao().InsertFeedBack(feedback);
            if (id > 0)
            {
                return Json(new
                {
                    status = true
                });
                //send mail
            }

            else
                return Json(new
                {
                    status = false
                });

        }

    }
}
