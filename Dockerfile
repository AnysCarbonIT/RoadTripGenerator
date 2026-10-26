FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY RoadTripGenerator.Api/RoadTripGenerator.Api.csproj RoadTripGenerator.Api/
COPY RoadTripGenerator.Application/RoadTripGenerator.Application.csproj RoadTripGenerator.Application/
COPY RoadTripGenerator.Domain/RoadTripGenerator.Domain.csproj RoadTripGenerator.Domain/
COPY RoadTripGenerator.Infrastructure/RoadTripGenerator.Infrastructure.csproj RoadTripGenerator.Infrastructure/
RUN dotnet restore RoadTripGenerator.Api/RoadTripGenerator.Api.csproj

COPY . .
RUN dotnet publish RoadTripGenerator.Api/RoadTripGenerator.Api.csproj \
    -c Release -o /app/publish --no-restore -p:UseAppHost=false

FROM build AS tests
WORKDIR /src
RUN dotnet restore RoadTripGenerator.IntegrationTests/RoadTripGenerator.IntegrationTests.csproj
ENTRYPOINT ["dotnet", "test", "RoadTripGenerator.IntegrationTests/RoadTripGenerator.IntegrationTests.csproj", "--no-restore", "--logger", "console;verbosity=normal", "-p:UseAppHost=false"]

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "RoadTripGenerator.Api.dll"]
