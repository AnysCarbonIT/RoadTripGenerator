namespace RoadTripGenerator.Application.Exceptions
{
    public class RoadTripNotFoundException : Exception
    {
        public RoadTripNotFoundException(): base("Aucun itinéraire trouvé pour cet utilisateur.") { }
    }
}
