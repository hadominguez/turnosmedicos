using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Turnos_Medicos.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.IO;

namespace Turnos_Medicos.Controllers
{
    [SessionCheck]
    public class LiquidacionesObraSocialController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();
        // GET: LiquidacionesObraSocial
        public ActionResult Index(DateTime? fecha_ini, DateTime? fecha_fin, string obra_social, int? id_obra_social)
        {
            EliminarMensaje();
            try
            {
                ViewBag.ObraSocial = db.ObraSocial.Where(p => p.Nombre != null).ToList();
                if (!(obra_social == "") && !(obra_social == null))
                {
                    ViewBag.ObraSocial = db.ObraSocial.Where(p => p.Nombre.Contains(obra_social)).ToList();
                }

                if (id_obra_social != null)
                {
                    ViewBag.ObraSocialCargada = id_obra_social;
                }

                if (fecha_ini.Equals(null) || fecha_fin.Equals(null))
                {
                    fecha_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    fecha_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    fecha_ini = fecha_ini.Value.AddDays(-31);
                    fecha_fin = fecha_fin.Value.AddDays(1);
                }
                ViewBag.Fecha_Ini = fecha_ini;
                ViewBag.Fecha_Fin = fecha_fin;
                var turnos = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin && p.Estado.Nombre == "Concurrio" && p.ObraSocialId == id_obra_social).OrderBy(p => p.Fecha).ThenBy(p => p.Hora);
                return View(turnos.ToList());
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index");
            }
        }

        public FileResult PrintPDF(DateTime? fecha_ini, DateTime? fecha_fin, int id_obra_social)
        {
            try
            {
                if (fecha_ini.Equals(null) || fecha_fin.Equals(null))
                {
                    fecha_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    fecha_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    fecha_ini = fecha_ini.Value.AddDays(-31);
                    fecha_fin = fecha_fin.Value.AddDays(1);
                }
                ViewBag.Fecha_Ini = fecha_ini;
                ViewBag.Fecha_Fin = fecha_fin;

                const string path = "~/Content/Template/Liquidacion.html";
                var contents = System.IO.File.ReadAllText(Server.MapPath(path));

                var shtml = string.Empty;

                shtml += $"<div><h1><strong><u>{db.ObraSocial.Where(p => p.Id == id_obra_social).First().Nombre}</u></strong></h1>";
                var medicoObraSociales = db.MedicoObraSocial.Where(p => p.ObraSocialId == id_obra_social);
                foreach (var medicoObraSocial in medicoObraSociales)
                {
                    shtml += $"<h3><strong><u>{medicoObraSocial.Medico.Persona.Apellido}, {medicoObraSocial.Medico.Persona.Nombre} - {medicoObraSocial.Medico.Matricula}</u></strong></h3>";
                    shtml += $"<table class='tableHead' width='100%' style='border-collapse:collapse;'><thead><tr><th>DNI</th><th>Apellido</th><th>Nombre</th><th>Nro. de Afiliado</th><th>Fecha</th><th>Costo</th></tr></thead><tbody>";
                    var turnos = db.Turno.Where(p => p.MedicoId == medicoObraSocial.MedicoId && p.ObraSocialId == medicoObraSocial.ObraSocialId &&
                                                p.Fecha >= fecha_ini && p.Fecha < fecha_fin && p.Estado.Nombre == "Concurrio" && p.ObraSocialId == id_obra_social).ToList();
                    if (!(turnos.Count >= 1))
                    {
                        shtml += "<tr><td style='border:1px solid black;' colspan='6'>Sin Turnos</td></tr>";
                    }
                    else
                    {
                        turnos.OrderBy(p => p.Fecha);
                        foreach (var turno in turnos)
                        {
                            shtml += $"<tr><td style='border:1px solid black;' width='20%'>{turno.Paciente.Persona.DNI}</td>";
                            shtml += $"<td style='border:1px solid black;' width='20%'>{turno.Paciente.Persona.Apellido}</td>";
                            shtml += $"<td style='border:1px solid black;' width='20%'>{turno.Paciente.Persona.Nombre}</td>";
                            shtml += $"<td style='border:1px solid black;' width='20%'>{turno.Paciente.PacienteObraSocial.Where(p => p.ObraSocialId == turno.ObraSocialId).First().NumeroAfiliado}</td>";
                            shtml += $"<td style='border:1px solid black;' width='20%'>{turno.Fecha}</td>";
                            shtml += $"<td style='border:1px solid black;' width='20%'>{turno.CostoTotal}</td></tr>";
                        }
                    }
                    shtml += "</tbody></table>";
                }
                shtml += "</div>";
                contents = contents.Replace("$cuadros", shtml);
                using (var stream = new MemoryStream())
                {
                    var sr = new StringReader(contents);
                    var pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    pdfDoc.SetPageSize(PageSize.A4);
                    var writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    pdfDoc.Close();
                    return File(stream.ToArray(), "application/pdf", "PDI.pdf");
                }
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                EliminarMensaje();
                var stream = new MemoryStream();
                return File(stream.ToArray(), "application/pdf", "PDI.pdf");
            }
        }




        public ActionResult EnviarLiquidacion(DateTime? fecha_ini, DateTime? fecha_fin, int id_obra_social)
        {
            try {
                if (fecha_ini.Equals(null) || fecha_fin.Equals(null))
                {
                    fecha_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    fecha_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                    fecha_ini = fecha_ini.Value.AddDays(-31);
                    fecha_fin = fecha_fin.Value.AddDays(1);
                }
                ViewBag.Fecha_Ini = fecha_ini;
                ViewBag.Fecha_Fin = fecha_fin;

                const string path = "~/Content/Template/Liquidacion.html";
                var contents = System.IO.File.ReadAllText(Server.MapPath(path));

                var shtml = string.Empty;
                ObraSocial obras = db.ObraSocial.Where(p => p.Id == id_obra_social).First();
                shtml += $"<div><h1><strong><u>{obras.Nombre}</u></strong></h1>";
                var medicoObraSociales = db.MedicoObraSocial.Where(p => p.ObraSocialId == id_obra_social);
                foreach (var medicoObraSocial in medicoObraSociales)
                {
                    shtml += $"<h3><strong><u>{medicoObraSocial.Medico.Persona.Apellido}, {medicoObraSocial.Medico.Persona.Nombre} - {medicoObraSocial.Medico.Matricula}</u></strong></h3>";
                    shtml += $"<table class='tableHead' width='100%' style='border-collapse:collapse;'><thead><tr><th>DNI</th><th>Apellido</th><th>Nombre</th><th>Nro. de Afiliado</th><th>Fecha</th><th>Costo</th></tr></thead><tbody>";
                    var turnos = db.Turno.Where(p => p.MedicoId == medicoObraSocial.MedicoId && p.ObraSocialId == medicoObraSocial.ObraSocialId &&
                                                p.Fecha >= fecha_ini && p.Fecha < fecha_fin && p.Estado.Nombre == "Concurrio" && p.ObraSocialId == id_obra_social).ToList();
                    if (!(turnos.Count >= 1))
                    {
                        shtml += "<tr><td style='border:1px solid black;' colspan='6'>Sin Turnos</td></tr>";
                    }
                    else
                    {
                        turnos.OrderBy(p => p.Fecha);
                        foreach (var turno in turnos)
                        {
                            shtml += $"<tr><td style='border:1px solid black;' width='20%'>{turno.Paciente.Persona.DNI}</td>";
                            shtml += $"<td style='border:1px solid black;' width='20%'>{turno.Paciente.Persona.Apellido}</td>";
                            shtml += $"<td style='border:1px solid black;' width='20%'>{turno.Paciente.Persona.Nombre}</td>";
                            shtml += $"<td style='border:1px solid black;' width='20%'>{turno.Paciente.PacienteObraSocial.Where(p => p.ObraSocialId == turno.ObraSocialId).First().NumeroAfiliado}</td>";
                            shtml += $"<td style='border:1px solid black;' width='20%'>{turno.Fecha.ToString("dd-MM-yyyy")}</td>";
                            shtml += $"<td style='border:1px solid black;' width='20%'>{turno.CostoTotal}</td></tr>";
                        }
                    }
                    shtml += "</tbody></table>";
                }
                shtml += "</div>";
                contents = contents.Replace("$cuadros", shtml);
                contents = contents.Replace("$fecha", DateTime.Now.ToString("dd-MM-yyyy"));

                var otro = new MemoryStream();
                const string path_mail = "~/Content/Template/EmailLiquidacion.html";
                var contents_mail = System.IO.File.ReadAllText(Server.MapPath(path_mail));

                using (var stream = new MemoryStream())
                {
                    var sr = new StringReader(contents);
                    var pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    pdfDoc.SetPageSize(PageSize.A4);
                    var writer = PdfWriter.GetInstance(pdfDoc, stream);
                    pdfDoc.Open();
                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    pdfDoc.Close();
                    otro = new MemoryStream(stream.ToArray());
                }

                SendEmailAdjunto(contents_mail, obras.Email, "Liquidacion - MedOffices", otro, "Liquidacion.pdf");

                MandarMensaje("Email Enviado", "Success");

                return RedirectToAction("Index", new { fecha_ini, fecha_fin, id_obra_social });
            
            }
            catch (Exception e)
            {
                MandarMensaje(e.Message, "Error");
                return RedirectToAction("Index");
            }
        }
    }
}
