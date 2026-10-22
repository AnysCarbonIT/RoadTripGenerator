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
        public Country Country { get; private set; } = null!;

        public  int Order { get; private set; }

        public void SetOrder(int order)
        {
            if (order < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(order), "L'ordre doit être supérieur à zéro.");
            }

            Order = order;
        }


    }

}
