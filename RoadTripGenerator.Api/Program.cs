using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Application.UseCases;
using RoadTripGenerator.Api.Mappers;
using RoadTripGenerator.Infrastructure.ExternalUpload;
using RoadTripGenerator.Infrastructure.Persistence;
using RoadTripGenerator.Infrastructure.Persistence.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<RoadTripGeneratorDbContext>(
    options => options.UseNpgsql(
        builder.Configuration.GetConnectionString("RoadTripGeneratorDb")));


// DI sur l'appel à l'API REST Countries (avec clé API commande dotnet user-secret)
string apiKey = builder.Configuration["RestCountries:ApiKey"] ?? throw new InvalidOperationException("Clé API REST Countries manquante."); 
builder.Services.AddHttpClient<ICountryApiClient, RestCountryClient>(client => 
{
    client.BaseAddress = new Uri("https://api.restcountries.com/"); // en dur ici 
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
});



builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IGetCountriesUseCase, GetCountriesUseCase>();
builder.Services.AddScoped<CountryResponseMapper>();
builder.Services.AddScoped<CountryInitializationService>();
builder.Services.AddScoped<IRoadTripRepository, RoadTripRepository>();
builder.Services.AddScoped<IGetRoadTripUseCase, GetRoadTripUseCase>();
builder.Services.AddScoped<RoadTripResponseMapper>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider
        .GetRequiredService<CountryInitializationService>();

    await initializer.InitializeCountriesAsync(CancellationToken.None);
}

app.Run();
