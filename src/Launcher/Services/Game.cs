using Launcher.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Services
{
    public interface IGame
    {
        string GamePath { get; }
        Task<bool> LoadProfiles();
        Task<bool> LoadAccounts();
        Task<bool> FindGame();
        string LocalPath { get; set; }
        Dictionary<string, Profile> Profiles { get; set; }
        Dictionary<string, Account> Accounts { get; set; }

        Account ActiveAccount { get; set; }
        string ClientToken { get; }
    }
    public class Game : IGame
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public Game(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _configuration = configuration;
        }

        public string GamePath => Environment.ExpandEnvironmentVariables(_configuration["AppSettings:GamePath"]);


        public async Task<bool> FindGame()
        {
            await Task.Yield();

            Console.Write("Obteniendo ruta del juego:");
            var installedPath = Environment.ExpandEnvironmentVariables(GamePath);

            var isPathValid = Directory.Exists(installedPath);
            if (isPathValid)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" [LISTO]");
                Console.WriteLine();
                Console.ResetColor();
                return true;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" [FALLIDO]");
                Console.WriteLine();
                Console.ResetColor();
                return false;
            }
        }

        public async Task<bool> LoadProfiles()
        {
            var file = Path.Combine(GamePath, "launcher_profiles.json");
            if (File.Exists(file))
            {
                var content = await File.ReadAllTextAsync(file);
                var profiles = JsonConvert.DeserializeObject<JObject>(content).Children().Where(m => m.Path == "profiles").ToList();
                var value = profiles.First().First().ToString();
                Profiles = JsonConvert.DeserializeObject<Dictionary<string, Profile>>(value);
                return Profiles.Any();
            }

            return false;
        }

        public async Task<bool> LoadAccounts()
        {
            var file = Path.Combine(GamePath, "launcher_accounts.json");
            if (File.Exists(file))
            {
                var content = await File.ReadAllTextAsync(file);
                var jobject = JsonConvert.DeserializeObject<JObject>(content);
                var profiles = jobject.Children().Where(m => m.Path == "accounts").ToList();
                var value = profiles.First().First().ToString();
                Accounts = JsonConvert.DeserializeObject<Dictionary<string, Account>>(value);

                var active = jobject.Children().FirstOrDefault(m => m.Path == "activeAccountLocalId")?.LastOrDefault();
                if (active != null)
                    ActiveAccount = Accounts[active.Value<string>()];

                var token = jobject.Children().FirstOrDefault(m => m.Path == "mojangClientToken")?.LastOrDefault();
                if (token != null)
                    ClientToken = token.Value<string>();


                return Accounts.Any();
            }

            return false;
        }

        public string LocalPath { get; set; }

        public Dictionary<string, Profile> Profiles { get; set; }
        public Dictionary<string, Account> Accounts { get; set; }

        public Account ActiveAccount { get; set; }
        public string ClientToken { get; private set; }

    }
}
