using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Turnos_Medicos.Models;
using System.Net.Mail;
using System.Web.Security;
using System.Web.Helpers;

namespace Turnos_Medicos.Controllers
{
    public class BaseController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        [NonAction]
        public void SendEmail(string body, string email, string titulo)
        {
            var fromEmail = new MailAddress("axel0lopez95@gmail.com", "Medico");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "estudiante-0";
            string subject = titulo;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
    }
}