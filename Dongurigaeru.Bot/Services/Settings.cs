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

using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Dongurigaeru.Bot.Data;

namespace Dongurigaeru.Bot.Services
{
    public class Settings
    {
        private const string FILE_NAME = "settings.json";
        public static string FilePath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FILE_NAME);
        private DongurigaeruSettings _settings;
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public static DongurigaeruSettings Create()
        {
            var settings = new DongurigaeruSettings();
            File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, _jsonOptions));
            return settings;
        }

        public void Load()
        {
            _settings = JsonSerializer.Deserialize<DongurigaeruSettings>(File.ReadAllText(FilePath));
        }

        public void Save()
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(_settings, _jsonOptions));
        }

        public async Task SaveAsync()
        {
            await JsonSerializer.SerializeAsync(File.OpenWrite(FilePath), _settings, _jsonOptions);
        }

        public DongurigaeruSettings Get()
        {
            return _settings;
        }

        public void Set(DongurigaeruSettings settings)
        {
            _settings = settings;
        }
    }
}