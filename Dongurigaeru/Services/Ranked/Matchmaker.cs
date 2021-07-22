// Copyright (C) 2021 mazziechai
// 
// This file is part of Dongurigaeru.
// 
// Dongurigaeru is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Dongurigaeru is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Dongurigaeru.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dongurigaeru.Data;

namespace Dongurigaeru.Services.Ranked
{
    /// <summary>
    /// Takes players, puts them in a queue, and then creates a suitable match
    /// for them.
    /// </summary>
    public class MatchmakerService
    {
        // Service dependencies
        private CalculatorService _calculator;
        private SettingsService _settings;
        private DatabaseService _db;

        public List<PlayerInfo>[] Queues { get; private set; }

        public MatchmakerService(
            CalculatorService calculator, SettingsService settings, DatabaseService db)
        {
            _calculator = calculator;
            _settings = settings;
            _db = db;
        }
    }

    /// <summary>
    /// Represents a list of Players with QueuedPlayerInfos.
    /// </summary>
    public class Queue
    {
        public string Platform { get; }

        public Dictionary<PlayerInfo, QueuedPlayerInfo> Players { get; init; } = new();

        public Queue(string platform)
        {
            Platform = platform;
        }

        /// <summary>
        /// Adds a Player to the queue with a new QueuedPlayerInfo.
        /// </summary>
        /// <param name="player"></param>
        /// <returns>True if the operation was successful.
        /// False if the queue already contains the Player or another
        /// error happened.</returns>
        public bool Add(PlayerInfo player)
        {
            return Players.TryAdd(player, new QueuedPlayerInfo());
        }

        /// <summary>
        /// Updates the boolean that tracks if a Player is in a match.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="state"></param>
        /// <returns>True if the operation was successful.
        /// False if the queue did not contain the Player or if the
        /// Player's match state is equal to the given state.</returns>
        public bool UpdateMatchState(PlayerInfo player, bool state)
        {
            if (!Players.ContainsKey(player))
            {
                return false;
            }
            if (Players[player].InMatch == state)
            {
                return false;
            }

            Players[player].InMatch = state;
            return true;
        }

        /// <summary>
        /// Updates all Players in the queue to have a new Matchmaking tolerance,
        /// given that they aren't currently in a match.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateMatchmakingTolerances()
        {
            await Task.Run(() =>
            {
                foreach (var player in Players.Keys)
                {
                    var info = Players[player];
                    if (!info.InMatch && (info.JoinedQueueTime - DateTime.UtcNow) > new TimeSpan(0, 3, 0))
                    {
                        info.MatchmakerTolerance += 25;
                    }
                }
            });
        }
    }

    public class MatchMadeEventArgs : EventArgs
    {
        public PlayerInfo Player1 { get; }
        public PlayerInfo Player2 { get; }

        public DateTime Time { get; }

        public MatchMadeEventArgs(PlayerInfo player1, PlayerInfo player2)
        {
            Player1 = player1;
            Player2 = player2;
            Time = DateTime.UtcNow;
        }
    }

    public class QueuedPlayerInfo
    {
        public bool InMatch { get; set; } = false;
        public double MatchmakerTolerance { get; set; } = 25;
        public DateTime JoinedQueueTime { get; set; } = DateTime.UtcNow;
    }
}
