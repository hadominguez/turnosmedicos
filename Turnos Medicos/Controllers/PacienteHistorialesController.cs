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
    public class PacienteHistorialesController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: PacienteHistoriales
        public ActionResult Index(int id, DateTime? fecha_ini, DateTime? fecha_fin, DateTime? fecha_ini_otro, DateTime? fecha_otro_fin)
        {
            int? medico = null;
            var pacienteHistorial = db.PacienteHistorial.Include(p => p.Paciente).Include(p => p.Turno).Where(p => p.PacienteId == id && p.Turno.MedicoId == -1).OrderByDescending(p => p.Fecha).ToList();
            ViewBag.PacienteHistorial = db.PacienteHistorial.Include(p => p.Paciente).Include(p => p.Turno).Where(p => p.PacienteId == id).OrderByDescending(p => p.Fecha).ToList();
            if (((Perfil)Session["perfil"]).Nombre == "Medico")
            {
                int? persona = ((Usuario)Session["user"]).PersonaId;
                var medicos = db.Medico.Where(p => p.PersonaId == persona).ToList();
                if(medicos.Count >= 1)
                {
                    medico = medicos.First().Id;
                    pacienteHistorial = db.PacienteHistorial.Include(p => p.Paciente).Include(p => p.Turno).Where(p => p.PacienteId == id && p.Turno.MedicoId == medico).OrderByDescending(p => p.Fecha).ToList();
                    ViewBag.PacienteHistorial = db.PacienteHistorial.Include(p => p.Paciente).Include(p => p.Turno).Where(p => p.PacienteId == id && p.Turno.MedicoId != medico).OrderByDescending(p => p.Fecha).ToList();
                }
            }

            if (!fecha_ini.Equals(null) && !fecha_fin.Equals(null))
            {
                pacienteHistorial = pacienteHistorial.Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin).ToList();
            }
            if (!fecha_ini_otro.Equals(null) && !fecha_otro_fin.Equals(null))
            {
                ViewBag.PacienteHistorial = ((List<PacienteHistorial>)ViewBag.PacienteHistorial).Where(p => p.Fecha >= fecha_ini_otro && p.Fecha < fecha_otro_fin).ToList();
            }

            ViewBag.PacienteAlergia = db.PacienteAlergia.Where(p => p.PacienteId == id).ToList();
            ViewBag.PacientePatologia = db.PacientePatologia.Where(p => p.PacienteId == id).ToList();
            ViewBag.Paciente = db.Paciente.Where(p => p.Id == id).First();
            return View(pacienteHistorial);
        }


        // GET: PacienteHistoriales/Create
        public ActionResult Create(int id)
        {
            PacienteHistorial historial = new PacienteHistorial();
            historial.PacienteId = db.Paciente.First(p => p.PersonaId == id).Id;
            historial.Fecha = DateTime.Now;

            return View(historial);
        }

        // POST: PacienteHistoriales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PacienteId,TurnoId,Observacion,Fecha")] PacienteHistorial pacienteHistorial)
        {
            //pacienteHistorial.Fecha = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.PacienteHistorial.Add(pacienteHistorial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PacienteId = new SelectList(db.Paciente, "Id", "Id", pacienteHistorial.PacienteId);
            ViewBag.TurnoId = new SelectList(db.Turno, "Id", "Descripcion", pacienteHistorial.TurnoId);
            return View(pacienteHistorial);
        }

        // GET: PacienteHistoriales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteHistorial pacienteHistorial = db.PacienteHistorial.Find(id);
            if (pacienteHistorial == null)
            {
                return HttpNotFound();
            }
            ViewBag.PacienteId = new SelectList(db.Paciente, "Id", "Id", pacienteHistorial.PacienteId);
            ViewBag.TurnoId = new SelectList(db.Turno, "Id", "Descripcion", pacienteHistorial.TurnoId);
            return View(pacienteHistorial);
        }

        // POST: PacienteHistoriales/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PacienteId,TurnoId,Observacion,Fecha")] PacienteHistorial pacienteHistorial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pacienteHistorial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PacienteId = new SelectList(db.Paciente, "Id", "Id", pacienteHistorial.PacienteId);
            ViewBag.TurnoId = new SelectList(db.Turno, "Id", "Descripcion", pacienteHistorial.TurnoId);
            return View(pacienteHistorial);
        }

        // GET: PacienteHistoriales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PacienteHistorial pacienteHistorial = db.PacienteHistorial.Find(id);
            if (pacienteHistorial == null)
            {
                return HttpNotFound();
            }
            return View(pacienteHistorial);
        }

        // POST: PacienteHistoriales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PacienteHistorial pacienteHistorial = db.PacienteHistorial.Find(id);
            db.PacienteHistorial.Remove(pacienteHistorial);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
