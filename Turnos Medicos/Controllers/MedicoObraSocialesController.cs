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

    public class MedicoObraSocialesController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: MedicoObraSociales
        public ActionResult Index()
        {
            var medicoObraSocial = db.MedicoObraSocial.Include(m => m.Medico).Include(m => m.ObraSocial);
            return View(medicoObraSocial.ToList());
        }

        // GET: MedicoObraSociales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoObraSocial medicoObraSocial = db.MedicoObraSocial.Find(id);
            if (medicoObraSocial == null)
            {
                return HttpNotFound();
            }
            return View(medicoObraSocial);
        }

        // GET: MedicoObraSociales/Create
        public ActionResult Create()
        {
            ViewBag.MedicoId = new SelectList(db.Medico, "Id", "Id");
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre");
            return View();
        }

        // POST: MedicoObraSociales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MedicoId,ObraSocialId,Numero")] MedicoObraSocial medicoObraSocial)
        {
            if (ModelState.IsValid)
            {
                db.MedicoObraSocial.Add(medicoObraSocial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MedicoId = new SelectList(db.Medico, "Id", "Id", medicoObraSocial.MedicoId);
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", medicoObraSocial.ObraSocialId);
            return View(medicoObraSocial);
        }

        // GET: MedicoObraSociales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoObraSocial medicoObraSocial = db.MedicoObraSocial.Find(id);
            if (medicoObraSocial == null)
            {
                return HttpNotFound();
            }
            ViewBag.MedicoId = new SelectList(db.Medico, "Id", "Id", medicoObraSocial.MedicoId);
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", medicoObraSocial.ObraSocialId);
            return View(medicoObraSocial);
        }

        // POST: MedicoObraSociales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MedicoId,ObraSocialId,Numero")] MedicoObraSocial medicoObraSocial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicoObraSocial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MedicoId = new SelectList(db.Medico, "Id", "Id", medicoObraSocial.MedicoId);
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", medicoObraSocial.ObraSocialId);
            return View(medicoObraSocial);
        }

        // GET: MedicoObraSociales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoObraSocial medicoObraSocial = db.MedicoObraSocial.Find(id);
            if (medicoObraSocial == null)
            {
                return HttpNotFound();
            }
            return View(medicoObraSocial);
        }

        // POST: MedicoObraSociales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MedicoObraSocial medicoObraSocial = db.MedicoObraSocial.Find(id);
            db.MedicoObraSocial.Remove(medicoObraSocial);
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
