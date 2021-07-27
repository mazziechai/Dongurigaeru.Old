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
using Dongurigaeru.Data;

namespace Dongurigaeru.Services
{
    public class SettingsService
    {
        private const string FILE_NAME = "settings.json";
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public static string FilePath { get; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), FILE_NAME);
        public DongurigaeruSettings Settings { get; private set; }

        public static DongurigaeruSettings Create()
        {
            var settings = new DongurigaeruSettings();
            File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, _jsonOptions));
            return settings;
        }

        public void Load()
        {
            Settings = JsonSerializer.Deserialize<DongurigaeruSettings>(File.ReadAllText(FilePath));
        }

        public void Save()
        {
            File.WriteAllText(FilePath, JsonSerializer.Serialize(Settings, _jsonOptions));
        }

        public async Task SaveAsync()
        {
            await JsonSerializer.SerializeAsync(File.OpenWrite(FilePath), Settings, _jsonOptions);
        }

        public void Set(DongurigaeruSettings settings)
        {
            Settings = settings;
        }
    }
}
