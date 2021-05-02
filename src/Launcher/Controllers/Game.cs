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

namespace Launcher.Controllers
{
    public interface IGame
    {
        string GamePath { get; }
        Task<bool> ValidateProfile();
        Task<bool> FindGame();
        string LocalPath { get; set; }
        Dictionary<string, Profile> Profiles { get; set; }
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

        public async Task<bool> ValidateProfile()
        {
            var profileFile = Path.Combine(GamePath, "launcher_profiles.json");
            if (File.Exists(profileFile))
            {
                var content = await File.ReadAllTextAsync(profileFile);
                var profiles = JsonConvert.DeserializeObject<JObject>(content).Children().Where(m => m.Path == "profiles").ToList();
                var value = profiles.First().First().ToString();
                Profiles = JsonConvert.DeserializeObject<Dictionary<string, Profile>>(value);
                return Profiles.Any();
            }

            return false;
        }

        public string LocalPath { get; set; }
        public Dictionary<string, Profile> Profiles { get; set; }


    }
}
