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
using System.Linq;
using System.Threading.Tasks;
using Dongurigaeru.Core.Ranked;

namespace Dongurigaeru.Services.Ranked
{
    public class MatchHandlerService
    {
        // Service dependencies
        private readonly MatchmakerService _matchmaker;
        private readonly RatingPeriodService _ratingPeriod;

        public List<Match> Matches { get; set; }

        public MatchHandlerService(MatchmakerService matchmaker, RatingPeriodService period)
        {
            _matchmaker = matchmaker;
            _ratingPeriod = period;

            _matchmaker.MatchMadeEvent += HandleMatchMade;
        }

        public async Task<Match> GetPlayerMatch(Player player)
        {
            return await Task.Run(() => Matches.Find(m => m.Players.Contains(player)));
        }

        protected async void HandleMatchMade(object sender, MatchMadeEventArgs e)
        {
            await Task.Run(() =>
            {
                Matches.Add(e.MadeMatch);
                e.MadeMatch.MatchStartedEvent += HandleMatchInProgress;
            });
        }

        protected async void HandleMatchInProgress(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                var match = sender as Match;
                match.MatchCompletedEvent += HandleMatchCompleted;
            });
        }

        protected async void HandleMatchCompleted(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                _ratingPeriod.RatingPeriod.AddMatch(sender as Match);
                Matches.Remove(sender as Match);
            });
        }
    }
}
