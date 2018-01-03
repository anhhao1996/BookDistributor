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
    public class Bill_ImportController : Controller
    {
        private PhatHanhSachEntities db = new PhatHanhSachEntities();

        // GET: Bill_Import
        public ActionResult Index()
        {
            var bill_Import = db.Bill_Import.Include(b => b.NXB);
            return View(bill_Import.ToList());
        }

        public ActionResult Deletectpn(int bookid)
        {
            var list = (List<Detail_Bill_Import>)Session["ctphieunhap"];
            list.RemoveAll(p => p.Book_id == bookid);
            return RedirectToAction("ThemChiTietPhieuNhap");
        }
        // GET: Bill_Import/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill_Import bill_Import = db.Bill_Import.Find(id);
            var a = (from b in db.Detail_Bill_Import
                    where b.Bill_Import_id == id
                    select b).ToList();
            ViewBag.ctpn = a;
            if (bill_Import == null)
            {
                return HttpNotFound();
            }
            return View(bill_Import);
        }

        // GET: Bill_Import/Create
        public ActionResult Create()
        {
            ViewBag.NXB_id = new SelectList(db.NXBs, "NXB_id", "NXB_name");
            return View();
        }

        // POST: Bill_Import/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Bill_Import_id,NXB_id,Total,Deliver,CreatedDate")] Bill_Import bill_Import)
        {

            if (ModelState.IsValid)
            {
                bill_Import.CreatedDate = DateTime.Now;
                var tempbillimport = bill_Import;
                Session["ctphieunhap"] = new List<Detail_Bill_Import>();
                Session["PhieuNhap"] = bill_Import;
                return RedirectToAction("ThemChiTietPhieuNhap");
            }

            ViewBag.NXB_id = new SelectList(db.NXBs, "NXB_id", "NXB_name", bill_Import.NXB_id);
            return View(bill_Import);
        }

        [HttpGet]
        public ActionResult ThemChiTietPhieuNhap()
        {
            Bill_Import bill = new Bill_Import();
            bill = (Bill_Import)Session["PhieuNhap"];
            var lstBookid = (from b in db.Books
                             where b.NXB_id == bill.NXB_id
                             select b).ToList();
            ViewBag.sach = new SelectList(lstBookid, "Book_id", "Book_name");
            //ViewBag.sach = new SelectList(db.Books,"Book_id","Book_name");
            //var phieunhap = (Bill_Import)TempData["PhieuNhap"];
            return View();
        }

        [HttpPost]
        public ActionResult ThemChiTietPhieuNhap(FormCollection chitiet)
        {
            //test
            Bill_Import bill = new Bill_Import();
            bill = (Bill_Import)Session["PhieuNhap"];
            var lstBookid = (from b in db.Books
                             where b.NXB_id == bill.NXB_id
                             select b).ToList();
            ViewBag.sach = new SelectList(lstBookid, "Book_id", "Book_name");
            //
            ViewBag.loi = null;
            if (Request.Form["add"] != null)
            {
                if (ModelState.IsValid)
                {
                    bool check = true;
                    foreach (var ctpn in (List<Detail_Bill_Import>)Session["ctphieunhap"])
                    {
                        if (ctpn.Book_id == Int32.Parse(chitiet["sach"].ToString()))
                        {
                            check = false;
                            ViewBag.loi = "Sách đã được thêm vào phiếu trước đó";
                            break;
                        }
                    }
                    if (check)
                    {
                        if (chitiet["sach"] == null)
                        {
                            ViewBag.loi = "Không có sách của NXB này";
                            goto baoloi;
                        }
                        var sach = db.Books.Find(Int32.Parse(chitiet["sach"].ToString()));
                        if (sach == null)
                        {
                            ViewBag.loi = "Mã sách không tồn tại";
                            goto baoloi;
                        }
                        else
                        {
                            int soluong = Int32.Parse(chitiet["soluong"].ToString());
                            if (soluong > 0)
                            {
                                Detail_Bill_Import ctpn = new Detail_Bill_Import();
                                ctpn.Bill_Import_id = (db.Bill_Import.Max(u => (int?)u.Bill_Import_id) != null ? db.Bill_Import.Max(u => u.Bill_Import_id) : 0) + 1;// db.Bill_Import.Count() + 1;
                                ctpn.Book_id = Int32.Parse(chitiet["sach"].ToString());
                                ctpn.Quantity = soluong;
                                ctpn.Cost = db.Books.Find(ctpn.Book_id).Cost_Import;
                                ctpn.Total = ctpn.Quantity * ctpn.Cost;
                                //((List<Int32>)Session["BookID"]).Add(ctpn.Book_id);
                                ((List<Detail_Bill_Import>)Session["ctphieunhap"]).Add(ctpn);
                            }
                            else
                            {
                                ViewBag.loi = "Nhập số lượng lớn hơn 0";
                                goto baoloi;
                            }
                           
                            
                        }
                        return RedirectToAction("ThemChiTietPhieuNhap");
                    }
                    else
                    {
                       //không làm gì để nó return View();
                    }
                }
            }
            else if (Request.Form["create"] != null)
            {
                if (ModelState.IsValid)
                {
                    if (((List<Detail_Bill_Import>)Session["ctphieunhap"]).Count == 0)
                    {
                        ViewBag.loi = "Không được để phiếu trống";
                        goto baoloi;
                    }
                    double tongTien = 0;
                    double? temptongtien = 0;
                    Bill_Import test = new Bill_Import();
                    test = (Bill_Import)Session["PhieuNhap"];
                    var luu = db.Bill_Import.Add(test);
                    db.SaveChanges();
                    foreach (var ctpn in (List<Detail_Bill_Import>)Session["ctphieunhap"])
                    {
                        temptongtien += (ctpn.Cost * ctpn.Quantity);
                        tongTien = double.Parse(temptongtien.ToString());
                        //tongSoLuong += ctpn.SoLuongNhap;
                        Detail_Bill_Import ctPhieuNhap = new Detail_Bill_Import();
                        ctPhieuNhap = ctpn;
                        db.Detail_Bill_Import.Add(ctPhieuNhap);
                        //db.SaveChanges();

                        //cập nhật tồn kho công ty
                        Inventory_Book tonkho = new Inventory_Book();
                        tonkho.UpdatedDate = DateTime.Now;
                        Inventory_Book tonkho1 = db.Inventory_Book.OrderByDescending(m => m.id).FirstOrDefault(m => m.Book_id == (int)ctpn.Book_id);
                        if (tonkho1 != null)
                        {
                            tonkho.Book_id = ctpn.Book_id;
                            tonkho.Quantity = tonkho1.Quantity + ctpn.Quantity;
                        }
                        else
                        {
                            tonkho.Book_id = ctpn.Book_id;
                            tonkho.Quantity = ctpn.Quantity;
                        }
                        db.Inventory_Book.Add(tonkho);
                    }

                    //addedPhieuNhap.TongSoLuong = tongSoLuong;
                    luu.Total = tongTien;
                    db.Bill_Import.Attach(luu);
                    db.Entry(luu).State = EntityState.Modified;
                    db.SaveChanges();
                    //cập nhật công nợ NXB
                    Debt_NXB debt = new Debt_NXB();
                    debt.update_date = DateTime.Now;
                    Debt_NXB debt1 = db.Debt_NXB.OrderByDescending(m => m.id).FirstOrDefault(m => m.NXB_id == (int)luu.NXB_id);
                    if (debt1 != null)
                    {
                        debt.NXB_id = luu.NXB_id;
                        debt.debt = debt1.debt + luu.Total;
                    }
                    else
                    {
                        debt.NXB_id = luu.NXB_id;
                        debt.debt = luu.Total;
                    }
                    db.Debt_NXB.Add(debt);

                    db.SaveChanges();


                    Session["ctphieunhap"] = null;
                    Session["PhieuNhap"] = null;

                    return RedirectToAction("Create");
                }
            }
            baoloi:
                return View();
        }

        // GET: Bill_Import/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill_Import bill_Import = db.Bill_Import.Find(id);
            if (bill_Import == null)
            {
                return HttpNotFound();
            }
            ViewBag.NXB_id = new SelectList(db.NXBs, "NXB_id", "NXB_name", bill_Import.NXB_id);
            return View(bill_Import);
        }

        // POST: Bill_Import/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Bill_Import_id,NXB_id,Total,Deliver,CreatedDate")] Bill_Import bill_Import)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bill_Import).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.NXB_id = new SelectList(db.NXBs, "NXB_id", "NXB_name", bill_Import.NXB_id);
            return View(bill_Import);
        }

        // GET: Bill_Import/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bill_Import bill_Import = db.Bill_Import.Find(id);
            if (bill_Import == null)
            {
                return HttpNotFound();
            }
            return View(bill_Import);
        }

        // POST: Bill_Import/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bill_Import bill_Import = db.Bill_Import.Find(id);
            db.Bill_Import.Remove(bill_Import);
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
