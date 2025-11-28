using ReservaYa.Models;
using ReservaYa.Models.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ReservaYa.Services
{
    public static class DetailsEspacioService
    {
        public static EspacioDetallesDto Buscar(int? id)
        {
            if (id == null) return null;

            using (var db = new DEVELOSERSEntities())
            {
                // Consulta principal con joins y navegación
                var resultado = db.Espacios
                    .AsNoTracking()
                    .Where(e => e.EspacioID == id)
                    .Select(e => new EspacioDetallesDto
                    {
                        EspacioId = e.EspacioID,
                        NombreEspacio = e.Nombre,

                        CategoriaId = e.CategoriaID,
                        NombreCategoria = e.Categorias.Nombre,

                        EspacioDetalleId = e.EspaciosDetalles
                            .Select(d => d.EspacioDetalleID)
                            .FirstOrDefault(),

                        ValorPorHora = e.EspaciosDetalles
                            .Select(d => d.ValorPorHora)
                            .FirstOrDefault(),

                        FechasDisponiblesIds = e.ReservasFechasDisponibles
                            .Select(r => r.FechasDisponibles.FechaDisponibleID)
                            .ToList(),

                        FechasDisponibles = e.ReservasFechasDisponibles
                            .Select(r => r.FechasDisponibles.Fecha)
                            .ToList()
                    })
                    .FirstOrDefault();
                return resultado;
            }
        }
    }

}