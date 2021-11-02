using Launcher.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Launcher
{
    public class Bootstrapper: IModule
    {
        private readonly IRegionManager _regionManager;

        public Bootstrapper(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(WelcomeView));
        }
    }
}