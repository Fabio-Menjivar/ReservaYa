using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservaYa.Models.Extras
{
    public class ReservaFechaDisponibleItemVM
    {
        public int ReservaFechaID { get; set; }
        public int FechaDisponibleID { get; set; }

        // Información de la fecha
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public string Tags { get; set; }
        public bool Disponible { get; set; }
    }
}