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
using Dongurigaeru.Data;
using Hydrangea.Glicko2.Interfaces;

namespace Dongurigaeru.Core.Ranked
{
    /// <summary>
    /// Extended version of the Glicko-2 RatingInfo class, adding bot specific
    /// functionality.
    /// </summary>
    public class Player : IRatingInfo
    {
        public int Id { get; }
        public string DisplayName { get; set; }

        public double Rating { get; private set; } = 1500;
        public double RatingDeviation { get; private set; } = 140;
        public double Volatility { get; private set; } = 0.06;

        public double WorkingRating { get; set; }
        public double WorkingRatingDeviation { get; set; }
        public double WorkingVolatility { get; set; }

        /// <summary>
        /// All Matches this player has played.
        /// </summary>
        public List<Match> Matches { get; init; } = new();
        /// <summary>
        /// 
        /// </summary>
        public List<string> Platforms { get; init; } = new();

        // Matchmaking queue stuff
        /// <summary>
        /// If this player is currently playing a match.
        /// </summary>
        public bool InMatch { get; set; } = false;
        /// <summary>
        /// If this player is currently in a matchmaking queue.
        /// </summary>
        public bool InQueue { get; set; } = false;
        /// <summary>
        /// When this player joined the queue. Null if the player is not in a queue.
        /// </summary>
        public DateTime JoinedQueueTime { get; set; }


        public Player(int id) => Id = id;

        public Player(int id, double rating, double rd, double volatility)
        {
            Id = id;
            Rating = rating;
            RatingDeviation = rd;
            Volatility = volatility;
        }

        /// <summary>
        /// Adds a platform to the Player if the platform isn't present.
        /// </summary>
        /// <param name="platform"></param>
        /// <returns>True if the operation was successful.
        /// False if Platforms already contained the given platform.</returns>
        public bool AddPlatform(string platform)
        {
            if (!Platforms.Contains(platform)) { Platforms.Add(platform); return true; }
            else return false;
        }

        /// <summary>
        /// Adds a MatchInfo to the Player if the MatchInfo contains the Player.
        /// </summary>
        /// <param name="match"></param>
        public void AddMatch(Match match)
        {
            if (match.Players[0] == this || match.Players[1] == this) Matches.Add(match);
            else throw new InvalidOperationException("MatchInfo did not contain this Player");
        }

        public void FinalizeChanges()
        {
            Rating = WorkingRating;
            RatingDeviation = WorkingRatingDeviation;
            Volatility = WorkingVolatility;
        }

        // Making equality operations with Players use Id
        /// <summary>
        /// Checks if the IDs are equal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(Player x, Player y) => x.Id == y.Id;
        /// <summary>
        /// Checks if the IDs aren't equal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(Player x, Player y) => x.Id != y.Id;

        public override bool Equals(object obj)
        {
            return obj is Player player && Id == player.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        // Making comparison operations with Players use rating data
        /// <summary>
        /// Checks if a Player's low end of their rating range (accounting for
        /// volatility) is greater than another Player's.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >(Player x, Player y) =>
            (x.Rating - x.RatingDeviation) / (1 + x.Volatility) > (y.Rating - y.RatingDeviation) / (1 + y.Volatility);
        /// <summary>
        /// Checks if a Player's low end of their rating range (accounting for
        /// volatility) is lesser than another Player's.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <(Player x, Player y) =>
            (x.Rating - x.RatingDeviation) / (1 + x.Volatility) < (y.Rating - y.RatingDeviation) / (1 + y.Volatility);
    }
}
