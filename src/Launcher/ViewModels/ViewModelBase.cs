using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media;
using Microsoft.Extensions.Logging;
using Prism.Regions;
using ReactiveUI;

namespace Launcher.ViewModels
{
    public abstract class ViewModelBase : ReactiveObject, INavigationAware
    {
        internal readonly IRegionManager _regionManager;
        internal readonly ILogger _logger;

        public ViewModelBase()
        {
            _regionManager = null!;
            _logger = null!;
        }

        public ViewModelBase(IRegionManager regionManager, ILoggerFactory loggerFactory)
        {
            _regionManager = regionManager;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }

        public ICommand BackCommand { get; set; } = null!;
        public ICommand NextCommand { get; set; } = null!;

        public Task InitializationNotifier { get; set; } = null!;
        public Task RenderNotifier { get; set; } = null!;

        public virtual Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }

        protected bool IsFirstRender { get; private set; } = true;

        public virtual Task OnRenderAsync(DrawingContext context)
        {
            if (IsFirstRender)
                IsFirstRender = false;

            return Task.CompletedTask;
        }

        #region INavigationAware

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        #endregion
    }
}