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
using System.IO;

namespace Turnos_Medicos.Controllers
{
    public class BaseController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        //[SessionCheck]
        [NonAction]
        public void SendEmail(string body, string email, string titulo)
        {
            //var fromEmail = new MailAddress("axel0lopez95@gmail.com", "Medico");
            var fromEmail = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["fromEmail"], System.Configuration.ConfigurationManager.AppSettings["fromName"]);
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "estudiante-0";
            string subject = titulo;

            var smtp = new SmtpClient
            {
                Host = System.Configuration.ConfigurationManager.AppSettings["mailHost"],
                Port = Int16.Parse(System.Configuration.ConfigurationManager.AppSettings["mailPort"]),
                EnableSsl = Boolean.Parse(System.Configuration.ConfigurationManager.AppSettings["enableSsl"]),
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


        [NonAction]
        public void SendEmailAdjunto(string body, string email, string titulo, MemoryStream adjunto, string adjunto_nombre)
        {
            Attachment data = new Attachment(adjunto, adjunto_nombre);
            var fromEmail = new MailAddress(System.Configuration.ConfigurationManager.AppSettings["fromEmail"], System.Configuration.ConfigurationManager.AppSettings["fromName"]);
            var toEmail = new MailAddress(email);
            var fromEmailPassword = System.Configuration.ConfigurationManager.AppSettings["fromEmailPassword"];
            string subject = titulo;

            var smtp = new SmtpClient
            {
                Host = System.Configuration.ConfigurationManager.AppSettings["mailHost"],
                Port = Int16.Parse(System.Configuration.ConfigurationManager.AppSettings["mailPort"]),
                EnableSsl = Boolean.Parse(System.Configuration.ConfigurationManager.AppSettings["enableSsl"]),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            message.Attachments.Add(data);
            smtp.Send(message);
        }

        public void EliminarMensaje()
        {
            if (Session == null && Session["user"] == null)
            {
                Session["AlertaContador"] = (int)Session["AlertaContador"] + 1;
            }
        }
        public void MandarMensaje(string mensaje, string tipo)
        {
            Session["Mensaje"] = mensaje;
            Session["Alerta"] = tipo;
            Session["AlertaContador"] = 0;
        }
    }
}