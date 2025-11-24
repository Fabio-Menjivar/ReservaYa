using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ReservaYa.Models
{
    // Modelo para representar una reserva en la vista de gestión
    public class ReservaUsuarioViewModel
    {
        [Display(Name = "ID Reserva")]
        public int ReservaID { get; set; }

        [Display(Name = "Espacio Reservado")]
        public string NombreEspacio { get; set; }

        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaReserva { get; set; }

        [Display(Name = "Hora Inicio")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm tt}")]
        public TimeSpan HoraInicio { get; set; }

        [Display(Name = "Hora Fin")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm tt}")]
        public TimeSpan HoraFin { get; set; }

        [Display(Name = "Monto Total")]
        [DisplayFormat(DataFormatString = "{0:C}")] // Formato de moneda
        public decimal MontoTotal { get; set; }

        // Propiedad para la lógica de visualización (por ejemplo, permitir cancelar)
        public bool EsPasada { get; set; }
       

    }
}