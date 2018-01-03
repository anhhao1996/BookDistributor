using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PHSach.Models;

namespace PHSach.Controllers
{
    public class Inventory_BookController : Controller
    {
        private PhatHanhSachEntities db = new PhatHanhSachEntities();

        public ActionResult Index()
        {
            var books = from f in (from a in (from b in db.Inventory_Book.Include(b => b.Book) group b by b.Book_id into gr select new { book_id = gr.Key, updated_date = gr.Max(x => x.UpdatedDate) })
                                   from d in db.Inventory_Book
                                   orderby d.UpdatedDate descending
                                   where a.book_id == d.Book_id && a.updated_date == d.UpdatedDate
                                   select d).Include(f => f.Book)
                        select f;

            ViewBag.listbook = books.ToList();
            return View();
        }
        // Bảo - 20/10/2017
        // Bảo 20/10/2017
        [HttpPost]
        public JsonResult HistoryInventory(string masach, string choosedate)
        {
            try
            {
                int id = int.Parse(masach);
                DateTime date = DateTime.Parse(choosedate);
                date = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                var inventory_Book = db.Inventory_Book.Include(c => c.Book)
                    .Where(c => c.Book_id == id && c.UpdatedDate <= date)
                    .OrderByDescending(c => c.UpdatedDate)
                    .Select(c => c).FirstOrDefault();
                if (inventory_Book != null)
                {
                    return Json(new { id = inventory_Book.Book_id, bookname = inventory_Book.Book.Book_name, quantity = inventory_Book.Quantity, updatedate = (inventory_Book.UpdatedDate)?.ToString("dd / MM / yyyy HH:mm:ss") });
                }
            }
            catch
            {
                return Json(new { });
            }
            return Json(new { });
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
