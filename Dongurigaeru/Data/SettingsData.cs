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
using System.Text.Json.Serialization;

namespace Dongurigaeru.Data
{
    public class DongurigaeruSettings
    {
        [JsonPropertyName("general")]
        public GeneralSettings General { get; set; } = new();

        [JsonPropertyName("discord")]
        public DiscordSettings Discord { get; set; } = new();

        [JsonPropertyName("database")]
        public DatabaseSettings Database { get; set; } = new();

        [JsonPropertyName("matchmaking")]
        public MatchmakingSettings Matchmaking { get; set; } = new();

        [JsonPropertyName("glicko2")]
        public Glicko2Settings Glicko2 { get; set; } = new();
    }

    public class GeneralSettings
    {
        // The default logging level is information, which is 2.
        [JsonPropertyName("loglevel")]
        public int LogLevel { get; set; } = 2;
    }

    public class DiscordSettings
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = "";

        [JsonPropertyName("prefix")]
        public string Prefix { get; set; } = ".";

        // The administrators are users who can manage sensitive bot functions.
        [JsonPropertyName("administrators")]
        public long[] Administrators { get; set; } = { 712104395747098656 };
    }

    public class DatabaseSettings
    {
        [JsonPropertyName("path")]
        public string Path { get; set; } = "database.db3";

        // This is in minutes.
        [JsonPropertyName("backup_interval")]
        public int BackupInterval { get; set; } = 15;
    }

    public class MatchmakingSettings
    {
        // This is in minutes.
        [JsonPropertyName("pending_match_lifetime")]
        public int PendingMatchLifetime { get; set; } = 5;

        // This is in minutes.
        [JsonPropertyName("in_progress_match_lifetime")]
        public int InProgressMatchLifetime { get; set; } = 60;

        // This is in minutes.
        [JsonPropertyName("cancel_match_lifetime")]
        public int CancelMatchLifetime { get; set; } = 5;

        // The score to win a match.
        [JsonPropertyName("first_to")]
        public int FirstTo { get; set; } = 10;

        [JsonPropertyName("platforms")]
        public List<string> Platforms { get; set; } = new List<string> { "PC", "Switch", "PS4" };
    }

    public class Glicko2Settings
    {
        public RatingSettings Rating { get; private set; } = new();

        public RatingPeriodSettings RatingPeriod { get; private set; } = new();


        public class RatingSettings
        {
            [JsonPropertyName("rating")]
            public int Rating { get; set; } = 1500;

            [JsonPropertyName("deviation")]
            public int RatingDeviation { get; set; } = 350;

            // σ
            [JsonPropertyName("volatility")]
            public double Volatility { get; set; } = 0.06;

            // τ
            [JsonPropertyName("constraint")]
            public double VolatilityConstraint { get; set; } = 0.75;

            // ϵ
            [JsonPropertyName("convergence_tolerance")]
            public double ConvergenceTolerance { get; set; } = 0.000001;
        }

        public class RatingPeriodSettings
        {
            // This is in hours.
            [JsonPropertyName("length")]
            public int Length { get; set; } = 72;

            // Defines the start of the interval between rating periods.
            [JsonPropertyName("start")]
            public DateTime StartDate { get; set; } = DateTime.Today;
        }

        [JsonPropertyName("win_score")]
        public double WinScore { get; set; } = 1;

        [JsonPropertyName("draw_score")]
        public double DrawScore { get; set; } = 0.5;

        [JsonPropertyName("loss_score")]
        public double LossScore { get; set; } = 0;
    }
}
