using AutoMapper;
using MagicVilla_VillaApi.Dto.VillaDto;
using MagicVilla_VillaApi.Dto.VillaNumberDto;

namespace MagicVilla_VillaApi.Mapper
{
    public class VillaProfile:Profile
    {
        public VillaProfile()
        {
            CreateMap<DtoVillaGet, Models.Villa>().ReverseMap();
            CreateMap<DtoVillaCreate, Models.Villa>().ReverseMap();
            CreateMap<DtoVillaUpdate, Models.Villa>().ReverseMap();
            CreateMap<DtoVillaGet,DtoVillaUpdate>();
            CreateMap<DtoVillaNumberGet, Models.VillaNumber>().ReverseMap();
            CreateMap<DtoVillaNumberCreate, Models.VillaNumber>().ReverseMap();
            CreateMap<DtoVillaNumberUpdate, Models.VillaNumber>().ReverseMap();

        }
    }
}
