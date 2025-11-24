using System.Collections.Generic;
using System.Linq;
using ReservaYa.Models;
using ReservaYa.Models.Extras;

namespace ReservaYa.Services
{
    public class EspaciosVMConvertService
    {
        // Convierte lista de entidades a lista de viewmodels
        public List<EspacioViewModel> Convert(List<Espacios> list)
        {
            if (list == null || !list.Any())
                return new List<EspacioViewModel>();

            var result = new List<EspacioViewModel>();

            foreach (var item in list)
            {
                var itemNow = Convert(item);
                result.Add(itemNow);
            }

            return result;
        }

        // Convierte una sola entidad a su ViewModel
        public EspacioViewModel Convert(Espacios espacio)
        {
            if (espacio == null)
                return null;

            EspacioViewModel result = new EspacioViewModel
            {
                EspacioID = espacio.EspacioID,
                Nombre = espacio.Nombre,
                CategoriaID = espacio.CategoriaID,
                Capacidad = espacio.Capacidad,
                Direccion = espacio.Direccion,
                UbicacionEnlace = espacio.UbicacionEnlace,
                Estacionamiento = espacio.Estacionamiento,
                Sanitarios = espacio.Sanitarios,
                AccesoSillaRuedas = espacio.AccesoSillaRuedas,
                ImagenPrev = espacio.ImagenPrev,
                Disponible = espacio.Disponible,
                EspacioIdCifrado = EncriptarService.EncriptarId(espacio.EspacioID)
            };

            return result;
        }

        // Convierte lista de viewmodels a lista de entidades
        public List<Espacios> Reverse(List<EspacioViewModel> list)
        {
            if (list == null || !list.Any())
                return new List<Espacios>();

            var result = new List<Espacios>();

            foreach (var item in list)
            {
                var itemNow = Reverse(item);
                result.Add(itemNow);
            }

            return result;
        }

        // Convierte un solo viewmodel a entidad
        public Espacios Reverse(EspacioViewModel espacio)
        {
            if (espacio == null)
                return null;

            var result = new Espacios
            {
                EspacioID = espacio.EspacioID,
                Nombre = espacio.Nombre,
                CategoriaID = espacio.CategoriaID,
                Capacidad = espacio.Capacidad,
                Direccion = espacio.Direccion,
                UbicacionEnlace = espacio.UbicacionEnlace,
                Estacionamiento = espacio.Estacionamiento,
                Sanitarios = espacio.Sanitarios,
                AccesoSillaRuedas = espacio.AccesoSillaRuedas,
                ImagenPrev = espacio.ImagenPrev,
                Disponible = espacio.Disponible
            };

            return result;
        }
    }
}
