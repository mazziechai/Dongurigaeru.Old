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
using Hydrangea.Glicko2.Interfaces;

namespace Dongurigaeru.Core.Ranked
{
    public class MatchRatingPeriod : IRatingPeriod
    {
        public List<Match> Matches { get; set; } = new();
        public HashSet<Player> Players { get; set; } = new();

        public DateTime StartTime { get; }
        public DateTime EndTime { get; }

        public List<IResult> Results { get { return new(Matches); } }
        public HashSet<IRatingInfo> Participants { get { return new(Players); } }

        public MatchRatingPeriod(DateTime startTime, TimeSpan length)
        {
            StartTime = startTime;
            EndTime = startTime + length;
        }

        /// <summary>
        /// Adds a match to Matches.
        /// </summary>
        /// <param name="match"></param>
        public void AddMatch(Match match)
        {
            Matches.Add(match);
            Players.UnionWith(match.Players);
        }
        /// <summary>
        /// Adds a batch of matches to Matches.
        /// </summary>
        /// <param name="matches"></param>
        public void AddMatches(IEnumerable<Match> matches)
        {
            Matches.AddRange(matches);
            foreach (var match in matches)
            {
                Players.UnionWith(match.Players);
            }
        }

        /// <summary>
        /// Returns all of the associated Matches of a Player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public List<Match> GetPlayerMatches(Player player)
        {
            List<Match> list = new();
            foreach (var match in Matches)
            {
                if (match.Players.Contains(player)) list.Add(match);
            }

            return list;
        }

        /// <summary>
        /// Clears Matches.
        /// </summary>
        public void Clear()
        {
            Matches.Clear();
        }

        /// <summary>
        /// Deprecated.
        /// </summary>
        /// <param name="result"></param>
        public void AddResult(IResult result)
        {
            Results.Add(result);
            Participants.UnionWith(result.Scores.Keys);
        }
        /// <summary>
        /// Deprecated.
        /// </summary>
        /// <param name="results"></param>
        public void AddResults(IEnumerable<IResult> results)
        {
            Results.AddRange(results);
            foreach (var result in Results)
            {
                Participants.UnionWith(result.Scores.Keys);
            }
        }
        /// <summary>
        /// Deprecated.
        /// </summary>
        /// <param name="rating"></param>
        /// <returns></returns>
        public List<IResult> GetParticipantResults(IRatingInfo rating)
        {
            List<IResult> resultList = new();

            foreach (var result in Results)
            {
                if (result.Scores.Keys.Contains(rating))
                {
                    resultList.Add(result);
                }
            }

            return resultList;
        }
    }
}
