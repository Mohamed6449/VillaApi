using AutoMapper;
using ClassLibrary1;
using MagicVilla_Web.CustomActionFilter;
using MagicVilla_Web.Dto.ApiResponses;
using MagicVilla_Web.Dto.VillaDto;
using MagicVilla_Web.Dto.VillaNumberDto;
using MagicVilla_Web.Services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    [ValidationModel]
    public class VillaNumberController : Controller
    {
        private readonly IVillaService _serviceVilla;
        private readonly IVillaNumberService _service;
        private readonly IMapper _mapper;
        public VillaNumberController(IVillaNumberService service, IMapper mapper, IVillaService serviceVilla)
        {
            _serviceVilla = serviceVilla;
            _mapper = mapper;
            _service = service;
        }
        [Authorize]
        public async Task <IActionResult> Index()
        {
            var Villas = new List<DtoVillaNumberGet>();
          var response= await _service.GetVillaNumbersAsync<ApiResponse>(HttpContext.Session.GetString(SD.KeySessionJWT));
            if(response != null)
            {
                if (response.Success)
                {
                    Villas = JsonConvert.DeserializeObject<List<DtoVillaNumberGet>>(Convert.ToString( response.result));
                }
            }
            return View(Villas);
        }
        public async Task <IActionResult> CreateVillaNumber()
        {

            ViewBag.VillaName =await GetListNameVilla();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillaNumber(DtoVillaNumberCreate model)
        {
            if (ModelState.IsValid)
            {
                var result = await _service.CreateVillaNumberAsync<ApiResponse>(model, HttpContext.Session.GetString(SD.KeySessionJWT));
                if (result != null && result.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                 ViewBag.VillaName =await GetListNameVilla();
                foreach (var i in result.Errors) {
                    ModelState.AddModelError("", i);
                 }
            }
            return View(model);
        }
        public async Task<IActionResult> UpdateVillaNumber(int Id)
        {

            var response = await _service.GetVillaNumberAsync<ApiResponse>(Id, HttpContext.Session.GetString(SD.KeySessionJWT));
            if (response != null && response.Success)
            {
                 ViewBag.VillaName =await GetListNameVilla();
                var villaGet = JsonConvert.DeserializeObject<DtoVillaNumberGet>(Convert.ToString(response.result));
               var villa = _mapper.Map<DtoVillaNumberUpdate>(villaGet);
                return View(villa);
            }
            foreach (var item in response.Errors)
            {
                ModelState.AddModelError("", item);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateVillaNumber(DtoVillaNumberUpdate model)
        {
            var result = await _service.UpdateVillaNumberAsync<ApiResponse>(model.VillaNumberId, model, HttpContext.Session.GetString(SD.KeySessionJWT));
            if (result.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            foreach (var i in result.Errors)
            {
                ModelState.AddModelError("", i);
            }
            ViewBag.VillaName =await GetListNameVilla();
            return View(model);
        }
        public async Task<IActionResult> DeleteVillaNumber(int Id)
        {
            var response = await _service.GetVillaNumberAsync<ApiResponse>(Id, HttpContext.Session.GetString(SD.KeySessionJWT));
            if (response != null && response.Success)
            {
                var villaGet = JsonConvert.DeserializeObject<DtoVillaNumberGet>(Convert.ToString(response.result));
                return View(villaGet);
            }
            foreach (var item in response.Errors)
            {
                ModelState.AddModelError("", item);
            }
            return NotFound();
        }
        [HttpPost]
        [ActionName("DeleteVillaNumber")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillaNumberPost(int Id)
        {
            var response = await _service.DeleteVillaNumberAsync<ApiResponse>(Id, HttpContext.Session.GetString(SD.KeySessionJWT) );
            if (response.Success)
            {
                return RedirectToAction(nameof(Index));
            }
            return View("Error");
        }


        public async Task<SelectList> GetListNameVilla()
        {
            var Villas = new List<DtoVillaGet>();
            var response = await _serviceVilla.GetVillasAsync<ApiResponse>(HttpContext.Session.GetString(SD.KeySessionJWT));
            if (response != null)
            {
                if (response.Success)
                {
                    Villas = JsonConvert.DeserializeObject<List<DtoVillaGet>>(Convert.ToString(response.result));
                }
            }

            return new SelectList(Villas, "Id", "Name");
        }
    }
}
