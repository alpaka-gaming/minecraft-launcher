using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Launcher.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prism.Regions;
using ReactiveUI;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using Timer = System.Timers.Timer;

namespace Launcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IAssetLoader _assetLoader;

        public MainWindowViewModel()
        {
            _assetLoader = null!;
        }

        public MainWindowViewModel(IRegionManager regionManager, ILoggerFactory loggerFactory) : base(regionManager, loggerFactory)
        {
            _assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();

            /* Background */
            var timer = new Timer(30000);
            timer.Elapsed += (_, _) => BackgroundInit();
            timer.Start();

            BackgroundInit();
        }


        private void BackgroundInit()
        {
            var start = 1;
            var end = 7;
            var rnd = new Random();
            var index = rnd.Next(start - 1, end + 1);
            if (index < start) index = start;
            else if (index > end) index = end;

            Background = new Bitmap(_assetLoader.Open(new Uri($"avares://Launcher/Assets/Images/Backgrounds/bg{index}.jpg")));
        }


        // private async void InvokeDownload(Game v)
        // {
        //     CancellationTokenSource cancelSource = new CancellationTokenSource();
        //
        //     var dlPath = "Downloads";
        //     var versions = await _downloader.GetVersionsAsync();
        //     await _downloader.Download(v.UUID, "1", dlPath, (_, _) =>
        //     {
        //         // if (v.StateChangeInfo.VersionState != VersionState.Downloading)
        //         // {
        //         //     Debug.WriteLine("Actual download started");
        //         //     v.StateChangeInfo.VersionState = VersionState.Downloading;
        //         //     if (total.HasValue)
        //         //         v.StateChangeInfo.TotalSize = total.Value;
        //         // }
        //         //
        //         // v.StateChangeInfo.DownloadedBytes = current;
        //     }, cancelSource.Token);
        // }


        private Bitmap _background = null!;

        public Bitmap Background
        {
            get => _background;
            set => this.RaiseAndSetIfChanged(ref _background, value);
        }
    }
}