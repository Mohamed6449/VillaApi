using MagicVilla_VillaApi.Dto.VillaNumberDto;

namespace MagicVilla_VillaApi.Services.Interfaces
{
    public interface IVillaNumberService
    {
        public Task<IEnumerable<DtoVillaNumberGet>> GetVillaNumberNumbersAsync();
        public Task<DtoVillaNumberGet> GetVillaNumberAsyncById(int id);
        public Task<bool> UpdateVillaNumber(int Id, DtoVillaNumberUpdate model);
        public Task<(bool Result, string error)> CreateVillaNumber(DtoVillaNumberCreate model);
       public  Task<bool> DeleteVillaNumber(int id);
    }
}
