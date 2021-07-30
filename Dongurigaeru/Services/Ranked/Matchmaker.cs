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
using Microsoft.Extensions.Hosting;

namespace Dongurigaeru.Services.Ranked
{
    /// <summary>
    /// Takes players, puts them in a queue, and then creates a suitable match
    /// for them.
    /// </summary>
    public class MatchmakerService : BackgroundService
    {
        /// <summary>
        /// All queues for players.
        /// </summary>
        public Dictionary<string, Queue> Queues { get; private set; }
        public List<Match> RecentMatches { get; private set; }

        public event EventHandler<MatchMadeEventArgs> MatchMadeEvent;

        public MatchmakerService()
        {
            Queues = new()
            {
                { "Switch", new Queue("Switch") },
                { "PC", new Queue("PC") },
                { "PS4", new Queue("PS4") }
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await FindMatches();
                await Task.Delay(5000, stoppingToken);
            }
        }

        public async Task<bool> AddToQueue(Player player, string platform, string game)
        {
            return await Task.Run(() => Queues[platform].Players[game].Add(player));
        }

        public async Task<bool> RemoveFromQueue(Player player, string platform, string game)
        {
            return await Task.Run(() => Queues[platform].Players[game].Remove(player));
        }
        public async Task<bool> RemoveFromAllQueues(Player player, string platform)
        {
            return await Task.Run(() =>
            {
                bool success = false;

                foreach (var game in Queues[platform].Players.Values)
                {
                    success = game.Remove(player);
                }

                return success;
            });
        }

        /// <summary>
        /// Looks through every queue to find matches for the players in that queue.
        /// </summary>
        /// <returns></returns>
        protected async Task FindMatches()
        {
            await Task.Run(async () =>
            {
                foreach (var platform in Queues)
                {
                    foreach (var game in platform.Value.Players.Values)
                    {
                        foreach (var player1 in game)
                        {
                            foreach (var player2 in game)
                            {
                                if (!player1.InMatch && player2.InMatch)
                                {
                                    if (Math.Abs(
                                       player1.Rating + player1.RatingDeviation - (
                                       player2.Rating + player2.RatingDeviation)) < 250)
                                    {
                                        Match match = new(player1, player2);
                                        await OnMatchMade(new MatchMadeEventArgs(match));
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        protected async Task OnMatchMade(MatchMadeEventArgs e)
        {
            await Task.Run(() => MatchMadeEvent?.Invoke(this, e));
        }
    }

    /// <summary>
    /// Represents a list of Players with QueuedPlayerInfos.
    /// </summary>
    public class Queue
    {
        public string Platform { get; }

        public Dictionary<string, HashSet<Player>> Players { get; init; }

        public Queue(string platform)
        {
            Platform = platform;
            Players = new()
            {
                { "PPC", new HashSet<Player>() },
                { "PPT", new HashSet<Player>() },
                { "PPT2", new HashSet<Player>() }
            };
        }
    }

    public class MatchMadeEventArgs : EventArgs
    {
        public Match MadeMatch { get; set; }

        public DateTime Time { get; set; }

        public MatchMadeEventArgs(Match match)
        {
            MadeMatch = match;
            Time = DateTime.UtcNow;
        }
    }
}
