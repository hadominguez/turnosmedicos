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
    public class PacientesController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: Pacientes
        public ActionResult Index(int? dni)
        {
            var paciente = db.Paciente.Include(p => p.Persona);
            if (!dni.Equals(null))
            {
                paciente = paciente.Where(p => p.Persona.DNI.Contains(dni.ToString()));
            }
            return View(paciente.ToList());
        }

        // GET: Pacientes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paciente paciente = db.Paciente.Find(id);
            if (paciente == null)
            {
                return HttpNotFound();
            }
            return View(paciente);
        }

        // GET: Pacientes/Create
        public ActionResult Create()
        {
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI");
            
            return View(new Persona());
        }

        // POST: Pacientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DNI,Apellido,Nombre,FechaNacimiento,Calle,Numero,Email,Telefono,Celular")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                db.Persona.Add(persona);
                db.SaveChanges();
                //return RedirectToAction("Index");
            }

            //return View(persona);

            Paciente paciente = new Paciente();
            paciente.PersonaId = persona.Id;
            paciente.Persona = persona;
            if (ModelState.IsValid)
            {
                db.Paciente.Add(paciente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", paciente.PersonaId);
            return View(paciente);
        }

        // GET: Pacientes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paciente paciente = db.Paciente.Find(id);
            Persona persona = db.Persona.First(p => p.Id == paciente.PersonaId);
            //Paciente paciente = db.Paciente.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", paciente.PersonaId);
            return View(persona);
        }

        // POST: Pacientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DNI,Apellido,Nombre,FechaNacimiento,Calle,Numero,Email,Telefono,Celular")] Persona persona)
        {
            if (ModelState.IsValid)
            {
                db.Entry(persona).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            Paciente paciente = db.Paciente.First(p => p.PersonaId == persona.Id);
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", paciente.PersonaId);
            return View(persona);
        }



        // GET: Pacientes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paciente paciente = db.Paciente.Find(id);
            if (paciente == null)
            {
                return HttpNotFound();
            }
            return View(paciente);
        }

        // POST: Pacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Paciente paciente = db.Paciente.Find(id);
            Persona persona = db.Persona.Find(paciente.PersonaId);
            db.Paciente.Remove(paciente);
            db.Persona.Remove(persona);
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
    }
}
