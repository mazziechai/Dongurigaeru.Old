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
using System.Threading;
using System.Threading.Tasks;
using Dongurigaeru.Core.Ranked;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Dongurigaeru.Services.Ranked
{
    /// <summary>
    /// Tracks and manages the Glicko-2 rating period.
    /// </summary>
    public class RatingPeriodService : BackgroundService
    {
        // Service dependencies
        private readonly SettingsService _settings;
        private readonly DatabaseService _db;

        public TimeSpan Length { get; }
        public MatchRatingPeriod RatingPeriod { get; set; }

        /// <summary>
        /// An event for when the current rating period ends and a new one is starting.
        /// </summary>
        public event EventHandler<RatingPeriodEndedEventArgs> RatingPeriodEndedEvent;

        /// <summary>
        /// Sets all required properties, then checks if there is a rating period
        /// in the database. If there isn't, make a new one. If there is, use that.
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="db"></param>
        public RatingPeriodService(SettingsService settings, DatabaseService db)
        {
            _settings = settings;
            _db = db;
            Length = TimeSpan.FromHours(_settings.Settings.Glicko2.RatingPeriod.Length);

            using var context = _db.Context;
            if (!context.RatingPeriods.Any())
            {
                var start = _settings.Settings.Glicko2.RatingPeriod.StartDate;

                MatchRatingPeriod period = new(start, Length);
                context.RatingPeriods.Add(period);
                context.SaveChanges();

                RatingPeriod = period;
            }
            else
            {
                RatingPeriod = context.RatingPeriods.Last();
            }
        }

        ~RatingPeriodService()
        {
            using var context = _db.Context;
            context.Update(RatingPeriod);
            context.SaveChanges();
        }

        /// <summary>
        /// Continually checks if the rating period has ended. If it has, raise
        /// the RatingPeriodEnded event.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.UtcNow > RatingPeriod.StartTime + Length)
                {
                    RatingPeriodEndedEventArgs e = new(RatingPeriod);
                    await OnRatingPeriodEnded(e);
                }

                await Task.Delay(5000, stoppingToken);
            }
        }

        /// <summary>
        /// Raises the event for when a rating period ends.
        /// </summary>
        /// <param name="e"></param>
        protected async Task OnRatingPeriodEnded(RatingPeriodEndedEventArgs e)
        {
            using var context = _db.Context;
            RatingPeriod = new(RatingPeriod.StartTime + Length, Length);
            await context.AddAsync(RatingPeriod);

            await Task.Run(() => RatingPeriodEndedEvent?.Invoke(this, e));
        }
    }

    /// <summary>
    /// Event args for when a rating period ends.
    /// </summary>
    public class RatingPeriodEndedEventArgs : EventArgs
    {
        public MatchRatingPeriod RatingPeriod { get; set; }

        public RatingPeriodEndedEventArgs(MatchRatingPeriod period)
        {
            RatingPeriod = period;
        }
    }
}
