using AutoMapper;
using ClassLibrary1;
using MagicVilla_Web.CustomActionFilter;
using MagicVilla_Web.Dto.ApiResponses;
using MagicVilla_Web.Dto.VillaDto;
using MagicVilla_Web.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    [ValidationModel]
    public class VillaController : Controller
    {
        private readonly IVillaService _service;
        private readonly IMapper _mapper;
        public VillaController(IVillaService service, IMapper mapper)
        {
            _mapper = mapper;
            _service = service;
        }
        public async Task <IActionResult> Index()
        {
            var Villas = new List<DtoVillaGet>();
          var response= await _service.GetVillasAsync<ApiResponse>(HttpContext.Session.GetString(SD.AccessToken));
            if(response != null)
            {
                if (response.Success)
                {
                    Villas = JsonConvert.DeserializeObject<List<DtoVillaGet>>(Convert.ToString( response.result));
                }
            }
            return View(Villas);
        }
        public IActionResult CreateVilla()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVilla(DtoVillaCreate model)
        {
            if (ModelState.IsValid)
            {
               var result= await _service.CreateVillaAsync<ApiResponse>(model, HttpContext.Session.GetString(SD.AccessToken));
                if (result != null && result.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }
        public async Task< IActionResult> UpdateVilla(int Id)
        {
            var villa=new DtoVillaUpdate();
             var response=await _service.GetVillaAsync<ApiResponse>(Id, HttpContext.Session.GetString(SD.AccessToken));
            if (response != null&& response.Success)
            {
               var villaGet = JsonConvert.DeserializeObject<DtoVillaGet>(Convert.ToString(response.result));
               villa = _mapper.Map<DtoVillaUpdate>(villaGet);
                return View(villa);
            }
            if (response != null)
            {
                foreach (var item in response.Errors)
                {
                    ModelState.AddModelError("", item);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVilla(DtoVillaUpdate model)
        {
            var result = await _service.UpdateVillaAsync<ApiResponse>(model.Id, model, HttpContext.Session.GetString(SD.AccessToken));
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        public async Task<IActionResult> DeleteVilla(int Id)
        {
            var response = await _service.GetVillaAsync<ApiResponse>(Id, HttpContext.Session.GetString(SD.AccessToken));
            if (response != null && response.Success)
            {
                var villaGet = JsonConvert.DeserializeObject<DtoVillaGet>(Convert.ToString(response.result));
                return View(villaGet);
            }
            if (response!=null)
            {

                foreach (var item in response.Errors)
                {
                    ModelState.AddModelError("", item);
                }
            }
            return NotFound();
        }
        [HttpPost]
        [ActionName("DeleteVilla")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaPost(int Id)
        {
           var response=await _service.DeleteVillaAsync<ApiResponse>(Id, HttpContext.Session.GetString(SD.AccessToken));
            if (response.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            return View("Error");
        }
    }
}
