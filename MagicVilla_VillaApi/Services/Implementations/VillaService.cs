using AutoMapper;
using AutoMapper.QueryableExtensions;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Dto.VillaDto;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Services.Interfaces;
using MagicVilla_VillaApi.SharedRepo;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace MagicVilla_VillaApi.Services.Implementations
{
    public class VillaService: IVillaService
    {
        private readonly IGenericRepo<Villa> _sharedRepo;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _Context;
        private readonly IFileService _fileService;
        public VillaService(IMapper mapper, ApplicationDbContext Context, IGenericRepo<Villa> sharedRepo, IFileService fileService)
        {
            _Context = Context;
            _mapper = mapper;
            _sharedRepo = sharedRepo;
            _fileService = fileService;
        }
        public async Task<IEnumerable<DtoVillaGet>> GetVillasAsync(Expression<Func<Villa,bool>>func=null, int pageNumber = 1, int pageSize = 6)
        {
            IQueryable<Villa> villas=_Context.Villas.AsQueryable();
            if (func != null)
            {
                villas= villas.Where(func);
            }
            if (pageNumber > 0)
            {
                if (pageSize > 100)
                {
                    pageSize = 100;
                }
                villas=villas.Skip(pageSize*(pageNumber-1)).Take(pageSize);
            }
            return  await _mapper.ProjectTo<DtoVillaGet>(villas).ToListAsync();
        }






        public async Task<DtoVillaGet> GetVillaAsyncById(int id)
        {
            return await _mapper.ProjectTo<DtoVillaGet>(_Context.Villas.AsQueryable()).FirstOrDefaultAsync(v => v.Id == id);

        }
        public async Task<bool> UpdateVilla(int Id, DtoVillaUpdate model, string baseUrl=null)
        {
            var villa = await _sharedRepo.GetAsync(v => v.Id == Id);
            if (villa == null || model.Id != Id)
            {
                return false;
            }
            var  Updatevilla = _mapper.Map(model, villa);
            Updatevilla.UpdatedDate= DateTime.UtcNow;
           var uploud=  await _fileService.UploudFileWithId(model.File, baseUrl, Updatevilla.Id);
            Updatevilla.UrlImg = uploud.Url;
            Updatevilla.LocalUrlImg = uploud.LocalPath;
            await _sharedRepo.UpdateAsync(Updatevilla);
            return true;
        }

        public async Task CreateVilla(DtoVillaCreate model,string baseUrl=null)
        {
            var villa = _mapper.Map<Villa>(model);
            villa =  await _sharedRepo.AddAsync(_mapper.Map<Villa>(model));

           var uploud= await _fileService.UploudFileWithId(model.File, baseUrl, villa.Id);
            villa.UrlImg = uploud.Url;
            villa.LocalUrlImg = uploud.LocalPath;
            await _sharedRepo.UpdateAsync(villa);
        }
        public async Task<bool> DeleteVilla(int id)
        {
            var villa = await _sharedRepo.GetAsync(v => v.Id == id);
            if (villa == null)
            {
                return false;
            }
            _fileService.Delete(villa.LocalUrlImg);
            await _sharedRepo.RemoveAsync(villa);
            return true;
        }
    }

}
