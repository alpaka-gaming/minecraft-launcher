using Flurl.Http;
using Launcher.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    public partial class FormMain : Form
    {
        private readonly ILogger _logger;

        public FormMain(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger(GetType());
            InitializeComponent();
            //Controls.Add(new Views.MainView(){ Dock =  DockStyle.Fill});
        }

        private async Task LoadServerData()
        {
            Program.PingServer();
        }

        private async Task LoadNewsAsync()
        {
            var url = Program.FeedUrl;
            var host = url.GetLeftPart(UriPartial.Authority);
            var news = await url.GetJsonAsync<Grid>();
            foreach (var item in news.ArticleGrid)
            {
                var controls = new Controls.NewCard() { Host = host };
                controls.Model = item;
                flowLayoutPanelNews.Controls.Add(controls);
            }
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            await LoadServerData();
            await LoadNewsAsync();
        }
    }
}
