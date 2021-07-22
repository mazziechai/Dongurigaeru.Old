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
using System.Threading.Tasks;
using Dongurigaeru.Bot.Data;
using Dongurigaeru.Bot.Services;
using Hydrangea.Glicko2;

namespace Dongurigaeru.Bot.Core
{
    /// <summary>
    /// Takes players, puts them in a queue, and then creates a suitable match
    /// for them.
    /// </summary>
    public class Matchmaker
    {
        private MatchmakingSettings _matchmakingSettings;
        private Glicko2 _calculator;
        private Database _database;
        private List<Player> _queue0 = new();
        private List<Player> _queue1 = new();
        private List<Player> _queue2 = new();

        public event EventHandler<MatchMadeEventArgs> MatchMade;

        public Matchmaker(MatchmakingSettings settings, Glicko2 g2, Database db)
        {
            _matchmakingSettings = settings;
            _calculator = g2;
            _database = db;
        }

        /// <summary>
        /// Adds a player to a queue by its respective integer.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="queue"></param>
        public void AddPlayerToQueue(Player player, int queue)
        {
            switch (queue)
            {
                case 0:
                    if (_queue0.Contains(player))
                    {
                        return;
                    }
                    _queue0.Add(player);
                    break;

                case 1:
                    if (_queue1.Contains(player))
                    {
                        return;
                    }
                    _queue1.Add(player);
                    break;

                case 2:
                    if (_queue2.Contains(player))
                    {
                        return;
                    }
                    _queue2.Add(player);
                    break;
            }
        }

        /// <summary>
        /// Removes a given player from a queue by its respective integer.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="queue"></param>
        public void RemovePlayerFromQueue(Player player, int queue)
        {
            switch (queue)
            {
                case 0:
                    _queue0.Remove(player);
                    break;

                case 1:
                    _queue1.Remove(player);
                    break;

                case 2:
                    _queue2.Remove(player);
                    break;
            }
        }

        /// <summary>
        /// Creates a Player from the database using their ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Player> CreatePlayer(int id)
        {
            using var db = _database.Get();
            return await db.Players.FindAsync(id);
        }

        private Task MatchmakingLoop()
        {
            while (true)
            {
                foreach (var player in _queue0)
                {
                    _ = Task.Run(() => AttemptMakeMatch(player, _queue0));
                }

                foreach (var player in _queue1)
                {
                    _ = Task.Run(() => AttemptMakeMatch(player, _queue1));
                }

                foreach (var player in _queue2)
                {
                    _ = Task.Run(() => AttemptMakeMatch(player, _queue2));
                }

                _ = Task.Run(async () => await Task.Delay(TimeSpan.FromSeconds(5)));
            }
        }

        /// <summary>
        /// Finds a suitable match for a player, looking through the entire
        /// queue given.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="queue"></param>
        /// <returns></returns>
        private void AttemptMakeMatch(Player player, List<Player> queue)
        {
            // range
            var player1high = player.Rating + player.RatingDeviation;
            var player1low = player.Rating - player.RatingDeviation;

            foreach (var player2 in queue)
            {
                if (player2.Equals(player))
                {
                    continue;
                }

                // range
                var player2high = player2.Rating + player2.RatingDeviation;
                var player2low = player2.Rating - player2.RatingDeviation;

                if (player1high >= player2low && player1low <= player2high)
                {
                    MatchMade?.Invoke(this, new MatchMadeEventArgs(player, player2));
                    return;
                }
            }
        }
    }

    public class MatchMadeEventArgs : EventArgs
    {
        public Player Player1 { get; }
        public Player Player2 { get; }

        public DateTime Time { get; }

        public MatchMadeEventArgs(Player player1, Player player2)
        {
            Player1 = player1;
            Player2 = player2;
            Time = DateTime.UtcNow;
        }
    }
}