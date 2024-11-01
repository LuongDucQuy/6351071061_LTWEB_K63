using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using THWebBookStore.Models;

namespace THWebBookStore.Contents
{
    public class HomeController : Controller
    {
        // GET: Home
        QLBansachEntities data = new QLBansachEntities();
        private List<SACH> LaySachMoi(int count)
        {
            return data.SACHes.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }
        public ActionResult Index()
        {

            ViewBag.chude = data.CHUDEs.ToList();
            ViewBag.nhaXB = data.NHAXUATBANs.ToList();
            var sachMoi = LaySachMoi(4);
            return View(sachMoi);
        }
        public ActionResult SPTheoChuDe(int id)
        {
            var sach = from s in data.SACHes where s.MaCD==id  select s;
            return View(sach);
        }
        public ActionResult SPTheoNXB(int id)
        {
            var sach = from s in data.SACHes where s.MaNXB == id select s;
            return View(sach);
        }
        public ActionResult Details(int id)
        {
            var sach = from s in data.SACHes where s.Masach == id select s;
            return View(sach.Single());
        }
    }
}