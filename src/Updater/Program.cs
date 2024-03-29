﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using Flurl.Http;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Updater.Models;

namespace Updater
{
    internal class Program
    {
        public static IConfiguration Configuracion { get; private set; }

        public static ServiceCollection Servicios { get; private set; }
        public static ServiceProvider Contenedor { get; private set; }

        #region AppSettings

        private static Uri Server => new(Configuracion["AppSettings:Server"]);
        private static string GamePath => Path.Combine(OperatingSystem.IsWindows() ? Environment.ExpandEnvironmentVariables("%appdata%") : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".minecraft");
        private static string Profile => Configuracion["AppSettings:Profile"];

        private static Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        private static string Name => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>()?.Product;

        #endregion

        #region Versions

        private static Dictionary<string, Version> Versions { get; set; }
        public static string LocalPath { get; set; }
        public static Dictionary<string, Profile> Profiles { get; set; }

        #endregion


        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            Console.WriteLine($"{Name} v{Version.ToString(3)}");
            Console.WriteLine("");

            Configuracion = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
#if DEBUG
                .AddJsonFile("appsettings.Development.json", true, true)
#endif
                .Build();

            // Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuracion)
                .CreateLogger();

            Servicios = new ServiceCollection();
            Servicios.AddSingleton(Configuracion);
            Contenedor = Servicios.BuildServiceProvider();

            // Execution
            try
            {
                MainAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                UnhandledException(e, new UnhandledExceptionEventArgs(e, true));
            }

            Console.WriteLine("");
            Console.WriteLine("Presione cualquier tecla para finalizar.");
            Console.ReadKey();
        }

        public static async Task MainAsync()
        {
            if (!await ConnectServer()) throw new OperationCanceledException("No se pudo conectar al servidor.");
            if (!await GetProfile()) throw new OperationCanceledException("No se pudo obtener el perfil del juego.");
            if (!await FindGame()) throw new OperationCanceledException("No se pudo encontrar al ruta del juego.");

            ValidateVersion();

            await PrintInfo();

            if (!await ValidateProfile()) throw new OperationCanceledException("No se pudo validar el perfil.");

            var profiles = Profiles.Where(m => !string.IsNullOrWhiteSpace(m.Value.Toolchain)).OrderByDescending(m => m.Value.Created);
            foreach (var profile in profiles)
            {
                var versionPath = $"{Versions["Minecraft"]}-{profile.Value.Toolchain}-{Versions[profile.Value.Toolchain]}".ToLowerInvariant();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Perfil: {profile.Value.Name} ({Versions[profile.Value.Toolchain]})");
                Console.ResetColor();

                var gamePath = profile.Value.GameDir;
                if (string.IsNullOrWhiteSpace(gamePath)) gamePath = GamePath;
                gamePath = Environment.ExpandEnvironmentVariables(gamePath);

                var serverDatFile = Path.Combine(gamePath, "servers.dat");
                if (!File.Exists(serverDatFile))
                {
                    try
                    {
                        var urlTemp = $"{Server}minecraft/profiles/{Profile}/servers.dat";
                        await urlTemp.DownloadFileAsync(serverDatFile);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                var folders = new[] {"mods", "resourcepacks", "shaderpacks"};
                foreach (var folder in folders)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"=> Procesando {folder}...");
                    Console.ResetColor();

                    var modFiles = await GetFiles(folder, versionPath);
                    modFiles = modFiles.Where(m => m.StartsWith("..")).ToList();
                    foreach (var item in modFiles)
                    {
                        var urlTemp = $"{Server}minecraft/downloads/{Profile}/{versionPath}/{folder}/{item}";
                        var localFolderPath = Path.Combine(gamePath, folder, item);
                        var jarFile = Path.ChangeExtension(localFolderPath, ".jar");
                        var zipFile = Path.ChangeExtension(localFolderPath, ".zip");
                        var ext = Path.GetExtension(localFolderPath);
                        var name = Path.GetFileNameWithoutExtension(localFolderPath);
                        try
                        {
                            if (File.Exists(localFolderPath))
                            {
                                var localLength = new FileInfo(localFolderPath).Length;
                                var remoteLength = await urlTemp.GetLengthAsync();

                                if (localLength != remoteLength)
                                    File.Delete(localFolderPath);
                            }

                            if (ext == ".jar" || ext == ".bak" || ext == ".zip")
                            {
                                if (!File.Exists(localFolderPath) && !File.Exists(jarFile) && !File.Exists(zipFile))
                                {
                                    Console.Write($"    Instalando: {name} ");
                                    var directoryInfo = new FileInfo(localFolderPath).Directory;
                                    if (directoryInfo != null)
                                    {
                                        var localFolder = directoryInfo.FullName;
                                        await urlTemp.DownloadFileAsync(localFolder);

                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.Write($"[LISTO]");
                                        Console.WriteLine();
                                        Console.ResetColor();
                                    }
                                }
                            }
                            else if (ext == ".rem")
                            {
                                if (File.Exists(jarFile) || File.Exists(zipFile))
                                {
                                    Console.Write($"    Eliminando: {name} ");
                                    if (File.Exists(jarFile)) File.Delete(jarFile);
                                    if (File.Exists(zipFile)) File.Delete(zipFile);
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write($"[LISTO]");
                                    Console.WriteLine();
                                    Console.ResetColor();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write($"[ERROR]");
                            Console.WriteLine();
                            Console.ResetColor();
                            Log.Logger.Error(e, e.Message);
                        }
                    }
                }

                Console.WriteLine("");
            }
        }

        private static bool PingServer()
        {
            try
            {
                var ping = new Ping();
                var result = ping.Send(Server.Host);
                return (result != null && result.Status == IPStatus.Success);
            }
            catch (PlatformNotSupportedException)
            {
                return true;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, e.Message);
                return false;
            }
        }

        private static async Task<bool> ConnectServer()
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

        private static void ValidateVersion()
        {
            if (Versions.ContainsKey("Updater"))
            {
                var minVersion = Versions["Updater"];
                if (Version < minVersion)
                {
                    var updaterUrl = "https://github.com/alpaka-gaming/minecraft-launcher/releases";
                    throw new InvalidOperationException($"Debe descargar la última versión del updater en:{Environment.NewLine}{updaterUrl}");
                }
            }
        }

        private static async Task<bool> GetProfile()
        {
            Console.Write("Obteniendo perfil:");
            try
            {
                var tempFile = new FileInfo(Path.GetTempFileName());
                var tempPath = tempFile.Directory?.FullName;
                await $"{Server}minecraft/profiles/{Profile}/versions.json".DownloadFileAsync(tempPath, tempFile.Name);
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
                Log.Logger.Error(e, e.Message);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" [ERROR]");
                Console.WriteLine();
                Console.ResetColor();

                return false;
            }
        }

        private static async Task<bool> FindGame()
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

        private static async Task<bool> ValidateProfile()
        {
            var profileFiles = new[] {"launcher_profiles.json", "launcher_profiles_microsoft_store.json"};
            foreach (var item in profileFiles)
            {
                var profileFile = Path.Combine(GamePath, item);
                if (File.Exists(profileFile))
                {
                    var content = await File.ReadAllTextAsync(profileFile);
                    var profiles = JsonConvert.DeserializeObject<JObject>(content).Children().Where(m => m.Path == "profiles").ToList();
                    var value = profiles.First().First().ToString();
                    Profiles = JsonConvert.DeserializeObject<Dictionary<string, Profile>>(value);
                    return Profiles.Any();
                }
            }

            return false;
        }

        private static async Task PrintInfo()
        {
            Console.WriteLine();
            var tempFile = new FileInfo(Path.GetTempFileName());
            if (tempFile.Directory != null)
                await $"{Server}minecraft/profiles/{Profile}/motd.txt".DownloadFileAsync(tempFile.Directory.FullName, tempFile.Name);
            if (File.Exists(tempFile.FullName))
            {
                var data = File.ReadAllLines(tempFile.FullName);
                foreach (var item in data)
                    Console.WriteLine(item);
            }

            Console.WriteLine();
        }

        private static async Task<List<string>> GetFiles(string folder, string versionId)
        {
            var fixString = new Func<string, string>(m => { return HttpUtility.UrlDecode(m.Replace("+", "%2b")); });

            var url = $"{Server}minecraft/downloads/{Profile}/{versionId}/{folder}";

            var files = new List<string>();
            using (var client = new HttpClient())
            {
                var response = await client.GetStreamAsync(url);
                using (var reader = new StreamReader(response))
                {
                    var html = reader.ReadToEnd();
                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var nodes = doc.DocumentNode.SelectNodes("html/body/pre/a");
                    var links = nodes.Select(m => m.Attributes.First(a => a.Name == "href").Value);
                    files.AddRange(links.Where(m => !m.StartsWith("?") && !m.StartsWith("/")).Select(m => fixString(m)));
                }
            }

            await Task.Yield();
            return files;
        }

        /* ***** */

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception) e.ExceptionObject;
            Log.Logger.Error(ex, ex.Message);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
    }
}
