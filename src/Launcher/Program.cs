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
using Launcher.Services;

namespace Launcher
{
    internal static class Program
    {

        public static IConfiguration Configuration { get; set; }
        public static ServiceProvider ServiceProvider { get; set; }

        internal static Version Version => Assembly.GetExecutingAssembly().GetName().Version;
        internal static string Name => Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>()?.Product;


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

            services.AddSingleton<IJava, Java>();
            services.AddSingleton<IServer, Server>();
            services.AddSingleton<IGame, Game>();
            services.AddSingleton<IComputer, Computer>();

            services.AddScoped<FormMain>();

            ServiceProvider = services.BuildServiceProvider();
            //var factory = ServiceProvider.GetService<ILoggerFactory>();
            //Logger = factory.CreateLogger(typeof(Program));
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var loggerFactory = Program.ServiceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger(typeof(Program));
            logger.LogError(ex, ex.Message);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }
    }
}