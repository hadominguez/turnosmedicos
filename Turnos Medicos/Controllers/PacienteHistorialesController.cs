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
    public class PacienteHistorialesController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();

        // GET: PacienteHistoriales
        public ActionResult Index(int id, DateTime? fecha_ini, DateTime? fecha_fin, DateTime? fecha_ini_otro, DateTime? fecha_otro_fin)
        {
            EliminarMensaje();
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

    }
}
