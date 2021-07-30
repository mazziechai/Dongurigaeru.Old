// Copyright (C) 2021 mazziechai
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dongurigaeru.Core.Ranked;
using Hydrangea.Glicko2;
using Hydrangea.Glicko2.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Dongurigaeru.Services.Ranked
{
    /// <summary>
    /// Calculates Glicko-2 ratings.
    /// </summary>
    public class CalculatorService
    {
        // Service dependencies
        private readonly SettingsService _settings;
        private readonly RatingPeriodService _ratingPeriod;

        private readonly ICalculator _calculator;

        public CalculatorService(SettingsService settings, RatingPeriodService ratingPeriod)
        {
            _settings = settings;
            _ratingPeriod = ratingPeriod;

            _calculator = new Calculator()
            {
                StandardRating = _settings.Settings.Glicko2.Rating.Rating,
                VolatilityConstraint = _settings.Settings.Glicko2.Rating.VolatilityConstraint,
                ConvergenceTolerance = _settings.Settings.Glicko2.Rating.ConvergenceTolerance
            };

            _ratingPeriod.RatingPeriodEndedEvent += HandleRatingPeriodEnded;
        }

        protected async void HandleRatingPeriodEnded(object sender, RatingPeriodEndedEventArgs e)
        {
            await Task.Run(() =>
            {
                HashSet<Player> updatedPlayers = new();

                foreach (var player in e.RatingPeriod.Players)
                {
                    updatedPlayers.Add(
                        (Player)_calculator.Rate(
                            player, _ratingPeriod.RatingPeriod.GetParticipantResults(player)));
                }

                foreach (var rating in updatedPlayers)
                {
                    rating.FinalizeChanges();
                }

                _ratingPeriod.RatingPeriod.Players = updatedPlayers;
            });
        }
    }
}
