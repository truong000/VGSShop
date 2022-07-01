using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using VGSShop.Models;

namespace VGSShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize()]
    public class AdminHomeController : Controller
    {
        private readonly VGSShopContext _context;
        public INotyfService _notyfService { get; }
        public AdminHomeController(VGSShopContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        public IActionResult Index()
        {
            ViewBag.ThongKeTongDoanhThu = ThongKeTongDoanhThu();
            ViewBag.ThongKeDonDatHang = ThongKeDonDatHang();
            ViewBag.ThongKeKhachHang = ThongKeKhachHang();
            return View();
        }
        [HttpPost]
        public string ThongKe(DateTime datestart, DateTime datestop)
        {
            var data = _context.Orders
                .Where(x => x.OrderDate >= datestart && x.OrderDate <= datestop && x.PaymentDate.HasValue)
                .Sum(x => x.TotalMoney);
            string model = data.ToString("#,##0 VNĐ");
            return model;
        }
        public decimal ThongKeTongDoanhThu()
        {
            decimal doanhthu = _context.Orders.Where(x => x.PaymentDate.HasValue).Sum(x => x.TotalMoney);
            return doanhthu;
        }
        public int ThongKeDonDatHang()
        {
            int donhang = _context.Orders.Count();
            return donhang;
        }
        public int ThongKeKhachHang()
        {
            int khachhang = _context.Customers.Count();
            return khachhang;
        }

        [HttpGet]
        public IActionResult GetReportByMonth(int month)
        {
            var param = new SqlParameter("@Month", month);
            var connStr = _context.Database.GetDbConnection().ConnectionString;
            using var conn = new SqlConnection(connStr);
            conn.Open();
            var sqlCommand = new SqlCommand();
            sqlCommand.Connection = conn;
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "Sp_GetReportProductByMonth";
            sqlCommand.Parameters.Add("@Month", System.Data.SqlDbType.Int);
            sqlCommand.Parameters["@Month"].Value = month;

            var reader = sqlCommand.ExecuteReader();
            var op = new List<Sp_GetReportProductByMonth>();
            while(reader.Read())
            {
                op.Add(new Sp_GetReportProductByMonth()
                {
                    Month = Convert.ToString(reader["month"]),
                    ProductName = Convert.ToString(reader["ProductName"]),
                    total = Convert.ToString(reader["total"])
                });
            }

            return Ok(op);
        }
    }


}
