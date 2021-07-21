using System;
using System.IO;
using System.Threading.Tasks;
using Dongurigaeru.Bot.Data;
using Dongurigaeru.Bot.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;

namespace Dongurigaeru.Bot
{
    public static class Program
    {
        internal static void Main() 
        {
            if (!File.Exists(Settings.FilePath))
            {
                var settings = Settings.Create();
                Console.WriteLine("First time setup, please enter a Discord token: ");

                settings.Discord.Token = Console.ReadLine();
                Console.WriteLine("Would you like to continue with the default settings? (Y/n)");
                
                if (Console.ReadLine().ToLower() == "n")
                {
                    Console.WriteLine("Exiting...");
                    return;
                }
            }

            MainAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {

        }
    }

    public class Dongurigaeru
    {
        public static DongurigaeruSettings Settings { get; set; }
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }


        public Dongurigaeru(DongurigaeruSettings settings)
        {
            Settings = settings;

            Client = new(new DiscordConfiguration
            {
                Token = Settings.Discord.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = (LogLevel)Settings.General.LogLevel
            });

            Commands = Client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new string[] {Settings.Discord.Prefix},
                CaseSensitive = false,
                EnableDms = false
            });
        }


        public async Task StartAsync() => await Client.ConnectAsync();
    }
}
