using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Turnos_Medicos.Models
{
    public class UsuarioLogin
    {
        [Display(Name = "Usuario")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Usuario required")]
        public string Usuario { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}