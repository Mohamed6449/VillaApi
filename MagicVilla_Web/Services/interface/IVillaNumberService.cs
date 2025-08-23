using MagicVilla_Web.Dto.VillaNumberDto;

namespace MagicVilla_Web.Services.interfaces
{
    public interface IVillaNumberService
    {
        public Task<T> GetVillaNumbersAsync<T>();

        public Task<T> GetVillaNumberAsync<T>(int Id);

        public Task<T> CreateVillaNumberAsync<T>(DtoVillaNumberCreate entity);

        public Task<T> UpdateVillaNumberAsync<T>(int Id, DtoVillaNumberUpdate entity);

        public Task<T> DeleteVillaNumberAsync<T>(int Id);


    }
}
