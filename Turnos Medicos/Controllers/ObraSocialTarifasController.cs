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
    public class ObraSocialTarifasController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: ObraSocialTarifas
        public ActionResult Index()
        {
            var obraSocialTarifa = db.ObraSocialTarifa.Include(o => o.Especialidad).Include(o => o.ObraSocial);
            return View(obraSocialTarifa.ToList());
        }

        // GET: ObraSocialTarifas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObraSocialTarifa obraSocialTarifa = db.ObraSocialTarifa.Find(id);
            if (obraSocialTarifa == null)
            {
                return HttpNotFound();
            }
            return View(obraSocialTarifa);
        }

        // GET: ObraSocialTarifas/Create
        public ActionResult Create()
        {
            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre");
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre");
            return View();
        }

        // POST: ObraSocialTarifas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ObraSocialId,EspecialidadId,tarifa")] ObraSocialTarifa obraSocialTarifa)
        {
            if (ModelState.IsValid)
            {
                db.ObraSocialTarifa.Add(obraSocialTarifa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", obraSocialTarifa.EspecialidadId);
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", obraSocialTarifa.ObraSocialId);
            return View(obraSocialTarifa);
        }

        // GET: ObraSocialTarifas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObraSocialTarifa obraSocialTarifa = db.ObraSocialTarifa.Find(id);
            if (obraSocialTarifa == null)
            {
                return HttpNotFound();
            }
            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", obraSocialTarifa.EspecialidadId);
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", obraSocialTarifa.ObraSocialId);
            return View(obraSocialTarifa);
        }

        // POST: ObraSocialTarifas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ObraSocialId,EspecialidadId,tarifa")] ObraSocialTarifa obraSocialTarifa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(obraSocialTarifa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", obraSocialTarifa.EspecialidadId);
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", obraSocialTarifa.ObraSocialId);
            return View(obraSocialTarifa);
        }

        // GET: ObraSocialTarifas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObraSocialTarifa obraSocialTarifa = db.ObraSocialTarifa.Find(id);
            if (obraSocialTarifa == null)
            {
                return HttpNotFound();
            }
            return View(obraSocialTarifa);
        }

        // POST: ObraSocialTarifas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ObraSocialTarifa obraSocialTarifa = db.ObraSocialTarifa.Find(id);
            db.ObraSocialTarifa.Remove(obraSocialTarifa);
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
