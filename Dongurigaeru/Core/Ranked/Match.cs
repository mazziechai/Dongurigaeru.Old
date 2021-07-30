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
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Hydrangea.Glicko2.Interfaces;

namespace Dongurigaeru.Core.Ranked
{
    /// <summary>
    /// Represents a match played by two Players.
    /// </summary>
    public class Match : IResult
    {
        public int Id { get; init; }

        public Player[] Players { get; }
        public ImmutableDictionary<IRatingInfo, double> Scores { get; private set; }

        /// <summary>
        /// The time the match began or null if the match hasn't started yet.
        /// </summary>
        public DateTime StartTime { get; private set; }
        /// <summary>
        /// The time the match ended or null if the match hasn't completed yet.
        /// </summary>
        public DateTime EndTime { get; private set; }
        /// <summary>
        /// The length of the match or null if the match hasn't completed yet.
        /// </summary>
        public TimeSpan Length { get { return EndTime - StartTime; } }

        public bool InProgress { get; private set; } = false;
        public bool Completed { get; private set; } = false;

        public event EventHandler MatchCompletedEvent;
        public event EventHandler MatchStartedEvent;

        /// <summary>
        /// Creates a match with two Players.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public Match(Player player1, Player player2)
        {
            // Both Players cannot be the same
            if (player1 == player2)
                throw new InvalidMatchDataException("Both Players cannot be the same");

            Players = new Player[] { player1, player2 };
        }
        /// <summary>
        /// Creates an in-progress match with two Players.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="startTime"></param>
        public Match(Player player1, Player player2, DateTime startTime)
            : this(player1, player2)
        {
            InProgress = true;
            StartTime = startTime;
        }
        /// <summary>
        /// Creates a completed match with two Players.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <param name="score1"></param>
        /// <param name="score2"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public Match
            (Player player1, Player player2, double score1, double score2,
            DateTime startTime, DateTime endTime) : this(player1, player2, startTime)
        {
            EndTime = endTime;
            var task = Task.Run(async () => await RecordScores(score1, score2));
            Task.WaitAll(task);
            ValidateData();
        }

        public bool IsDraw()
        {
            if (Completed)
            {

                if (Scores.Values.Distinct().Any()) return true;
                else return false;

            }
            else
            {
                throw new InvalidOperationException("Attempted to check if an incomplete match was a draw");
            }
        }

        /// <summary>
        /// Marks a match as complete, sets the score of each player, and sets
        /// the MatchResult to a new Result.
        /// </summary>
        /// <param name="score1"></param>
        /// <param name="score2"></param>
        public async Task Complete(double score1, double score2)
        {
            if (InProgress && !Completed)
            {
                InProgress = false;
                EndTime = DateTime.UtcNow;

                await RecordScores(score1, score2);
                await OnCompleted(new EventArgs());
            }
            else throw new InvalidOperationException("Attempted to complete a match not in progress");
        }

        /// <summary>
        /// Marks a match as in-progress.
        /// </summary>
        public async Task Start()
        {
            if (!InProgress && !Completed)
            {
                InProgress = true;
                StartTime = DateTime.UtcNow;

                await OnStarted(new EventArgs());
            }
            else throw new InvalidOperationException("Attempted to start an already in progress or completed match");
        }

        /// <summary>
        /// Marks a match as complete and sets a score for each player.
        /// </summary>
        /// <param name="score1"></param>
        /// <param name="score2"></param>
        private async Task RecordScores(double score1, double score2)
        {
            if (!Completed)
            {
                Completed = true;

                await Task.Run(() =>
                Scores = new Dictionary<IRatingInfo, double>()
                {
                    { Players[0], score1 }, { Players[1], score2 }
                }.ToImmutableDictionary());
            }
            else throw new InvalidOperationException("Attempted to record scores after match has been completed");
        }

        /// <summary>
        /// Throws exceptions if data is invalid.
        /// </summary>
        private void ValidateData()
        {
            // InProgress and Completed cannot both be true
            if (InProgress && Completed)
                throw new InvalidMatchDataException("InProgress and Completed cannot both be true");

            // Both Player scores cannot be the same
            if (Scores[Players[0]] == Scores[Players[1]])
                throw new InvalidMatchDataException("Both Player scores cannot be the same");

            // The start time cannot be later than the end time
            if (StartTime > EndTime)
                throw new InvalidMatchDataException("StartTime cannot be later than EndTime");

            // StartTime cannot be equal to EndTime
            if (StartTime == EndTime)
                throw new InvalidMatchDataException("StartTime cannot be equal to EndTime");
        }

        protected async Task OnCompleted(EventArgs e)
        {
            await Task.Run(() => MatchCompletedEvent?.Invoke(this, e));
        }

        protected async Task OnStarted(EventArgs e)
        {
            await Task.Run(() => MatchStartedEvent?.Invoke(this, e));
        }
    }
}
