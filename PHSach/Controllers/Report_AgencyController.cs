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
    public class Report_AgencyController : Controller
    {
        private PhatHanhSachEntities db = new PhatHanhSachEntities();

        // GET: Report_Agency
        public ActionResult Index()
        {
            var report_Agency = db.Report_Agency.Include(r => r.Agency);
                return View(report_Agency.ToList());
        }

        public ActionResult Deletectpx(int bookid)
        {
            var list = (List<Detail_Report_Agency>)Session["ctphieubaocao"];
            list.RemoveAll(p => p.Book_id == bookid);
            return RedirectToAction("ThemChiTietPhieuBaoCao");
        }
        // GET: Report_Agency/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report_Agency report_Agency = db.Report_Agency.Find(id);
            var a = (from b in db.Detail_Report_Agency
                     where b.Report_id == id
                     select b).ToList();
            ViewBag.ctpbc = a;
            if (report_Agency == null)
            {
                return HttpNotFound();
            }
            return View(report_Agency);
        }

        public ActionResult Deletectpbc(int bookid)
        {
            var list = (List<Detail_Report_Agency>)Session["ctphieubaocao"];
            list.RemoveAll(p => p.Book_id == bookid);
            return RedirectToAction("ThemChiTietPhieuBaoCao");
        }

        // GET: Report_Agency/Create
        public ActionResult Create()
        {
            ViewBag.Agency_id = null;
            Report_Agency report_Agency = new Report_Agency();
            report_Agency = (Report_Agency)Session["PhieuBaoCao"];
            /*var lstAgencyid = (from c in db.Debt_Agency
                                where ! (from d in db.Debt_Agency
                                         where d.debt <= 0
                                         select d.Agency_id).Contains(c.Agency_id)
                                select c.Agency_id).Distinct().ToList();*/
            var lstAgencydb = db.Agencies;
            List<Debt_Agency> lstDebtAgency = new List<Debt_Agency>();
            foreach (Agency agen in lstAgencydb)
            {
                Debt_Agency debtdaily = db.Debt_Agency.OrderByDescending(m => m.id).FirstOrDefault(m => (m.Agency_id == agen.Agency_id));
                lstDebtAgency.Add(debtdaily);
            }

            List<Agency> lstAgencyUI = new List<Agency>();
            foreach (Debt_Agency debtdailyitem in lstDebtAgency)
            {
                if (debtdailyitem.debt > 0)
                {
                    Agency agenSelect = db.Agencies.SingleOrDefault(n => n.Agency_id == debtdailyitem.Agency_id);
                    lstAgencyUI.Add(agenSelect);
                }
            }
            ViewBag.Agency_id = new SelectList(lstAgencyUI, "Agency_id", "Agency_name");
            return View();
        }

        // POST: Report_Agency/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Report_id,Agency_id,Total_Quantity,CreatedDate")] Report_Agency report_Agency)
        {
            if (ModelState.IsValid)
            {
                report_Agency.CreatedDate = DateTime.Now;
               // var tempreportagency = report_Agency;
                Session["ctphieubaocao"] = new List<Detail_Report_Agency>();
                Session["PhieuBaoCao"] = report_Agency;
                return RedirectToAction("ThemChiTietPhieuBaoCao");
            }

            ViewBag.Agency_id = new SelectList(db.Agencies, "Agency_id", "Agency_name", report_Agency.Agency_id);
            return View(report_Agency);
        }


        [HttpGet]
        public ActionResult ThemChiTietPhieuBaoCao()
        {
            Report_Agency report_Agency = new Report_Agency();
            report_Agency = (Report_Agency)Session["PhieuBaoCao"];
            var lstBookid = (from b in db.Inventory_Agency
                                   where b.Agency_id == report_Agency.Agency_id
                                   select b.Book_id).Distinct().ToList();
            List<Book> lstBooks = new List<Book>();
            foreach(var item in lstBookid)
            {
                Book book = db.Books.SingleOrDefault(n => n.Book_id == item);
                lstBooks.Add(book);
            }
            ViewBag.sach = new SelectList(lstBooks, "Book_id", "Book_name");
            //var phieunhap = (Bill_Import)TempData["PhieuNhap"];
            return View();
        }

        [HttpPost]
        public ActionResult ThemChiTietPhieuBaoCao(FormCollection chitiet)
        {
            //
            Report_Agency report_Agency = new Report_Agency();
            report_Agency = (Report_Agency)Session["PhieuBaoCao"];
            var lstBookid = (from b in db.Inventory_Agency
                             where b.Agency_id == report_Agency.Agency_id
                             select b.Book_id).Distinct().ToList();
            List<Book> lstBooks = new List<Book>();
            foreach (var item in lstBookid)
            {
                Book book = db.Books.SingleOrDefault(n => n.Book_id == item);
                lstBooks.Add(book);
            }
            ViewBag.sach = new SelectList(lstBooks, "Book_id", "Book_name");
            //
            ViewBag.loi = null;
            if (Request.Form["add"] != null)
            {
                if (ModelState.IsValid)
                {
                    bool check = true;
                    foreach (var ctpbc in (List<Detail_Report_Agency>)Session["ctphieubaocao"])
                    {
                        if (ctpbc.Book_id == Int32.Parse(chitiet["sach"].ToString()))
                        {
                            check = false;
                            ViewBag.loi = "Đã tồn tại sách trong phiếu";
                            break;
                        }
                    }
                    if (check)
                    {
                        if (chitiet["sach"] == null)
                        {
                            ViewBag.loi = "Không tồn tại sách để báo cáo";
                            goto baoloi;
                        }
                        var sach = db.Books.Find(Int32.Parse(chitiet["sach"].ToString()));
                        int soluong = Int32.Parse(chitiet["soluong"].ToString());
                        Report_Agency idagency = (Report_Agency)Session["PhieuBaoCao"];                 
                        Inventory_Agency tonkhodaily = db.Inventory_Agency.OrderByDescending(m => m.id).FirstOrDefault(m => (m.Agency_id == idagency.Agency_id && m.Book_id == (int)sach.Book_id));
                        if (sach == null)
                        {
                            ViewBag.loi = "Mã sách không tồn tại";
                            goto baoloi;
                        }
                        else
                        {
                            if (tonkhodaily == null)
                            {
                                ViewBag.loi = "Không có tồn kho";
                                goto baoloi;
                            }
                            else
                            {
                                if(soluong > 0)
                                {
                                    if (soluong <= (tonkhodaily.deliver_quantity - tonkhodaily.repay_quantity))
                                    {
                                        Detail_Report_Agency ctpbc = new Detail_Report_Agency();
                                        ctpbc.Report_id = (db.Report_Agency.Max(u => (int?)u.Report_id) != null ? db.Report_Agency.Max(u => u.Report_id) : 0) + 1;
                                        ctpbc.Book_id = Int32.Parse(chitiet["sach"].ToString());
                                        ctpbc.quantity = Int32.Parse(chitiet["soluong"].ToString());
                                        //((List<Int32>)Session["BookID"]).Add(ctpbc.Book_id);
                                        ((List<Detail_Report_Agency>)Session["ctphieubaocao"]).Add(ctpbc);
                                    }
                                    else
                                    {
                                        ViewBag.loi = "Vượt quá số lượng báo cáo";
                                        goto baoloi;
                                    }
                                }
                                else
                                {
                                    ViewBag.loi = "Nhập số lượng lớn hơn 0";
                                    goto baoloi;
                                }
                                
                            }

                        }
                        return RedirectToAction("ThemChiTietPhieuBaoCao");
                    }
                    else
                    {
                        
                    }
                }
            }
            else if (Request.Form["create"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (((List<Detail_Report_Agency>)Session["ctphieubaocao"]).Count == 0)
                    {
                        ViewBag.loi = "Không được để phiếu trống";
                        goto baoloi;
                    }
                    double tongTien = 0;
                    double? temptongtien = 0;
                    Report_Agency test = new Report_Agency();
                    test = (Report_Agency)Session["PhieuBaoCao"];
                    var luu = db.Report_Agency.Add(test);
                    db.SaveChanges();
                    foreach (var ctpbc in (List<Detail_Report_Agency>)Session["ctphieubaocao"])
                    {
                        temptongtien += ctpbc.quantity * double.Parse(db.Books.Find(ctpbc.Book_id).Cost_Export.ToString()); ;
                        tongTien = double.Parse(temptongtien.ToString());
                        //tongSoLuong += ctpn.SoLuongNhap;
                        Detail_Report_Agency ctPhieuBaoCao = new Detail_Report_Agency();
                        ctPhieuBaoCao = ctpbc;
                        db.Detail_Report_Agency.Add(ctPhieuBaoCao);
                        //db.SaveChanges();

                        //cập nhật tồn kho đại lý
                        Inventory_Agency tonkhodaily = new Inventory_Agency();
                        Inventory_Agency tonkhodaily1 = db.Inventory_Agency.OrderByDescending(n => n.id).FirstOrDefault(n => n.Book_id == (int)ctpbc.Book_id && n.Agency_id == (int)ctpbc.Report_Agency.Agency_id);
                        tonkhodaily.UpdatedDate = DateTime.Now;
                        if (tonkhodaily1 != null)
                        {
                            if (tonkhodaily1.repay_quantity == null)
                            {
                                tonkhodaily.Book_id = ctpbc.Book_id;
                                tonkhodaily.Agency_id = (int)luu.Agency_id;
                                tonkhodaily.repay_quantity = ctpbc.quantity;
                                tonkhodaily.deliver_quantity = tonkhodaily1.deliver_quantity;
                            }
                            else
                            {
                                tonkhodaily.Book_id = ctpbc.Book_id;
                                tonkhodaily.Agency_id = (int)luu.Agency_id;
                                tonkhodaily.repay_quantity = tonkhodaily1.repay_quantity + ctpbc.quantity;
                                tonkhodaily.deliver_quantity = tonkhodaily1.deliver_quantity;
                            }
                        }
                        //else
                        //{
                        //    tonkhodaily.Book_id = ctpbc.Book_id;
                        //    tonkhodaily.Agency_id = (int)luu.Agency_id;
                        //    tonkhodaily.repay_quantity = ctpbc.quantity;
                        //    tonkhodaily.deliver_quantity = ctpbc.quantity;
                        //}
                        db.Inventory_Agency.Add(tonkhodaily);
                        //db.SaveChanges();

                        //cập nhật báo cáo đến NXB
                        int idNXB = int.Parse(db.Books.Find(ctpbc.Book_id).NXB_id.ToString());
                        Report_NXB a = db.Report_NXB.FirstOrDefault(m => m.status == 0 && m.NXB_id == idNXB);
                        if(a == null)
                        {
                            Report_NXB reportNXB = new Report_NXB();
                            reportNXB.NXB_id = idNXB;
                            reportNXB.update_date = DateTime.Now;
                            reportNXB.status = 0;
                            reportNXB.total = ctpbc.quantity * double.Parse(db.Books.Find(ctpbc.Book_id).Cost_Import.ToString());
                            db.Report_NXB.Add(reportNXB);
                            db.SaveChanges();
                        }
                        else
                        {
                            a.total += ctpbc.quantity * double.Parse(db.Books.Find(ctpbc.Book_id).Cost_Import.ToString());
                            a.update_date = DateTime.Now;
                            db.Report_NXB.Attach(a);
                            db.Entry(a).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    //Lưu phiếu - tính tổng tiền phiếu
                    //addedPhieuNhap.TongSoLuong = tongSoLuong;
                    luu.Total = tongTien;
                    db.Report_Agency.Attach(luu);
                    db.Entry(luu).State = EntityState.Modified;

                    //cập nhật công nợ đại lý
                    Debt_Agency debt = new Debt_Agency();
                    Debt_Agency debt1 = db.Debt_Agency.OrderByDescending(m => m.id).FirstOrDefault(m => m.Agency_id == (int)luu.Agency_id);
                    debt.update_date = DateTime.Now;
                    if (debt1 != null)
                    {
                        debt.Agency_id = (int)luu.Agency_id;
                        debt.debt = (double)(debt1.debt - luu.Total);
                        debt.repay = Double.Parse(luu.Total.ToString());
                    }
                    //else
                    //{
                    //    debt.Agency_id = (int)luu.Agency_id;
                    //    debt.debt = (double)luu.Total;
                    //}
                    db.Debt_Agency.Add(debt);

                    db.SaveChanges();


                    Session["ctphieubaocao"] = null;
                    Session["PhieuBaoCao"] = null;

                    return RedirectToAction("Create");
                }
            }
            baoloi:
            return View();
        }
        // GET: Report_Agency/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report_Agency report_Agency = db.Report_Agency.Find(id);
            if (report_Agency == null)
            {
                return HttpNotFound();
            }
            ViewBag.Agency_id = new SelectList(db.Agencies, "Agency_id", "Agency_name", report_Agency.Agency_id);
            return View(report_Agency);
        }

        // POST: Report_Agency/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Report_id,Agency_id,Total_Quantity,CreatedDate")] Report_Agency report_Agency)
        {
            if (ModelState.IsValid)
            {
                db.Entry(report_Agency).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Agency_id = new SelectList(db.Agencies, "Agency_id", "Agency_name", report_Agency.Agency_id);
            return View(report_Agency);
        }

        // GET: Report_Agency/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report_Agency report_Agency = db.Report_Agency.Find(id);
            if (report_Agency == null)
            {
                return HttpNotFound();
            }
            return View(report_Agency);
        }

        // POST: Report_Agency/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Report_Agency report_Agency = db.Report_Agency.Find(id);
            db.Report_Agency.Remove(report_Agency);
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
