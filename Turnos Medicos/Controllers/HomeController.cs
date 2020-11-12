using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using System.Net;

namespace Turnos_Medicos.Controllers
{
    [SessionCheck]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            EliminarMensaje();
            return View();
        }

        public ActionResult About()
        {
            EliminarMensaje();
                ViewBag.Message = "Quienes somos?";

                return View();
        }

        public ActionResult Contact()
        {

            EliminarMensaje();
            ViewBag.Message = "Ponete en contacto con nosotros.";

                return View();
        }

        public ActionResult EspecialidadesDisponibles()
        {

            EliminarMensaje();
            ViewBag.Message = "Este es nuestro conjunto de especialidades que tenemos para ofrecerte.";

            return View();
        }

        public ActionResult ProfesionalesDisponibles()
        {

            EliminarMensaje();
            ViewBag.Message = "Este es nuestro grupo de Profesionales.";

            return View();
        }
    }
}