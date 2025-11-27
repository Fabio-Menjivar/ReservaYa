using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservaYa.Models.Extras
{
    public class ReservasFechaDisponiblesViewModel
    {
        public int EspacioId { get; set; }
        public string EspacioName { get; set; }

        public List<ReservaFechaDisponibleItemVM> Fechas { get; set; }
    }

}