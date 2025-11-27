using ReservaYa.Models;
using ReservaYa.Models.Extras;
using ReservaYa.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReservaYa.Controllers
{
    public class FechasDisponiblesController : Controller
    {
        public ActionResult Create(string idEspacio)
        {
            if (string.IsNullOrWhiteSpace(idEspacio))
                return RedirectToAction("Index", "Home");

            int idReal = EncriptarService.DescriptarId(idEspacio);

            using (var db = new DEVELOSERSEntities())
            {
                var espacioNombre = db.Espacios
                                     .Where(x => x.EspacioID == idReal)
                                     .Select(x => x.Nombre)
                                     .FirstOrDefault();

                if (espacioNombre == null)
                    return HttpNotFound("No existe el espacio");

                ViewBag.EspacioIdCifrado = idEspacio;
                ViewBag.EspacioName = espacioNombre;

                // Crear modelo base
                var model = new ReservaFechaDisponibleItemVM
                {
                    Fecha = DateTime.Today,
                    HoraInicio = new TimeSpan(8, 0, 0),
                    HoraFin = new TimeSpan(10, 0, 0),
                    Disponible = true
                };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReservaFechaDisponibleItemVM model, string EspacioIdCifrado)
        {
            if (string.IsNullOrWhiteSpace(EspacioIdCifrado))
                return RedirectToAction("Index", "Home");

            int idReal = EncriptarService.DescriptarId(EspacioIdCifrado);

            // Validación simple
            if (model.HoraFin <= model.HoraInicio)
            {
                ModelState.AddModelError("", "La hora de fin debe ser mayor que la hora de inicio.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.EspacioIdCifrado = EspacioIdCifrado;
                return View(model);
            }

            using (var db = new DEVELOSERSEntities())
            {
                // ===========================
                // VALIDAR SOLAPES
                // ===========================
                var solape = (from rf in db.ReservasFechasDisponibles
                              join fd in db.FechasDisponibles
                                on rf.FechaDisponibleID equals fd.FechaDisponibleID
                              where rf.EspacioID == idReal
                                 && fd.Fecha == model.Fecha
                                 && (
                                        // Inicio dentro de otra reserva
                                        (model.HoraInicio >= fd.HoraInicio && model.HoraInicio < fd.HoraFin)
                                        ||
                                        // Fin dentro de otra reserva
                                        (model.HoraFin > fd.HoraInicio && model.HoraFin <= fd.HoraFin)
                                        ||
                                        // La nueva cubre completamente a otra
                                        (model.HoraInicio <= fd.HoraInicio && model.HoraFin >= fd.HoraFin)
                                    )
                              select fd).Any();

                if (solape)
                {
                    ModelState.AddModelError("", "El rango horario se solapa con una fecha ya existente.");
                    ViewBag.EspacioIdCifrado = EspacioIdCifrado;
                    return View(model);
                }

                // ===========================
                // 1) CREAR FECHA DISPONIBLE
                // ===========================
                var nuevaFecha = new FechasDisponibles
                {
                    Fecha = model.Fecha,
                    HoraInicio = model.HoraInicio,
                    HoraFin = model.HoraFin,
                    Disponible = model.Disponible,
                    Tags = model.Tags
                };

                db.FechasDisponibles.Add(nuevaFecha);
                db.SaveChanges();

                // ===========================
                // 2) CREAR RELACIÓN RESERVA-FECHA
                // ===========================
                var relacion = new ReservasFechasDisponibles
                {
                    EspacioID = idReal,
                    FechaDisponibleID = nuevaFecha.FechaDisponibleID
                };

                db.ReservasFechasDisponibles.Add(relacion);
                db.SaveChanges();

                TempData["mensaje"] = "Fecha creada correctamente.";
            }

            return RedirectToAction("Index", "ReservasFechasDisponibles", new { idEspacio = EspacioIdCifrado });
        }




    }
}