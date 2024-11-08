using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using THWebBookStore.Models;

namespace THWebBookStore.Controllers
{
    public class NguoiDungController : Controller
    {
        private QLBansachEntities db = new QLBansachEntities();
        // GET: NguoiDung
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection collection, KHACHHANG kh)
        {
            var hoten = collection["HotenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            var xacnhanmk = collection["Matkhaunhaplai"];
            var email = collection["Email"];
            var dienThoai = collection["Dienthoai"];
            var diaChi = collection["DiaChi"];
            var ngaysinh = String.Format("{0:MM/dd/yyyy}", collection["Ngaysinh"]);
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên khách hàng không được để trống";
            }
            else if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi2"] = "Phải nhập tên đăng nhập";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi3"] = "Phải nhập mật khẩu";
            }
            else if (xacnhanmk != matkhau)
            {
                ViewData["Loi4"] = "Nhập đúng mật khẩu";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi5"] = "Phải nhập email";
            }
            else if (String.IsNullOrEmpty(dienThoai))
            {
                ViewData["Loi6"] = "Phải nhập số điện thoại";
            }
            else if (String.IsNullOrEmpty(dienThoai))
            {
                ViewData["Loi7"] = "Phải nhập địa chỉ";
            }
            else
            {
                kh.HoTen = hoten;
                kh.Taikhoan = tendn;
                kh.Matkhau = matkhau;
                kh.Email = email;
                kh.DienthoaiKH = dienThoai;
                kh.DiachiKH = diaChi;
                kh.Ngaysinh = DateTime.Parse(ngaysinh);
                db.KHACHHANGs.Add(kh);
                db.SaveChanges();
                return RedirectToAction("DangNhap");
            }
            return this.DangKy();
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var mk = collection["Matkhau"];
            if (string.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            else if (string.IsNullOrEmpty(mk))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }
            else
            {
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(i => i.Taikhoan == tendn && i.Matkhau == mk);
                if (kh != null)
                {
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công";
                    Session["Taikhoan"] = kh;
                    Session["UserName"] = kh.HoTen;
                    return RedirectToAction("Index", "Home");
                }
                else ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }

        public ActionResult DangXuat()
        {
            Session.Remove("Taikhoan");
            FormsAuthentication.SignOut();
            return RedirectToAction("DangNhap");
        }
    }
}