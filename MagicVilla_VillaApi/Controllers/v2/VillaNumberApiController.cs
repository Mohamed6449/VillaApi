using AutoMapper;
using CQRS_test.CustomActionFilter;
using MagicVilla_VillaApi.Dto.ApiResponses;
using MagicVilla_VillaApi.Dto.VillaNumberDto;
using MagicVilla_VillaApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaApi.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ValidationModel]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    [ApiVersion("2.0")]
    public class VillaNumberApiController : ControllerBase
    {
        private readonly ApiResponse _ApiResponse;
        private readonly IMapper _mapper;
        private readonly IVillaNumberService _villaNumberService;
        
        public VillaNumberApiController(IVillaNumberService villaService, IMapper mapper)
        {
            _mapper = mapper;
            _villaNumberService = villaService;
            _ApiResponse = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task <ActionResult<ApiResponse>> GetVillas()
        {
            _ApiResponse.statusCode = HttpStatusCode.OK;
            _ApiResponse.result = await _villaNumberService.GetVillaNumberNumbersAsync();
            return Ok(_ApiResponse);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> GetVilla(int id)
        {
            var villa = await _villaNumberService.GetVillaNumberAsyncById(id);
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
        public async Task<ActionResult<ApiResponse>> CreateVilla([FromBody]DtoVillaNumberCreate model)
        {

             var Result=await  _villaNumberService.CreateVillaNumber( model);
            if (!Result.Result)
            {
                _ApiResponse.statusCode = HttpStatusCode.BadRequest;
                _ApiResponse.Success = false;
                _ApiResponse.Errors.Add(Result.error);
                return Ok(_ApiResponse);
            }
            _ApiResponse.statusCode = HttpStatusCode.Created;
            _ApiResponse.result = model;
            return Ok(_ApiResponse);
        }




        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> UpdateVilla(int id,[FromBody]DtoVillaNumberUpdate model)
        {
          var result= await  _villaNumberService.UpdateVillaNumber(id, model);
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
            var result = await _villaNumberService.DeleteVillaNumber(id);
            if (!result)
            {
                _ApiResponse.statusCode = HttpStatusCode.NotFound;
                _ApiResponse.Success = false;
                return Ok(_ApiResponse);
            }
            _ApiResponse.statusCode = HttpStatusCode.NoContent;
            return Ok(_ApiResponse);
            
        }



    }
}
