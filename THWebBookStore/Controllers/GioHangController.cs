using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THWebBookStore.Models;

namespace THWebBookStore.Controllers
{
    public class GioHangController : Controller
    {
        QLBansachEntities data = new QLBansachEntities();
        // GET: GioHang

        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if(lstGioHang == null)
            {
                lstGioHang= new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }

        public ActionResult ThemGioHang(int maSach, string url)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanPham = lstGioHang.Find(n => n.iMaSach == maSach);
            if(sanPham == null)
            {
                sanPham = new GioHang(maSach);
                lstGioHang.Add(sanPham);
                return Redirect(url);
            }
            else
            {
                sanPham.iSoLuong++;
                return Redirect(url);
            }
        }

        private int TongSoLuong()
        {
            int iTongSL = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSL = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSL;
        }

        private double TongTien()
        {
            double iTongTien = 0; 
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if(lstGioHang != null)
            {
                iTongTien = lstGioHang.Sum(n => n.dThanhTien);
            }
            return iTongTien;
        }

        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if(lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        public ActionResult XoaGioHang(int iMaSP)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang SanPham = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSP);

            if (SanPham != null)
            {
                lstGioHang.RemoveAll(n => n.iMaSach == iMaSP);
                TempData["ThongBao"] = "Xóa sản phẩm khỏi giỏ hàng thành công!";
                return RedirectToAction("GioHang");
            }

            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            return View("GioHang");
        }

        public ActionResult XoaTatCaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult CapNhatGioHang(int iMaSP, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sanPham = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSP);
            if(sanPham != null)
            {
                sanPham.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");
        }


        public JsonResult GetTongSoLuong()
        {
            int tongSoLuong = TongSoLuong(); // Sử dụng phương thức TongSoLuong đã có
            return Json(new { TongSoLuong = tongSoLuong }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }

        public ActionResult DatHang(FormCollection collection)
        {
            //Them don hang
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<GioHang> gh = LayGioHang();
            ddh.MaKH = kh.MaKH;
            ddh.Ngaydat = DateTime.Now;
            var ngaygiao = String.Format("{0:MM/dd/yyyy}", collection["Ngaygiao"]);
            ddh.Ngaygiao = DateTime.Parse(ngaygiao);
            ddh.Tinhtranggiaohang = false;
            ddh.Dathanhtoan = false;
            data.DONDATHANGs.Add(ddh);
            data.SaveChanges();
            //Them chi tiet don hang
            foreach (var item in gh)
            {
                CHITIETDONTHANG ctdh = new CHITIETDONTHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.Masach = item.iMaSach;
                ctdh.Soluong = item.iSoLuong;
                ctdh.Dongia = (decimal)item.dDonGia;
                data.CHITIETDONTHANGs.Add(ctdh);
            }
            data.SaveChanges();
            Session["Giohang"] = null;
            return RedirectToAction("Xacnhandonhang", "Giohang");
        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }
    }
}