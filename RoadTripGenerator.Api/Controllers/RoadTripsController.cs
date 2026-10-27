using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RoadTripGenerator.Api.DTO;
using RoadTripGenerator.Api.Mappers;
using RoadTripGenerator.Application.UseCases;

namespace RoadTripGenerator.Api.Controllers
{
    [ApiController]
    [Route("api/roadtrip")]
    public class RoadTripsController : Controller
    {
        private readonly GetRoadTripUseCase _getRoadTripUseCase;
        private readonly ReplaceRoadTripUseCase _replaceRoadTripUseCase;
        private readonly AddCountryUseCase _AddCountryUseCase;
        private readonly DeleteCountryUseCase _DeleteCountryUseCase;
        private readonly RoadTripResponseMapper _mapper;
        private string UserId { get; set; } = string.Empty;

        public RoadTripsController(
            GetRoadTripUseCase getRoadTripUseCase,
            ReplaceRoadTripUseCase replaceRoadTripUseCase,
            AddCountryUseCase addCountryUseCase,
            DeleteCountryUseCase deleteCountryUseCase,
            RoadTripResponseMapper mapper)
        {
            _getRoadTripUseCase = getRoadTripUseCase;
            _replaceRoadTripUseCase = replaceRoadTripUseCase;
            _AddCountryUseCase = addCountryUseCase;
            _DeleteCountryUseCase = deleteCountryUseCase;
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
            var roadTrip = await _replaceRoadTripUseCase.ExecuteAsync(UserId, request.Countries, cancellationToken);
            return Ok(_mapper.MapToResponse(roadTrip));
        }

        [HttpPost ("country")]
        public async Task<ActionResult<RoadTripResponseDto>> AddCountryAsync([FromBody] AddCountryRequestDto request, CancellationToken cancellationToken)
        {
            var roadTrip = await _AddCountryUseCase.ExecuteAsync(UserId, request.Cca3, cancellationToken);
            return Ok(_mapper.MapToResponse(roadTrip));
        }

        [HttpDelete("country/{cca3}")]
        public async Task<ActionResult<RoadTripResponseDto>> DeleteCountryAsync([FromRoute] string cca3, CancellationToken cancellationToken)
        {
            var roadTrip = await _DeleteCountryUseCase.ExecuteAsync(UserId, cca3, cancellationToken);
            return Ok(_mapper.MapToResponse(roadTrip));
        }

        // Ce contrôle sera exécuté avant chacune des futures actions RoadTrip grace a OnActionExecuting provenant de :Controller. 
        // Il vérifie la présence du header X - User - ID et le stocke dans UserId pour les autres actions 
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!Request.Headers.TryGetValue("X-User-ID", out var values)|| values.Count != 1|| string.IsNullOrWhiteSpace(values[0]))
            {
                context.Result = Problem(statusCode: StatusCodes.Status401Unauthorized, title: "Identifiant utilisateur manquant ou invalide", detail: "Le header X-User-ID doit contenir un seul identifiant non vide.");
                return;
            }

            // Les futures actions transmettront cet identifiant à leurs use cases.
            UserId = values[0]!;
            base.OnActionExecuting(context);
        }
    }
}
