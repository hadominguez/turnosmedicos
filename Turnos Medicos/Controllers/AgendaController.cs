using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Turnos_Medicos.Models;

namespace Turnos_Medicos.Controllers
{
    [SessionCheck]
    public class AgendaController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();
        // GET: Agenda
        public ActionResult Index(DateTime? fecha_ini, DateTime? fecha_fin)
        {
            EliminarMensaje();
            if (fecha_ini.Equals(null) || fecha_fin.Equals(null))
            {
                fecha_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                fecha_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                fecha_fin = fecha_fin.Value.AddDays(31);
            }
            var turnos = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin).OrderBy(p => p.Fecha).ThenBy(p => p.Hora);

            /*if (((Perfil)Session["perfil"]).Nombre == "Paciente")
            {
               turnos = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin && p.Estado.Nombre == "Pendiente").OrderBy(p => p.Fecha).ThenBy(p => p.Hora);
            }*/

            Usuario usuario = (Usuario)Session["user"];
            switch (((Perfil)Session["perfil"]).Nombre)
            {
                case "Paciente":
                    turnos = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin && p.Estado.Nombre == "Pendiente").OrderBy(p => p.Fecha).ThenBy(p => p.Hora);
                    break;
                case "Medico":
                    turnos = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin && p.Medico.Persona.Id == usuario.PersonaId ).OrderBy(p => p.Fecha).ThenBy(p => p.Hora);
                    break;
                default:
                    break;
            }

            return View(turnos.ToList());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string m_apellido, string m_nombre, string especialidad, string estado, DateTime? fecha_ini, DateTime? fecha_fin)
        {
            EliminarMensaje();

            if (fecha_ini.Equals(null) || fecha_fin.Equals(null))
            {
                fecha_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                fecha_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                fecha_fin = fecha_fin.Value.AddDays(31);
            }

            var turno = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin).OrderBy(p => p.Fecha).ThenBy(p => p.Hora).ToList();

            /*if (((Perfil)Session["perfil"]).Nombre == "Paciente")
            {
                turno = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin && p.Estado.Nombre == "Pendiente").OrderBy(p => p.Fecha).ThenBy(p => p.Hora).ToList();
            }*/


            Usuario usuario = (Usuario)Session["user"];
            switch (((Perfil)Session["perfil"]).Nombre)
            {
                case "Paciente":
                    turno = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin && p.Estado.Nombre == "Pendiente").OrderBy(p => p.Fecha).ThenBy(p => p.Hora).ToList();
                    break;
                case "Medico":
                    turno = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin && p.Medico.Persona.Id == usuario.PersonaId).OrderBy(p => p.Fecha).ThenBy(p => p.Hora).ToList();
                    break;
                default:
                    break;
            }


            if (!(m_apellido == "") || !(m_nombre == "") || !(especialidad == "") || !(estado == ""))
            {
                turno = turno.Where(p => p.Medico.Persona.Apellido.Contains(m_apellido)
                    && p.Medico.Persona.Nombre.Contains(m_nombre)
                    && p.Especialidad.Nombre.Contains(especialidad)
                    && p.Estado.Nombre.Contains(estado)).ToList();
            }
           
            return View(turno);
        }



        public ActionResult Create()
        {
            EliminarMensaje();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DateTime fecha_ini, DateTime fecha_fin)
        {
            EliminarMensaje();
            try
            {
                int contar_cargado = 0;
                var medicos = db.Medico.Include(m => m.Especialidad).Include(m => m.Persona);

                for (DateTime c = fecha_ini; c <= fecha_fin; c = c.AddDays(1))
                {
                    DateTime fin = c;
                    fin = fin.AddDays(1);
                    List<Turno> turno_existente = (from turnos in db.Turno
                                                   where turnos.Fecha >= c && turnos.Fecha < fin
                                                   select turnos).ToList();
                    if (turno_existente.Count == 0)
                    {
                        var dia = (int)c.DayOfWeek;
                        List<MedicoHorario> medi_horario = (from horas in db.MedicoHorario
                                                            where horas.Dia == dia
                                                            select horas).ToList();
                        foreach (MedicoHorario hora in medi_horario)
                        {
                            Medico medico = db.Medico.Single(p => p.Id == hora.MedicoId);
                            Especialidad especialidad = db.Especialidad.Single(p => p.Id == medico.EspecialidadId);
                            TimeSpan hora_carga = hora.Inicio;
                            while(hora_carga <= hora.Fin)
                            {
                                Turno turno = new Turno();
                                turno.ConsultorioId = hora.ConsultorioId;
                                turno.EspecialidadId = especialidad.Id;
                                turno.EstadoId = 1;
                                turno.MedicoId = hora.MedicoId;
                                turno.ObraSocialId = null;
                                turno.PacienteId = null;
                                turno.Fecha = c;
                                turno.Hora = hora_carga;
                                turno.Descripcion = "";
                                turno.ObraSocialTarifa = null;
                                turno.CostoTotal = null;
                                turno.Pagado = false;
                                if (ModelState.IsValid)
                                {
                                    contar_cargado = contar_cargado + 1;
                                    db.Turno.Add(turno);
                                    db.SaveChanges();
                                }
                                TimeSpan minutos = new TimeSpan(0, especialidad.Tiempo, 0);
                                hora_carga = hora_carga.Add(minutos);
                            }
                        }
                    }
                }
                if (contar_cargado >= 1)
                {
                    MandarMensaje("Turnos creados", "Success");
                }
                else
                {
                    MandarMensaje("No hubo turnos para crear", "Warning");
                }
                return RedirectToAction("Index", "Agenda");
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", "Agenda");
            }
        }

        public ActionResult CreateMedico(int? id_medico, int? dni, string apellido, string nombre)
        {
            EliminarMensaje();
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
        public ActionResult CreateMedico(int? id_medico, DateTime fecha_ini, DateTime fecha_fin)
        {
            EliminarMensaje();
            try
            {
                int contar_cargado = 0;
                for (DateTime c = fecha_ini; c <= fecha_fin; c = c.AddDays(1))
                {
                    DateTime fin = c;
                    fin = fin.AddDays(1);
                    List<Turno> turno_existente = (from turnos in db.Turno
                                                   where turnos.Fecha >= c && turnos.Fecha < fin
                                                   select turnos).ToList();
                    if (turno_existente.Count == 0)
                    {
                        var dia = (int)c.DayOfWeek;
                        List<MedicoHorario> medi_horario = (from horas in db.MedicoHorario
                                                            where horas.Dia == dia
                                                            && horas.MedicoId == id_medico
                                                            select horas).ToList();
                        foreach (MedicoHorario hora in medi_horario)
                        {
                            Medico medico = db.Medico.Single(p => p.Id == hora.MedicoId && p.Id == id_medico);
                            Especialidad especialidad = db.Especialidad.Single(p => p.Id == medico.EspecialidadId);
                            TimeSpan hora_carga = hora.Inicio;
                            while (hora_carga <= hora.Fin)
                            {
                                Turno turno = new Turno();
                                turno.ConsultorioId = hora.ConsultorioId;
                                turno.EspecialidadId = especialidad.Id;
                                turno.EstadoId = 1;
                                turno.MedicoId = hora.MedicoId;
                                turno.ObraSocialId = null;
                                turno.PacienteId = null;
                                turno.Fecha = c;
                                turno.Hora = hora_carga;
                                turno.Descripcion = "";
                                turno.ObraSocialTarifa = null;
                                turno.CostoTotal = null;
                                turno.Pagado = false;
                                if (ModelState.IsValid)
                                {
                                    contar_cargado = contar_cargado + 1;
                                    db.Turno.Add(turno);
                                    db.SaveChanges();
                                }
                                TimeSpan minutos = new TimeSpan(0, especialidad.Tiempo, 0);
                                hora_carga = hora_carga.Add(minutos);
                            }
                        }
                    }
                }
                if (contar_cargado >= 1)
                {
                    MandarMensaje("Turnos creados", "Success");
                }
                else
                {
                    MandarMensaje("No hubo turnos para crear", "Warning");
                }
                return RedirectToAction("Index", "Agenda");
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index", "Agenda");
            }
        }

    }

}