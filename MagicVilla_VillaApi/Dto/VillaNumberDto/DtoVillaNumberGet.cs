using AutoMapper;
using MagicVilla_VillaApi.Dto.VillaDto;
using MagicVilla_VillaApi.Models;

namespace MagicVilla_VillaApi.Dto.VillaNumberDto
{
    public class DtoVillaNumberGet
    {
        public int VillaNumberId { get; set; }

        public string SpitialDetails { get; set; }
        public int VillaId { get; set; }
        public DtoVillaGet dtoVillaGet { get; set; }
    }
}

