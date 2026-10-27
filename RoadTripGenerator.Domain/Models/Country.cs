
using System;
using System.Collections.Generic;
using System.Linq;
using RoadTripGenerator.Domain.Exceptions;

namespace RoadTripGenerator.Domain.Models
{
    public class Country
    {
        public  string Cca3 { get; private set; } = null!;

        public string Name { get; private set; } = null!;

        public  string Region { get; private set; } = null!;

        public  long Population { get; private set; }

        private List<string> _capitalCities = new List<string>();
        public  string Flag { get; private set; } = null!;


        public IReadOnlyList<string> CapitalCities
        {
            get
            {
                return _capitalCities.AsReadOnly();
            }
        }

        private Country()
        {
        }

        public Country(
            string cca3,
            string name,
            string region,
            long population,
            List<string> capitalCities,
            string flag)
        {
            ValidateCountryCode(cca3);

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainValidationException("Le nom du pays est obligatoire.");
            }

            if (string.IsNullOrWhiteSpace(region))
            {
                throw new DomainValidationException("La région du pays est obligatoire.");
            }

            if (population < 0)
            {
                throw new DomainValidationException("La population ne peut pas être négative.");
            }

            if (capitalCities == null)
            {
                throw new DomainValidationException("La liste des capitales est obligatoire.");
            }

            if (capitalCities.Any(string.IsNullOrWhiteSpace))
            {
                throw new DomainValidationException("Une capitale ne peut pas être vide.");
            }

            if (string.IsNullOrWhiteSpace(flag))
            {
                throw new DomainValidationException("Le drapeau est obligatoire.");
            }

            Cca3 = cca3;
            Name = name;
            Region = region;
            Population = population;
            Flag = flag;
            _capitalCities = new List<string>(capitalCities);
        }

        public static void ValidateCountryCode(string cca3)
        {
            if (string.IsNullOrWhiteSpace(cca3))
            {
                throw new DomainValidationException("Le code pays est obligatoire.");
            }

            if (cca3.Length != 3 || !cca3.All(c => c >= 'A' && c <= 'Z'))
            {
                throw new DomainValidationException("Le code pays doit contenir trois lettres majuscules.");
            }
        }
    }
}
