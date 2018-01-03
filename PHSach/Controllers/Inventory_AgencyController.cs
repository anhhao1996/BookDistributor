using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PHSach.Models;
using Microsoft.Ajax.Utilities;

namespace PHSach.Controllers
{
    public class Inventory_AgencyController : Controller
    {
        private PhatHanhSachEntities db = new PhatHanhSachEntities();

        public ActionResult Index()
        {
            var Agency_books = from f in (from a in (from b in db.Inventory_Agency.Include(b => b.Book) group b by new { b.Agency_id , b.Book_id } into gr select new { Agency_id = gr.Key.Agency_id,book_id =gr.Key.Book_id , updated_date = gr.Max(x => x.UpdatedDate) })
                                   from d in db.Inventory_Agency
                                   orderby d.Agency_id ascending
                                   where a.Agency_id == d.Agency_id && a.book_id == d.Book_id && a.updated_date == d.UpdatedDate
                                   select d)
                               select f;
            var agency = db.Agencies.Select(x => new { x.Agency_id, x.Agency_name }).ToDictionary(x => x.Agency_id, x => x.Agency_name);
            ViewBag.listbook = Agency_books.ToList();
            ViewBag.agency = agency;
            return View();
        }
        // Bảo - 20/10/2017
        // Bảo 20/10/2017
        [HttpPost]
        public JsonResult HistoryAgency(string madaily,string masach, string choosedate)
        {
            try
            {
                int id = int.Parse(masach);
                int iddaily = int.Parse(madaily);
                DateTime date = DateTime.Parse(choosedate);
                date = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                var inventory_book = db.Inventory_Agency
                    .Where(c =>c.Agency_id==iddaily && c.Book_id == id && c.UpdatedDate <= date)
                    .OrderByDescending(c => c.UpdatedDate)
                    .Select(c => c).FirstOrDefault();
                if (inventory_book != null)
                {
                    return Json(new { iddaily= inventory_book.Agency_id,id = inventory_book.Book_id, bookname = inventory_book.Book.Book_name, quantity = inventory_book.deliver_quantity- inventory_book.repay_quantity, updatedate = (inventory_book.UpdatedDate)?.ToString("dd / MM / yyyy HH:mm:ss") });
                }
            }
            catch
            {
                return Json(new { });
            }
            return Json(new { });
        }
        [HttpGet]
        public JsonResult listbookbyagency(string id)
        {
            int iddaily = int.Parse(id);
            var listbook = (from a in db.Inventory_Agency.Include(a => a.Book) where a.Agency_id == iddaily select new {id= a.Book_id, name=a.Book.Book_name }).Distinct().ToList();
            //Session["listbookid"] = listbook;
            return Json(listbook, JsonRequestBehavior.AllowGet);
            //return listbook;
        }
        // Bảo 20/10/2017
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
