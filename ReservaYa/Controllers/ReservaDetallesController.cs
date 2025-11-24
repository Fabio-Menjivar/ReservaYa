using ReservaYa.Models;
using ReservaYa.Models.Extras;
using ReservaYa.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ReservaYa.Controllers
{////Servicio que relaciona , categorias, fechas disponibles ,espacio detalles ,categorias
    public class ReservaDetallesController : Controller
    {              
        private EspacioDetallesDto _EspacioDetallesDto;
        public ActionResult Mostrar(int? id) 
        {                       
            if (id == null) return new HttpNotFoundResult();

            //servicio auxialiar
            _EspacioDetallesDto = DetailsEspacioService.Buscar(id);

            var bd = new DEVELOSERSEntities();

            //Datos auxiliares relevantes a Espacios
            ViewBag.FechasDp = _EspacioDetallesDto.FechasDisponibles;
            ViewBag.FechasDpId = _EspacioDetallesDto.FechasDisponiblesIds;
            ViewBag.CategoriaName = _EspacioDetallesDto.NombreCategoria;
            ViewBag.EspaciosDtId = _EspacioDetallesDto.EspacioDetalleId;
            ViewBag.EspaciosDtVh = _EspacioDetallesDto.ValorPorHora;

            //Mandamos un modelo para visualizar algunos datos
            var espacio = bd.Espacios
            .AsNoTracking()
            .FirstOrDefault(x => x.EspacioID == _EspacioDetallesDto.EspacioId);

            if (espacio == null)
                return new HttpNotFoundResult();

            return View(espacio);

        }
    }
}