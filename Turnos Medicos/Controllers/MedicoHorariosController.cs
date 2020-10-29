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

    public class MedicoHorariosController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: MedicoHorarios
        public ActionResult Index()
        {
            var medicoHorario = db.MedicoHorario.Include(m => m.Consultorio).Include(m => m.Medico);
            return View(medicoHorario.ToList());
        }

        // GET: MedicoHorarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoHorario medicoHorario = db.MedicoHorario.Find(id);
            if (medicoHorario == null)
            {
                return HttpNotFound();
            }
            return View(medicoHorario);
        }

        // GET: MedicoHorarios/Create
        public ActionResult Create()
        {
            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre");
            ViewBag.MedicoId = new SelectList(db.Medico, "Id", "Id");
            return View();
        }

        // POST: MedicoHorarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MedicoId,ConsultorioId,Dia,Inicio,Fin")] MedicoHorario medicoHorario)
        {
            if (ModelState.IsValid)
            {
                db.MedicoHorario.Add(medicoHorario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", medicoHorario.ConsultorioId);
            ViewBag.MedicoId = new SelectList(db.Medico, "Id", "Id", medicoHorario.MedicoId);
            return View(medicoHorario);
        }

        // GET: MedicoHorarios/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoHorario medicoHorario = db.MedicoHorario.Find(id);
            if (medicoHorario == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", medicoHorario.ConsultorioId);
            ViewBag.MedicoId = new SelectList(db.Medico, "Id", "Id", medicoHorario.MedicoId);
            return View(medicoHorario);
        }

        // POST: MedicoHorarios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,MedicoId,ConsultorioId,Dia,Inicio,Fin")] MedicoHorario medicoHorario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medicoHorario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", medicoHorario.ConsultorioId);
            ViewBag.MedicoId = new SelectList(db.Medico, "Id", "Id", medicoHorario.MedicoId);
            return View(medicoHorario);
        }

        // GET: MedicoHorarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedicoHorario medicoHorario = db.MedicoHorario.Find(id);
            if (medicoHorario == null)
            {
                return HttpNotFound();
            }
            return View(medicoHorario);
        }

        // POST: MedicoHorarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MedicoHorario medicoHorario = db.MedicoHorario.Find(id);
            db.MedicoHorario.Remove(medicoHorario);
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
