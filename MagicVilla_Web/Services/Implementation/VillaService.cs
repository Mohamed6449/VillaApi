using ClassLibrary1;
using MagicVilla_Web.Dto;
using MagicVilla_Web.Dto.VillaDto;
using MagicVilla_Web.Services.@interface;
using MagicVilla_Web.Services.interfaces;

namespace MagicVilla_Web.Services.Implementation
{
    public class VillaService :BaseService, IVillaService
    {
        private readonly ApiRequest _apiRequest;
        private readonly string Url;
        public VillaService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            Url = configuration.GetValue<string>("ApiUrl:applicationUrl")+ "/api/v1/VillaApi/";
            _apiRequest = new();
        }
        public async Task<T> CreateVillaAsync<T>(DtoVillaCreate entity, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                url = Url,
                model = entity,
                apiType = SD.ApiType.Post,
                token = token


            });



        }

        public async Task<T> DeleteVillaAsync<T>(int Id, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                url = Url+Id,
                apiType = SD.ApiType.Delete,
                token = token

            });
        }

        public async Task<T> GetVillaAsync<T>(int Id, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                url = Url+Id,
                apiType = SD.ApiType.Get,
                token = token

            });
        }

        public async Task<T> GetVillasAsync<T>(string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                url = Url,
                apiType = SD.ApiType.Get,
                token = token

            });
        }

        public async Task<T> UpdateVillaAsync<T>(int Id, DtoVillaUpdate entity, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                url = Url+Id,
                model = entity,
                apiType = SD.ApiType.Put,
                token = token

            });
        }
    }
}
