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
using Dongurigaeru.Bot.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Dongurigaeru.Bot.Services
{
    public class Database
    {
        private readonly DatabaseContext _db;

        public Database(Settings settings)
        {
            _db = new DatabaseContext(settings.Get().Database);
        }

        public DatabaseContext Get()
        {
            return _db;
        }
    }

    public class DatabaseContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
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