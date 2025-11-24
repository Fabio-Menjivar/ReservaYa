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
            var espaciosVM= transform.Convert(todos);
            ViewBag.CategoriaID = new SelectList(db.Categorias, "CategoriaID", "Nombre");          
            return View(espaciosVM);
        }

        public ActionResult Create()
        {
            //crear nuevo usuario
            Espacios espacio = new Espacios() 
            {   ImagenPrev="",
                Disponible=false
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
                 * Después de SaveChanges(), Entity Framework actualiza el objeto espacio en memoria,
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
            if (!id.Any()|| id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

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
        public ActionResult Delete(EspacioViewModel espacio)
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
        public ActionResult DetallesEspacio()
        {
            //TODO
            /*
             partial view de 
                *valor x hora
                *categoria
             */

            //viewbag lo necesario*

            return View();
        }
    }
}