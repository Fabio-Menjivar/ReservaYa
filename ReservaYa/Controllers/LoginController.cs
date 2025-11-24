using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ReservaYa.Models;
using System.Collections.Generic;

namespace ReservaYa.Controllers
{
    public class LoginController : Controller
    {
        private DEVELOSERSEntities db = new DEVELOSERSEntities();

        // GET: Login/Login
        public ActionResult Login()
        {
            if (Session["UsuarioID"] != null)
            {
                int? rolId = Session["RolID"] as int?;
                if (rolId == 1)
                {
                    return RedirectToAction("Create", "GestionEspacios");
                }
                else if (rolId == 2)
                {
                    return RedirectToAction("Index", "GestionEspacios");
                }
            }
            return View();
        }

        // POST: Login/Login (Lógica de Comparación de Bytes Corregida)
        [HttpPost]
        public ActionResult Login(string Correo, string Contrasena)
        {
            if (string.IsNullOrEmpty(Correo) || string.IsNullOrEmpty(Contrasena))
            {
                ViewBag.Mensaje = "Por favor, complete todos los campos.";
                return View();
            }

            // 1. Buscamos el usuario por Correo (buscamos por string limpio en la DB).
            var usuario = db.Usuarios
                .Where(u => u.Activo == true)
                .AsEnumerable()
                .FirstOrDefault(u =>
                    Encoding.UTF8.GetString(u.Correo).Trim()
                        .Equals(Correo.Trim(), StringComparison.OrdinalIgnoreCase)
                );

            if (usuario == null)
            {
                // Si el usuario no existe o está inactivo.
                ViewBag.Mensaje = "Correo o contraseña incorrectos.";
                return View();
            }

            // 2. Comparamos la Contraseña (En Bytes)

            // Convertimos la contraseña ingresada a su arreglo de bytes (como se hizo en el registro).
            byte[] contraBytesInput = Encoding.UTF8.GetBytes(Contrasena.Trim());

            // Limpiamos los arrays de bytes de relleno (padding) si existen
            // Esto es crucial para la comparación binaria
            byte[] contraDB = usuario.Contrasena.Where(b => b != 0).ToArray();
            byte[] contraInput = contraBytesInput.Where(b => b != 0).ToArray();

            // 3. Verificamos la igualdad de los bytes limpios
            if (contraDB.SequenceEqual(contraInput))
            {
                // INICIO DE SESIÓN EXITOSO
                Session["UsuarioID"] = usuario.UsuarioID;
                Session["NombreUsuario"] = $"{usuario.Nombres} {usuario.Apellidos}";
                Session["RolID"] = usuario.RolID;

                // Redirección por Rol
                if (usuario.RolID == 1)
                {
                    return RedirectToAction("Homepage", "GestionEspacios");
                }
                else if (usuario.RolID == 2)
                {
                    return RedirectToAction("Index", "GestionEspacios");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // Si llegamos aquí, la contraseña binaria no coincidió.
            ViewBag.Mensaje = "Correo o contraseña incorrectos.";
            return View();
        }

        // GET: Login/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Login/Register 
        [HttpPost]
        public ActionResult Register(string Nombres, string Apellidos, DateTime FechaNacimiento, string Correo, string Contrasena)
        {
            if (string.IsNullOrEmpty(Correo) || string.IsNullOrEmpty(Contrasena))
            {
                ViewBag.Mensaje = "Todos los campos son obligatorios.";
                return View();
            }

            byte[] correoBytes = Encoding.UTF8.GetBytes(Correo.Trim());
            byte[] contraBytes = Encoding.UTF8.GetBytes(Contrasena.Trim());

            // Búsqueda de correo existente usando string limpio
            bool correoExiste = db.Usuarios
                .AsEnumerable()
                .Any(u => Encoding.UTF8.GetString(u.Correo).Trim()
                    .Equals(Correo.Trim(), StringComparison.OrdinalIgnoreCase));

            if (correoExiste)
            {
                ViewBag.Mensaje = "Ya existe un usuario con ese correo.";
                return View();
            }


            Usuarios nuevo = new Usuarios
            {
                Nombres = Nombres,
                Apellidos = Apellidos,
                FechaNacimiento = FechaNacimiento,
                Correo = correoBytes,
                Contrasena = contraBytes,
                RolID = 2, // Usuario común
                Activo = true
            };

            db.Usuarios.Add(nuevo);
            db.SaveChanges();

            ViewBag.Mensaje = "Registro exitoso. Ahora puede iniciar sesión.";
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login");
        }
    }
}