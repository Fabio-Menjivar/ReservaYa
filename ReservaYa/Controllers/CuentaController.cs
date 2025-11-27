using ReservaYa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ReservaYa.Controllers
{

    public class CuentaController : Controller
    {

        private readonly DEVELOSERSEntities db = new DEVELOSERSEntities();
        // =====================================
        //     PERFIL DEL USUARIO LOGUEADO
        // =====================================
        public ActionResult Perfil()
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Login");

            int usuarioId = (int)Session["UsuarioID"];

            var usuario = db.Usuarios.FirstOrDefault(u => u.UsuarioID == usuarioId);

            if (usuario == null)
                return HttpNotFound();


            // Convertir varbinary → string para mostrar
            ViewBag.Correo = usuario.Correo != null
                ? Encoding.UTF8.GetString(usuario.Correo)
                : "";

            return View(usuario);
        }



        // =====================================
        //     GET: Cambiar Credenciales
        // =====================================
        [HttpGet]
        public ActionResult CambiarCredenciales()
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Login");

            int usuarioId = (int)Session["UsuarioID"];
            var usuario = db.Usuarios.FirstOrDefault(u => u.UsuarioID == usuarioId);

            if (usuario == null)
                return HttpNotFound();

            var model = new CambiarCredencialesViewModel
            {
                CorreoActual = usuario.Correo != null
                    ? Encoding.UTF8.GetString(usuario.Correo)
                    : ""
            };

            return View(model);
        }



        // =====================================
        //     POST: Cambiar Credenciales
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarCredenciales(CambiarCredencialesViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Login");

            int usuarioId = (int)Session["UsuarioID"];
            var usuario = db.Usuarios.FirstOrDefault(u => u.UsuarioID == usuarioId);

            if (usuario == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return View(model);
            }


            // Convertir VARBINARY → STRING
            string correoBD = Encoding.UTF8.GetString(usuario.Correo);
            string contrasenaBD = Encoding.UTF8.GetString(usuario.Contrasena);


            // VALIDACIONES
            if (correoBD != model.CorreoActual)
            {
                ModelState.AddModelError("", "El correo actual no coincide.");
                return View(model);
            }

            if (contrasenaBD != model.ContrasenaActual)
            {
                ModelState.AddModelError("", "La contraseña actual es incorrecta.");
                return View(model);
            }


            // GUARDAR NUEVOS VALORES (STRING → VARBINARY)
            usuario.Correo = Encoding.UTF8.GetBytes(model.NuevoCorreo);
            usuario.Contrasena = Encoding.UTF8.GetBytes(model.NuevaContrasena);

            db.SaveChanges();


            TempData["Mensaje"] = "Credenciales actualizadas correctamente.";

            return RedirectToAction("Perfil");
        }
    }
}
