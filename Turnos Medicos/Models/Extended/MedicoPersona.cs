using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Turnos_Medicos.Models.Extended
{
    public class MedicoPersona
    {
        public int Id { get; set; }
        public int PersonaId { get; set; }
        public string DNI { get; set; }
        public string Apellido { get; set; }
        public string Nombre { get; set; }
        public System.DateTime FechaNacimiento { get; set; }
        public string Calle { get; set; }
        public int Numero { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public int Matricula { get; set; }
        public int EspecialidadId { get; set; }
        public virtual Especialidad Especialidad { get; set; }
    }
}