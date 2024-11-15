using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THWebBookStore.Models;

namespace THWebBookStore.Controllers
{
    public class AdminController : Controller
    {
        private QLBansachEntities db = new QLBansachEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login() {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                ADMIN ad = db.ADMINs.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if(ad != null)
                {
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewBag.ThongBao = "Tên tài khoản hoặc mật khẩu không đúng";
                }
            }
            return View();
        }

        public ActionResult Sach(int ?page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            return View(db.SACHes.ToList().OrderBy(n=>n.Masach).ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult themMoiSach()
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult themMoiSach(SACH sach, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            if (fileUpload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/img"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    sach.Anhbia = fileName;
                    try
                    {
                        db.SACHes.Add(sach);
                        db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        foreach (var validationErrors in ex.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                            }
                        }
                        // Bạn có thể thêm ViewBag hoặc ViewData để hiển thị thông báo lỗi lên giao diện
                        ViewBag.Error = "Có lỗi xảy ra khi lưu sách. Vui lòng kiểm tra lại dữ liệu.";
                        return View(sach);
                    }
                }
            }
            return RedirectToAction("Sach");
        }

        public ActionResult ChiTietSach(int id)
        {
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.MaSach = sach.Masach;
            if(sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }

        [HttpGet]
        public ActionResult XoaSach(int id)
        {
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.MaSach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);
        }

        [HttpPost, ActionName("XoaSach")]
        public ActionResult XacNhanXoa(int id)
        {
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            ViewBag.MaSach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.SACHes.Remove(sach);
            db.SaveChanges();
            return RedirectToAction("Sach");
        }

        public ActionResult SuaSach(int id)
        {
            SACH sach = db.SACHes.SingleOrDefault(n => n.Masach == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            return View(sach);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaSach(SACH sach, HttpPostedFileBase fileUpload)
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            if (fileUpload == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/img"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }
                    sach.Anhbia = fileName;
                    db.SACHes.AddOrUpdate(sach);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Sach");
        }

        public ActionResult ThongKeSach()
        {
            // Lấy tên chủ đề và số lượng sách theo chủ đề từ cơ sở dữ liệu
            var thongKe = db.SACHes
                .GroupBy(s => s.CHUDE.TenChuDe)
                .Select(g => new { TenChuDe = g.Key, SoLuong = g.Count() })
                .ToList();

            // Truyền dữ liệu tên chủ đề và số lượng sách sang View
            ViewBag.TenChuDe = thongKe.Select(x => x.TenChuDe).ToList();
            ViewBag.SoLuongSach = thongKe.Select(x => x.SoLuong).ToList();

            return View();
        }
    }
}