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
                if (fecha_ini.Equals(null) || fecha_fin.Equals(null))
                {
                    fecha_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    fecha_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    fecha_fin = fecha_fin.Value.AddDays(31);
                }
                var turnos = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin).OrderBy(p => p.Fecha).ThenBy(p => p.Hora);
                return View(turnos.ToList());
        }

        /*
                [HttpPost]
                [ValidateAntiForgeryToken]
                public ActionResult Index(DateTime fecha_ini, DateTime fecha_fin)
                {
                        //return RedirectToAction( "Index" , new RouteValueDictionary(new { Controller = "Agenda", Action = "Index", fecha_ini = fecha_ini, fecha_fin = fecha_fin  }));
                        return RedirectToAction("Index", "Agenda", new { fecha_ini = fecha_ini, fecha_fin = fecha_fin });
                }
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string Nombre_Medico, DateTime? fecha_ini, DateTime? fecha_fin)
        {
            /*
            var turnoslist = (from turnos in db.Turno
                              where (turnos.Medico.Persona.Nombre == Nombre_Medico ||
                                    turnos.Medico.Persona.Apellido == Nombre_Medico) &&
                                    turnos.Estado.Nombre == "Asignado"
                                    select turnos).ToList();
                                    */
            if (fecha_ini.Equals(null) || fecha_fin.Equals(null))
            {
                fecha_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                fecha_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                fecha_fin = fecha_fin.Value.AddDays(31);
            }

            var turno = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente);

            turno = turno.Where(p => p.Medico.Persona.Nombre.Contains(Nombre_Medico) || p.Medico.Persona.Apellido.Contains(Nombre_Medico));

            var turno2 = turno.Where(p => p.Estado.Nombre == "Asignado");

            var turno3 = turno2.Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin).OrderBy(p => p.Fecha).ThenBy(p => p.Hora);

            return View(turno3.ToList());
            
            //return RedirectToAction( "Index" , new RouteValueDictionary(new { Controller = "Agenda", Action = "Index", fecha_ini = fecha_ini, fecha_fin = fecha_fin  }));
            //return RedirectToAction("Index", "Agenda", new { fecha_ini = fecha_ini, fecha_fin = fecha_fin });
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DateTime fecha_ini, DateTime fecha_fin)
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

        public ActionResult Busqueda(string Nombre_Medico)
        {
            var turno = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente);

            turno = turno.Where(p => p.Medico.Persona.Nombre.Contains(Nombre_Medico));
            return View(turno.ToList());

        }


        public ActionResult Calendar()
        {
                /*  var id = 1;*/
                var turnoslist = (from turnos in db.Turno
                                  where turnos.Estado.Nombre == "Asignado"
                                  select turnos).ToList();

                return Json(turnoslist.ToList().AsEnumerable().Select(e => new
                {

                    title = e.Descripcion,
                    start = e.Fecha.ToString("yyyy-MM-dd"),
                    color = "yellow",
                    textColor = "black",
                    especialidad = e.EspecialidadId
                    /*
                    start = e.Fecha.GetDateTimeFormats("yyyy-MM-dd HH:mm:ss")
                    */
                }), JsonRequestBehavior.AllowGet);
        }

        


    }

}