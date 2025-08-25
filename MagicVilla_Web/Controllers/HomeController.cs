using AutoMapper;
using ClassLibrary1;
using MagicVilla_Web.Dto.ApiResponses;
using MagicVilla_Web.Dto.VillaDto;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MagicVilla_Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IVillaService _service;
        private readonly IMapper _mapper;
        public HomeController(IVillaService service, IMapper mapper)
        {
            _mapper = mapper;
            _service = service;
        }
        public async Task<IActionResult> Index()
        {
            var Villas = new List<DtoVillaGet>();
            var response = await _service.GetVillasAsync<ApiResponse>(HttpContext.Session.GetString(SD.KeySessionJWT));
            if (response != null)
            {
                if (response.Success)
                {
                    Villas = JsonConvert.DeserializeObject<List<DtoVillaGet>>(Convert.ToString(response.result));
                }
            }
            return View(Villas);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
