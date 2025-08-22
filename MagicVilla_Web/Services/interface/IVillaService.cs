using MagicVilla_Web.Dto.VillaDto;

namespace MagicVilla_Web.Services.interfaces
{
    public interface IVillaService
    {
        public Task<T> GetVillasAsync<T>();

        public Task<T> GetVillaAsync<T>(int Id);

        public Task<T> CreateVillaAsync<T>(DtoVillaCreate entity);

        public Task<T> UpdateVillaAsync<T>(int Id ,DtoVillaUpdate entity);

        public Task<T> DeleteVillaAsync<T>(int Id);

        
    }
}
