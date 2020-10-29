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
    public class PacientePatologiasController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: PacientePatologias/Create
        public ActionResult Create(int id)
        {
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                where paciente.Id == id
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View();
        }

        // POST: PacientePatologias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PacienteId,Descripcion")] PacientePatologia pacientePatologia)
        {
            if (ModelState.IsValid)
            {
                pacientePatologia.Fecha = DateTime.Now;
                db.PacientePatologia.Add(pacientePatologia);
                db.SaveChanges();
                return RedirectToAction("Index", "PacienteHistoriales", new { id = pacientePatologia.PacienteId });
            }

            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                where paciente.Id == pacientePatologia.Id
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View(pacientePatologia);
        }

        // GET: PacientePatologias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacientePatologia pacientePatologia = db.PacientePatologia.Find(id);
            if (pacientePatologia == null)
            {
                return HttpNotFound();
            }
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                where paciente.Id == id
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View(pacientePatologia);
        }

        // POST: PacientePatologias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PacienteId,Descripcion,Fecha")] PacientePatologia pacientePatologia)
        {
            if (ModelState.IsValid)
            {
                //pacientePatologia.Fecha = DateTime.Now;
                db.Entry(pacientePatologia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "PacienteHistoriales", new { id = pacientePatologia.PacienteId });
            }
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                where paciente.Id == pacientePatologia.Id
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View(pacientePatologia);
        }

        // GET: PacientePatologias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacientePatologia pacientePatologia = db.PacientePatologia.Find(id);
            if (pacientePatologia == null)
            {
                return HttpNotFound();
            }
            return View(pacientePatologia);
        }

        // POST: PacientePatologias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PacientePatologia pacientePatologia = db.PacientePatologia.Find(id);
            db.PacientePatologia.Remove(pacientePatologia);
            db.SaveChanges();
            return RedirectToAction("Index", "PacienteHistoriales", new { id = pacientePatologia.PacienteId });
        }

    }
}
