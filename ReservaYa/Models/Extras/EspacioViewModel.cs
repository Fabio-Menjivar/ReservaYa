using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReservaYa.Models.Extras
{
    public class EspacioViewModel
    {
            public int EspacioID { get; set; }
            [Required]
            [MaxLength(50, ErrorMessage = "Longitud maxima excedida")]
            public string Nombre { get; set; }
            [Required]
            [Display(Name = "Categoria")]    
        public int CategoriaID { get; set; }
            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "La capacidad debe ser mayor que 0")]
            //[MinLength(1,ErrorMessage ="Almenos 1 debe contener")] // solo funciona con listas u array , esto da error en valores 'normales'
            [Display(Name = "Capacidad Máxima")] //Hacer lo mismo con los demas
            public int Capacidad { get; set; }
            [Required]
            [MaxLength(100, ErrorMessage = "Longitud maxima excedida")]
            public string Direccion { get; set; }
            [Url(ErrorMessage = "Enlace no valido")]
            public string UbicacionEnlace { get; set; }
            public bool Estacionamiento { get; set; }
            public bool Sanitarios { get; set; }
            public bool AccesoSillaRuedas { get; set; }
            public string ImagenPrev { get; set; }
            [Display(Name = "Disponible?")]
            public bool Disponible { get; set; }

            // extras solo para la vista
            public string EspacioIdCifrado { get; set; }            
    }
}