using ClassLibrary1;
using MagicVilla_Web.Dto;
using MagicVilla_Web.Dto.ApiResponses;
using MagicVilla_Web.Services.@interface;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace MagicVilla_Web.Services.Implementation
{
    public class BaseService: IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ApiResponse _apiResponse { get; set; }
        public BaseService(IHttpClientFactory httpClientFactory)
        { 
            _httpClientFactory = httpClientFactory;
            _apiResponse = new ApiResponse() ;
        }


        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            var client= _httpClientFactory.CreateClient("MagicVilla");
            HttpRequestMessage message = new HttpRequestMessage();
            message.RequestUri =new Uri( apiRequest.url);
            message.Headers.Add("Accept", "application/json");
            switch (apiRequest.apiType)
            {
                case SD.ApiType.Put:
                    message.Method=HttpMethod.Put;
                    break;
                case SD.ApiType.Post:
                    message.Method = HttpMethod.Post;
                    break;
                case SD.ApiType.Delete:
                    message.Method = HttpMethod.Delete;
                    break;
                default:
                    message.Method = HttpMethod.Get;
                    break;
            }
            if (apiRequest.model != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.model),Encoding.UTF8, "application/json");
            }
            var response=  await client.SendAsync(message);
            var dserilize = JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            return dserilize;
        }
    }
}
