namespace RoadTripGenerator.Domain.Exceptions
{
    public class CountryNotInRoadTripException : Exception
    {
        public CountryNotInRoadTripException(): base("Ce pays n'existe pas dans l'itinéraire.") { }
    }
}
