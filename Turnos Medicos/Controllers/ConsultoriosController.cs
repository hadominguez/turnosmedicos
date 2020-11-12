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
    public class ConsultoriosController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: Consultorios
        public ActionResult Index()
        {
            EliminarMensaje();
            return View(db.Consultorio.ToList());
        }


        // GET: Consultorios/Create
        public ActionResult Create()
        {
            EliminarMensaje();
            return View();
        }

        // POST: Consultorios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre")] Consultorio consultorio)
        {
            EliminarMensaje();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Consultorio.Add(consultorio);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(consultorio);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index");
            }
        }

        // GET: Consultorios/Edit/5
        public ActionResult Edit(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consultorio consultorio = db.Consultorio.Find(id);
            if (consultorio == null)
            {
                return HttpNotFound();
            }
            return View(consultorio);
        }

        // POST: Consultorios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre")] Consultorio consultorio)
        {
            EliminarMensaje();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(consultorio).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(consultorio);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index");
            }
        }

        // GET: Consultorios/Delete/5
        public ActionResult Delete(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Consultorio consultorio = db.Consultorio.Find(id);
            if (consultorio == null)
            {
                return HttpNotFound();
            }
            return View(consultorio);
        }

        // POST: Consultorios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EliminarMensaje();
            try
            {
                Consultorio consultorio = db.Consultorio.Find(id);
                db.Consultorio.Remove(consultorio);
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
