using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using RoadTripGenerator.Application.Interfaces;
using RoadTripGenerator.Infrastructure.ExternalUpload;
using RoadTripGenerator.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<RoadTripGeneratorDbContext>(
    options => options.UseNpgsql(
        builder.Configuration.GetConnectionString("RoadTripGeneratorDb")));


// DI sur l'appel à l'API REST Countries (avec clé API commande dotnet user-secret)
string apiKey = builder.Configuration["RestCountries:ApiKey"] ?? throw new InvalidOperationException("Clé API REST Countries manquante."); ; 
builder.Services.AddHttpClient<ICountryApiClient, RestCountryClient>(client => 
{
    client.BaseAddress = new Uri("https://api.restcountries.com/");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
});



var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
