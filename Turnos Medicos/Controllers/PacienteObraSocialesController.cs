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
    public class PacienteObraSocialesController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: PacienteObraSociales
        public ActionResult Index(int id)
        {
            EliminarMensaje();
            ViewBag.Paciente = db.Paciente.Where(p => p.Id == id).First();
            var pacienteObraSocial = db.PacienteObraSocial.Where(p => p.PacienteId == id);
            return View(pacienteObraSocial.ToList());
        }

        // GET: PacienteObraSociales/Create
        public ActionResult Create(int? id, string obra_social, int? id_obra_social)
        {
            EliminarMensaje();
            PacienteObraSocial pa_obra = new PacienteObraSocial();
            if (id != null)
            {
                pa_obra.Paciente = db.Paciente.Where(p => p.Id == id).First();
                pa_obra.PacienteId = pa_obra.Paciente.Id;
            }

            var obra = (from obras in db.PacienteObraSocial
                         where obras.PacienteId == pa_obra.PacienteId
                         select obras.ObraSocialId).ToList();

            ViewBag.ObraSocial = db.ObraSocial.Where(p => p.Nombre != null && !obra.Contains(p.Id)).ToList();
            if (!(obra_social == "") && !(obra_social == null))
            {
                ViewBag.ObraSocial = db.ObraSocial.Where(p => p.Nombre.Contains(obra_social) && !obra.Contains(p.Id)).ToList();
            }
            if (id_obra_social != null)
            {
                pa_obra.ObraSocial = db.ObraSocial.Where(p => p.Id == id_obra_social).First();
                pa_obra.ObraSocialId = pa_obra.ObraSocial.Id;
                ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                    join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                    where paciente.Id == pa_obra.PacienteId
                                                    select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
                ViewBag.ObraSocialId = new SelectList(from obras in db.ObraSocial
                                                      where obras.Id == id_obra_social
                                                      select new { Id = obras.Id, Nombre = obras.Nombre }, "Id", "Nombre");
            }
            return View(pa_obra);
        }



        // POST: PacienteObraSociales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PacienteId,ObraSocialId,NumeroAfiliado")] PacienteObraSocial pacienteObraSocial)
        {
            EliminarMensaje();
            try {
                if (ModelState.IsValid)
                {
                    db.PacienteObraSocial.Add(pacienteObraSocial);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = pacienteObraSocial.PacienteId });
                }

                ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", pacienteObraSocial.ObraSocialId);
                ViewBag.PacienteId = new SelectList(db.Paciente, "Id", "Id", pacienteObraSocial.PacienteId);
                return View(pacienteObraSocial);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", new { id = pacienteObraSocial.PacienteId });
            }
        }



        // GET: PacienteObraSociales/Edit/5
        public ActionResult Edit(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteObraSocial pacienteObraSocial = db.PacienteObraSocial.Find(id);
            if (pacienteObraSocial == null)
            {
                return HttpNotFound();
            }
            ViewBag.ObraSocialId = new SelectList(from obra in db.ObraSocial
                                                  join p_obra in db.PacienteObraSocial on obra.Id equals p_obra.ObraSocialId
                                                  where id == p_obra.Id
                                                  select new { Id = obra.Id, Nombre = obra.Nombre }, "Id", "Nombre");
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                join p_obra in db.PacienteObraSocial on paciente.Id equals p_obra.PacienteId
                                                where id == p_obra.Id
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View(pacienteObraSocial);
        }

        // POST: PacienteObraSociales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PacienteId,ObraSocialId,NumeroAfiliado")] PacienteObraSocial pacienteObraSocial)
        {
            EliminarMensaje();
            try {
                if (ModelState.IsValid)
                {
                    db.Entry(pacienteObraSocial).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = pacienteObraSocial.PacienteId });
                }
                ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", pacienteObraSocial.ObraSocialId);
                ViewBag.PacienteId = new SelectList(db.Paciente, "Id", "Nombre", pacienteObraSocial.PacienteId);
                return View(pacienteObraSocial);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", new { id = pacienteObraSocial.PacienteId });
            }
        }



        // GET: PacienteObraSocia.les/Delete/5
        public ActionResult Delete(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteObraSocial pacienteObraSocial = db.PacienteObraSocial.Find(id);
            if (pacienteObraSocial == null)
            {
                return HttpNotFound();
            }
            return View(pacienteObraSocial);
        }

        // POST: PacienteObraSociales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EliminarMensaje();
            try
            {
                PacienteObraSocial pacienteObraSocial = db.PacienteObraSocial.Find(id);
                db.PacienteObraSocial.Remove(pacienteObraSocial);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = pacienteObraSocial.PacienteId });
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", new { id = db.PacienteObraSocial.First(p => p.Id == id).PacienteId });
            }
        }
    }
}
