using ClassLibrary1;
using MagicVilla_Web.Dto;
using MagicVilla_Web.Dto.VillaDto;
using MagicVilla_Web.Dto.VillaNumberDto;
using MagicVilla_Web.Services.@interface;
using MagicVilla_Web.Services.interfaces;

namespace MagicVilla_Web.Services.Implementation
{
    public class VillaNumberService : BaseService, IVillaNumberService
    {
        private readonly ApiRequest _apiRequest;
        private readonly string Url;
        public VillaNumberService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            Url = configuration.GetValue<string>("ApiUrl:applicationUrl") + "/api/v1/VillaNumberApi/";
            _apiRequest = new();
        }
        public async Task<T> CreateVillaNumberAsync<T>(DtoVillaNumberCreate entity,string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                url = Url,
                model = entity,
                apiType = SD.ApiType.Post,
                token = token

            });



        }

        public async Task<T> DeleteVillaNumberAsync<T>(int Id, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                url = Url + Id,
                apiType = SD.ApiType.Delete,
                token = token

            });
        }

        public async Task<T> GetVillaNumberAsync<T>(int Id, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                url = Url + Id,
                apiType = SD.ApiType.Get,
                token = token

            });
        }

        public async Task<T> GetVillaNumbersAsync<T>(string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                url = Url,
                apiType = SD.ApiType.Get,
                token = token

            });
        }

        public async Task<T> UpdateVillaNumberAsync<T>(int Id, DtoVillaNumberUpdate entity, string token)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                url = Url + Id,
                model = entity,
                apiType = SD.ApiType.Put,
                token = token

            });
        }
    }
}
