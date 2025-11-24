using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using ReservaYa.Models;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ReservaYa.Controllers
{
    public class ReservaEspacioController : Controller
    {
        private DEVELOSERSEntities db = new DEVELOSERSEntities();

        // Función auxiliar para cargar las tarjetas de espacios
        private List<ReservaEspaciosModelo.EspacioCard> CargarEspaciosDisponibles()
        {
            return db.Espacios
              .Where(e => e.Disponible == true)
              .Select(e => new ReservaEspaciosModelo.EspacioCard
              {
                  EspacioID = e.EspacioID,
                  Nombre = e.Nombre,
                  Capacidad = e.Capacidad,
                  ImagenPrevUrl = e.ImagenPrev
              })
              .ToList();
        }

        // Acción GET: Muestra el formulario de reserva
        public ActionResult ReservaEspacioVista()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var espacios = CargarEspaciosDisponibles();
            var viewModel = new ReservaEspaciosModelo
            {
                EspaciosDisponibles = espacios
            };

            return View(viewModel);
        }

        // Acción POST: Procesa la solicitud de reserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearReserva(ReservaEspaciosModelo modelo)
        {
            int? usuarioId = Session["UsuarioID"] as int?;
            if (!usuarioId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            // 1. VALIDACIÓN DE LÓGICA DE TIEMPO: Hora Fin debe ser posterior a Hora Inicio
            if (modelo.Hora >= modelo.HoraFin)
            {
                ModelState.AddModelError("HoraFin", "La hora de fin debe ser posterior a la hora de inicio.");
            }

            // 2. VALIDACIÓN GENERAL
            if (!modelo.EspacioIDSeleccionado.HasValue || !ModelState.IsValid)
            {
                modelo.EspaciosDisponibles = CargarEspaciosDisponibles();
                if (!modelo.EspacioIDSeleccionado.HasValue)
                {
                    ModelState.AddModelError("", "Debe seleccionar un espacio de la lista inferior.");
                }
                return View("ReservaEspacioVista", modelo);
            }

            try
            {
                // CALCULO DINÁMICO DE LA DURACIÓN BASADO EN HORA INICIO Y HORA FIN
                TimeSpan duracion = modelo.HoraFin - modelo.Hora;
                decimal duracionHoras = (decimal)duracion.TotalHours; // Duración dinámica

                if (duracionHoras <= 0)
                {
                    // Esto debería ser capturado por la validación superior, pero es un doble chequeo
                    ModelState.AddModelError("HoraFin", "La duración de la reserva debe ser mayor a cero.");
                    modelo.EspaciosDisponibles = CargarEspaciosDisponibles();
                    return View("ReservaEspacioVista", modelo);
                }

                // 1. OBTENER VALOR POR HORA
                var detalle = db.EspaciosDetalles
          .FirstOrDefault(d => d.EspacioID == modelo.EspacioIDSeleccionado);

                if (detalle == null)
                {
                    throw new Exception($"Error de configuración: No se encontró el valor por hora para el EspacioID {modelo.EspacioIDSeleccionado.Value}.");
                }

                decimal montoTotal = detalle.ValorPorHora * duracionHoras;

                // A) CREAR Y GUARDAR EL REGISTRO DE FECHA DISPONIBLE (El slot de tiempo)
                var nuevaFecha = new FechasDisponibles
                {
                    Fecha = modelo.Fecha,
                    HoraInicio = modelo.Hora,
                    HoraFin = modelo.HoraFin, // USANDO EL VALOR DEL USUARIO

                };
                db.FechasDisponibles.Add(nuevaFecha);
                db.SaveChanges();

                int fechaDisponibleId = nuevaFecha.FechaDisponibleID;


                // B) CREAR Y GUARDAR EL REGISTRO INTERMEDIO (ReservasFechasDisponibles)
                var nuevaReservaFecha = new ReservasFechasDisponibles
                {
                    EspacioID = modelo.EspacioIDSeleccionado.Value,
                    FechaDisponibleID = fechaDisponibleId
                };
                db.ReservasFechasDisponibles.Add(nuevaReservaFecha);
                db.SaveChanges();

                int reservaFechaIdGenerado = nuevaReservaFecha.ReservaFechaID;


                // C) CREAR LA RESERVA FINAL
                var nuevaReserva = new Reservas
                {
                    UsuarioID = usuarioId.Value,
                    MontoTotal = montoTotal,

                    ReservaFechaID = reservaFechaIdGenerado
                };
                db.Reservas.Add(nuevaReserva);
                db.SaveChanges(); // Guarda la reserva final

                // ----------------------------------------------------
                // REDIRECCIÓN FINAL
                // ----------------------------------------------------
                return RedirectToAction("MisReservas", "GestionReservas");
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += " Detalle interno: " + ex.InnerException.Message;
                }

                ModelState.AddModelError("", "Error al intentar guardar la reserva. " + errorMessage);

                modelo.EspaciosDisponibles = CargarEspaciosDisponibles();
                return View("ReservaEspacioVista", modelo);
            }
        }

        // Liberación de recursos de la BD
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}