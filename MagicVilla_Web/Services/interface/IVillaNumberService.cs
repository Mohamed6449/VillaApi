using MagicVilla_Web.Dto.VillaNumberDto;

namespace MagicVilla_Web.Services.interfaces
{
    public interface IVillaNumberService
    {
        public Task<T> GetVillaNumbersAsync<T>(string token);

        public Task<T> GetVillaNumberAsync<T>(int Id, string token);

        public Task<T> CreateVillaNumberAsync<T>(DtoVillaNumberCreate entity, string token);

        public Task<T> UpdateVillaNumberAsync<T>(int Id, DtoVillaNumberUpdate entity, string token);

        public Task<T> DeleteVillaNumberAsync<T>(int Id, string token);


    }
}
