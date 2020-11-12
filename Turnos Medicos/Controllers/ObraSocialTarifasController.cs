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
    public class ObraSocialTarifasController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: ObraSocialTarifas
        public ActionResult Index(int id)
        {
            EliminarMensaje();
            ViewBag.ObraSocial = db.ObraSocial.Where(p => p.Id == id).First();
            var obraSocialTarifa = db.ObraSocialTarifa.Where(p => p.ObraSocialId == id).ToList();
            return View(obraSocialTarifa);
        }

        // GET: ObraSocialTarifas/Create
        public ActionResult Create(int? id, string especialidad, int? id_especialidad)
        {
            EliminarMensaje();
            ObraSocialTarifa tarifa = new ObraSocialTarifa();
            if(id != null)
            {
                tarifa.ObraSocial = db.ObraSocial.Where(p => p.Id == id).First();
                tarifa.ObraSocialId = tarifa.ObraSocial.Id;
            }
            //var obras = db.ObraSocialTarifa.Where(p => p.ObraSocialId == tarifa.ObraSocialId).ToList();

            var espec =(from tari in db.ObraSocialTarifa
            where tari.ObraSocialId == tarifa.ObraSocialId
            select tari.EspecialidadId).ToList();

            ViewBag.Especialidad = db.Especialidad.Where(p => p.Nombre != null && !espec.Contains(p.Id)).ToList();
            if (!(especialidad == ""))
            {
                ViewBag.Especialidad = db.Especialidad.Where(p => p.Nombre.Contains(especialidad) && !espec.Contains(p.Id)).ToList();
            }
            if (id_especialidad != null)
            {
                tarifa.Especialidad = db.Especialidad.Where(p => p.Id == id_especialidad).First();
                tarifa.EspecialidadId = tarifa.Especialidad.Id;
                ViewBag.EspecialidadId = new SelectList(from espe in db.Especialidad
                                                        where espe.Id == tarifa.EspecialidadId
                                                      select new { Id = espe.Id, Nombre = espe.Nombre }, "Id", "Nombre");
                ViewBag.ObraSocialId = new SelectList(from obra in db.ObraSocial
                                                    where obra.Id == tarifa.ObraSocialId
                                                    select new { Id = obra.Id, Nombre = obra.Nombre }, "Id", "Nombre");
            }
            return View(tarifa);
        }

        // POST: ObraSocialTarifas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ObraSocialId,EspecialidadId,tarifa")] ObraSocialTarifa obraSocialTarifa)
        {
            EliminarMensaje();
            try
            {
                var espec = (from espe in db.Especialidad
                        where espe.Id == obraSocialTarifa.EspecialidadId
                        select espe).ToList();
                if (ModelState.IsValid && espec.Count < 1)
                {
                    db.ObraSocialTarifa.Add(obraSocialTarifa);
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = obraSocialTarifa.ObraSocialId });
                }

                ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", obraSocialTarifa.EspecialidadId);
                ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", obraSocialTarifa.ObraSocialId);
                return View(obraSocialTarifa);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", new { id = obraSocialTarifa.ObraSocialId });
            }
        }

        // GET: ObraSocialTarifas/Edit/5
        public ActionResult Edit(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ObraSocialTarifa obraSocialTarifa = db.ObraSocialTarifa.Find(id);
            if (obraSocialTarifa == null)
            {
                return HttpNotFound();
            }
            ViewBag.EspecialidadId = new SelectList(from espe in db.Especialidad
                                                    join tarifa in db.ObraSocialTarifa on espe.Id equals tarifa.EspecialidadId
                                                    where id == tarifa.Id
                                                    select new { Id = espe.Id, Nombre = espe.Nombre }, "Id", "Nombre");
            ViewBag.ObraSocialId = new SelectList(from obra in db.ObraSocial
                                                  join tarifa in db.ObraSocialTarifa on obra.Id equals tarifa.ObraSocialId
                                                  where id == tarifa.Id
                                                  select new { Id = obra.Id, Nombre = obra.Nombre }, "Id", "Nombre");
            return View(obraSocialTarifa);
        }

        // POST: ObraSocialTarifas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ObraSocialId,EspecialidadId,tarifa")] ObraSocialTarifa obraSocialTarifa)
        {
            EliminarMensaje();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(obraSocialTarifa).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", new { id = obraSocialTarifa.ObraSocialId });
                }
                ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", obraSocialTarifa.EspecialidadId);
                ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", obraSocialTarifa.ObraSocialId);
                return View(obraSocialTarifa);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", new { id = obraSocialTarifa.ObraSocialId });
            }
        }

        // GET: ObraSocialTarifas/Delete/5
        public ActionResult Delete(int? id)
        {
            EliminarMensaje();
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
            EliminarMensaje();
            try
            {
                ObraSocialTarifa obraSocialTarifa = db.ObraSocialTarifa.Find(id);
                db.ObraSocialTarifa.Remove(obraSocialTarifa);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = obraSocialTarifa.ObraSocialId });
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", new { id = db.ObraSocialTarifa.First(p => p.Id == id).ObraSocialId });
            }
        }

    }
}
