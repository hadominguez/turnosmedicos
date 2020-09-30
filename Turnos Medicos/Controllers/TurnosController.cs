using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Turnos_Medicos.Models;

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
            turno.PacienteId = null;
            turno.ObraSocialId = null;
            turno.Descripcion = "";
            //db.Entry(turno).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
