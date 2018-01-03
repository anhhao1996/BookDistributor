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
    public class Report_NXBController : Controller
    {
        private PhatHanhSachEntities db = new PhatHanhSachEntities();

        // GET: Report_NXB
        public ActionResult Index()
        {
            var report_NXB = db.Report_NXB.Include(r => r.NXB);
            return View(report_NXB.ToList());
        }

       
        public ActionResult Edit(int? id)
        {
            Report_NXB report = db.Report_NXB.Find(id);
            report.status = 1;
            db.Entry(report).State = EntityState.Modified;
            //test gap
            Debt_NXB debt = new Debt_NXB();
            Debt_NXB debt1 = db.Debt_NXB.OrderByDescending(m => m.id).FirstOrDefault(m => m.NXB_id == (int)report.NXB_id);
            debt.update_date = DateTime.Now;
            if (debt1 != null)
            {
                debt.NXB_id = (int)report.NXB_id;
                debt.debt = (double)(debt1.debt - report.total);
                debt.repay = Double.Parse(report.total.ToString());
            }
            db.Debt_NXB.Add(debt);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
