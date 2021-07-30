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

using Dongurigaeru.Core.Ranked;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dongurigaeru.Data
{
    public class PlayerEntityTypeConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Rating)
                .IsRequired();
            builder.Property(p => p.RatingDeviation)
                .IsRequired();
            builder.Property(p => p.Volatility)
                .IsRequired();

            builder.Ignore(p => p.WorkingRating);
            builder.Ignore(p => p.WorkingRatingDeviation);
            builder.Ignore(p => p.WorkingVolatility);
        }
    }

    public class MatchEntityTypeConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> builder)
        {
            builder.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.Players)
                .IsRequired();
        }
    }

    public class RatingPeriodEntityTypeConfiguration : IEntityTypeConfiguration<MatchRatingPeriod>
    {
        public void Configure(EntityTypeBuilder<MatchRatingPeriod> builder)
        {
            builder.HasKey(p => p.StartTime);

            builder.Property(p => p.EndTime)
                .IsRequired();
        }
    }
}
