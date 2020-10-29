//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Turnos_Medicos.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Turno
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Turno()
        {
            this.PacienteHistorial = new HashSet<PacienteHistorial>();
        }
    
        public int Id { get; set; }
        public int EstadoId { get; set; }
        public int MedicoId { get; set; }
        public Nullable<int> PacienteId { get; set; }
        public Nullable<int> ObraSocialId { get; set; }
        public int ConsultorioId { get; set; }
        public int EspecialidadId { get; set; }
        public System.DateTime Fecha { get; set; }
        public System.TimeSpan Hora { get; set; }
        public string Descripcion { get; set; }
        public Nullable<decimal> ObraSocialTarifa { get; set; }
        public Nullable<decimal> CostoTotal { get; set; }
        public Nullable<bool> Pagado { get; set; }
    
        public virtual Consultorio Consultorio { get; set; }
        public virtual Especialidad Especialidad { get; set; }
        public virtual Estado Estado { get; set; }
        public virtual Medico Medico { get; set; }
        public virtual ObraSocial ObraSocial { get; set; }
        public virtual Paciente Paciente { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PacienteHistorial> PacienteHistorial { get; set; }
    }
}
