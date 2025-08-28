using MagicVilla_VillaApi.Dto.VillaDto;
using MagicVilla_VillaApi.Models;
using System.Linq.Expressions;


namespace MagicVilla_VillaApi.Services.Interfaces
{
    public interface IVillaService
    {
        public Task<IEnumerable<DtoVillaGet>> GetVillasAsync(Expression< Func<Villa,bool>> func=null,int pageNumber=1,int pageSize=6);
        public Task<DtoVillaGet> GetVillaAsyncById(int id);
        public Task<bool> UpdateVilla(int Id, DtoVillaUpdate model, string baseUrl = null);
        public Task CreateVilla(DtoVillaCreate model, string baseUrl=null);
       public  Task<bool> DeleteVilla(int id);
    }
}
