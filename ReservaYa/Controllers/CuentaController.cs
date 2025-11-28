using ReservaYa.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO; // 🚨 NECESARIO para manejar archivos de imagen

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

            // 🚨 CRÍTICO: Asegurarse de que la sesión tenga la ruta más reciente para el _Layout
            Session["RutaImagen"] = usuario.RutaImagen;

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
                // Mostramos el correo actual sin cifrar
                CorreoActual = usuario.Correo != null
                    ? Encoding.UTF8.GetString(usuario.Correo)
                    : ""
            };

            return View(model);
        }



        // =====================================
        //     POST: Cambiar Credenciales (ACTUALIZADO para campos opcionales)
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarCredenciales(CambiarCredencialesViewModel model)
        {
            // La validación de ModelState ahora solo verifica los campos con [Required] (CorreoActual y ContrasenaActual)
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

            // Convertir VARBINARY → STRING para comparación
            string contrasenaBD = Encoding.UTF8.GetString(usuario.Contrasena);
            string correoBD = Encoding.UTF8.GetString(usuario.Correo);


            // 1. VALIDACIÓN CRÍTICA: Contraseña Actual
            if (!contrasenaBD.Equals(model.ContrasenaActual))
            {
                ModelState.AddModelError("ContrasenaActual", "La contraseña actual es incorrecta.");
                return View(model);
            }


            bool huboCambio = false;

            // 2. Lógica Condicional para actualizar CORREO
            if (!string.IsNullOrEmpty(model.NuevoCorreo))
            {
                // Validación para asegurar que el Correo no sea igual al actual
                if (model.NuevoCorreo.Equals(correoBD, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("NuevoCorreo", "El nuevo correo debe ser diferente al correo actual.");
                    return View(model);
                }

                // Validación de confirmación (el ModelState.IsValid ya verifica que ConfirmarCorreo == NuevoCorreo)
                if (model.NuevoCorreo != model.ConfirmarCorreo)
                {
                    ModelState.AddModelError("ConfirmarCorreo", "Los correos no coinciden.");
                    return View(model);
                }

                // Guardar nuevo valor
                usuario.Correo = Encoding.UTF8.GetBytes(model.NuevoCorreo.Trim());
                huboCambio = true;
            }


            // 3. Lógica Condicional para actualizar CONTRASEÑA
            if (!string.IsNullOrEmpty(model.NuevaContrasena))
            {
                // Validación para asegurar que el password no sea igual al actual
                if (model.NuevaContrasena.Equals(contrasenaBD))
                {
                    ModelState.AddModelError("NuevaContrasena", "La nueva contraseña debe ser diferente a la actual.");
                    return View(model);
                }

                // Validación de confirmación (el ModelState.IsValid ya verifica que ConfirmarContrasena == NuevaContrasena)
                if (model.NuevaContrasena != model.ConfirmarContrasena)
                {
                    ModelState.AddModelError("ConfirmarContrasena", "Las contraseñas no coinciden.");
                    return View(model);
                }


                // Guardar nuevo valor
                usuario.Contrasena = Encoding.UTF8.GetBytes(model.NuevaContrasena.Trim());
                huboCambio = true;
            }


            // 4. Finalización
            if (!huboCambio)
            {
                TempData["Error"] = "No se detectó ningún cambio. Por favor, ingrese un nuevo correo o una nueva contraseña.";
                return View(model);
            }

            // GUARDAMOS
            db.SaveChanges();
            TempData["Mensaje"] = "Credenciales actualizadas correctamente.";

            // Forzar cierre de sesión por seguridad después de un cambio de credenciales
            Session.Clear();
            Session.Abandon();
            TempData["Mensaje"] = "Credenciales actualizadas. Por favor, inicie sesión con su nueva información.";
            return RedirectToAction("Login", "Login");
        }


        // =====================================
        //     GET: Cambiar Imagen
        // =====================================
        [HttpGet]
        public ActionResult CambiarImagen()
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Login");

            return View();
        }

        // =====================================
        //     POST: Cambiar Imagen
        // =====================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CambiarImagen(HttpPostedFileBase nuevaImagen)
        {
            if (Session["UsuarioID"] == null)
                return RedirectToAction("Login", "Login");

            if (nuevaImagen != null && nuevaImagen.ContentLength > 0)
            {
                if (!nuevaImagen.ContentType.StartsWith("image/"))
                {
                    TempData["Error"] = "El archivo debe ser una imagen válida.";
                    return RedirectToAction("CambiarImagen");
                }

                try
                {
                    int usuarioId = (int)Session["UsuarioID"];
                    var usuario = db.Usuarios.FirstOrDefault(u => u.UsuarioID == usuarioId);

                    if (usuario == null)
                    {
                        TempData["Error"] = "Usuario no encontrado.";
                        return RedirectToAction("Perfil");
                    }

                    // --- LÓGICA DE GUARDADO DE IMAGEN ---
                    string extension = Path.GetExtension(nuevaImagen.FileName);
                    string nombreArchivo = usuarioId.ToString() + "_" + Guid.NewGuid().ToString() + extension;

                    string rutaCarpeta = Server.MapPath("~/Content/Uploads/Perfiles/");
                    string rutaGuardado = Path.Combine(rutaCarpeta, nombreArchivo);

                    // Crear la carpeta si no existe
                    if (!Directory.Exists(rutaCarpeta))
                    {
                        Directory.CreateDirectory(rutaCarpeta);
                    }

                    // Opcional: Eliminar la imagen anterior del servidor
                    if (!string.IsNullOrEmpty(usuario.RutaImagen))
                    {
                        string rutaAnterior = Server.MapPath(usuario.RutaImagen);
                        if (System.IO.File.Exists(rutaAnterior))
                        {
                            System.IO.File.Delete(rutaAnterior);
                        }
                    }

                    // Guardar el nuevo archivo
                    nuevaImagen.SaveAs(rutaGuardado);

                    // Actualizar la ruta en la base de datos
                    usuario.RutaImagen = "/Content/Uploads/Perfiles/" + nombreArchivo;

                    db.SaveChanges();

                    // 🚨 CRÍTICO: Actualizar la Sesión para el Layout
                    Session["RutaImagen"] = usuario.RutaImagen;

                    TempData["Mensaje"] = "Imagen de perfil actualizada correctamente.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al subir la imagen. Revise los permisos de escritura del servidor web. Causa: " + ex.Message;
                    return RedirectToAction("CambiarImagen");
                }
            }
            else
            {
                TempData["Error"] = "Por favor, selecciona un archivo para subir.";
                return RedirectToAction("CambiarImagen");
            }

            return RedirectToAction("Perfil");
        }
    }
}