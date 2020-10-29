using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;

namespace Turnos_Medicos.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //if (Session["user"] != null)
            //{
                return View();
            //}
            //return RedirectToAction("Login", "Usuarios");
        }

        public ActionResult About()
        {
                ViewBag.Message = "Your application description page.";

                return View();
        }

        public ActionResult Contact()
        {
                ViewBag.Message = "Your contact page.";

                return View();
        }
    }
}