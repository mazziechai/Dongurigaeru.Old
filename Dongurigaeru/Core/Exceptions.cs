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

namespace Dongurigaeru.Core
{
    [Serializable]
    public class InvalidMatchDataException : Exception
    {
        public InvalidMatchDataException() : base() { }
        public InvalidMatchDataException(string message) : base(message) { }
        public InvalidMatchDataException(string message, Exception inner) : base(message, inner) { }

        protected InvalidMatchDataException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context
            ) : base(info, context) { }
    }
}
