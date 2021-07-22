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
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Dongurigaeru.Services
{
    public class DatabaseService
    {
        private readonly DatabaseSettings _dbSettings;

        public DatabaseContext Context
        {
            get
            {
                return new DatabaseContext(_dbSettings);
            }
        }

        public DatabaseService(SettingsService settings)
        {
            _dbSettings = settings.Settings.Database;
        }

        /// <summary>
        /// Batch updates players in the database from a List of those players
        /// to be updated.
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public async Task UpdatePlayers(List<PlayerInfo> players)
        {
            using var db = Context;
            foreach (var player in players)
            {
                var query = await db.Players.FindAsync(player.DiscordId);

                if (query is not null)
                {
                    db.Entry(query).CurrentValues.SetValues(player);
                }
                else
                {
                    db.Players.Add(player);
                }
            }
        }
    }

    public class DatabaseContext : DbContext
    {
        public DbSet<PlayerInfo> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<RatingPeriod> RatingPeriods { get; set; }
        public DbSet<PendingMatch> PendingMatches { get; set; }

        private readonly string _connectionString;

        public DatabaseContext(DatabaseSettings settings)
        {
            var connectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = settings.Path
            };

            _connectionString = connectionString.ToString();

            Database.SetCommandTimeout(TimeSpan.FromSeconds(30));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }
    }
}
