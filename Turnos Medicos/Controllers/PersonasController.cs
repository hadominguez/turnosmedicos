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
    public class PersonasController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: Personas
        public ActionResult Index(int? dni, string apellido, string nombre)
        {
            var persona = db.Persona.Where(p => p.Id >= 0);
            string nuevo_dni = dni.ToString();
            if (!(nuevo_dni == "") || !(apellido == "") || !(nombre == ""))
            {
                persona = persona.Where(p => p.DNI.Contains(nuevo_dni) && p.Apellido.Contains(apellido) && p.Nombre.Contains(nombre));
            }
            return View(persona.ToList());
        }

        // GET: Personas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = db.Persona.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // GET: Personas/Create
        public ActionResult Create()
        {
            return View(new Persona());
        }

        // POST: Personas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DNI,Apellido,Nombre,FechaNacimiento,Calle,Numero,Email,Telefono,Celular")] Persona persona)
        {
            var perso = db.Persona.Where(p => p.DNI == persona.DNI).ToList();
            Paciente paciente = new Paciente();

            if (ModelState.IsValid)
            {
                if (perso.Count < 1)
                {
                    db.Persona.Add(persona);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", paciente.PersonaId);
            return View(paciente);
        }


        // GET: Personas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = db.Persona.First(p => p.Id == id);
            //Paciente paciente = db.Paciente.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", id);
            return View(persona);
        }

        // POST: Personas/Edit/5
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
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", persona.Id);
            return View(persona);
        }


        // GET: Personas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Persona persona = db.Persona.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            return View(persona);
        }

        // POST: Personas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Persona persona = db.Persona.Find(id);
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
