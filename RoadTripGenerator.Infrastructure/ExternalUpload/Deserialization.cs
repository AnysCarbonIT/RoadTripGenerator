using RoadTripGenerator.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RoadTripGenerator.Infrastructure.ExternalUpload
{
    public class Deserialization
    {
        public RestCountriesResponseDto Deserialize(string json)
        {
            RestCountriesResponseDto? result = JsonSerializer.Deserialize<RestCountriesResponseDto>(json);
            if (result == null)
            {
                throw new Exception(" la réponse api est null.");
            }
            return result;  
        }
    }
}
