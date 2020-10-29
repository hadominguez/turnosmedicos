using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Turnos_Medicos.Models;
using Turnos_Medicos.Controllers;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Xml;
using System.IO;
using System.Drawing;

namespace Turnos_Medicos.Controllers
{
    public class LiquidacionesController : BaseController
    {
        private TurnosMedicosEntities db = new TurnosMedicosEntities();
        // GET: Liquidaciones
        public ActionResult Index(DateTime? fecha_ini, DateTime? fecha_fin)
        {
            if (fecha_ini.Equals(null) || fecha_fin.Equals(null))
            {
                fecha_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                fecha_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                fecha_ini = fecha_ini.Value.AddDays(-31);
                fecha_fin = fecha_fin.Value.AddDays(1);
            }
            var turnos = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin && p.Estado.Nombre == "Concurrio").OrderBy(p => p.Fecha).ThenBy(p => p.Hora);
            return View(turnos.ToList());
        }


        public ActionResult Medico(DateTime? fecha_ini, DateTime? fecha_fin)
        {
            if (fecha_ini.Equals(null) || fecha_fin.Equals(null))
            {
                fecha_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                fecha_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                fecha_ini = fecha_ini.Value.AddDays(-31);
                fecha_fin = fecha_fin.Value.AddDays(1);
            }
            var turnos = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin && p.Estado.Nombre == "Concurrio").OrderBy(p => p.Fecha).ThenBy(p => p.Hora);
            return View(turnos.ToList());
        }



        public ActionResult PrintPDF()
        {

            DateTime? fecha_ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime? fecha_fin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                fecha_ini = fecha_ini.Value.AddDays(-31);
                fecha_fin = fecha_fin.Value.AddDays(1);
            var turnos = db.Turno.Include(t => t.Consultorio).Include(t => t.Especialidad).Include(t => t.Estado).Include(t => t.Medico).Include(t => t.ObraSocial).Include(t => t.Paciente).Where(p => p.Fecha >= fecha_ini && p.Fecha < fecha_fin && p.PacienteId != null).OrderBy(p => p.Fecha).ThenBy(p => p.Hora);
            List<Turno> Data = turnos.ToList();

            MemoryStream ms = new MemoryStream();
            Document doc = new Document(PageSize.A4, 0f, 0f, 0f, 0f);
            doc.SetPageSize(PageSize.A4);
            doc.SetMargins(20f, 20f, 20f, 20f);
            PdfPTable _pdfTable = new PdfPTable(3);
            _pdfTable.WidthPercentage = 100;
            _pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            iTextSharp.text.Font _fontStyle = FontFactory.GetFont("Arial", 8f, 1);
            PdfWriter.GetInstance(doc, ms);
            doc.Open();
            doc.NewPage();
            _pdfTable.SetWidths(new float[] { 20f, 150, 100f });


            /* Columna 1 */
            PdfPCell _pdfPCell = new PdfPCell(new Phrase("Apellido", _fontStyle))
            {
                Colspan = 3,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = 0,
                BackgroundColor = BaseColor.WHITE,
                ExtraParagraphSpace = 0
            };
            _pdfTable.AddCell(_pdfPCell);
            _pdfTable.CompleteRow();

            /* Columna 2 */
            _pdfPCell = new PdfPCell(new Phrase("Nombre", _fontStyle))
            {
                Colspan = 3,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = 0,
                BackgroundColor = BaseColor.WHITE,
                ExtraParagraphSpace = 0
            };
            _pdfTable.AddCell(_pdfPCell);
            _pdfTable.CompleteRow();


            /* Columna 3 */
            _pdfPCell = new PdfPCell(new Phrase("Especialidad", _fontStyle))
            {
                Colspan = 3,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = 0,
                BackgroundColor = BaseColor.WHITE,
                ExtraParagraphSpace = 0
            };
            _pdfTable.AddCell(_pdfPCell);
            _pdfTable.CompleteRow();


            /*foreach (Turno turno_s in turnos)
            {
                _pdfPCell = new PdfPCell(new Phrase(turno_s.Paciente.Persona.Apellido, _fontStyle));
                _pdfTable.AddCell(_pdfPCell);
                _pdfPCell = new PdfPCell(new Phrase(turno_s.Paciente.Persona.Nombre, _fontStyle));
                _pdfTable.AddCell(_pdfPCell);
                _pdfPCell = new PdfPCell(new Phrase(turno_s.Especialidad.Nombre, _fontStyle));
                _pdfTable.AddCell(_pdfPCell);

                _pdfTable.CompleteRow();
            }*/

            Byte[] byteS = ms.ToArray();
            doc.Close();

            return  File(byteS, "application/pdf");





        }

    }
}
