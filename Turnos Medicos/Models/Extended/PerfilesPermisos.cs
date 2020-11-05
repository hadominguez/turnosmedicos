using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Turnos_Medicos.Models.Extended
{
    public class PerfilesPermisos
    {
        public int? Id { get; set; }
        public int? Permiso { get; set; }
        public string Descripcion { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
    }
}