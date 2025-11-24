using ReservaYa.Models;
using ReservaYa.Models.Extras;
using ReservaYa.Services;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace ReservaYa.Controllers
{
    public class EspaciosDetallesController : Controller
    {
        private EspacioDetallesVMConvertService _transform = new EspacioDetallesVMConvertService();
        
        private string _nombre;        
        public ActionResult Index(string idEspacio) // cifrado
        {
            if (idEspacio== null || idEspacio.Equals(string.Empty)) { return new HttpStatusCodeResult(HttpStatusCode.Conflict); }

            int idEspacioDecd = EncriptarService.DescriptarId(idEspacio);
                        
            ViewBag.EspacioId = idEspacio; // valor incriptado para redireccionar

            using (var db = new DEVELOSERSEntities())
            {
                var espacio = db.Espacios.AsNoTracking().Where(x => x.EspacioID == idEspacioDecd).FirstOrDefault();
                var EspacioDT = db.EspaciosDetalles.AsNoTracking().Where(x => x.EspacioID == idEspacioDecd).FirstOrDefault();

                _nombre = espacio.Nombre;
                ViewBag.Name = _nombre;
                if (EspacioDT != null)
                {
                    ViewBag.ExisteDetalle = true; //validamos si esta vacio                    
                    var model = _transform.ToViewModel(EspacioDT);
                    return View(model);
                }
                else
                    ViewBag.ExisteDetalle = false;
                    return View();
            }            
        }
        public ActionResult Nuevo(string idEspacio) //incriptado
        {
            var model = new EspacioDetalleViewModel { IdEspacioEncriptada = idEspacio};
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]      
        public ActionResult Nuevo(EspacioDetalleViewModel espacioDT) 
        {
            if (ModelState.IsValid)
            {
                //guardado
                using (var db = new DEVELOSERSEntities())
                {
                    //solucionamos el problema
                    EspaciosDetalles espacio = new EspaciosDetalles
                    {
                        ValorPorHora = espacioDT.ValorXHora,
                        EspacioID = EncriptarService.DescriptarId(espacioDT.IdEspacioEncriptada),                        
                    };
                    db.EspaciosDetalles.Add(espacio);
                    db.SaveChanges();
                }
            }
            else             
                return View(espacioDT);

            var idEspacio = espacioDT.IdEspacioEncriptada;
            //Retorno con exito            
            return RedirectToAction("Index","EspaciosDetalles", new { idEspacio = idEspacio });
        }

        public ActionResult Eliminar(string idEspacio)
        {
            using (var db = new DEVELOSERSEntities()) 
            {
                var id = EncriptarService.DescriptarId(idEspacio);
                var espacioDT = db.EspaciosDetalles.Where(x=>x.EspacioID == id).FirstOrDefault();

                db.EspaciosDetalles.Remove(espacioDT);
                db.SaveChanges();

                return RedirectToAction("Index", "EspaciosDetalles", new { idEspacio = idEspacio });

            }
        }

    }
}