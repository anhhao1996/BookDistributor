using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PHSach.Models;
using Newtonsoft.Json;

namespace PHSach.Controllers
{
    public class Debt_NXBController : Controller
    {
        private PhatHanhSachEntities db = new PhatHanhSachEntities();

        // GET: Debt_NXB
        public ActionResult index()
        {
            List<Debt_NXB> list = null;
            var debt_NXB = (from c in db.Debt_NXB group c by c.NXB into grp select new { NXB_id = grp.Key.NXB_id, updatedate = grp.Max(x => x.update_date) }).ToArray();

            if (debt_NXB != null)
            {
                list = new List<Debt_NXB>();
                foreach (var item in debt_NXB)
                {
                    var result = (from a in db.Debt_NXB where a.NXB_id == item.NXB_id && a.update_date == item.updatedate select a).Include(a => a.NXB).FirstOrDefault() as Debt_NXB;
                    list.Add(result);
                }
            }
            return View(list);
        }
        [HttpGet]
        public JsonResult all()
        {
            List<Debt_NXB> list = null;
            var debt_NXB = (from c in db.Debt_NXB group c by c.NXB into grp select new { NXB_id = grp.Key.NXB_id, updatedate = grp.Max(x => x.update_date) }).ToArray();
            if (debt_NXB != null)
            {
                list = new List<Debt_NXB>();
                foreach (var item in debt_NXB)
                {
                    var result = (from a in db.Debt_NXB where a.NXB_id == item.NXB_id && a.update_date == item.updatedate select a).Include(a => a.NXB).FirstOrDefault() as Debt_NXB;
                    list.Add(result);
                }
            }
            var newlist = list.Select(a => new { id = a.NXB_id, name = a.NXB.NXB_name, debt = a.debt, updatedate = a.update_date }).ToList();
            var json = JsonConvert.SerializeObject(newlist, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

            return Json(json, JsonRequestBehavior.AllowGet);
        }
        // bao - 21/10/2017
        public bool IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }
        [HttpGet]
        public JsonResult debtcalculate(string keyword, string date)
        {
            int id = 0;
            if (IsNumber(keyword))
            {
                id = int.Parse(keyword);
            }
            DateTime newdate = DateTime.Parse(date);
            newdate = new DateTime(newdate.Year, newdate.Month, newdate.Day, 23, 59, 59);
            var debt_nxb = (from c in (from a in db.Debt_NXB
                                       where (a.NXB_id == id || a.NXB.NXB_name.ToLower().Contains(keyword.ToLower())) && a.update_date <= newdate
                                       group a by a.NXB_id into gr
                                       select new { id = gr.Key, update = gr.Max(x => x.update_date) })
                            from d in db.Debt_NXB
                            where d.NXB_id == c.id && d.update_date == c.update
                            select new { id = d.NXB_id, name = d.NXB.NXB_name, debt = d.debt, updatedate = d.update_date }).ToList();
            if (debt_nxb.Count > 0)
            {
                var list = JsonConvert.SerializeObject(debt_nxb,
                  Formatting.None,
                  new JsonSerializerSettings()
                  {
                      ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                  });
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        // end
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
