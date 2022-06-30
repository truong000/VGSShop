using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using VGSShop.Models;
using VGSShop.ModelsView;

namespace VGSShop.Controllers
{
    public class FeedBackController : Controller
    {
        private readonly VGSShopContext _context;
        public INotyfService _notyfService { get; }

        public FeedBackController(VGSShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [Route("lien-he.html", Name = "FeedBack")]
        public IActionResult FeedBack()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Send(string name, string mobile, string address, string email, string content)
        {
            var feedback = new Feedback();
            feedback.Name = name;
            feedback.Email = email;
            feedback.CreatedDate = DateTime.Now;
            feedback.Phone = mobile;
            feedback.Content = content;
            feedback.Address = address;
            var id = new FeedBack_Dao().InsertFeedBack(feedback);
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