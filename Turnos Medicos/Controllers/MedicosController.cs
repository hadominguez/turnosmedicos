using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Turnos_Medicos.Models;
using Turnos_Medicos.Models.Extended;

namespace Turnos_Medicos.Controllers
{
  
    public class MedicosController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: Medicos
        public ActionResult Index(int? dni, string apellido, string nombre)
        {
            var medico = db.Medico.Include(p => p.Persona);
            string nuevo_dni = dni.ToString();
            if (!(nuevo_dni == "") || !(apellido == "") || !(nombre == ""))
            {
                medico = medico.Where(p => p.Persona.DNI.Contains(nuevo_dni) && p.Persona.Apellido.Contains(apellido) && p.Persona.Nombre.Contains(nombre));
            }
            return View(medico.ToList());
        }


        // GET: Medicos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medico medico = db.Medico.Find(id);
            if (medico == null)
            {
                return HttpNotFound();
            }
            return View(medico);
        }

        // GET: Medicos/Create
        public ActionResult Create(string especialidad, int? id_especialidad)
        {
            MedicoPersona medico = new MedicoPersona();
            if (id_especialidad != null)
            {
                //Especialidad espec = db.Especialidad.Where(p => p.Id == id_especialidad).First();
                medico.Especialidad = db.Especialidad.Where(p => p.Id == id_especialidad).First();
                medico.EspecialidadId = medico.Especialidad.Id;
            }

            ViewBag.Especialidad = db.Especialidad.Where(p => p.Nombre != null).ToList();
            if (!(especialidad == ""))
            {
                ViewBag.Especialidad = db.Especialidad.Where(p => p.Nombre.Contains(especialidad)).ToList();
            }
            ViewBag.EspecialidadId = new SelectList(from espe in db.Especialidad
                                                    where espe.Id == medico.EspecialidadId
                                                    select new { Id = espe.Id, Nombre = espe.Nombre }, "Id", "Nombre");
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI");
            return View(medico);
        }

        // POST: Medicos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DNI,Apellido,Nombre,FechaNacimiento,Calle,Numero,Email,Telefono,Celular,Matricula,EspecialidadId")] MedicoPersona medicopersona)
        {
            Persona persona = new Persona();
            persona.DNI = medicopersona.DNI;
            persona.Apellido = medicopersona.Apellido;
            persona.Nombre = medicopersona.Nombre;
            persona.FechaNacimiento = medicopersona.FechaNacimiento;
            persona.Calle = medicopersona.Calle;
            persona.Numero = medicopersona.Numero;
            persona.Email = medicopersona.Email;
            persona.Telefono = medicopersona.Telefono;
            persona.Celular = medicopersona.Celular;

            Medico medico = new Medico();
            medico.Matricula = medicopersona.Matricula;
            medico.EspecialidadId = medicopersona.EspecialidadId;

            var perso = db.Persona.Where(p => p.DNI == medicopersona.DNI).ToList();

            if (ModelState.IsValid && perso.Count < 1)
            {
                db.Persona.Add(persona);
                db.SaveChanges();

                Usuario user = new Usuario();
                string clave = medicopersona.DNI;
                user.Password = Crypto.Hash(clave);
                user.Bloqueado = false;
                user.PersonaId = persona.Id;
                user.PerfilId = db.Perfil.Where(p => p.Nombre == "Medico").First().Id;
                user.Recuperacion = Guid.NewGuid().ToString();

                db.Usuario.Add(user);
                db.SaveChanges();

                medico.Persona = db.Persona.Where(p => p.DNI == medicopersona.DNI).First();
                medico.PersonaId = persona.Id;

                if (ModelState.IsValid)
                {
                    db.Medico.Add(medico);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", medico.EspecialidadId);
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", medico.PersonaId);
            return RedirectToAction("Create");
        }

        // GET: Medicos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medico medico = db.Medico.Find(id);
            if (medico == null)
            {
                return HttpNotFound();
            }
            Persona persona = db.Persona.Find(medico.PersonaId);
            MedicoPersona medico_p = new MedicoPersona();
            medico_p.Id = medico.Id;
            medico_p.PersonaId = medico.PersonaId;
            medico_p.DNI = persona.DNI;
            medico_p.Apellido = persona.Apellido;
            medico_p.Nombre = persona.Nombre;
            medico_p.FechaNacimiento = persona.FechaNacimiento;
            medico_p.Calle = persona.Calle;
            medico_p.Numero = persona.Numero;
            medico_p.Email = persona.Email;
            medico_p.Telefono = persona.Telefono;
            medico_p.Celular = persona.Celular;
            medico_p.Matricula = medico.Matricula;
            medico_p.EspecialidadId = medico.EspecialidadId;

            medico_p.Especialidad = medico.Especialidad;

            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", medico.EspecialidadId);
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", medico.PersonaId);
            return View(medico_p);
        }

        // POST: Medicos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PersonaId,DNI,Apellido,Nombre,FechaNacimiento,Calle,Numero,Email,Telefono,Celular,Matricula,EspecialidadId")] MedicoPersona medicopersona)
        {
            Medico medico = db.Medico.Find(medicopersona.Id);
            Persona persona = db.Persona.Find(medicopersona.PersonaId);

            persona.DNI = medicopersona.DNI;
            persona.Apellido = medicopersona.Apellido;
            persona.Nombre = medicopersona.Nombre;
            persona.FechaNacimiento = medicopersona.FechaNacimiento;
            persona.Calle = medicopersona.Calle;
            persona.Numero = medicopersona.Numero;
            persona.Email = medicopersona.Email;
            persona.Telefono = medicopersona.Telefono;
            persona.Celular = medicopersona.Celular;

            medico.Matricula = medicopersona.Matricula;
            medico.EspecialidadId = medicopersona.EspecialidadId;

            if (ModelState.IsValid)
            {
                db.Entry(medico).State = EntityState.Modified;
                db.SaveChanges();
                db.Entry(persona).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", medico.EspecialidadId);
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", medico.PersonaId);
            return RedirectToAction("Index");
        }

        // GET: Medicos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Medico medico = db.Medico.Find(id);
            if (medico == null)
            {
                return HttpNotFound();
            }
            return View(medico);
        }

        // POST: Medicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Medico medico = db.Medico.Find(id);
            db.Medico.Remove(medico);
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
