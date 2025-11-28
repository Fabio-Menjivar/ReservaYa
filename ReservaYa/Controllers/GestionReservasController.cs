using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using ReservaYa.Models;

namespace ReservaYa.Controllers
{
    public class GestionReservasController : Controller
    {

        private readonly DEVELOSERSEntities _context = new DEVELOSERSEntities();

        // ---------------------------------------------------------------------
        // ACCIÓN 1: MisReservas() - Muestra las reservas del usuario actual
        // ---------------------------------------------------------------------
        public async Task<ActionResult> MisReservas()
        {
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int usuarioIdActual = (int)Session["UsuarioID"];

            var reservasDelUsuario = await (
                from r in _context.Reservas
                join rfd in _context.ReservasFechasDisponibles
                on r.ReservaFechaID equals rfd.ReservaFechaID
                join e in _context.Espacios
                on rfd.EspacioID equals e.EspacioID
                join fd in _context.FechasDisponibles
                on rfd.FechaDisponibleID equals fd.FechaDisponibleID
                where r.UsuarioID == usuarioIdActual
                orderby fd.Fecha descending, fd.HoraInicio descending
                select new ReservaUsuarioViewModel
                {
                    ReservaID = r.ReservaID,
                    NombreEspacio = e.Nombre,
                    MontoTotal = r.MontoTotal,
                    FechaReserva = fd.Fecha,
                    HoraInicio = fd.HoraInicio,
                    HoraFin = fd.HoraFin
                }
            ).ToListAsync();

            reservasDelUsuario.ForEach(res =>
            {
                var fechaCompleta = res.FechaReserva.Date + res.HoraInicio;
                res.EsPasada = fechaCompleta < DateTime.Now;
            });

            return View(reservasDelUsuario);
        }

        // ---------------------------------------------------------------------
        // ACCIÓN 2: Editar(int id) - Carga la reserva para edición (GET)
        // ---------------------------------------------------------------------
        public ActionResult Editar(int id)
        {
            var reserva = _context.Reservas.FirstOrDefault(r => r.ReservaID == id);
            if (reserva == null) return HttpNotFound();

            var rfd = _context.ReservasFechasDisponibles
                .Include(x => x.FechasDisponibles)
                .Include(x => x.Espacios)
                .FirstOrDefault(x => x.ReservaFechaID == reserva.ReservaFechaID);

            if (rfd == null) return HttpNotFound();

            var vm = new ReservaUsuarioViewModel
            {
                ReservaID = reserva.ReservaID,
                MontoTotal = reserva.MontoTotal,
                NombreEspacio = rfd.Espacios.Nombre,
                FechaReserva = rfd.FechasDisponibles.Fecha,
                HoraInicio = rfd.FechasDisponibles.HoraInicio,
                HoraFin = rfd.FechasDisponibles.HoraFin
            };

            
            // Para dropdown de espacios
            ViewBag.Espacios = _context.Espacios
                .Select(e => new SelectListItem
                {
                    Value = e.EspacioID.ToString(),
                    Text = e.Nombre,
                    Selected = (e.EspacioID == id)   // Seleccionar por defecto
                }).ToList();


            return View(vm);
        }

        // ---------------------------------------------------------------------
        // ACCIÓN 3: Editar(ReservaUsuarioViewModel model, int? EspacioID) [HttpPost]
        //           SOLUCIÓN FINAL: Usamos int? para controlar el binding
        // ---------------------------------------------------------------------
        [HttpPost]
        public ActionResult Editar(ReservaUsuarioViewModel model, int? EspacioID) // 👈 CORRECCIÓN CLAVE: int?
        {
            // 1. VALIDACIÓN EXPLÍCITA para mostrar el mensaje de error solicitado
            if (!EspacioID.HasValue || EspacioID.Value <= 0)
            {
                ModelState.AddModelError("EspacioID", "Por favor, seleccione un espacio de la lista.");
            }

            if (!ModelState.IsValid)
            {
                // Recargar ViewBag si hay errores de validación
                ViewBag.Espacios = _context.Espacios
                    .Select(e => new SelectListItem
                    {
                        Value = e.EspacioID.ToString(),
                        Text = e.Nombre
                    }).ToList();
                return View(model);
            }

            // A partir de aquí, EspacioID tiene un valor válido
            int espacioIdSeleccionado = EspacioID.Value;

            var reserva = _context.Reservas.FirstOrDefault(r => r.ReservaID == model.ReservaID);
            if (reserva == null) return HttpNotFound();

            var rfd = _context.ReservasFechasDisponibles
                .Include(f => f.FechasDisponibles)
                .FirstOrDefault(f => f.ReservaFechaID == reserva.ReservaFechaID);

            if (rfd == null) return HttpNotFound();

            // 1️⃣ Actualizar Espacio
            rfd.EspacioID = espacioIdSeleccionado;

            // 2️⃣ Actualizar Fecha y Horas
            rfd.FechasDisponibles.Fecha = model.FechaReserva;
            rfd.FechasDisponibles.HoraInicio = model.HoraInicio;
            rfd.FechasDisponibles.HoraFin = model.HoraFin;

            // 3️⃣ Recalcular monto
            TimeSpan duracion = model.HoraFin - model.HoraInicio;

            // Obtener el precio por hora del ESPACIO SELECCIONADO
            var detalleEspacio = _context.EspaciosDetalles.FirstOrDefault(d => d.EspacioID == espacioIdSeleccionado);
            decimal precioHora = detalleEspacio?.ValorPorHora ?? 5.0m;

            reserva.MontoTotal = (decimal)duracion.TotalHours * precioHora;

            _context.SaveChanges();

            TempData["Mensaje"] = "La reserva fue actualizada correctamente.";
            return RedirectToAction("MisReservas");
        }


        // ---------------------------------------------------------------------
        // ACCIÓN 4: Cancelar una reserva (Lógica de eliminación en DB)
        // ---------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cancelar(int id)
        {
            // ... (Código sin cambios)
            if (Session["UsuarioID"] == null)
            {
                TempData["Error"] = "Sesión expirada. Por favor, inicia sesión de nuevo.";
                return RedirectToAction("Login", "Account");
            }
            int usuarioIdActual = (int)Session["UsuarioID"];

            try
            {
                var reserva = await _context.Reservas
                    .Where(r => r.ReservaID == id && r.UsuarioID == usuarioIdActual)
                    .FirstOrDefaultAsync();

                if (reserva == null)
                {
                    TempData["Error"] = "Error: La reserva no fue encontrada o no te pertenece.";
                    return RedirectToAction("MisReservas");
                }

                int reservaFechaId = (int)reserva.ReservaFechaID;

                var reservaFechaDisponible = await _context.ReservasFechasDisponibles
                    .Where(rfd => rfd.ReservaFechaID == reservaFechaId)
                    .FirstOrDefaultAsync();

                if (reservaFechaDisponible != null)
                {
                    var fechaDisponible = await _context.FechasDisponibles
                        .Where(fd => fd.FechaDisponibleID == reservaFechaDisponible.FechaDisponibleID)
                        .FirstOrDefaultAsync();

                    _context.ReservasFechasDisponibles.Remove(reservaFechaDisponible);

                    if (fechaDisponible != null)
                    {
                        _context.FechasDisponibles.Remove(fechaDisponible);
                    }
                }

                _context.Reservas.Remove(reserva);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = $"La reserva #{id} fue cancelada y el espacio liberado.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error en el servidor al intentar cancelar la reserva. Intenta de nuevo.";
            }

            return RedirectToAction("MisReservas");
        }

        // Liberación de recursos de la BD
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}