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

namespace Dongurigaeru.Data
{
    /// <summary>
    /// Data about a player, contains their rating information.
    /// </summary>
    public class PlayerInfo
    {
        [Column("id")]
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

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
        public List<MatchInfo> Matches { get; set; }

        [Column("platforms")]
        public List<string> Platforms { get; set; }
    }

    /// <summary>
    /// A recorded match.
    /// </summary>
    public class MatchInfo
    {
        [Column("id")]
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("player1")]
        [Required]
        public PlayerInfo Player1 { get; set; }

        [Column("player1_score")]
        public int Player1Score { get; set; }

        [Column("player2")]
        [Required]
        public PlayerInfo Player2 { get; set; }

        [Column("player2_score")]
        public int Player2Score { get; set; }
        
        [Column("start_time")]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        public DateTime EndTime { get; set; }

        [Column("in_progress")]
        [Required]
        public bool InProgress { get; set; }

        [Column("completed")]
        [Required]
        public bool Completed { get; set; }
    }

    /// <summary>
    /// All the matches completed during a Glicko-2 rating period.
    /// </summary>
    public class RatingPeriodInfo
    {
        [Column("start_time")]
        [Key]
        [Required]
        public DateTime StartTime { get; set; }

        [Column("end_time")]
        [Required]
        public DateTime EndTime { get; set; }

        [Column("matches")]
        public List<MatchInfo> Matches { get; set; }
    }
}
