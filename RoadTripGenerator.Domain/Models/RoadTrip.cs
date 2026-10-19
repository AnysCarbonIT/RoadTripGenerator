using System;
using System.Collections.Generic;
using System.Text;

namespace RoadTripGenerator.Domain.Models
{
    public class RoadTrip
    {
        public  int Id { get; set; }
        public required string UserId { get; set; }
        public ICollection<Step> Steps { get; set; } = new List<Step>();
    }
}
