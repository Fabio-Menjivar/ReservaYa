using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReservaYa.Controllers
{
    public class FechasDisponiblesController : Controller
    {
        // GET: FechasDisponibles
        public ActionResult Index(string idFechaDP)
        {

            return View();
        }
    }
}