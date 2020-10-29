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
        public ActionResult Index(int id)
        {
            ViewBag.Medico = db.Medico.Where(p => p.Id == id).First();
            var medicoObraSocial = db.MedicoObraSocial.Include(m => m.Medico).Include(m => m.ObraSocial).Where(p => p.MedicoId == id);
            return View(medicoObraSocial.ToList());
        }



        // GET: MedicoObraSociales/Create
        public ActionResult Create(int? id, string obra_social, int? id_obra_social)
        {
            MedicoObraSocial m_obra = new MedicoObraSocial();
            if (id != null)
            {
                m_obra.Medico = db.Medico.Where(p => p.Id == id).First();
                m_obra.MedicoId = m_obra.Medico.Id;
                ViewBag.MedicoId = new SelectList(from medico in db.Medico
                                                  join persona in db.Persona on medico.PersonaId equals persona.Id
                                                  where medico.Id == id
                                                  select new { Id = medico.Id, Nombre = persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            }

            var obra = (from obras in db.MedicoObraSocial
                        where obras.MedicoId == m_obra.MedicoId
                        select obras.ObraSocialId).ToList();

            ViewBag.ObraSocial = db.ObraSocial.Where(p => p.Nombre != null && !obra.Contains(p.Id)).ToList();
            if (!(obra_social == "") && !(obra_social == null))
            {
                ViewBag.ObraSocial = db.ObraSocial.Where(p => p.Nombre.Contains(obra_social) && !obra.Contains(p.Id)).ToList();
            }
            if (id_obra_social != null)
            {
                m_obra.ObraSocial = db.ObraSocial.Where(p => p.Id == id_obra_social).First();
                m_obra.ObraSocialId = m_obra.ObraSocial.Id;
                ViewBag.MedicoId = new SelectList(from medico in db.Medico
                                                    join persona in db.Persona on medico.PersonaId equals persona.Id
                                                    where medico.Id == m_obra.MedicoId
                                                    select new { Id = medico.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
                ViewBag.ObraSocialId = new SelectList(from obras in db.ObraSocial
                                                      where obras.Id == id_obra_social
                                                      select new { Id = obras.Id, Nombre = obras.Nombre }, "Id", "Nombre");
            }
            return View(m_obra);
        }

        // POST: MedicoObraSociales/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,MedicoId,ObraSocialId,Numero")] MedicoObraSocial medicoObraSocial)
        {

            if (ModelState.IsValid )
            {
                db.MedicoObraSocial.Add(medicoObraSocial);
                db.SaveChanges();
                return RedirectToAction("Index", new { Id = medicoObraSocial.MedicoId });
            }

            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", medicoObraSocial.ObraSocialId);
            ViewBag.MedicoId = new SelectList(db.Medico, "Id", "Id", medicoObraSocial.MedicoId);
            return RedirectToAction("Index", new { Id = medicoObraSocial.MedicoId });
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
            return RedirectToAction("Index", new { Id = medicoObraSocial.MedicoId });
        }

    }
}
