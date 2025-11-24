// 📁 Models/ReservaEspaciosModelo.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class ReservaEspaciosModelo
{
    // === PROPIEDADES DEL FORMULARIO (Inputs) ===

    public int? EspacioIDSeleccionado { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [Display(Name = "Fecha de Reserva")]
    [DataType(DataType.Date)]
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "La hora es obligatoria.")]
    [Display(Name = "Hora de Inicio")]
    [DataType(DataType.Time)]
    public TimeSpan Hora { get; set; } // Representa HoraInicio

    // PROPIEDAD AÑADIDA: Hora de Fin
    [Required(ErrorMessage = "La hora de fin es obligatoria.")]
    [Display(Name = "Hora de Fin")]
    [DataType(DataType.Time)]
    public TimeSpan HoraFin { get; set; }


    [Required(ErrorMessage = "Debe especificar el número de personas.")]
    [Range(1, 1000, ErrorMessage = "La capacidad mínima es 1 persona.")]
    [Display(Name = "Personas")]
    public int Personas { get; set; }


    // === PROPIEDADES PARA LA LISTA DE ESPACIOS (Outputs) ===

    public class EspacioCard
    {
        public int EspacioID { get; set; }
        public string Nombre { get; set; }
        public int Capacidad { get; set; }
        public string ImagenPrevUrl { get; set; }
    }

    public List<EspacioCard> EspaciosDisponibles { get; set; } = new List<EspacioCard>();
}