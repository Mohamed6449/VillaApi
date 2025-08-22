using MagicVilla_Web.Dto;
using MagicVilla_Web.Dto.ApiResponses;

namespace MagicVilla_Web.Services.@interface
{
    public interface IBaseService
    {
        public ApiResponse _apiResponse { get; set; }

        public Task<T> SendAsync<T>(ApiRequest ApiRequest);
    }
}
