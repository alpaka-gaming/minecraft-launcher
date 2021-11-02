using System;
using System.Drawing;
using System.Reactive;
using System.Timers;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Launcher.Views;
using Microsoft.Extensions.Logging;
using Prism.Regions;
using ReactiveUI;
using Bitmap = Avalonia.Media.Imaging.Bitmap;

namespace Launcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            getRandomBackground();
        }

        public MainWindowViewModel(IRegionManager regionManager, ILoggerFactory loggerFactory) : base(regionManager, loggerFactory)
        {
            _regionManager.RequestNavigate("MainRegion", nameof(WelcomeView));
            var timer = new Timer(30000);
            timer.Elapsed += (sender, args) => getRandomBackground();
            getRandomBackground();
            timer.Start();
        }

        private void getRandomBackground(object? state = null)
        {
            var start = 1;
            var end = 7;
            var rnd = new Random();
            var index = rnd.Next(start - 1, end + 1);
            if (index < start) index = start;
            else if (index > end) index = end;

            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            Background = new Bitmap(assets.Open(new Uri($"avares://Launcher/Assets/Images/Backgrounds/bg{index}.jpg")));
        }

        private Bitmap _background = null!;

        public Bitmap Background
        {
            get => _background;
            set => this.RaiseAndSetIfChanged(ref _background, value);
        }
    }
}