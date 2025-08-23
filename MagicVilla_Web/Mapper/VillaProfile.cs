using AutoMapper;
using MagicVilla_Web.Dto.VillaDto;
using MagicVilla_Web.Dto.VillaNumberDto;

namespace MagicVilla_Web.Mapper
{
    public class VillaProfile:Profile
    {
        public VillaProfile()
        {
            CreateMap<DtoVillaCreate, DtoVillaGet>().ReverseMap();
            CreateMap<DtoVillaUpdate, DtoVillaGet>().ReverseMap();
            CreateMap<DtoVillaUpdate, DtoVillaCreate>().ReverseMap();
            CreateMap<DtoVillaNumberCreate, DtoVillaNumberGet>().ReverseMap();
            CreateMap<DtoVillaNumberUpdate, DtoVillaNumberGet>().ReverseMap();
            CreateMap<DtoVillaNumberUpdate, DtoVillaNumberCreate>().ReverseMap();
        }
    }
}
