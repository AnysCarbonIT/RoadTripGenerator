using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using RoadTripGenerator.Api.DTO;
using RoadTripGenerator.Api.Mappers;
using RoadTripGenerator.Application.Interfaces;

namespace RoadTripGenerator.Api.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly IGetCountriesUseCase _getCountriesUseCase;
        private readonly CountryResponseMapper _mapper;

        public CountriesController(IGetCountriesUseCase getCountriesUseCase,CountryResponseMapper mapper)
        {
            _getCountriesUseCase = getCountriesUseCase;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CountryResponseDto>>> GetCountriesAsync(
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, 100)] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if ((long)(page - 1) * pageSize > int.MaxValue)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest,title: "Pagination invalide", detail: "Le numéro de page est trop grand pour cette taille de page.");
            }

            var countries = await _getCountriesUseCase.ExecuteAsync(
                page, pageSize, cancellationToken);

            var response = countries.Select(country => _mapper.MapToResponse(country)).ToList();

            return Ok(response);
        }
    }
}
