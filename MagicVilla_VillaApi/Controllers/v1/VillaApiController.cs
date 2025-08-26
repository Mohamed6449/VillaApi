using AutoMapper;
using CQRS_test.CustomActionFilter;
using MagicVilla_VillaApi.Dto.ApiResponses;
using MagicVilla_VillaApi.Dto.VillaDto;
using MagicVilla_VillaApi.Models;
using MagicVilla_VillaApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaApi.Controllers.v1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ValidationModel]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    public class VillaApiController : ControllerBase
    {
        private readonly ApiResponse _ApiResponse;
        private readonly IMapper _mapper;
        private readonly IVillaService _villaService;
        
        public VillaApiController(IVillaService villaService, IMapper mapper)
        {
            _mapper = mapper;
            _villaService = villaService;
            _ApiResponse = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        [ResponseCache(Duration =30)]
        public async Task <ActionResult<ApiResponse>> GetVillas([FromQuery(Name ="Filter")]int? occumpancy,[FromQuery(Name="Search")]string search, int pageNumber = 1, int pageSize = 6)
        {
            IEnumerable<DtoVillaGet> villas;
            if (occumpancy > 0)
            {
                villas = await _villaService.GetVillasAsync(A=>A.Occupancy==occumpancy,pageNumber,pageSize);
            }
            else
            {
                villas  = await _villaService.GetVillasAsync(pageNumber:pageNumber,pageSize:pageSize);
            }
            if (search != null)
            {
                villas = villas.Where(W=>W.Name.ToLower().Contains(search.ToLower()));
            }
            _ApiResponse.result = villas;
             _ApiResponse.statusCode = HttpStatusCode.OK;
            return Ok(_ApiResponse);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ApiResponse>> GetVilla(int id)
        {
            
            var villa = await _villaService.GetVillaAsyncById(id);
            if (villa == null)
            {
               _ApiResponse.statusCode = HttpStatusCode.NotFound;
                _ApiResponse.Success = false;
                return Ok(_ApiResponse);
            }
            _ApiResponse.statusCode = HttpStatusCode.OK;
            _ApiResponse.result = villa;
            return Ok(_ApiResponse);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> CreateVilla([FromBody]DtoVillaCreate model)
        {

             await  _villaService.CreateVilla( model);
            _ApiResponse.statusCode = HttpStatusCode.Created;
            _ApiResponse.result = model;
            return Ok(_ApiResponse);
        }




        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> UpdateVilla(int id,[FromBody]DtoVillaUpdate model)
        {
          var result= await  _villaService.UpdateVilla(id, model);
            if (!result)
            {
                _ApiResponse.statusCode = HttpStatusCode.NotFound;
                _ApiResponse.Success = false;
                return Ok(_ApiResponse);
           
            }
            _ApiResponse.statusCode = HttpStatusCode.NoContent;
            _ApiResponse.result = model;
          
            return Ok(_ApiResponse);
        }



        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteVilla(int id)
        {
            var result = await _villaService.DeleteVilla(id);
            if (!result)
            {
                _ApiResponse.statusCode = HttpStatusCode.NotFound;
                _ApiResponse.Success = false;
                return Ok(_ApiResponse);
            }
            _ApiResponse.statusCode = HttpStatusCode.NoContent;
            return Ok(_ApiResponse);
            
        }



        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> PatchVilla(int id, [FromBody]JsonPatchDocument<DtoVillaUpdate> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }
            var DtoGet=await _villaService.GetVillaAsyncById(id);
            var dtoUpdate= _mapper.Map<DtoVillaUpdate>(DtoGet);
            patchDoc.ApplyTo(dtoUpdate);

           var result= await _villaService.UpdateVilla(id, dtoUpdate);

            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
