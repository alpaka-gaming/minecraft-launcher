using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Launcher.ViewModels;

namespace Launcher.Views
{
    public class WelcomeView : UserControl
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnInitialized()
        {
            if (DataContext != null)
            {
                var viewModel = (DataContext as ViewModelBase);
                if (viewModel != null)
                    viewModel.InitializationNotifier = viewModel.OnInitializedAsync();
            }

            base.OnInitialized();
        }

        public override void Render(DrawingContext context)
        {
            if (DataContext != null)
            {
                var viewModel = (DataContext as ViewModelBase);
                if (viewModel != null)
                    viewModel.RenderNotifier = viewModel.OnRenderAsync(context);
            }

            base.Render(context);
        }
    }
}