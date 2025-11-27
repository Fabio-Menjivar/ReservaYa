using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace ReservaYa.Models
{
    public class CambiarCredencialesViewModel
    {
        // -------- CORREO -------------
        [Required]
        [EmailAddress]
        [Display(Name = "Correo Actual")]
        public string CorreoActual { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Nuevo Correo")]
        public string NuevoCorreo { get; set; }

        [Required]
        [EmailAddress]
        [Compare("NuevoCorreo", ErrorMessage = "Los correos no coinciden.")]
        [Display(Name = "Confirmar Nuevo Correo")]
        public string ConfirmarCorreo { get; set; }

        // -------- CONTRASEÑA ---------
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string ContrasenaActual { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6)]
        [Display(Name = "Nueva Contraseña")]
        public string NuevaContrasena { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar Nueva Contraseña")]
        public string ConfirmarContrasena { get; set; }
    }
}