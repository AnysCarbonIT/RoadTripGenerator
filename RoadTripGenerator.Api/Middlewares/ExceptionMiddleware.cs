using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;    
namespace RoadTripGenerator.Api.Middlewares
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using RoadTripGenerator.Domain.Exceptions;
    using RoadTripGenerator.Application.Exceptions;

    public class ExceptionMiddleware : IExceptionHandler
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            // Une réponse déjà envoyée ne peut plus être remplacée.
            if (httpContext.Response.HasStarted)
            {
                _logger.LogInformation("Non prise en charge par le middleware d'exception. TraceId :{TraceId}", httpContext.TraceIdentifier);
                return false;
            }

            // Le client est parti : ne pas écrire dans la connexion fermée.
            if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
            {
                _logger.LogInformation("Requête annulée par le client. TraceId : {TraceId}", httpContext.TraceIdentifier);
                httpContext.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
                return true;
            }

            var (status, title, detail) = exception switch
            {
                BadHttpRequestException badRequest => (badRequest.StatusCode,"Requête HTTP invalide","La requête HTTP ne peut pas être traitée."),

                // Seules les erreurs métier explicites sont présentées au client.
                DomainValidationException validation => (StatusCodes.Status400BadRequest,"Données invalides",validation.Message),

                RoadTripNotFoundException or CountryNotInRoadTripException => (StatusCodes.Status404NotFound,"Ressource introuvable",exception.Message),

                DbUpdateConcurrencyException => (StatusCodes.Status409Conflict,"Conflit de modification","Les données ont changé. Rechargez-les avant de réessayer."),

                HttpRequestException or InvalidDataException => (StatusCodes.Status502BadGateway,"Réponse du service externe invalide","Un service externe n'a pas fourni une réponse exploitable."),

                TimeoutException => (StatusCodes.Status504GatewayTimeout,"Délai dépassé","Une opération n'a pas répondu dans le délai prévu."),

                // Erreurs de base, configuration ou bugs : aucun détail interne exposé. (_ représente le cas par défaut)
                _ => (StatusCodes.Status500InternalServerError,"Erreur interne","Une erreur inattendue est survenue. Utilisez l'identifiant de suivi pour signaler le problème.")
            };

            if (status >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Erreur HTTP {StatusCode} coté serveur. TraceId : {TraceId}", status, httpContext.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning(exception, "Requête refusée : HTTP {StatusCode}. TraceId : {TraceId}", status, httpContext.TraceIdentifier);
            }

            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path.Value
            };
            problem.Extensions["traceId"] = httpContext.TraceIdentifier;
            // Preparation de la réponse JSON 
            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(problem,options: null,contentType: "application/problem+json",cancellationToken: cancellationToken);

            return true;
        }
    }
}
