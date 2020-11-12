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

namespace Turnos_Medicos.Controllers
{
    [SessionCheck]
    public class PacientesController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: Pacientes
        public ActionResult Index(int? dni, string apellido, string nombre)
        {
            EliminarMensaje();
            var paciente = db.Paciente.Include(p => p.Persona);
            string nuevo_dni = dni.ToString();
            if (!(nuevo_dni == "") || !(apellido == "") || !(nombre == ""))
            {
                paciente = paciente.Where(p => p.Persona.DNI.Contains(nuevo_dni) && p.Persona.Apellido.Contains(apellido) && p.Persona.Nombre.Contains(nombre));
            }
            return View(paciente.ToList());
        }

        // GET: Pacientes/Details/5
        public ActionResult Details(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paciente paciente = db.Paciente.Find(id);
            if (paciente == null)
            {
                return HttpNotFound();
            }
            return View(paciente);
        }

        // GET: Pacientes/Create
        public ActionResult Create()
        {
            EliminarMensaje();
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI");
            
            return View(new Persona());
        }

        // POST: Pacientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DNI,Apellido,Nombre,FechaNacimiento,Calle,Numero,Email,Telefono,Celular")] Persona persona)
        {
            EliminarMensaje();
            try {
                var perso = db.Persona.Where(p => p.DNI == persona.DNI).ToList();
                Paciente paciente = new Paciente();

                if (ModelState.IsValid)
                {
                    if (perso.Count < 1)
                    {
                        db.Persona.Add(persona);
                        db.SaveChanges();
                    }

                    var usuarios = db.Usuario.Where(p => p.Identificador == persona.DNI).ToList();

                    if (usuarios.Count < 1)
                    {
                        Usuario user = new Usuario();
                        string clave = persona.DNI;

                        char[] letras = "qwertyuiopasdfghjklñzxcvbnm1234567890".ToCharArray();
                        Random rand = new Random();
                        string random_string = "";
                        for (int i = 0; i < 10; i++)
                        {
                            random_string += letras[rand.Next(0, 36)].ToString();
                        }

                        user.Password = Crypto.Hash(random_string);
                        user.Bloqueado = false;
                        user.PersonaId = persona.Id;
                        user.PerfilId = db.Perfil.Where(p => p.Nombre == "Paciente").First().Id;
                        user.Recuperacion = Guid.NewGuid().ToString();

                        db.Usuario.Add(user);
                        db.SaveChanges();


                        const string path = "~/Content/Template/EmailCreacionUsuario.html";
                        var contents = System.IO.File.ReadAllText(Server.MapPath(path));

                        contents = contents.Replace("$fecha", DateTime.Now.ToString("dd-MM-yyyy"));
                        contents = contents.Replace("$usuario", user.Identificador);
                        contents = contents.Replace("$clave", random_string);
                        string nombre = persona.Apellido + ", " + persona.Nombre;
                        contents = contents.Replace("$nombre", nombre);

                        SendEmail(contents, user.Email, "Creacion de Usuario de MedOffices");


                    }
                    else
                    {
                        var usuario = db.Usuario.Where(p => p.Identificador == persona.DNI).First();
                        usuario.PerfilId = db.Perfil.Where(p => p.Nombre == "Paciente").First().Id;
                        db.Entry(usuario).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var pacientes = db.Medico.Where(p => p.Persona.DNI == persona.DNI).ToList();

                    if (pacientes.Count < 1)
                    {
                        paciente.PersonaId = persona.Id;
                        paciente.Persona = persona;
                        db.Paciente.Add(paciente);
                        db.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }

                ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", paciente.PersonaId);
                return View(paciente);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index");
            }   

        }

        // GET: Pacientes/Edit/5
        public ActionResult Edit(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paciente paciente = db.Paciente.Find(id);
            Persona persona = db.Persona.First(p => p.Id == paciente.PersonaId);
            //Paciente paciente = db.Paciente.Find(id);
            if (persona == null)
            {
                return HttpNotFound();
            }
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", paciente.PersonaId);
            return View(persona);
        }

        // POST: Pacientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DNI,Apellido,Nombre,FechaNacimiento,Calle,Numero,Email,Telefono,Celular")] Persona persona)
        {
            EliminarMensaje();
            try { 
                if (ModelState.IsValid)
                {
                    db.Entry(persona).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                Paciente paciente = db.Paciente.First(p => p.PersonaId == persona.Id);
                ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", paciente.PersonaId);
                return View(persona);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index");
            }   
        }



        // GET: Pacientes/Delete/5
        public ActionResult Delete(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Paciente paciente = db.Paciente.Find(id);
            if (paciente == null)
            {
                return HttpNotFound();
            }
            return View(paciente);
        }

        // POST: Pacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EliminarMensaje();
            try
            {
                Paciente paciente = db.Paciente.Find(id);
                Persona persona = db.Persona.Find(paciente.PersonaId);
                db.Paciente.Remove(paciente);
                db.Persona.Remove(persona);
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
