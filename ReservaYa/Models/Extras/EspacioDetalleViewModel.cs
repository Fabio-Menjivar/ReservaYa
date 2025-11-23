using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ReservaYa.Models.Extras
{
    public class EspacioDetalleViewModel
    {
        public string Nombre { get; set; }
        public string IdEspacioEncriptada { get; set; }
        [Required]
        [DataType(DataType.Currency,ErrorMessage ="Ingrese un valor valido")]
        public decimal ValorXHora { get; set; }
    }
}