using MagicVilla_VillaApi.Dto.VillaDto;

namespace MagicVilla_VillaApi.Services.Interfaces
{
    public interface IVillaService
    {
        public Task<IEnumerable<DtoVillaGet>> GetVillasAsync();
        public Task<DtoVillaGet> GetVillaAsyncById(int id);
        public Task<bool> UpdateVilla(int Id, DtoVillaUpdate model);
        public Task CreateVilla(DtoVillaCreate model);
       public  Task<bool> DeleteVilla(int id);
    }
}
