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
    [SessionCheck]
    public class PacienteAlergiasController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();


        // GET: PacienteAlergias/Create
        public ActionResult Create(int id)
        {
            EliminarMensaje();
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                where paciente.Id == id
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View();
        }

        // POST: PacienteAlergias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PacienteId,Descripcion")] PacienteAlergia pacienteAlergia)
        {
            EliminarMensaje();
            try {
                if (ModelState.IsValid)
                {
                    pacienteAlergia.Fecha = DateTime.Now;
                    db.PacienteAlergia.Add(pacienteAlergia);
                    db.SaveChanges();
                    return RedirectToAction("Index", "PacienteHistoriales", new { id = pacienteAlergia.PacienteId });
                }

                ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                    join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                    where paciente.Id == pacienteAlergia.Id
                                                    select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
                return View(pacienteAlergia);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", "PacienteHistoriales", new { id = pacienteAlergia.PacienteId });
            }
        }

        // GET: PacienteAlergias/Edit/5
        public ActionResult Edit(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteAlergia pacienteAlergia = db.PacienteAlergia.Find(id);
            if (pacienteAlergia == null)
            {
                return HttpNotFound();
            }
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                where paciente.Id == id
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View(pacienteAlergia);
        }

        // POST: PacienteAlergias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PacienteId,Descripcion,Fecha")] PacienteAlergia pacienteAlergia)
        {
            EliminarMensaje();
            try
            {
                if (ModelState.IsValid)
                {
                    //pacienteAlergia.Fecha = DateTime.Now;
                    db.Entry(pacienteAlergia).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "PacienteHistoriales", new { id = pacienteAlergia.PacienteId });
                }
                ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                    join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                    where paciente.Id == pacienteAlergia.Id
                                                    select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
                return View(pacienteAlergia);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", "PacienteHistoriales", new { id = pacienteAlergia.PacienteId });
            }
        }

        // GET: PacienteAlergias/Delete/5
        public ActionResult Delete(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteAlergia pacienteAlergia = db.PacienteAlergia.Find(id);
            if (pacienteAlergia == null)
            {
                return HttpNotFound();
            }
            return View(pacienteAlergia);
        }

        // POST: PacienteAlergias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EliminarMensaje();
            try
            {
                PacienteAlergia pacienteAlergia = db.PacienteAlergia.Find(id);
                db.PacienteAlergia.Remove(pacienteAlergia);
                db.SaveChanges();
                return RedirectToAction("Index", "PacienteHistoriales", new { id = pacienteAlergia.PacienteId });
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", new { id = db.PacienteAlergia.First(p => p.Id == id).PacienteId });
            }
        }

    }
}
