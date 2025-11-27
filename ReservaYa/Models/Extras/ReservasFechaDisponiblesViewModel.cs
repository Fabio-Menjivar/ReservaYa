using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservaYa.Models.Extras
{
    public class ReservasFechaDisponiblesViewModel
    {
        public string EspacioIdCifrado { get; set; }
        public string EspacioName { get; set; }

        public List<ReservaFechaDisponibleItemVM> Fechas { get; set; }
    }

}