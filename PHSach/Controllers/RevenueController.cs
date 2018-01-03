using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHSach.Models;

namespace PHSach.Controllers
{
    public class RevenueController : Controller
    {
        private PhatHanhSachEntities db = new PhatHanhSachEntities();
        // GET: Revenue
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult calculateByday(string startdate, string finishdate)
        {

            DateTime datestart = DateTime.Parse(startdate);
            datestart = new DateTime(datestart.Year, datestart.Month, datestart.Day, 0, 0,0);
            DateTime datefinish = DateTime.Parse(finishdate);
            datefinish = new DateTime(datefinish.Year, datefinish.Month, datefinish.Day, 23, 59, 59);


            var find_id = from D in (from A in db.Report_Agency
                                     where A.CreatedDate >= datestart && A.CreatedDate <= datefinish
                                     from B in db.Detail_Report_Agency
                                     where B.Report_id == A.Report_id
                                     group B by B.Book_id into grp
                                     select new { Book_id = grp.Key, quantity = grp.Sum(x => x.quantity) })
                          from C in db.Books
                          where C.Book_id == D.Book_id
                          select new { C.Book_id, C.Cost_Export, C.Cost_Import, D.quantity };
            double? total = 0;
            foreach (var item in find_id)
            {
                total += item.Cost_Export  * item.quantity;
            }

            return Json(total, JsonRequestBehavior.AllowGet);
        }
    }
}