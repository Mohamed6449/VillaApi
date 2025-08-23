using AutoMapper;
using CQRS_test.ViewModels.Identity;
using MagicVilla_VillaApi.Dto.Identity;
using MagicVilla_VillaApi.Dto.VillaDto;
using MagicVilla_VillaApi.Dto.VillaNumberDto;
using MagicVilla_VillaApi.Models;

namespace MagicVilla_VillaApi.Mapper
{
    public class VillaProfile:Profile
    {
        public VillaProfile()
        {
            CreateMap<DtoVillaGet, Villa>().ReverseMap();
            CreateMap<DtoVillaCreate, Villa>().ReverseMap();
            CreateMap<DtoVillaUpdate, Villa>().ReverseMap();
            CreateMap<DtoVillaGet,DtoVillaUpdate>();
            CreateMap<VillaNumber, DtoVillaNumberGet>().ForMember(dest=> dest.dtoVillaGet,opt=>opt.MapFrom(src=>src.villa));
            CreateMap<DtoVillaNumberCreate,VillaNumber>().ReverseMap();
            CreateMap<DtoVillaNumberUpdate,VillaNumber>().ReverseMap();
            CreateMap<RegisterViewModel, User>();
            CreateMap<User, DtoUser>();

        }
    }
}
