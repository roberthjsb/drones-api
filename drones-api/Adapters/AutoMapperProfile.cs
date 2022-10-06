using AutoMapper;
using drones_api.DTOS;
using drones_api.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace drones_api.Adapters
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterDronDTO, Dron>();
            CreateMap<MedicineDto, Medicine>()
               .ForMember(
        dest => dest.Image,
        opt => opt.MapFrom(src => this.Convert(src.Image)));
            CreateMap<Dron, DronDtoResult>();
        }

        private byte[] Convert(IFormFile form)
        {
            using(var ms = new MemoryStream())
            {
                if (form == null) return ms.ToArray();
                form.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
