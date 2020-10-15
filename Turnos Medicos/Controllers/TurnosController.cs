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

namespace Turnos_Medicos.Controllers
{
    public class TurnosController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: Turnos
        public ActionResult Index()
        {
            var turno = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha.Year == DateTime.Now.Year && p.Fecha.Month == DateTime.Now.Month && p.Fecha.Day == DateTime.Now.Day);
            return View(turno.ToList());
        }

        // GET: Turnos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turno turno = db.Turno.Find(id);
            if (turno == null)
            {
                return HttpNotFound();
            }
            return View(turno);
        }

        
        // GET: Turnos/Create
        public ActionResult Create()
        {
            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre");
            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre");
            ViewBag.EstadoId = new SelectList(db.Estado, "Id", "Nombre");
            ViewBag.MedicoId = new SelectList(from medico in db.Medico
                               join persona in db.Persona on medico.PersonaId equals persona.Id
                               select new { Id = medico.Id, Nombre = persona.Apellido + ", " + persona.Nombre } , "Id", "Nombre");
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre");
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                 join persona in db.Persona on paciente.PersonaId equals persona.Id
                                 select new { Id = paciente.Id, Nombre = persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EstadoId,MedicoId,PacienteId,ObraSocialId,ConsultorioId,EspecialidadId,Fecha,Hora,Descripcion,ObraSocialTarifa,CostoTotal,Pagado")] Turno turno)
        {
            //var day = (int)DateTime.Now.DayOfWeek;
            if (!turno.MedicoId.Equals(null))
            {
                db.Turno.Add(turno);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", turno.ConsultorioId);
            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", turno.EspecialidadId);
            ViewBag.EstadoId = new SelectList(db.Estado, "Id", "Nombre", turno.EstadoId);
            ViewBag.MedicoId = new SelectList(from medico in db.Medico
                                              join persona in db.Persona on medico.PersonaId equals persona.Id
                                              where medico.Id == turno.MedicoId
                                              select new { Id = medico.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", turno.ObraSocialId);
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                where paciente.Id == turno.PacienteId
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View(turno);
        }

        public ActionResult Asignar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turno turno = db.Turno.Find(id);
            if (turno == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", turno.ConsultorioId);
            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", turno.EspecialidadId);
            ViewBag.EstadoId = new SelectList(db.Estado, "Id", "Nombre", turno.EstadoId);
            ViewBag.MedicoId = new SelectList(from medico in db.Medico
                                              join persona in db.Persona on medico.PersonaId equals persona.Id
                                              where medico.Id == turno.MedicoId
                                              select new { Id = medico.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", turno.ObraSocialId);
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View(turno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Asignar([Bind(Include = "Id,EstadoId,MedicoId,PacienteId,ObraSocialId,ConsultorioId,EspecialidadId,Fecha,Hora,Descripcion,ObraSocialTarifa,CostoTotal,Pagado")] Turno turno)
        {
            turno.EstadoId = 2;
            int pacienteId = (int)turno.PacienteId;
            var obra_so = (from paci_obra in db.PacienteObraSocial
                          join tarifa in db.ObraSocialTarifa on paci_obra.ObraSocialId equals tarifa.ObraSocialId
                          where paci_obra.PacienteId == pacienteId
                          select tarifa).ToList();
            if(obra_so.Count >= 1)
            {
                turno.ObraSocialId = obra_so.First().ObraSocialId;
            }

            if (!turno.Id.Equals(null))
            {
                db.Entry(turno).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", turno.ConsultorioId);
            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", turno.EspecialidadId);
            ViewBag.EstadoId = new SelectList(db.Estado, "Id", "Nombre", turno.EstadoId);
            ViewBag.MedicoId = new SelectList(from medico in db.Medico
                                              join persona in db.Persona on medico.PersonaId equals persona.Id
                                              where medico.Id == turno.MedicoId
                                              select new { Id = medico.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", turno.ObraSocialId);
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                where paciente.Id == turno.PacienteId
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View(turno);
        }



        // GET: Turnos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turno turno = db.Turno.Find(id);
            if (turno == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", turno.ConsultorioId);
            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", turno.EspecialidadId);
            ViewBag.EstadoId = new SelectList(db.Estado, "Id", "Nombre", turno.EstadoId);
            ViewBag.MedicoId = new SelectList(from medico in db.Medico
                                              join persona in db.Persona on medico.PersonaId equals persona.Id
                                              where medico.Id == turno.MedicoId
                                              select new { Id = medico.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", turno.ObraSocialId);
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                where paciente.Id == turno.PacienteId
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View(turno);
        }

        // POST: Turnos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EstadoId,MedicoId,PacienteId,ObraSocialId,ConsultorioId,EspecialidadId,Fecha,Hora,Descripcion,ObraSocialTarifa,CostoTotal,Pagado")] Turno turno)
        {
            if (!turno.Id.Equals(null))
            {
                db.Entry(turno).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", turno.ConsultorioId);
            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", turno.EspecialidadId);
            ViewBag.EstadoId = new SelectList(db.Estado, "Id", "Nombre", turno.EstadoId);
            ViewBag.MedicoId = new SelectList(from medico in db.Medico
                                              join persona in db.Persona on medico.PersonaId equals persona.Id
                                              where medico.Id == turno.MedicoId
                                              select new { Id = medico.Id, Nombre = persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", turno.ObraSocialId);
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                where paciente.Id == turno.PacienteId
                                                select new { Id = paciente.Id, Nombre = persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View(turno);
        }

        // GET: Turnos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turno turno = db.Turno.Find(id);
            if (turno == null)
            {
                return HttpNotFound();
            }
            return View(turno);
        }

        // POST: Turnos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            /*
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 465);
            smtp.Credentials = new NetworkCredential("axel0lopez95@gmail.com", "estudiante-0"); //("axel0lopez95@gmail.com", "estudiante-0");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("axel0lopez95@gmail.com", "Hospital");
            mail.To.Add(new MailAddress("axe_lopez95@yahoo.com"));
            mail.Subject = "Mensaje de prueba";
            mail.IsBodyHtml = true;

            mail.Body = "<body>" +
                "<h1>Prueba de cuerpo de mensaje</h1>" +
                "</body>";

            smtp.Send(mail);*/


            Turno turno = db.Turno.Find(id);
            db.Turno.Remove(turno);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        public ActionResult Cancelar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turno turno = db.Turno.Find(id);
            if (turno == null)
            {
                return HttpNotFound();
            }
            return View(turno);
        }


        [HttpPost, ActionName("Cancelar")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelarConfirmado(int id)
        {
            Turno turno = db.Turno.Find(id);
            string body = "<br/>Su Turno del dia " + turno.Fecha.ToString("yyyy-MM-dd") + " fue cancelado.<br/>Solicite uno nuevo.<br/>";
            string email = (from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                where paciente.Id == turno.PacienteId
                                                select persona).First().Email;
            string titulo = "Turno Cancelado";

            turno.PacienteId = null;
            turno.ObraSocialId = null;
            turno.Descripcion = "";
            turno.EstadoId = 1;

            //db.Entry(turno).State = EntityState.Modified;
            db.SaveChanges();

            SendEmail(body, email, titulo);
            return RedirectToAction("Index");
        }

<<<<<<< HEAD
        public ActionResult mostrarTurno(string sortOrder)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            var students = from s in db.Medico
                           select s;
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Persona);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.Especialidad);
                    break;
            }
            return View(students.ToList());
        }


=======


        // GET: Turnos/Create
        public ActionResult Historial(int id)
        {
            PacienteHistorial historial = new PacienteHistorial();
            historial.PacienteId = (int)db.Turno.First(p => p.Id == id).PacienteId;
            historial.Fecha = DateTime.Now;
            historial.TurnoId = id;

            return View(historial);
        }

        // POST: Turnos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Historial([Bind(Include = "Id,PacienteId,TurnoId,Observacion,Fecha")] PacienteHistorial pacienteHistorial)
        {
            pacienteHistorial.Fecha = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.PacienteHistorial.Add(pacienteHistorial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PacienteId = new SelectList(db.Paciente, "Id", "Id", pacienteHistorial.PacienteId);
            ViewBag.TurnoId = new SelectList(db.Turno, "Id", "Descripcion", pacienteHistorial.TurnoId);
            return View(pacienteHistorial);
        }




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

>>>>>>> develop
    }
}
