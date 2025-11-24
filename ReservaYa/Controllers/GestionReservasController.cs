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
            // ... VALIDACIÓN Y OBTENCIÓN DE usuarioIdActual (Se mantiene igual) ...
            if (Session["UsuarioID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int usuarioIdActual = (int)Session["UsuarioID"];

            // MODIFICACIÓN CLAVE EN LA CONSULTA:
            var reservasDelUsuario = await (
                from r in _context.Reservas

                    // 1. Unir Reserva con la tabla intermedia (ReservasFechasDisponibles)
                join rfd in _context.ReservasFechasDisponibles
                on r.ReservaFechaID equals rfd.ReservaFechaID

                // 2. Unir la tabla intermedia con Espacios
                join e in _context.Espacios
                on rfd.EspacioID equals e.EspacioID

                // 3. Unir la tabla intermedia con FechasDisponibles
                join fd in _context.FechasDisponibles
                on rfd.FechaDisponibleID equals fd.FechaDisponibleID

                where r.UsuarioID == usuarioIdActual // Filtro principal

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

            // 4. Lógica para marcar si la reserva ya pasó
            reservasDelUsuario.ForEach(res =>
            {
                var fechaCompleta = res.FechaReserva.Date + res.HoraInicio;
                res.EsPasada = fechaCompleta < DateTime.Now;
            });

            // 5. Retorna la vista MisReservas.cshtml
            return View(reservasDelUsuario);
        }

        //EDITAR
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
                                           Text = e.Nombre
                                       }).ToList();

            return View(vm);
        }

        [HttpPost]
        public ActionResult Editar(ReservaUsuarioViewModel model, int EspacioID)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Espacios = _context.Espacios
                    .Select(e => new SelectListItem
                    {
                        Value = e.EspacioID.ToString(),
                        Text = e.Nombre
                    }).ToList();
                return View(model);
            }

            var reserva = _context.Reservas.FirstOrDefault(r => r.ReservaID == model.ReservaID);
            if (reserva == null) return HttpNotFound();

            var rfd = _context.ReservasFechasDisponibles
                              .Include(f => f.FechasDisponibles)
                              .FirstOrDefault(f => f.ReservaFechaID == reserva.ReservaFechaID);

            if (rfd == null) return HttpNotFound();

            // 1️⃣ Actualizar Espacio
            rfd.EspacioID = EspacioID;

            // 2️⃣ Actualizar Fecha y Horas
            rfd.FechasDisponibles.Fecha = model.FechaReserva;
            rfd.FechasDisponibles.HoraInicio = model.HoraInicio;
            rfd.FechasDisponibles.HoraFin = model.HoraFin;

            // 3️⃣ Recalcular monto
            TimeSpan duracion = model.HoraFin - model.HoraInicio;
            decimal precioHora = 5; // ← si tienes precio por espacio, se pone aquí
            reserva.MontoTotal = (decimal)duracion.TotalHours * precioHora;

            _context.SaveChanges();

            TempData["Mensaje"] = "La reserva fue actualizada correctamente.";
            return RedirectToAction("MisReservas");
        }


        // ---------------------------------------------------------------------
        // ACCIÓN 2: Cancelar una reserva (Lógica de eliminación en DB)
        // ---------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cancelar(int id)
        {
            // 1. VALIDACIÓN DE SESIÓN Y SEGURIDAD
            if (Session["UsuarioID"] == null)
            {
                TempData["Error"] = "Sesión expirada. Por favor, inicia sesión de nuevo.";
                return RedirectToAction("Login", "Account");
            }
            int usuarioIdActual = (int)Session["UsuarioID"];

            try
            {
                // 2. ENCONTRAR Y VALIDAR PROPIEDAD DE LA RESERVA
                var reserva = await _context.Reservas
                                            .Where(r => r.ReservaID == id && r.UsuarioID == usuarioIdActual)
                                            .FirstOrDefaultAsync();

                if (reserva == null)
                {
                    TempData["Error"] = "Error: La reserva no fue encontrada o no te pertenece.";
                    return RedirectToAction("MisReservas");
                }

                int reservaFechaId = (int)reserva.ReservaFechaID;

                // 3. ELIMINAR REGISTROS DEPENDIENTES

                var reservaFechaDisponible = await _context.ReservasFechasDisponibles
                    .Where(rfd => rfd.ReservaFechaID == reservaFechaId)
                    .FirstOrDefaultAsync();

                if (reservaFechaDisponible != null)
                {
                    _context.ReservasFechasDisponibles.Remove(reservaFechaDisponible);
                }

                // 6. ELIMINAR LA RESERVA PRINCIPAL
                _context.Reservas.Remove(reserva);

                // 7. GUARDAR CAMBIOS EN LA BASE DE DATOS
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = $"La reserva #{id} fue cancelada y el espacio liberado.";
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error en el servidor al intentar cancelar la reserva. Intenta de nuevo.";
            }

            return RedirectToAction("MisReservas");
        }

    }



}