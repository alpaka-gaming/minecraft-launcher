using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Infrastructure.Interfaces;

namespace Launcher
{
    public class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void Show()
        {
            Width = Screens.Primary.Bounds.Width / 2.0;
            Height = Screens.Primary.Bounds.Height / 2.0;
            var x = (int) ((Screens.Primary.Bounds.Width / 2.0) - Width / 2);
            var y = (int) ((Screens.Primary.Bounds.Height / 2.0) - Height / 2);
            Position = new PixelPoint(x, y);
            base.Show();
        }
        
        internal static IMusicPlayer MusicPlayer { get; set; } = null!;
        
        protected override void OnClosing(CancelEventArgs e)
        {
            MusicPlayer?.Stop();
            base.OnClosing(e);
        }
    }
}