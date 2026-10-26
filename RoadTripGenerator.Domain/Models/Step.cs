using System;
using System.Collections.Generic;
using System.Text;

namespace RoadTripGenerator.Domain.Models
{
    public class Step
    {
        public int Id { get; private set; }
        public int RoadTripId { get; private set; }
        public RoadTrip Roadtrip { get; private set; } = null!;

        public string CountryCca3 { get; private set; } = null!;
        public Country Country { get; private set; } = null!;

        public  int Order { get; private set; }

        // Constructeur utilisé par EF Core
        private Step()
        {
        }


        internal Step( string countryCca3, int order)
        {
            if(string.IsNullOrWhiteSpace(countryCca3))
            {
                throw new ArgumentException("Le code pays est obligatoire.", nameof(countryCca3));
            }   
            if (order < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(order), "L'ordre doit être supérieur à zéro.");
            }
            CountryCca3 = countryCca3;
            Order = order;
        }
        internal void SetOrder(int order)
        {
            if (order < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(order), "L'ordre doit être supérieur à zéro.");
            }

            Order = order;
        }


    }

}
