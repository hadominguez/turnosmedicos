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
   
    public class PacienteObraSocialesController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: PacienteObraSociales
        public ActionResult Index()
        {
            var pacienteObraSocial = db.PacienteObraSocial.Include(p => p.ObraSocial).Include(p => p.Paciente);
            return View(pacienteObraSocial.ToList());
        }

        // GET: PacienteObraSociales/Details/5
        public ActionResult Details(int? id)
        {
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

        // GET: PacienteObraSociales/Create
        public ActionResult Create()
        {
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre");
            ViewBag.PacienteId = new SelectList(db.Paciente, "Id", "Id");
            return View();
        }

        // POST: PacienteObraSociales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PacienteId,ObraSocialId,NumeroAfiliado")] PacienteObraSocial pacienteObraSocial)
        {
            if (ModelState.IsValid)
            {
                db.PacienteObraSocial.Add(pacienteObraSocial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", pacienteObraSocial.ObraSocialId);
            ViewBag.PacienteId = new SelectList(db.Paciente, "Id", "Id", pacienteObraSocial.PacienteId);
            return View(pacienteObraSocial);
        }

        // GET: PacienteObraSociales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteObraSocial pacienteObraSocial = db.PacienteObraSocial.Find(id);
            if (pacienteObraSocial == null)
            {
                return HttpNotFound();
            }
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", pacienteObraSocial.ObraSocialId);
            ViewBag.PacienteId = new SelectList(db.Paciente, "Id", "Id", pacienteObraSocial.PacienteId);
            return View(pacienteObraSocial);
        }

        // POST: PacienteObraSociales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PacienteId,ObraSocialId,NumeroAfiliado")] PacienteObraSocial pacienteObraSocial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pacienteObraSocial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", pacienteObraSocial.ObraSocialId);
            ViewBag.PacienteId = new SelectList(db.Paciente, "Id", "Id", pacienteObraSocial.PacienteId);
            return View(pacienteObraSocial);
        }

        // GET: PacienteObraSociales/Delete/5
        public ActionResult Delete(int? id)
        {
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
            PacienteObraSocial pacienteObraSocial = db.PacienteObraSocial.Find(id);
            db.PacienteObraSocial.Remove(pacienteObraSocial);
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
