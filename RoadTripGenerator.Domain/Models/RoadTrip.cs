using System;
using System.Collections.Generic;
using System.Text;

namespace RoadTripGenerator.Domain.Models
{
    public class RoadTrip
    {
        public  int Id { get; private set; }
        public string UserId { get; private set; } = null!;
        private readonly List<Step> _steps = new List<Step>();
        public IReadOnlyCollection<Step> Steps { get { return _steps.AsReadOnly(); } }

        private RoadTrip() { }

        public RoadTrip(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("L'identifiant de l'utilisateur est obligatoire.", nameof(userId));
            }
            UserId = userId;
        }
        public void AddCountry(string countryCca3)
        {
            Country.ValidateCountryCode(countryCca3);

            if (_steps.Any(step => step.CountryCca3 == countryCca3))
            {
                throw new ArgumentException("Ce pays existe déjà dans l'itinéraire.");
            }

            int order = _steps.Count + 1;

            Step newStep = new Step(countryCca3, order);

            _steps.Add(newStep);
        }
        public void RemoveCountry(string countryCca3)
        {
            Country.ValidateCountryCode(countryCca3);
            Step? stepToRemove = _steps.FirstOrDefault(step => step.CountryCca3 == countryCca3);
            if (stepToRemove == null)
            {
                throw new ArgumentException("Ce pays n'existe pas dans l'itinéraire.");
            }
            _steps.Remove(stepToRemove);
            _steps.Sort((left, right) => left.Order.CompareTo(right.Order));
            // Réordonner les étapes restantes
            for (int i = 0; i < _steps.Count; i++)
            {
                _steps[i].SetOrder(i + 1);
            }
        }
        public void ReplaceCountries(IReadOnlyList<string> countries)
        {
            if (countries == null)
            {
                throw new ArgumentException("La liste des pays est obligatoire.",nameof(countries));
            }

            // Valider toute la liste avant de modifier les étapes
            foreach (string countryCode in countries)
            {
                Country.ValidateCountryCode(countryCode);
            }

            if (countries.Distinct().Count() != countries.Count)
            {
                throw new ArgumentException("Un pays ne peut pas apparaître plusieurs fois dans l'itinéraire.");
            }

            _steps.Clear();

            for (int index = 0; index < countries.Count; index++)
            {
                Step newStep = new Step(countries[index], index + 1);

                _steps.Add(newStep);
            }
        }
    }

}
