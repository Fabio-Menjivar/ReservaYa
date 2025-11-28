// CambiarCredencialesViewModel.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace ReservaYa.Models
{
    public class CambiarCredencialesViewModel
    {
        // -------- CORREO ACTUAL -------------
        [Required(ErrorMessage = "El campo {0} es obligatorio para realizar cualquier cambio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [Display(Name = "Correo Actual (Lectura)")]
        // El Correo Actual es obligatorio (para mostrarlo)
        public string CorreoActual { get; set; }

        // -------- NUEVO CORREO (Opcional) ---------
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [Display(Name = "Nuevo Correo")]
        
        public string NuevoCorreo { get; set; }

        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [Compare("NuevoCorreo", ErrorMessage = "Los correos no coinciden.")]
        [Display(Name = "Confirmar Nuevo Correo")]
        
        public string ConfirmarCorreo { get; set; }

        // -------- CONTRASEÑA ACTUAL ---------
        // CRÍTICO: Se requiere la contraseña actual para autenticar el cambio
        [Required(ErrorMessage = "El campo {0} es obligatorio para realizar cualquier cambio.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string ContrasenaActual { get; set; }

        // -------- NUEVA CONTRASEÑA (Opcional) ---------
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "La {0} debe tener al menos {1} caracteres.")]
        [Display(Name = "Nueva Contraseña")]
        
        public string NuevaContrasena { get; set; }

        [DataType(DataType.Password)]
        [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar Nueva Contraseña")]
       
        public string ConfirmarContrasena { get; set; }
    }
}