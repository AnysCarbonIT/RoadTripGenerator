using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RoadTripGenerator.Api.DTO;
using RoadTripGenerator.Api.Mappers;
using RoadTripGenerator.Application.Interfaces;

namespace RoadTripGenerator.Api.Controllers
{
    [ApiController]
    [Route("api/roadtrip")]
    public class RoadTripsController : Controller
    {
        private readonly IGetRoadTripUseCase _getRoadTripUseCase;
        private readonly IReplaceRoadTripUseCase _replaceRoadTripUseCase;
        private readonly IAddCountryUseCase _AddCountryUseCase;
        private readonly RoadTripResponseMapper _mapper;
        private string UserId { get; set; } = string.Empty;

        public RoadTripsController(
            IGetRoadTripUseCase getRoadTripUseCase,
            IReplaceRoadTripUseCase replaceRoadTripUseCase,
            IAddCountryUseCase addCountryUseCase,
            RoadTripResponseMapper mapper)
        {
            _getRoadTripUseCase = getRoadTripUseCase;
            _replaceRoadTripUseCase = replaceRoadTripUseCase;
            _AddCountryUseCase = addCountryUseCase;
            _mapper = mapper;
        }

        [HttpGet]

        public async Task<ActionResult<RoadTripResponseDto>> GetRoadTripAsync(CancellationToken cancellationToken)
        {
            var roadTrip = await _getRoadTripUseCase.ExecuteAsync(UserId, cancellationToken);
            return Ok(_mapper.MapToResponse(roadTrip));
        }

        [HttpPut]

        public async Task<ActionResult<RoadTripResponseDto>> ReplaceRoadTripAsync([FromBody] ReplaceRoadTripRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var roadTrip = await _replaceRoadTripUseCase.ExecuteAsync(UserId, request.Countries, cancellationToken);
                return Ok(_mapper.MapToResponse(roadTrip));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost ("country")]
        public async Task<ActionResult<RoadTripResponseDto>> AddCountryAsync([FromBody] AddCountryRequestDto request, CancellationToken cancellationToken)
        {
            try
            {
                var roadTrip = await _AddCountryUseCase.ExecuteAsync(UserId, request.Cca3, cancellationToken);
                return Ok(_mapper.MapToResponse(roadTrip));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        // Ce contrôle sera exécuté avant chacune des futures actions RoadTripgrace a OnActionExecuting provenant de :Controller. 
        // Il vérifie la présence du header X - User - ID et le stocke dans UserId pour les autres actions 
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!Request.Headers.TryGetValue("X-User-ID", out var values)|| values.Count != 1|| string.IsNullOrWhiteSpace(values[0]))
            {
                context.Result = BadRequest("Le header X-User-ID doit contenir un seul identifiant non vide.");
                return;
            }

            // Les futures actions transmettront cet identifiant à leurs use cases.
            UserId = values[0]!;
            base.OnActionExecuting(context);
        }
    }
}
