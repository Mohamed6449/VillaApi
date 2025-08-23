using AutoMapper;
using MagicVilla_VillaApi.Data;
using MagicVilla_VillaApi.Dto.VillaNumberDto;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Services.Interfaces;
using MagicVilla_VillaApi.SharedRepo;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaApi.Services.Implementations
{
    public class VillaNumberService : IVillaNumberService
    {
        private readonly IGenericRepo<VillaNumber> _sharedRepo;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _Context;

        public VillaNumberService(IMapper mapper, ApplicationDbContext Context, IGenericRepo<VillaNumber> sharedRepo)
        {
            _Context = Context;
            _mapper = mapper;
            _sharedRepo = sharedRepo;
        }
        public async Task<IEnumerable<DtoVillaNumberGet>> GetVillaNumberNumbersAsync()
        {
            var VillaNumber = await _mapper.ProjectTo<DtoVillaNumberGet>(_Context.VillaNumbers.Include(i=>i.villa).AsQueryable()).ToListAsync();
            return VillaNumber;
        }
        public async Task<DtoVillaNumberGet> GetVillaNumberAsyncById(int id)
        {
            return await _mapper.ProjectTo<DtoVillaNumberGet>(_Context.VillaNumbers).FirstOrDefaultAsync(v => v.VillaNumberId == id);

        }
        public async Task<bool> UpdateVillaNumber(int Id, DtoVillaNumberUpdate model)
        {
            var villaNumber = await _sharedRepo.GetAsync(v =>v.VillaNumberId == Id);
            if (villaNumber == null)
            {
                return false;
            }
            var VillaNumber = _mapper.Map(model, villaNumber);
            VillaNumber.UpdatedDate= DateTime.UtcNow;
            await _sharedRepo.UpdateAsync(VillaNumber);
            return true;
        }

        public async Task<(bool Result,string error)> CreateVillaNumber(DtoVillaNumberCreate model)
        {
            if(await _sharedRepo.GetAsync(G => G.VillaNumberId == model.VillaNumberId) != null)
            {
                return (false, "Villa Number Id Is Exists");
            }
            if(! _Context.Villas.Any(A=>A.Id==model.VillaId))
            {
                return (false, "Villa Id Not Exists");
            }
            var villaNumber = _mapper.Map<VillaNumber>(model);
            await _sharedRepo.AddAsync(villaNumber);
            return (true, "o");
        }
        public async Task<bool> DeleteVillaNumber(int id)
        {
            var VillaNumber = await _sharedRepo.GetAsync(v => v.VillaNumberId == id);
            if (VillaNumber == null)
            {
                return false;
            }
            await _sharedRepo.RemoveAsync(VillaNumber);
            return true;
        }

    }

}
