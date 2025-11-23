using ReservaYa.Models;
using ReservaYa.Models.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservaYa.Services
{
    public class EspacioDetallesVMConvertService
    {
        // ENTIDAD → VIEWMODEL
        public EspacioDetalleViewModel ToViewModel(EspaciosDetalles entity)
        {
            if (entity == null) return null;

            return new EspacioDetalleViewModel
            {
                Nombre = entity.Espacios?.Nombre,
                IdEspacioEncriptada = EncriptarService.EncriptarId(entity.EspacioID),
                ValorXHora = entity.ValorPorHora
            };
        }

        // VIEWMODEL → ENTIDAD
        public EspaciosDetalles ToEntity(EspacioDetalleViewModel vm)
        {
            if (vm == null) return null;

            return new EspaciosDetalles
            {
                EspacioID = EncriptarService.DescriptarId(vm.IdEspacioEncriptada),
                ValorPorHora = vm.ValorXHora
            };
        }

        // LISTA ENTIDADES → LISTA VIEWMODELS
        public List<EspacioDetalleViewModel> ToViewModelList(List<EspaciosDetalles> entities)
        {
            if (entities == null) return new List<EspacioDetalleViewModel>();

            return entities
                .Select(e => ToViewModel(e))
                .ToList();
        }

        // LISTA VIEWMODELS → LISTA ENTIDADES
        public List<EspaciosDetalles> ToEntityList(List<EspacioDetalleViewModel> vms)
        {
            if (vms == null) return new List<EspaciosDetalles>();

            return vms
                .Select(vm => ToEntity(vm))
                .ToList();
        }
    }

}