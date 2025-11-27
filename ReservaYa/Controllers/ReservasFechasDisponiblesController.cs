using ReservaYa.Models;
using ReservaYa.Models.Extras;
using ReservaYa.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReservaYa.Controllers
{
    public class ReservasFechasDisponiblesController : Controller
    {
        // GET: ReservasFechasDisponibles
        public ActionResult Index(string idEspacio)
        {
            var id = EncriptarService.DescriptarId(idEspacio);

            using (var db = new DEVELOSERSEntities())
            {
                //obtener nombre del espacio 
                var espacioName = db.Espacios
                    .AsNoTracking()
                    .Where(x => x.EspacioID == id)
                    .Select(x => x.Nombre)
                    .FirstOrDefault();

                // Cargar relaciones (puede estar vacía)
                var relaciones = db.ReservasFechasDisponibles
                    .AsNoTracking()
                    .Where(x => x.EspacioID == id)
                    .ToList();

                //Si no hay relaciones → devolver ViewModel sin fechas
                if (!relaciones.Any())
                {
                    ViewBag.HayRegistros = false;

                    var modelVacio = new ReservasFechaDisponiblesViewModel
                    {
                        EspacioIdCifrado = idEspacio,
                        EspacioName = espacioName,
                        Fechas = new List<ReservaFechaDisponibleItemVM>()
                    };

                    return View(modelVacio);
                }

                //Hay relaciones
                ViewBag.HayRegistros = true;

                //  JOIN con FechasDisponibles
                var fechas = from r in relaciones
                             join f in db.FechasDisponibles.AsNoTracking()
                                on r.FechaDisponibleID equals f.FechaDisponibleID
                             select new ReservaFechaDisponibleItemVM
                             {
                                 ReservaFechaID = r.ReservaFechaID,
                                 FechaDisponibleID = f.FechaDisponibleID,
                                 Fecha = f.Fecha,
                                 HoraInicio = f.HoraInicio,
                                 HoraFin = f.HoraFin,
                                 Disponible = f.Disponible,
                                 Tags = f.Tags 
                             };


                // Model final
                var model = new ReservasFechaDisponiblesViewModel
                {
                    EspacioIdCifrado = idEspacio,
                    EspacioName = espacioName,
                    Fechas = fechas.ToList()
                };

                return View(model);
            }
        }

        // GET: FechasDisponibles/Delete/5
        public ActionResult Delete(int id)
        {
            using (var db = new DEVELOSERSEntities())
            {
                var entidad = db.ReservasFechasDisponibles
                    .Include("FechasDisponibles")
                    .Where(x => x.ReservaFechaID == id)
                    .FirstOrDefault();

                if (entidad == null)
                    return HttpNotFound();

                var vm = new ReservaFechaDisponibleItemVM
                {
                    //nombre propiedad incorrecto ej Tags = entidad.FechaDisponible.Tags 
                    ReservaFechaID = entidad.ReservaFechaID,
                    FechaDisponibleID = entidad.FechaDisponibleID,
                    Fecha = entidad.FechasDisponibles.Fecha,
                    HoraInicio = entidad.FechasDisponibles.HoraInicio,
                    HoraFin = entidad.FechasDisponibles.HoraFin,
                    Disponible = entidad.FechasDisponibles.Disponible,
                    Tags = entidad.FechasDisponibles.Tags
                };

                // Para volver a la pantalla anterior
                ViewBag.IdEspacio = EncriptarService.EncriptarId(entidad.EspacioID);
                return View(vm);
            }
        }



        // POST: FechasDisponibles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ReservaFechaDisponibleItemVM model)
        {
            using (var db = new DEVELOSERSEntities())
            {
                // Obtiene el registro de la tabla puente (ReservasFechaDisponibles)
                var reserva = db.ReservasFechasDisponibles
                    .Where(x => x.ReservaFechaID == model.ReservaFechaID)
                    .FirstOrDefault();

                if (reserva == null)
                    return HttpNotFound();

                // Guardamos id del espacio antes de borrar
                string idEspacioCifrado = EncriptarService.EncriptarId(reserva.EspacioID);

                // Eliminamos primero la relación
                db.ReservasFechasDisponibles.Remove(reserva);

                // Luego se elimina la fecha disponible asociada
                var fecha = db.FechasDisponibles
                    .Where(f => f.FechaDisponibleID == reserva.FechaDisponibleID)
                    .FirstOrDefault();

                if (fecha != null)
                    db.FechasDisponibles.Remove(fecha);

                // Guardar cambios
                db.SaveChanges();

                return RedirectToAction(
                    "Mostrar",
                    "GestionEspacios", new { id = idEspacioCifrado });
            }
        }


    }
}