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
                // Filtramos la tabla intermedia por EspacioID
                var relaciones = db.ReservasFechasDisponibles
                    .AsNoTracking()
                    .Where(x => x.EspacioID == id)
                    .ToList();

                if (!relaciones.Any())
                {
                    ViewBag.HayRegistros = false;
                    return View(new ReservasFechaDisponiblesViewModel());
                }

                ViewBag.HayRegistros = true;

                // Obtenemos nombre del espacio
                var espacioId = relaciones.First().EspacioID;

                var espacioName = db.Espacios.AsNoTracking()
                                .Where(x => x.EspacioID == espacioId)
                                .Select(x => x.Nombre)
                                .FirstOrDefault();

                // JOIN con FechasDisponibles
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
                                 Disponible = f.Disponible
                             };

                // Armamos el modelo final
                var model = new ReservasFechaDisponiblesViewModel
                {
                    EspacioId = espacioId,
                    EspacioName = espacioName,
                    Fechas = fechas.ToList()
                };

                return View(model);
            }
        }
    }
}