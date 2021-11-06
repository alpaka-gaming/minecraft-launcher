using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Prism.Regions;
using ReactiveUI;

namespace Launcher.ViewModels
{
    public class WelcomeViewModel : ViewModelBase
    {
        private readonly IDownloader _downloader;
        
        public WelcomeViewModel()
        {
            _downloader = null!;
            Versions = new ObservableCollection<Game>();
        }

        public WelcomeViewModel(IRegionManager regionManager, IDownloader downloader, ILoggerFactory loggerFactory) : base(regionManager, loggerFactory)
        {
            _downloader = downloader;
            Versions = new ObservableCollection<Game>();
        }
        
        private ObservableCollection<Game> _versions = null!;

        public ObservableCollection<Game> Versions
        {
            get => _versions;
            set => this.RaiseAndSetIfChanged(ref _versions, value);
        }
        
        public override async Task OnInitializedAsync()
        {
            var items = await _downloader.GetVersionsAsync();
            items = items
                .Where(m => Program.MinecraftOptions.Betas && m.IsBeta || m.IsBeta == false)
                .Where(m => m.Version >= Program.MinecraftOptions.MinVersion);
            Versions = new ObservableCollection<Game>(items);
            await base.OnInitializedAsync();
        }
        
    }
    
}