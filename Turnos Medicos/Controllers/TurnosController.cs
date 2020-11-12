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
using Turnos_Medicos.Controllers;

namespace Turnos_Medicos.Controllers
{
    [SessionCheck]
    public class TurnosController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: Turnos
        public ActionResult Index(int? dni, string apellido, string nombre, string m_apellido, string m_nombre, string especialidad)
        {
            EliminarMensaje();
            var turno = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha.Year == DateTime.Now.Year && p.Fecha.Month == DateTime.Now.Month && p.Fecha.Day == DateTime.Now.Day && p.Estado.Nombre == "Asignado");
            Usuario usuario = (Usuario)Session["user"];
            switch (((Perfil)Session["perfil"]).Nombre)
            {
                case "Paciente":
                    turno = turno.Where(p => p.Paciente.PersonaId == usuario.PersonaId);
                    break;
                case "Medico":
                    turno = turno.Where(p => p.Medico.PersonaId == usuario.PersonaId);
                    break;
                default:
                    break;
            }

            string nuevo_dni = dni.ToString();
            if (!(m_apellido == "") || !(m_nombre == "") || !(especialidad == ""))
            {
                turno = turno.Where(p => p.Medico.Persona.Apellido.Contains(m_apellido)
                    && p.Medico.Persona.Nombre.Contains(m_nombre)
                    && p.Especialidad.Nombre.Contains(especialidad));
            }
            if (!(nuevo_dni == "") || !(apellido == "") || !(nombre == ""))
            {
                turno = turno.Where(p => p.Paciente.Persona.DNI.Contains(nuevo_dni)
                    && p.Paciente.Persona.Apellido.Contains(apellido)
                    && p.Paciente.Persona.Nombre.Contains(nombre));
            }
            return View(turno.ToList());
        }

        // GET: Turnos/Details/5
        public ActionResult Details(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turno turno = db.Turno.Find(id);
            if (turno == null)
            {
                return HttpNotFound();
            }
            return View(turno);
        }

        
        public ActionResult Asignar(int? id, int? dni, string apellido, string nombre, int? id_paciente)
        {
            EliminarMensaje();
            Usuario usuario = (Usuario)Session["user"];
            switch (((Perfil)Session["perfil"]).Nombre)
            {
                case "Paciente":
                    id_paciente = db.Paciente.Where(p => p.PersonaId == usuario.PersonaId).First().Id;
                    break;
                default:
                    break;
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turno turno = db.Turno.Find(id);
            if (turno == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", turno.ConsultorioId);
            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", turno.EspecialidadId);
            ViewBag.EstadoId = new SelectList(db.Estado, "Id", "Nombre", turno.EstadoId);
            ViewBag.MedicoId = new SelectList(from medico in db.Medico
                                              join persona in db.Persona on medico.PersonaId equals persona.Id
                                              where medico.Id == turno.MedicoId
                                              select new { Id = medico.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", turno.ObraSocialId);
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            string nuevo_dni = dni.ToString();
            if (!(nuevo_dni == "") || !(apellido == "") || !(nombre == ""))
            {
                ViewBag.ListadoPaciente = (from paciente in db.Paciente
                                                    join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                    where persona.Apellido.Contains(apellido) && persona.Nombre.Contains(nombre) && persona.DNI.Contains(nuevo_dni)
                                           select paciente);
            }
            if(id_paciente != null)
            {
                ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                    join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                    where paciente.Id == id_paciente
                                                    select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
                turno.Paciente = db.Paciente.Where(p => p.Id == id_paciente).First();
                turno.PacienteId = turno.Paciente.Id;
            }
            return View(turno);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Asignar([Bind(Include = "Id,EstadoId,MedicoId,PacienteId,ObraSocialId,ConsultorioId,EspecialidadId,Fecha,Hora,Descripcion,ObraSocialTarifa,CostoTotal,Pagado")] Turno turno)
        {
            EliminarMensaje();
            try {
                turno.EstadoId = 2;
                int pacienteId = (int)turno.PacienteId;
                int especialidadId = (int)turno.EspecialidadId;
                var obra_so = (from paci_obra in db.PacienteObraSocial
                               join tarifa in db.ObraSocialTarifa on paci_obra.ObraSocialId equals tarifa.ObraSocialId
                               join especia in db.Especialidad on tarifa.EspecialidadId equals especia.Id
                               join medic in db.Medico on especia.Id equals medic.EspecialidadId
                               join medic_obra in db.MedicoObraSocial on medic.Id equals medic_obra.MedicoId
                               where paci_obra.PacienteId == pacienteId
                               && especia.Id == especialidadId
                               && paci_obra.ObraSocialId == medic_obra.ObraSocialId
                               select tarifa).ToList();
                if(obra_so.Count >= 1)
                {
                    turno.ObraSocialId = obra_so.First().ObraSocialId;
                    turno.ObraSocialTarifa = obra_so.First().tarifa;
                    turno.CostoTotal = obra_so.First().tarifa;
                }
                else
                {
                    turno.CostoTotal = db.Especialidad.Where(p => p.Id == especialidadId).First().tarifa;
                }

                if (!turno.Id.Equals(null))
                {
                    db.Entry(turno).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", turno.ConsultorioId);
                ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", turno.EspecialidadId);
                ViewBag.EstadoId = new SelectList(db.Estado, "Id", "Nombre", turno.EstadoId);
                ViewBag.MedicoId = new SelectList(from medico in db.Medico
                                                  join persona in db.Persona on medico.PersonaId equals persona.Id
                                                  where medico.Id == turno.MedicoId
                                                  select new { Id = medico.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
                ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", turno.ObraSocialId);
                ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                    join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                    where paciente.Id == turno.PacienteId
                                                    select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
                return View(turno);

            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", "Agenda");
            }
        }



        // GET: Turnos/Edit/5
        public ActionResult Edit(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turno turno = db.Turno.Find(id);
            if (turno == null)
            {
                return HttpNotFound();
            }
            ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", turno.ConsultorioId);
            ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", turno.EspecialidadId);
            ViewBag.EstadoId = new SelectList(db.Estado, "Id", "Nombre", turno.EstadoId);
            ViewBag.MedicoId = new SelectList(from medico in db.Medico
                                              join persona in db.Persona on medico.PersonaId equals persona.Id
                                              where medico.Id == turno.MedicoId
                                              select new { Id = medico.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", turno.ObraSocialId);
            ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                where paciente.Id == turno.PacienteId
                                                select new { Id = paciente.Id, Nombre = persona.DNI + " - " + persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
            return View(turno);
        }

        // POST: Turnos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EstadoId,MedicoId,PacienteId,ObraSocialId,ConsultorioId,EspecialidadId,Fecha,Hora,Descripcion,ObraSocialTarifa,CostoTotal,Pagado")] Turno turno)
        {
            EliminarMensaje();
            try
            {
                if (!turno.Id.Equals(null))
                {
                    db.Entry(turno).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.ConsultorioId = new SelectList(db.Consultorio, "Id", "Nombre", turno.ConsultorioId);
                ViewBag.EspecialidadId = new SelectList(db.Especialidad, "Id", "Nombre", turno.EspecialidadId);
                ViewBag.EstadoId = new SelectList(db.Estado, "Id", "Nombre", turno.EstadoId);
                ViewBag.MedicoId = new SelectList(from medico in db.Medico
                                                  join persona in db.Persona on medico.PersonaId equals persona.Id
                                                  where medico.Id == turno.MedicoId
                                                  select new { Id = medico.Id, Nombre = persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
                ViewBag.ObraSocialId = new SelectList(db.ObraSocial, "Id", "Nombre", turno.ObraSocialId);
                ViewBag.PacienteId = new SelectList(from paciente in db.Paciente
                                                    join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                    where paciente.Id == turno.PacienteId
                                                    select new { Id = paciente.Id, Nombre = persona.Apellido + ", " + persona.Nombre }, "Id", "Nombre");
                return View(turno);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index");
            }
        }

        public ActionResult Cancelar(int? id)
        {
            EliminarMensaje();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Turno turno = db.Turno.Find(id);
            if (turno == null)
            {
                return HttpNotFound();
            }
            return View(turno);
        }


        [HttpPost, ActionName("Cancelar")]
        [ValidateAntiForgeryToken]
        public ActionResult CancelarConfirmado(int id)
        {
            EliminarMensaje();
            try
            {
                Turno turno = db.Turno.Find(id);
                string body = "<br/>Su Turno del dia " + turno.Fecha.ToString("yyyy-MM-dd") + " fue cancelado.<br/>Solicite uno nuevo.<br/>";
                string email = (from paciente in db.Paciente
                                                    join persona in db.Persona on paciente.PersonaId equals persona.Id
                                                    where paciente.Id == turno.PacienteId
                                                    select persona).First().Email;
                string titulo = "Turno Cancelado";

                turno.PacienteId = null;
                turno.ObraSocialId = null;
                turno.Descripcion = "";
                turno.EstadoId = 1;

                //db.Entry(turno).State = EntityState.Modified;
                db.SaveChanges();

                this.SendEmail(body, email, titulo);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index");
            }
        }



        public ActionResult CancelarMedico(int? id_medico, int? dni, string apellido, string nombre)
        {
            EliminarMensaje();
            Usuario usuario = (Usuario)Session["user"];
            switch (((Perfil)Session["perfil"]).Nombre)
            {
                case "Medico":
                    id_medico = db.Medico.Where(p => p.PersonaId == usuario.PersonaId).First().Id;
                    break;
                default:
                    break;
            }

            string nuevo_dni = dni.ToString();
            ViewBag.ListadoMedico = (from medico in db.Medico
                                     join persona in db.Persona on medico.PersonaId equals persona.Id
                                     where persona.Apellido.Contains(apellido) && persona.Nombre.Contains(nombre) && persona.DNI.Contains(nuevo_dni)
                                     select medico);
            if (id_medico != null)
            {

                ViewBag.Medico = db.Medico.Where(p => p.Id == id_medico).First();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelarMedico(int? id_medico, DateTime fecha_ini, DateTime fecha_fin)
        {
            EliminarMensaje();
            try
            {
                List<Turno> turno_existente = (from turnos in db.Turno
                                           where turnos.Fecha >= fecha_ini && turnos.Fecha < fecha_fin && turnos.MedicoId == id_medico
                                           select turnos).ToList();
                foreach (Turno turno in turno_existente)
                {
                    if (turno.PacienteId != null) {
                        //Turno turno = db.Turno.Find(id);
                        string body = "<br/>Su Turno del dia " + turno.Fecha.ToString("yyyy-MM-dd") + " fue cancelado.<br/>Solicite uno nuevo.<br/>";
                        string email = (from paciente in db.Paciente
                                        join persona in db.Persona on paciente.PersonaId equals persona.Id
                                        where paciente.Id == turno.PacienteId
                                        select persona).First().Email;
                        string titulo = "Turno Cancelado";
                        SendEmail(body, email, titulo);
                    }

                    turno.PacienteId = null;
                    turno.ObraSocialId = null;
                    turno.Descripcion = "";
                    turno.EstadoId = 1;

                    //db.Entry(turno).State = EntityState.Modified;
                    db.SaveChanges();

                    db.Turno.Remove(turno);
                    db.SaveChanges();

                }
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index");
            }
        }



        // GET: Turnos/Create
        public ActionResult Historial(int id)
        {
            EliminarMensaje();
            PacienteHistorial historial = new PacienteHistorial();
            historial.PacienteId = (int)db.Turno.First(p => p.Id == id).PacienteId;
            historial.Fecha = DateTime.Now;
            historial.TurnoId = id;

            return View(historial);
        }

        // POST: Turnos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Historial([Bind(Include = "Id,PacienteId,TurnoId,Observacion,Fecha")] PacienteHistorial pacienteHistorial)
        {
            EliminarMensaje();
            try
            {
                pacienteHistorial.Fecha = DateTime.Now;
                if (ModelState.IsValid)
                {
                    db.PacienteHistorial.Add(pacienteHistorial);
                    db.SaveChanges();

                    Turno turno = new Turno();
                    turno.Estado = db.Estado.Where(p => p.Nombre == "Concurrio").First();
                    turno.EstadoId = turno.Estado.Id;
                    db.Entry(turno).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.PacienteId = new SelectList(db.Paciente, "Id", "Id", pacienteHistorial.PacienteId);
                ViewBag.TurnoId = new SelectList(db.Turno, "Id", "Descripcion", pacienteHistorial.TurnoId);
                return View(pacienteHistorial);
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Historial", new { id = pacienteHistorial.PacienteId });
            }
        }


        // GET: Turnos
        public ActionResult Presencialidad(int? dni, string apellido, string nombre)
        {
            EliminarMensaje();
            DateTime? fecha_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            fecha_ini = fecha_ini.Value.AddDays(-1);
            DateTime? fecha_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            fecha_fin = fecha_fin.Value.AddDays(1);

            var turno = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente)
                                                                        .Where(p => (p.Estado.Nombre == "Asignado" || p.Estado.Nombre == "Concurrio" || p.Estado.Nombre == "No Concurrio") 
                                                                        && p.Fecha >= fecha_ini && p.Fecha < fecha_fin);
            string nuevo_dni = dni.ToString();
            if (dni != null || !(apellido == "" || apellido == null) || !(nombre == "" || nombre == null))
            {
                turno = turno.Where(p => p.Paciente.Persona.DNI.Contains(nuevo_dni)
                    && p.Paciente.Persona.Apellido.Contains(apellido)
                    && p.Paciente.Persona.Nombre.Contains(nombre));
            }
            return View(turno.ToList());
        }


        // GET: Turnos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Presencialidad(int id, int estado)
        {
            EliminarMensaje();
            try
            {
                Turno turno = db.Turno.Where(p => p.Id == id).First();
                Estado estados = db.Estado.Where(p => p.Id == estado).First();
                turno.EstadoId = estados.Id;
                if (!turno.Id.Equals(null))
                {
                    db.Entry(turno).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Presencialidad");
                }
                return View();
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Presencialidad");
            }
        }


    }
}
