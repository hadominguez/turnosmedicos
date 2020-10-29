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
    public class MedicoHorariosController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: MedicoHorarios
        public ActionResult Index(int id)
        {
            ViewBag.Medico = db.Medico.Where(p => p.Id == id).First();
            var medicoHorario = db.MedicoHorario.Include(m => m.Consultorio).Include(m => m.Medico).Where(p => p.MedicoId == id);
            return View(medicoHorario.ToList());
        }

        // GET: MedicoHorarios/Create
        public ActionResult Create(int id)
        {
            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre");
            ViewBag.MedicoId = new SelectList(from medico in db.Medico
                                              join persona in db.Persona on medico.PersonaId equals persona.Id
                                              where medico.Id == id
                                              select new { Id = medico.Id, Nombre = persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            MedicoHorario hora = new MedicoHorario();
            hora.MedicoId = id;
            return View(hora);
        }

        // POST: MedicoHorarios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MedicoId,ConsultorioId,Dia,Inicio,Fin")] MedicoHorario medicoHorario)
        {
            medicoHorario.Medico = db.Medico.Where(p => p.Id == medicoHorario.MedicoId).First();
            var medicos = db.MedicoHorario.Where(p => p.ConsultorioId == medicoHorario.ConsultorioId && p.Dia == medicoHorario.Dia 
                                                && ((p.Inicio <= medicoHorario.Inicio && p.Fin > medicoHorario.Inicio) || (p.Inicio <= medicoHorario.Fin && p.Fin > medicoHorario.Fin))).ToList();
            if (ModelState.IsValid && medicos.Count < 1)
            {
                db.MedicoHorario.Add(medicoHorario);
                db.SaveChanges();
                return RedirectToAction("Index", new { Id = medicoHorario.MedicoId });
            }

            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", medicoHorario.ConsultorioId);
            ViewBag.MedicoId = new SelectList(db.Medico, "Id", "Id", medicoHorario.MedicoId);
            return RedirectToAction("Index", new { Id = medicoHorario.MedicoId });
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
            return RedirectToAction("Index", new { Id = medicoHorario.MedicoId });
        }
    }
}
