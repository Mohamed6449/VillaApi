using MagicVilla_Web.Dto.VillaDto;

namespace MagicVilla_Web.Services.interfaces
{
    public interface IVillaService
    {
        public Task<T> GetVillasAsync<T>(string token);

        public Task<T> GetVillaAsync<T>(int Id, string token);

        public Task<T> CreateVillaAsync<T>(DtoVillaCreate entity, string token);

        public Task<T> UpdateVillaAsync<T>(int Id ,DtoVillaUpdate entity, string token);

        public Task<T> DeleteVillaAsync<T>(int Id, string token);

        
    }
}
