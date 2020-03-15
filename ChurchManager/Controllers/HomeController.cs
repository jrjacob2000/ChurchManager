using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChurchManager.Controllers
{

    public class HomeController : Controller
    {

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult GetLineChart()
        {
            var result = new Entry[] {
                new Entry() { value = 200, month = "2019-01" },
                new Entry() { value = 200, month = "2019-01" },
                new Entry() { value = 200, month = "2019-01" },
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private class Entry
        {
            public int value { get; set; }
            public string month { get; set; }
        }
    }
}