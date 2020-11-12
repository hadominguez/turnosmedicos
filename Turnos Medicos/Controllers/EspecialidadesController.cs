﻿using System;
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
    public class EspecialidadesController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: Especialidades
        public ActionResult Index(string especialidad)
        {
            EliminarMensaje();
            var especialidades = db.Especialidad.Where(p => p.Nombre != null);
            if (!(especialidad == ""))
            {
                especialidades = especialidades.Where(p => p.Nombre.Contains(especialidad));
            }

            return View(especialidades.ToList());
        }

        // GET: Especialidades/Create
        public ActionResult Create()
        {
            EliminarMensaje();
            return View();
        }

        // POST: Especialidades/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Tiempo,tarifa")] Especialidad especialidad)
        {
            EliminarMensaje();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Especialidad.Add(especialidad);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(especialidad);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", "Especialidades");
            }
        }

        // GET: Especialidades/Edit/5
        public ActionResult Edit(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Especialidad especialidad = db.Especialidad.Find(id);
            if (especialidad == null)
            {
                return HttpNotFound();
            }
            return View(especialidad);
        }

        // POST: Especialidades/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Tiempo,tarifa")] Especialidad especialidad)
        {
            EliminarMensaje();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(especialidad).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(especialidad);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", "Especialidades");
            }
        }

        // GET: Especialidades/Delete/5
        public ActionResult Delete(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Especialidad especialidad = db.Especialidad.Find(id);
            if (especialidad == null)
            {
                return HttpNotFound();
            }
            return View(especialidad);
        }

        // POST: Especialidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EliminarMensaje();
            try
            {
                Especialidad especialidad = db.Especialidad.Find(id);
                db.Especialidad.Remove(especialidad);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", "Especialidades");
            }
        }

    }
}
