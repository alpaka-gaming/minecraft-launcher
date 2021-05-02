using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher.Controls
{
    public partial class NewCard : UserControl
    {
        public NewCard()
        {
            InitializeComponent();
        }

        private Model.ArticleGrid _model;
        public Model.ArticleGrid Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }

        public string Host { get; set; }

        private void OnPropertyChanged()
        {
            if (_model != null)
            {
                labelTitle.DataBindings.Add("Text", Model.DefaultTile, "Title");
                labelSubTitle.DataBindings.Add("Text", Model.DefaultTile, "SubHeader");
                labelDate.DataBindings.Add("Text", Model, "PublishDate");
                buttonMore.DataBindings.Add("Tag", Model, "ArticleUrl");
                pictureBoxImage.DataBindings.Add("Tag", Model.DefaultTile.Image, "ImageURL");
            }
            else
            {
                labelTitle.DataBindings.Clear();
            }

        }

        private void buttonMore_Click(object sender, EventArgs e)
        {
            var btn = (sender as Button);
            if (btn.Tag != null)
                OpenUrl(Host + btn.Tag.ToString());
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private string _imagePath;
        private void NewCard_Load(object sender, EventArgs e)
        {
            if (pictureBoxImage.Tag != null)
            {
                var url = Host + pictureBoxImage.Tag.ToString();
                var folder = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                var _path = Path.Combine(Path.GetTempPath(), folder);
                if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);
                _imagePath = Path.Combine(Path.GetTempPath(), folder , Model.ArticleUrl.Split("/").Last());
                if (!File.Exists(_imagePath))
                {
                    using (WebClient client = new WebClient())
                    {
                        client.DownloadFileCompleted += Client_DownloadFileCompleted;
                        client.DownloadFileAsync(new Uri(url), _imagePath);
                    }
                }
                else
                {
                    pictureBoxImage.Image = Image.FromFile(_imagePath);
                }
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null && !e.Cancelled)
            {
                if (!string.IsNullOrWhiteSpace(_imagePath) && System.IO.File.Exists(_imagePath))
                {
                    pictureBoxImage.Image = Image.FromFile(_imagePath);
                }
            }
        }
    }
}
