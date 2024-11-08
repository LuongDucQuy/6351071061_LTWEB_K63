using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace THWebBookStore.Models
{
    public class GioHang
    {
        QLBansachEntities data = new QLBansachEntities();
        public int iMaSach { set; get; }
        public string sTenSach { set; get; }
        public string sAnhBia { set; get; }
        public Double dDonGia { set; get; }
        public int iSoLuong { set; get; }
        public Double dThanhTien
        {
            get { return iSoLuong * dDonGia; }
        }
        public GioHang(int maSach)
        {
            iMaSach = maSach;
            SACH sach = data.SACHes.Single(m => m.Masach == iMaSach);
            sTenSach = sach.Tensach;
            sAnhBia = sach.Anhbia;
            dDonGia = double.Parse(sach.Giaban.ToString());
            iSoLuong = 1;
        }
    }
}