using AutoMapper;
using AutoMapper.QueryableExtensions;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Dto.VillaDto;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Services.Interfaces;
using MagicVilla_VillaApi.SharedRepo;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaApi.Services.Implementations
{
    public class VillaService: IVillaService
    {
        private readonly IGenericRepo<Villa> _sharedRepo;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _Context;
        public VillaService(IMapper mapper, ApplicationDbContext Context, IGenericRepo<Villa> sharedRepo)
        {
            _Context = Context;
            _mapper = mapper;
            _sharedRepo = sharedRepo;
        }
        public async Task<IEnumerable<DtoVillaGet>> GetVillasAsync()
        {
            var villa = await _mapper.ProjectTo<DtoVillaGet>(_Context.Villas.AsQueryable()).ToListAsync();
            return villa;
        }
        public async Task<DtoVillaGet> GetVillaAsyncById(int id)
        {
            return await _mapper.ProjectTo<DtoVillaGet>(_Context.Villas.AsQueryable()).FirstOrDefaultAsync(v => v.Id == id);

        }
        public async Task<bool> UpdateVilla(int Id, DtoVillaUpdate model)
        {
            if (await _sharedRepo.GetAsync(v => v.Id == Id) == null || model.Id != Id)
            {
                return false;
            }
            var villa = _mapper.Map<Villa>(model);
            villa.UpdatedDate= DateTime.UtcNow;
            await _sharedRepo.UpdateAsync(villa);
            return true;
        }

        public async Task CreateVilla(DtoVillaCreate model)
        {
            await _sharedRepo.AddAsync(_mapper.Map<Villa>(model));

        }
        public async Task<bool> DeleteVilla(int id)
        {
            var villa = await _sharedRepo.GetAsync(v => v.Id == id);
            if (villa == null)
            {
                return false;
            }
            await _sharedRepo.RemoveAsync(villa);
            return true;
        }
    }

}
