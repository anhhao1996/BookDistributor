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
    public class Debt_AgencyController : Controller
    {
        private PhatHanhSachEntities db = new PhatHanhSachEntities();

        // GET: Debt_Agency
        public ActionResult Index()
        {
            List<Debt_Agency> list = null;
            var debt_agency = (from c in db.Debt_Agency group c by c.Agency_id into grp select new { Agency_id = grp.Key, updatedate = grp.Max(x => x.update_date) }).ToArray();

            if (debt_agency != null)
            {
                list = new List<Debt_Agency>();
                foreach (var item in debt_agency)
                {
                    Debt_Agency agency = new Debt_Agency();
                    agency = (from a in db.Debt_Agency where a.Agency_id == item.Agency_id && a.update_date == item.updatedate select a).Include(a => a.Agency).FirstOrDefault();
                    list.Add(agency);
                }
            }
            return View(list);
        }

        public bool IsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }
        //Bảo - 10/20/2017
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
            var debt_agency = (from c in (from a in db.Debt_Agency
                                          where (a.Agency_id == id || a.Agency.Agency_name.ToLower().Contains(keyword.ToLower())) && a.update_date <= newdate
                                          group a by a.Agency_id into gr
                                          select new { id = gr.Key, update = gr.Max(x => x.update_date) })
                               from d in db.Debt_Agency
                               where d.Agency_id == c.id && d.update_date == c.update
                               select new { id = d.Agency_id, name = d.Agency.Agency_name, debt = d.debt, updatedate = d.update_date }).ToList();

            if (debt_agency.Count > 0)
            {
                var list = JsonConvert.SerializeObject(debt_agency,
                  Formatting.None,
                  new JsonSerializerSettings()
                  {
                      ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                  });
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            return Json(new {}, JsonRequestBehavior.AllowGet);
        }

        //Bảo - 10/20/2017
        [HttpGet]
        public JsonResult all()
        {
            List<Debt_Agency> list = null;
            var debt_Agency = (from c in db.Debt_Agency group c by c.Agency_id into grp select new { Agency_id = grp.Key, updatedate = grp.Max(x => x.update_date) }).ToArray();
            if (debt_Agency != null)
            {
                list = new List<Debt_Agency>();
                foreach (var item in debt_Agency)
                {
                    var result = (from a in db.Debt_Agency where a.Agency_id == item.Agency_id && a.update_date == item.updatedate select a).Include(a => a.Agency).FirstOrDefault() as Debt_Agency;
                    list.Add(result);
                }
            }
            var newlist = list.Select(a => new { id = a.Agency_id, name = a.Agency.Agency_name, debt = a.debt, updatedate = a.update_date }).ToList();
            var json = JsonConvert.SerializeObject(newlist, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore });

            return Json(json, JsonRequestBehavior.AllowGet);
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
