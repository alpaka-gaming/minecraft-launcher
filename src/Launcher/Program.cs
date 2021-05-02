using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Launcher.Models;
using System.Net.NetworkInformation;
using Flurl.Http;
using System.Windows.Forms;

namespace Launcher
{
    internal static class Program
    {

        public static IConfiguration Configuration { get; set; }
        public static ILogger Logger { get; set; }

        //public static ServiceCollection Services { get; set; }
        public static ServiceProvider ServiceProvider { get; set; }

        #region AppSettings

        internal static Uri FeedUrl => new(Configuration["AppSettings:FeedUrl"]);
        internal static Uri FastDownload => new(Configuration["AppSettings:FastDownloadUrl"]);
        internal static string Server => new(Configuration["AppSettings:ServerUrl"]);
        internal static string GamePath => Environment.ExpandEnvironmentVariables(Configuration["AppSettings:GamePath"]);
        internal static string Profile => Configuration["AppSettings:Profile"];

        private static Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        private static string Name => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>()?.Product;

        #endregion

        #region Versions

        private static Dictionary<string, Version> Versions { get; set; }
        public static string LocalPath { get; set; }
        public static Dictionary<string, Profile> Profiles { get; set; }

        #endregion

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            ConfigureServices();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(ServiceProvider.GetService<FormMain>());
        }

        static void ConfigureServices()
        {
            Configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", false, true)
#if DEBUG
                .AddJsonFile("appsettings.Development.json", true, true)
#endif
                .Build();

            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddNLog(new NLogLoggingConfiguration(Configuration.GetSection("NLog")));
            }).AddOptions();

            services.AddSingleton(Configuration);

            services.AddScoped<FormMain>();

            ServiceProvider = services.BuildServiceProvider();
            var factory = ServiceProvider.GetService<ILoggerFactory>();
            Logger = factory.CreateLogger(typeof(Program));
        }

        #region Methods

        internal static bool PingServer()
        {
            try
            {
                var ping = new Ping();
                var result = ping.Send(Server);
                return (result != null && result.Status == IPStatus.Success);
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                return false;
            }
        }
        internal static bool PingFastDownload()
        {
            try
            {
                var ping = new Ping();
                var result = ping.Send(FastDownload.Host);
                return (result != null && result.Status == IPStatus.Success);
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                return false;
            }
        }

        internal static async Task<bool> ConnectServer()
        {
            var attempts = 1;
            var isServerOn = false;
            while (!isServerOn)
            {
                if (attempts >= 5) break;
                Console.Write("Conectando con el servidor:");
                isServerOn = PingServer();
                if (isServerOn)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(" [LISTO]");
                    Console.WriteLine();
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(" [FALLIDO]");
                    Console.WriteLine();
                    Console.ResetColor();
                    Console.WriteLine($"Reintentando en {attempts} segundos.");
                    await Task.Delay(attempts * 1000);
                }

                attempts++;
            }

            return isServerOn;
        }

        internal static bool ValidateVersion()
        {
            if (Versions.ContainsKey("Updater"))
            {
                var minVersion = Versions["Updater"];
                if (Version < minVersion)
                {
                    throw new InvalidOperationException("Debe descargar la ultima versión del updater.");
                }
            }

            return true;
        }

        internal static async Task<bool> GetProfile()
        {
            Console.Write("Obteniendo perfil:");
            try
            {
                var tempFile = new FileInfo(Path.GetTempFileName());
                var tempPath = tempFile.Directory?.FullName;
                await $"{FastDownload}/{Profile}.json".DownloadFileAsync(tempPath, tempFile.Name);
                var data = await File.ReadAllTextAsync(tempFile.FullName);
                var definition = JsonConvert.DeserializeObject<JObject>(data);

                Versions = new Dictionary<string, Version>();
                foreach (var item in definition.Children())
                    Versions.Add(item.Path, new Version(item.First().ToString()));

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(" [LISTO]");
                Console.WriteLine();
                Console.ResetColor();

                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" [ERROR]");
                Console.WriteLine();
                Console.ResetColor();

                return false;
            }
        }

        internal static async Task<bool> FindGame()
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

        internal static async Task<bool> ValidateProfile()
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

        internal static async Task<List<string>> GetFiles(string folder)
        {
            var fixString = new Func<string, string>(m => { return HttpUtility.UrlDecode(m.Replace("+", "%2b")); });

            var url = $"{FastDownload}{folder}";

            var files = new List<string>();
            var request = (HttpWebRequest)WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var html = reader.ReadToEnd();
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);
                    var nodes = doc.DocumentNode.SelectNodes("html/body/pre/a");
                    var links = nodes.Select(m => m.Attributes.First(a => a.Name == "href").Value);
                    files.AddRange(links.Where(m => !m.StartsWith("?") && !m.StartsWith("/")).Select(m => fixString(m)));
                }
            }

            await Task.Yield();
            return files;
        }

        #endregion

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Logger.LogError(ex, ex.Message);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
    }
}