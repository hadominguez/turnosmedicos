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
    public class ObraSocialesController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: ObraSociales
        public ActionResult Index()
        {
            return View(db.ObraSocial.ToList());
        }

        // GET: ObraSociales/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObraSocial obraSocial = db.ObraSocial.Find(id);
            if (obraSocial == null)
            {
                return HttpNotFound();
            }
            return View(obraSocial);
        }

        // GET: ObraSociales/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ObraSociales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Email,Telefono")] ObraSocial obraSocial)
        {
            if (ModelState.IsValid)
            {
                db.ObraSocial.Add(obraSocial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(obraSocial);
        }

        // GET: ObraSociales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObraSocial obraSocial = db.ObraSocial.Find(id);
            if (obraSocial == null)
            {
                return HttpNotFound();
            }
            return View(obraSocial);
        }

        // POST: ObraSociales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Email,Telefono")] ObraSocial obraSocial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(obraSocial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(obraSocial);
        }

        // GET: ObraSociales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObraSocial obraSocial = db.ObraSocial.Find(id);
            if (obraSocial == null)
            {
                return HttpNotFound();
            }
            return View(obraSocial);
        }

        // POST: ObraSociales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ObraSocial obraSocial = db.ObraSocial.Find(id);
            db.ObraSocial.Remove(obraSocial);
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
