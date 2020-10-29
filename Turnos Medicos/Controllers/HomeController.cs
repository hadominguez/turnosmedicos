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
            if (Session["user"] != null)
            {
                return View();
            }
            return RedirectToAction("Login", "Usuarios");
        }

        public ActionResult About()
        {
                ViewBag.Message = "Quienes somos?";

                return View();
        }

        public ActionResult Contact()
        {
                ViewBag.Message = "Ponete en contacto con nosotros.";

                return View();
        }

        public ActionResult EspecialidadesDisponibles()
        {
            ViewBag.Message = "Este es nuestro conjunto de especialidades que tenemos para ofrecerte.";

            return View();
        }

        public ActionResult ProfesionalesDisponibles()
        {
            ViewBag.Message = "Este es nuestro grupo de Profesionales.";

            return View();
        }
    }
}