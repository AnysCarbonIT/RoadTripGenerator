using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using RoadTripGenerator.Api.Middlewares;
using RoadTripGenerator.Application.Exceptions;
using RoadTripGenerator.Domain.Exceptions;

namespace RoadTripGenerator.IntegrationTests
{
    public class ExceptionMiddlewareTests
    {
        private static IEnumerable<TestCaseData> Errors()
        {
            yield return new TestCaseData(new DomainValidationException("Code invalide"), 400);
            yield return new TestCaseData(new CountryNotInRoadTripException(), 404);
            yield return new TestCaseData(new RoadTripNotFoundException(), 404);
            yield return new TestCaseData(new DbUpdateConcurrencyException("private-detail"), 409);
            yield return new TestCaseData(new HttpRequestException("private-detail"), 502);
            yield return new TestCaseData(new InvalidDataException("private-detail"), 502);
            yield return new TestCaseData(new TimeoutException("private-detail"), 504);
            yield return new TestCaseData(new InvalidOperationException("private-detail"), 500);
            yield return new TestCaseData(new ArgumentException("private-detail"), 500);
            yield return new TestCaseData(new DbUpdateException("private-detail"), 500);
            yield return new TestCaseData(new KeyNotFoundException("private-detail"), 500);
        }

        [TestCaseSource(nameof(Errors))]
        public async Task HandleException_ReturnsExpectedProblemWithoutTechnicalDetails(Exception exception, int expectedStatus)
        {
            using var services = new ServiceCollection().AddOptions().BuildServiceProvider();
            var context = new DefaultHttpContext { RequestServices = services };
            context.Request.Path = "/api/roadtrip";
            await using var body = new MemoryStream();
            context.Response.Body = body;
            var handler = new ExceptionMiddleware(NullLogger<ExceptionMiddleware>.Instance);

            bool handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

            Assert.That(handled, Is.True);
            Assert.That(context.Response.StatusCode, Is.EqualTo(expectedStatus));
            Assert.That(context.Response.ContentType, Does.StartWith("application/problem+json"));
            body.Position = 0;
            using var json = await JsonDocument.ParseAsync(body);
            Assert.That(json.RootElement.GetProperty("status").GetInt32(), Is.EqualTo(expectedStatus));
            Assert.That(json.RootElement.GetProperty("traceId").GetString(), Is.EqualTo(context.TraceIdentifier));
            Assert.That(json.RootElement.GetRawText(), Does.Not.Contain("private-detail"));
        }

        [Test]
        public async Task HandleClientCancellation_DoesNotWriteResponseBody()
        {
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            var context = new DefaultHttpContext { RequestAborted = cancellation.Token };
            await using var body = new MemoryStream();
            context.Response.Body = body;
            var handler = new ExceptionMiddleware(NullLogger<ExceptionMiddleware>.Instance);

            bool handled = await handler.TryHandleAsync(context, new OperationCanceledException(), cancellation.Token);

            Assert.That(handled, Is.True);
            Assert.That(context.Response.StatusCode, Is.EqualTo(499));
            Assert.That(body.Length, Is.Zero);
        }
    }
}
