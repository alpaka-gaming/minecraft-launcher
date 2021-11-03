using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Dialogs;
using Avalonia.ReactiveUI;
using Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Launcher
{
    public static class Program
    {
        public static bool IsSingleViewLifetime =>
            Environment.GetCommandLineArgs()
                .Any(a => a == "--fbdev" || a == "--drm");

        internal static IConfiguration Configuration { get; set; } = null!;
        internal static HttpClient HttpClient { get; set; } = null!;

        public static MinecraftOptions MinecraftOptions { get; set; } = null!;

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        private static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
#if DEBUG
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
#endif
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            MinecraftOptions = new MinecraftOptions();
            Configuration.Bind("Minecraft", MinecraftOptions);
            
            HttpClient = new HttpClient();

            double GetScaling()
            {
                var idx = Array.IndexOf(args, "--scaling");
                if (idx != 0 && args.Length > idx + 1 &&
                    double.TryParse(args[idx + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out var scaling))
                    return scaling;
                return 1;
            }

            //Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            try
            {
                if (args.Contains("--wait-for-attach"))
                {
                    Console.WriteLine("Attach debugger and use 'Set next statement'");
                    while (true)
                    {
                        Thread.Sleep(100);
                        if (Debugger.IsAttached)
                            break;
                    }
                }

                Log.Information("Application Starting");
                var builder = BuildAvaloniaApp();
                if (args.Contains("--fbdev"))
                {
                    SilenceConsole();
                    return builder.StartLinuxFbDev(args, scaling: GetScaling());
                }
                else if (args.Contains("--drm"))
                {
                    SilenceConsole();
                    return builder.StartLinuxDrm(args, scaling: GetScaling());
                }
                else
                    return builder.StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static async void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (Log.Logger != null)
            {
                var ex = (Exception) e.ExceptionObject;
                Log.Fatal(ex, "The Application failed to start");
                await Task.Yield();
                //await MessageBox.Show((Window)(Application.Current as PrismApplication)?.MainWindow!, ex.Message, "Error", MessageBox.MessageBoxButtons.Ok);
            }
        }

        private static void SilenceConsole()
        {
            new Thread(() =>
            {
                Console.CursorVisible = false;
                while (true)
                    Console.ReadKey(true);
                // ReSharper disable once FunctionNeverReturns
            }) {IsBackground = true}.Start();
        }

        public static bool IsProduction()
        {
#if DEBUG
            return false;
#else
            return true;
#endif
        }

        public static AppBuilder BuildAvaloniaApp() =>
            AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .With(new X11PlatformOptions {EnableMultiTouch = true, UseDBusMenu = true})
                .With(new Win32PlatformOptions {EnableMultitouch = true, AllowEglInitialization = true})
                .UseSkia()
                .UseReactiveUI()
                .UseManagedSystemDialogs()
                .LogToTrace();
    }
}