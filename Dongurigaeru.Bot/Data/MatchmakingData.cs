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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dongurigaeru.Bot.Data
{
    /// <summary>
    /// Data about a player, contains their rating information.
    /// </summary>
    public class PlayerInfo
    {
        [Column("id")]
        [Key]
        [Required]
        public int DiscordId { get; set; }

        [Column("display_name")]
        public string DisplayName { get; set; }

        [Column("rating")]
        [Required]
        public double Rating { get; set; }

        [Column("rating_deviation")]
        [Required]
        public double RatingDeviation { get; set; }

        [Column("volatility")]
        [Required]
        public double Volatility { get; set; }

        [Column("matches")]
        public List<Match> Matches { get; set; }

        [Column("platforms")]
        public List<string> Platforms { get; set; }
    }

    /// <summary>
    /// A recorded match.
    /// </summary>
    public class Match
    {
        [Column("id")]
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("time")]
        [Required]
        public DateTime Time { get; set; }

        [Column("player1")]
        [Required]
        public PlayerInfo Player1 { get; set; }

        [Column("player1_score")]
        [Required]
        public int Player1Score { get; set; }

        [Column("player2")]
        [Required]
        public PlayerInfo Player2 { get; set; }

        [Column("player2_score")]
        [Required]
        public int Player2Score { get; set; }
    }

    /// <summary>
    /// All the matches completed during a Glicko-2 rating period.
    /// </summary>
    public class RatingPeriod
    {
        [Column("start_time")]
        [Key]
        [Required]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        [Required]
        public DateTime EndTime { get; set; }

        [Column("matches")]
        public List<Match> Matches { get; set; }
    }

    /// <summary>
    /// Information about a match currently being played. This is stored
    /// in the database in case the bot is restarted or crashes so the
    /// match can still be reported.
    /// </summary>
    public class PendingMatch
    {
        [Column("id")]
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("accepted")]
        [Required]
        public bool Accepted { get; set; }

        [Column("player1")]
        [Required]
        public PlayerInfo Player1 { get; set; }

        [Column("player2")]
        [Required]
        public PlayerInfo Player2 { get; set; }
    }
}
