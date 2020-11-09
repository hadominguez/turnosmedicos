using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Turnos_Medicos.Models;
using System.Net.Mail;
using System.Web.Security;
using System.Web.Helpers;
using Turnos_Medicos.Controllers;
using Turnos_Medicos.Models.Extended;

namespace Turnos_Medicos.Controllers
{
    public class UsuariosController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();


        [SessionCheck]
        public ActionResult Index()
        {
            var usuario = db.Usuario.Include(u => u.Perfil).Include(u => u.Persona);
            return View(usuario.ToList());
        }

        [SessionCheck]
        [HttpGet]
        public ActionResult Create(int? Id, int? dni, string apellido, string nombre)
        {
            var personas = db.Persona.Include(p => p.Id);
            string nuevo_dni = dni.ToString();
            if (!(nuevo_dni == "") || !(apellido == "") || !(nombre == ""))
            {
                personas = personas.Where(p => p.DNI.Contains(nuevo_dni) && p.Apellido.Contains(apellido) && p.Nombre.Contains(nombre));
            }
            
            ViewBag.PersonaId = new SelectList(from persona in db.Persona
                                               where persona.Id == Id
                                               select new { Id = persona.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            ViewBag.PerfilId = new SelectList(from perfil in db.Perfil
                                                select new { Id = perfil.Id, Nombre = perfil.Nombre }, "Id", "Nombre");

            var usuarios_creados = (from usu in db.Usuario
                         select usu.PersonaId).ToList();

            ViewBag.ListadoPersona = (from persona in db.Persona
                                     where persona.Apellido.Contains(apellido) && persona.Nombre.Contains(nombre) && persona.DNI.Contains(nuevo_dni) && !usuarios_creados.Contains(persona.Id)
                                     select persona).ToList();

            Usuario usuario = new Usuario();
            if(Id != null)
            {
                Persona persona = db.Persona.First(p => p.Id == Id);
                usuario.Email = persona.Email;
                usuario.Identificador = persona.DNI;
                usuario.PersonaId = persona.Id;
                usuario.Persona = persona;

                char[] letras = "qwertyuiopasdfghjklñzxcvbnm1234567890".ToCharArray();
                Random rand = new Random();
                string random_string = "";
                for (int i = 0; i < 10; i++)
                {
                    random_string += letras[rand.Next(0, 36)].ToString();
                }
                usuario.Password = random_string;
                usuario.Bloqueado = false;
            }

            return View(usuario);
        }

      
        //Registration POST action 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Identificador,Email,Password,PersonaId,PerfilId")] Usuario user)
        {
            bool Status = false;
            string message = "";
            var usuario = db.Usuario.FirstOrDefault(p => p.Persona.DNI == user.Identificador.ToString());
            //
            // Model Validation 
            if (ModelState.IsValid && usuario == null)
            {
                if (db.Usuario.Where(p => p.Email == user.Email ).ToList().Count >= 1)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                string clave = user.Password;
                user.Password = Crypto.Hash(user.Password);
                user.Bloqueado = false;
                user.Recuperacion = Guid.NewGuid().ToString();

                db.Usuario.Add(user);
                db.SaveChanges();

                //Send Email to User
                string titulo = "Tu cuanta fue creada correctamente";

                string body = "<br/><br/>Cuenta creada" +
                    " <br/><br/><p>Email: " + user.Email + "</p><p>Clave: " + clave + "</p>";

                SendEmail(body, user.Email, titulo);
                message = "Registration successfully done." + user.Email;
                Status = true;

            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }


        // GET: Usuarios1/Edit/5
        [SessionCheck]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.PerfilId = new SelectList(db.Perfil, "Id", "Nombre", usuario.PerfilId);
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", usuario.PersonaId);
            return View(usuario);
        }

        // POST: Usuarios1/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [SessionCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Identificador,Email,Password,Bloqueado,PerfilId,PersonaId")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PerfilId = new SelectList(db.Perfil, "Id", "Nombre", usuario.PerfilId);
            ViewBag.PersonaId = new SelectList(db.Persona, "Id", "DNI", usuario.PersonaId);
            return View(usuario);
        }


        // POST: Usuarios1/Delete/5
        [SessionCheck]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuario.Find(id);
            db.Usuario.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        /*
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }
        //Registration POST action 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Include = "Id,Identificador,Email,Password")] Usuario user)
        {
            bool Status = false;
            Paciente paciente = null;
            string message = "";
            var persona = db.Persona.First(p => p.DNI == user.Identificador.ToString());
            if (persona != null)
            {
                paciente = db.Paciente.FirstOrDefault(p => p.PersonaId == persona.Id);
            }
            //
            // Model Validation 
            if (ModelState.IsValid && paciente != null)
            {
                var verificar = (from usuario in db.Usuario
                                 where usuario.Email == user.Email
                                 select usuario).ToList();
                if (verificar.Count >= 1)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                string clave = user.Password;
                user.Password = Crypto.Hash(user.Password);
                user.Bloqueado = false;
                user.PersonaId = persona.Id;
                if(paciente != null)
                {
                    user.PerfilId = db.Perfil.First(p => p.Nombre == "Paciente").Id;
                }

                db.Usuario.Add(user);
                db.SaveChanges();

                //Send Email to User

                string titulo = "Tu cuanta fue creada correctamente";

                string body = "<br/><br/>Cuenta creada" +
                    " <br/><br/><p>Email: " + user.Email + "</p><p>Clave: " + clave + "</p>";

                SendEmail(body, user.Email, titulo);
                message = "Registration successfully done. Account activation link " +
                    " has been sent to your email:" + user.Email;
                Status = true;
                return RedirectToAction("Login", "Usuarios");

            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }*/


        //Login 
        [HttpGet]
        public ActionResult Login()
        {
            if(Session["user"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            /*ViewBag.PerfilId = new SelectList(from perfil in db.Perfil
                                                select new { Id = perfil.Id, Nombre = perfil.Nombre }, "Id", "Nombre");
            ViewBag.PersonaId = new SelectList(from persona in db.Persona
                                              select new { Id = persona.Id, Nombre = persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");*/
            return View();
        }

        //Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UsuarioLogin login, string ReturnUrl = "")
        {
            string message = "";
            
                var user = db.Usuario.Where(a => a.Identificador == login.Usuario && a.Bloqueado == false).FirstOrDefault();
                if (user != null)
                {
                    if (string.Compare(Crypto.Hash(login.Password), user.Password) == 0)
                    {
                        
                        int timeout = 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(login.Usuario, false, timeout);
                        FormsAuthentication.SetAuthCookie(login.Usuario, false);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);
                        Session["user"] = user;
                        Session["perfil"] = user.Perfil;

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            

            ViewBag.Message = message;
            return View();
        }


        //Logout
        [SessionCheck]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["user"] = null;
            return RedirectToAction("Login", "Usuarios");
        }


        /*
        [NonAction]
        public void SendVerificationLinkEmail(string email, string clave)
        {
            var fromEmail = new MailAddress("axel0lopez95@gmail.com", "Medico");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "estudiante-0";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }*/


        [SessionCheck]
        public ActionResult Perfiles(int? id)
        {
            ViewBag.ListaPerfilPermiso = null;
            var perfiles = db.Perfil.Where(p => p.Id >= 0);
            if (id != null)
            {
                perfiles = perfiles.Where(p => p.Id == id);
                var permisos = db.Permiso.Where(p => p.Id >= 0);
                var perfil_p = db.PerfilPermiso.Where(p => p.PerfilId == id);
                List<PerfilesPermisos> lista = new List<PerfilesPermisos>();
                foreach(Permiso per in permisos)
                {
                    PerfilesPermisos nuevo = new PerfilesPermisos();
                    nuevo.Id = null;
                    nuevo.Permiso = per.Id;
                    nuevo.Descripcion = per.Descripcion;
                    nuevo.Controller = per.Controller;
                    nuevo.Action = per.Action;
                    foreach (PerfilPermiso perf in perfil_p)
                    {
                        if(per.Id == perf.PermisoId)
                        {
                            nuevo.Id = perf.PerfilId;
                        }
                    }
                    lista.Add(nuevo);
                }

                ViewBag.ListaPerfilPermiso = lista;
            }
            return View(perfiles);
        }

        [SessionCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Perfiles(int id, int permiso, int estado)
        {
            if(estado == 1)
            {
                PerfilPermiso perfil_p = db.PerfilPermiso.Where(p => p.PerfilId == id && p.PermisoId == permiso).First();
                db.PerfilPermiso.Remove(perfil_p);
                db.SaveChanges();
                return RedirectToAction("Perfiles", new { id = id });
            }
            else if (estado ==2)
            {
                PerfilPermiso perfil_p = new PerfilPermiso();
                perfil_p.PerfilId = id;
                perfil_p.PermisoId = permiso;
                perfil_p.Perfil = db.Perfil.First(p => p.Id == id);
                perfil_p.Permiso = db.Permiso.First(p => p.Id == permiso);
                db.PerfilPermiso.Add(perfil_p);
                db.SaveChanges();
                return RedirectToAction("Perfiles", new { id = id });
            }
            return RedirectToAction("Perfiles", new { id = id });
        }


    }
}
