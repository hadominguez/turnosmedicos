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
    public class ObraSocialesController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: ObraSociales
        public ActionResult Index(string obra_social)
        {
            EliminarMensaje();
            var obrasocial = db.ObraSocial.Where(p => p.Nombre != null);
            if (!(obra_social == ""))
            {
                obrasocial = obrasocial.Where(p => p.Nombre.Contains(obra_social));
            }

            return View(obrasocial.ToList());
        }

        // GET: ObraSociales/Create
        public ActionResult Create()
        {
            EliminarMensaje();
            return View();
        }

        // POST: ObraSociales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Email,Telefono")] ObraSocial obraSocial)
        {
            EliminarMensaje();
            try {
                if (ModelState.IsValid)
                {
                    db.ObraSocial.Add(obraSocial);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(obraSocial);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index");
            }
        }

        // GET: ObraSociales/Edit/5
        public ActionResult Edit(int? id)
        {
            EliminarMensaje();
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
            EliminarMensaje();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(obraSocial).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(obraSocial);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index");
            }
        }

        // GET: ObraSociales/Delete/5
        public ActionResult Delete(int? id)
        {
            EliminarMensaje();
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
            EliminarMensaje();
            try
            {
                ObraSocial obraSocial = db.ObraSocial.Find(id);
                db.ObraSocial.Remove(obraSocial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index");
            }
        }
    }
}
