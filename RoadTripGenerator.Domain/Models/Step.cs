using System;
using System.Collections.Generic;
using System.Text;

namespace RoadTripGenerator.Domain.Models
{
    public class Step
    {
        public int Id { get; set; }
        public int RoadTripId { get; set; }
        public RoadTrip Roadtrip { get; set; } = null!;

        public required string CountryCca3 { get; set; }
        public Country Country { get; set; } = null!;

        public  int Order { get; set; }


    }

}
