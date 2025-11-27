using ReservaYa.Models;
using ReservaYa.Models.Extras;
using ReservaYa.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ReservaYa.Controllers
{
    public class GestionEspaciosController : Controller
    {
        // GET: GestionEspacios
        private readonly DEVELOSERSEntities db = new DEVELOSERSEntities();
        private readonly EspaciosVMConvertService transform = new EspaciosVMConvertService();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Homepage()
        {
            List<Espacios> todos = db.Espacios.AsNoTracking().ToList();
            //convertir a VM
            var espaciosVM = transform.Convert(todos);
            ViewBag.CategoriaID = new SelectList(db.Categorias, "CategoriaID", "Nombre");
            return View(espaciosVM);
        }

        public ActionResult Create()
        {
            //crear nuevo usuario
            Espacios espacio = new Espacios()
            { ImagenPrev = "",
                Disponible = false
            };
            var espaciosVM = transform.Convert(espacio);

            // Cargar categorías para el dropdown
            ViewBag.CategoriaID = new SelectList(db.Categorias, "CategoriaID", "Nombre");
            return View(espaciosVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EspacioViewModel espacio)
        {
            if (ModelState.IsValid)
            {
                espacio.ImagenPrev = null;
                espacio.Disponible = false;
                //TODO: convertir view model a model
                var model = transform.Reverse(espacio);
                //Guardamos cambios
                db.Espacios.Add(model);
                db.SaveChanges();
                /*
                 * Después de SaveChanges(), Entity Framework actualiza el objeto espacioDT en memoria,
                 * incluyendo la propiedad EspacioID recién generada.
                 */
                return RedirectToAction("Homepage");
            }


            // Si falla la validación, volver a cargar el dropdown
            ViewBag.CategoriaID = new SelectList(db.Categorias, "CategoriaID", "Nombre", espacio.CategoriaID);
            return View(espacio);
        }
        public ActionResult Update(string id)
        {
            if (id == null) { return new HttpNotFoundResult(); }
            //Buscar si existe
            var espacio = db.Espacios.Find(EncriptarService.DescriptarId(id));
            //Convertimos
            var resultVM = transform.Convert(espacio);
            // Cargar categorías para el dropdown
            ViewBag.CategoriaID = new SelectList(db.Categorias, "CategoriaID", "Nombre", espacio.CategoriaID);
            return View(resultVM); //busca y regresa
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(EspacioViewModel espacio)
        {
            if (ModelState.IsValid)
            {
                var original = db.Espacios.Find(espacio.EspacioID);
                if (original == null) { return HttpNotFound(); }

                // Copia los valores del modelo recibido al original rastreado por EF
                db.Entry(original).CurrentValues.SetValues(transform.Reverse(espacio));
                db.SaveChanges();
                return RedirectToAction("Homepage");

            }
            // Si hay errores, recargamos el dropdown
            ViewBag.CategoriaID = new SelectList(db.Categorias, "CategoriaID", "Nombre", espacio.CategoriaID);
            return View(espacio);
        }
        // GET: Espacios/Delete/5
        [HttpGet]
        public ActionResult Delete(string id)
        {
            if (!id.Any() || id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var espacio = db.Espacios.Find(EncriptarService.DescriptarId(id));
            if (espacio == null) return HttpNotFound();
            var espacioVM = transform.Convert(espacio);
            //cifrado por parametro
            espacioVM.EspacioIdCifrado = id;
            // Puedes pasar el modelo directamente a la vista para mostrar la info
            return View(espacioVM);
        }

        // POST: Espacios/Delete/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(EspacioViewModel espacio)  //borrado en cascada
        {
            var espacioOriginal = db.Espacios.Find(espacio.EspacioID);
            if (espacioOriginal == null)
                return HttpNotFound();

            // 1. BORRAR Reservas que dependen del Espacio (indirectamente)
            var reservasRelacionadas = db.Reservas
                .Where(r => r.ReservaFechaID != null &&
                            db.ReservasFechasDisponibles
                            .Any(x => x.ReservaFechaID == r.ReservaFechaID
                                   && x.EspacioID == espacio.EspacioID))
                .ToList();

            foreach (var r in reservasRelacionadas)
                db.Reservas.Remove(r);

            // 2. BORRAR ReservasFechasDisponibles del Espacio
            var reservasFechas = db.ReservasFechasDisponibles
                .Where(x => x.EspacioID == espacio.EspacioID)
                .ToList();

            foreach (var rf in reservasFechas)
                db.ReservasFechasDisponibles.Remove(rf);

            // 3. BORRAR EspaciosDetalles
            var detalles = db.EspaciosDetalles
                .Where(x => x.EspacioID == espacio.EspacioID)
                .ToList();

            foreach (var d in detalles)
                db.EspaciosDetalles.Remove(d);

            // 4. BORRAR Espacio
            db.Espacios.Remove(espacioOriginal);

            // GUARDAR TODO
            db.SaveChanges();

            return RedirectToAction("Homepage");
        }

        //delete que desactiva el espacioDT pero no lo borra


        public ActionResult Mostrar(string id) //idcrifrado
        {
            if (string.IsNullOrEmpty(id))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            int idReal = EncriptarService.DescriptarId(id);

            var espacio = db.Espacios.Find(idReal);
            if (espacio == null)
                return HttpNotFound();

            // Convertir a EspacioViewModel
            var vm = transform.Convert(espacio);


            // Cargar nombre de categoría
            ViewBag.CategoriaName = db.Categorias
                .Where(c => c.CategoriaID == espacio.CategoriaID)
                .Select(c => c.Nombre)
                .FirstOrDefault();

            // ======================================================
            // NUEVO → Cargar Fechas Disponibles usando tu ViewModel
            // ======================================================
            var fechasVM = (from e in db.ReservasFechasDisponibles
                            where e.EspacioID == espacio.EspacioID
                            join u in db.FechasDisponibles on e.FechaDisponibleID equals u.FechaDisponibleID
                            select new ReservaFechaDisponibleItemVM // Proyectamos directamente al tipo VM
                            {
                                ReservaFechaID = e.ReservaFechaID,
                                FechaDisponibleID = u.FechaDisponibleID,
                                Fecha = u.Fecha,
                                HoraInicio = u.HoraInicio,
                                HoraFin = u.HoraFin,
                                Tags = u.Tags,
                                Disponible = u.Disponible
                            }).ToList();

            // Enviar un ViewModel estructurado
            var fechasWrapper = new ReservasFechaDisponiblesViewModel
            {
                EspacioIdCifrado = id,
                EspacioName = espacio.Nombre,
                Fechas = fechasVM
            };

            ViewBag.FechasDisponibles = fechasWrapper;
            // (Si quieres evitar ViewBag, te explico cómo integrarlo directo en la vista Mostrar)

            return View(vm);
        }

        // GET: Espacios/SubirImagen/5
        public ActionResult SubirImagen(string idEspacio)
        {
            return View();
        }

        // POST: Espacios/SubirImagen/5
        [HttpPost]
        public ActionResult SubirImagen(string idEspacio, HttpPostedFileBase archivo)
        {
            var id = EncriptarService.DescriptarId(idEspacio);

            if (archivo == null || archivo.ContentLength == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar una imagen.");
                return View();
            }

            // Validar extensión
            var ext = Path.GetExtension(archivo.FileName).ToLower();
            var permitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            if (!permitidas.Contains(ext))
            {
                ModelState.AddModelError("", "Formato de imagen no permitido.");
                return View();
            }

            // Crear carpeta si no existe
            var folder = Server.MapPath("~/Content/Uploads/Espacios/Images/");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // Crear nombre único
            string fileName = $"espacio_{id}_{Guid.NewGuid()}{ext}";
            string rutaFinal = Path.Combine(folder, fileName);

            // Guardar archivo
            archivo.SaveAs(rutaFinal);

            // Guardar en la BD
            using (var db = new DEVELOSERSEntities())
            {
                var espacio = db.Espacios.Find(id);
                if (espacio != null)
                {
                    espacio.ImagenPrev = fileName;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Mostrar", new { id=idEspacio });
        }

        // GET: Espacios/CambiarImagen/5
        public ActionResult CambiarImagen(string idEspacio)
        {
            var idReal = EncriptarService.DescriptarId(idEspacio);
            using (var db = new DEVELOSERSEntities ())
            {
                var imagen = db.Espacios.AsNoTracking().Where(x => x.EspacioID == idReal).Select(x=>x.ImagenPrev).FirstOrDefault();
                ViewBag.ImagenActual = imagen;
            }
            return View();
        }

        // POST: Espacios/CambiarImagen/5
        [HttpPost]
        public ActionResult CambiarImagen(string idEspacio, HttpPostedFileBase archivo)
        {
            var id = EncriptarService.DescriptarId(idEspacio);

            if (archivo == null || archivo.ContentLength == 0)
            {
                ModelState.AddModelError("", "Debe seleccionar una imagen.");
                return View();
            }

            // Validar extensión
            var ext = Path.GetExtension(archivo.FileName).ToLower();
            var permitidas = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            if (!permitidas.Contains(ext))
            {
                ModelState.AddModelError("", "Formato no permitido.");
                return View();
            }

            // Carpeta destino
            var carpeta = Server.MapPath("~/Content/Uploads/Espacios/Images/");
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            // Nuevo nombre
            string fileName = $"espacio_{id}_{Guid.NewGuid()}{ext}";
            string rutaNueva = Path.Combine(carpeta, fileName);

            using (var db = new DEVELOSERSEntities())
            {
                var espacio = db.Espacios.Find(id);
                if (espacio == null)
                    return HttpNotFound();

                // Borrar imagen anterior
                if (!string.IsNullOrEmpty(espacio.ImagenPrev))
                {
                    var rutaAnterior = Path.Combine(carpeta, espacio.ImagenPrev);
                    if (System.IO.File.Exists(rutaAnterior))
                        System.IO.File.Delete(rutaAnterior);
                }

                // Guardar nueva imagen
                archivo.SaveAs(rutaNueva);

                // Guardar en BD
                espacio.ImagenPrev = fileName;
                db.SaveChanges();
            }

            return RedirectToAction("Mostrar", new { id=idEspacio });
        }

        public ActionResult CambiarEstado(string idEspacio, bool activar)
        {
            // Desencriptar ID real
            var id = EncriptarService.DescriptarId(idEspacio);

            using (var db = new DEVELOSERSEntities())
            {
                // Cargar el espacio SIN AsNoTracking (para poder guardar cambios)
                var espacio = db.Espacios
                                .Where(x => x.EspacioID == id)
                                .FirstOrDefault();

                if (espacio == null)
                {
                    TempData["Error"] = "El espacio no existe.";
                    return RedirectToAction("Index");
                }

                // VALIDACIONES
                bool tieneImagen = !string.IsNullOrEmpty(espacio.ImagenPrev);

                bool tieneDetalles = db.EspaciosDetalles
                                       .Any(d => d.EspacioID == id);

                bool tieneFechas = db.ReservasFechasDisponibles
                                     .Any(f => f.EspacioID == id);

                // SI FALTA ALGO → NO ACTIVAR
                if (!tieneImagen || !tieneDetalles || !tieneFechas)
                {
                    TempData["Error"] =
                        "No se puede activar el espacio. Debe tener: " +
                        (tieneImagen ? "" : " Imagen |") +
                        (tieneDetalles ? "" : " Tarifa |") +
                        (tieneFechas ? "" : " Fechas disponibles para reservar |");

                    return RedirectToAction("Mostrar", new { id = idEspacio });
                }

                // SI TODO OK → Activar o desactivar
                espacio.Disponible = activar;

                db.SaveChanges();
            }

            TempData["Exito"] = activar
                ? "El espacio ha sido activado correctamente."
                : "El espacio ha sido desactivado correctamente.";

            return RedirectToAction("Mostrar", new { id = idEspacio });
        }



    }
}