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
    public class AgendaController : Controller
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();
        // GET: Agenda
        public ActionResult Index(DateTime? fecha_ini, DateTime? fecha_fin)
        {
            if (Session["user"] != null)
            {
                if (fecha_ini.Equals(null) || fecha_fin.Equals(null))
                {
                    fecha_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    fecha_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    fecha_fin = fecha_fin.Value.AddDays(31);
                }
                var turnos = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin).OrderBy(p => p.Fecha).ThenBy(p => p.Hora);
                return View(turnos.ToList());
            }
            return RedirectToAction("Login", "Usuarios");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(DateTime fecha_ini, DateTime fecha_fin)
        {
            if (Session["user"] != null)
            {
                return RedirectToAction( "Index" , new RouteValueDictionary(new { Controller = "Index", Action = "Index", fecha_ini = fecha_ini, fecha_fin = fecha_fin  }));
            }
            return RedirectToAction("Login", "Usuarios");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DateTime fecha_ini, DateTime fecha_fin)
        {
            if (Session["user"] != null)
            {
                for (DateTime c = fecha_ini; c <= fecha_fin; c = c.AddDays(1))
                {
                    DateTime fin = c;
                    fin = fin.AddDays(1);
                    List <Turno> turno_existente = (from turnos in db.Turno
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
                                    db.Turno.Add(turno);
                                    db.SaveChanges();
                                }
                                TimeSpan minutos = new TimeSpan(0, especialidad.Tiempo, 0);
                                hora_carga = hora_carga.Add(minutos);
                            }
                        }
                    }
                }

                return RedirectToAction("Index", "Agenda");

             }
            return RedirectToAction("Login", "Usuarios");
        }
    }
}