using Launcher.Views;
using Microsoft.Extensions.Logging;
using Prism.Regions;

namespace Launcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
        }

        public MainWindowViewModel(IRegionManager regionManager, ILoggerFactory loggerFactory) : base(regionManager, loggerFactory)
        {
            _regionManager.RequestNavigate("MainRegion", nameof(WelcomeView));
        }
    }
}