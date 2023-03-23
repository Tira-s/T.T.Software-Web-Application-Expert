using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly sqltestContext _db;
        private IConfiguration Configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, sqltestContext db)
        {
            _logger = logger;
            Configuration = configuration;
            _db = db;
        }

        public IActionResult Index()
        {
            List<MockData> data = this._db.MockData.OrderBy(x => x.RunningNumber).ToList();
            ViewBag.alertmsg = TempData["alertmsg"] != null ? TempData["alertmsg"].ToString() : null;
            return View(data);
        }

        [Route("Home/GetMockData")]
        public JsonResult GetMockData()
        {
            return Json(new { data = this._db.MockData });
        }

        [Route("Home/GetMockData/{id}")]
        public JsonResult GetMockData(int id)
        {
            return Json(new { data = this._db.MockData.Where(x => x.RunningNumber == id) });
        }

        [HttpPost]
        [Route("/Home/CreateNew")]
        public IActionResult CreateNew()
        {
            var date  = new DateTime();
            var new_hn = DateTime.UtcNow.ToString("fffffffd-f");
            var data = _db.MockData.Add(new MockData
            {
                Hn = new_hn,
                FirstName = Request.Form["fName"],
                LastName  = Request.Form["lName"],
                Phone = Request.Form["Phone"],
                Email = Request.Form["Email"],
            });
            TempData["alertmsg"] = "เพิ่มข้อมูล " + new_hn + " เรียบร้อยเเล้ว";

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/Home/Save")]
        public IActionResult Save()
        {
            var Hn = Request.Form["Hn"].ToString();
            var fName = Request.Form["fName"];
            var lName = Request.Form["lName"];
            var Phone = Request.Form["Phone"];
            var Email = Request.Form["Email"];

            var update = _db.MockData.Where(x => x.Hn == Hn).FirstOrDefault();
            if (update != null)
            {
                update.FirstName = fName;
                update.LastName = lName;
                update.Phone = Phone;
                update.Email = Email;
                TempData["alertmsg"] = "อัพเดตข้อมูล " + Hn + " เรียบร้อยเเล้ว";
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Route("Home/Delete/{hn}")]
        public IActionResult Delete(string hn)
        {
            var del =  _db.MockData.Where(x => x.Hn == hn).FirstOrDefault();
            if (del != null)
            {
                 _db.MockData.Remove(del);
                TempData["alertmsg"] = "ลบข้อมูล " + hn + " เรียบร้อยเเล้ว";
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}