using Flurl.Http;
using Launcher.Controllers;
using Launcher.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Launcher
{
    public partial class FormMain : Form
    {
        private readonly ILogger _logger;
        private readonly IServer _server;
        private readonly IJava _java;
        private readonly IGame _game;

        public FormMain(IServer server, IJava java, IGame game, ILoggerFactory logger)
        {
            _server = server;
            _java = java;
            _game = game;
            _logger = logger.CreateLogger(GetType());

            InitializeComponent();

            labelVersion.Text = $"v{Program.Version}";
        }

        private async Task LoadServerData()
        {
            var server = await _server.ConnectAsync();

            var status = new[] {
                string.Format(Properties.Resources.ServerStatus, server ? Properties.Resources.OkStatus : Properties.Resources.FailStatus),
                string.Format(Properties.Resources.FastDownloadStatus, server ? Properties.Resources.OkStatus : Properties.Resources.FailStatus)};
            labelServer.Text = string.Join(Environment.NewLine, status);
            labelServer.ForeColor = (!server) ? Color.OrangeRed : Color.DarkOliveGreen;

            var gamePath = await _game.FindGame();
            var profile = await _server.GetProfile();
            var valids = await _game.ValidateProfile();

            buttonPlay.Enabled = server && gamePath && profile && valids;

        }

        private async Task LoadNewsAsync()
        {
            var url = _server.FeedUrl;
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

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            //-Dos.name=Windows 10
            //-Dos.version=10.0
            //-Djava.library.path=C:\Users\ennerperez\AppData\Roaming\.minecraft\bin\f1ba-7820-0074-58ec
            //-Dminecraft.launcher.brand=minecraft-launcher
            //-Dminecraft.launcher.version=2.2.2311
            //-cp C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\minecraftforge\forge\1.16.5-36.1.4\forge-1.16.5-36.1.4.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm\9.0\asm-9.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm-commons\9.0\asm-commons-9.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm-tree\9.0\asm-tree-9.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm-util\9.0\asm-util-9.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\ow2\asm\asm-analysis\9.0\asm-analysis-9.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\cpw\mods\modlauncher\8.0.9\modlauncher-8.0.9.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\cpw\mods\grossjava9hacks\1.3.3\grossjava9hacks-1.3.3.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\minecraftforge\accesstransformers\3.0.1\accesstransformers-3.0.1.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\antlr\antlr4-runtime\4.9.1\antlr4-runtime-4.9.1.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\minecraftforge\eventbus\4.0.0\eventbus-4.0.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\minecraftforge\forgespi\3.2.0\forgespi-3.2.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\minecraftforge\coremods\4.0.6\coremods-4.0.6.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\minecraftforge\unsafe\0.2.0\unsafe-0.2.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\com\electronwill\night-config\core\3.6.3\core-3.6.3.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\com\electronwill\night-config\toml\3.6.3\toml-3.6.3.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\jline\jline\3.12.1\jline-3.12.1.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\apache\maven\maven-artifact\3.6.3\maven-artifact-3.6.3.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\jodah\typetools\0.8.3\typetools-0.8.3.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-api\2.11.2\log4j-api-2.11.2.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-core\2.11.2\log4j-core-2.11.2.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\minecrell\terminalconsoleappender\1.2.0\terminalconsoleappender-1.2.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\sf\jopt-simple\jopt-simple\5.0.4\jopt-simple-5.0.4.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\spongepowered\mixin\0.8.2\mixin-0.8.2.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\minecraftforge\nashorn-core-compat\15.1.1.1\nashorn-core-compat-15.1.1.1.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\com\mojang\patchy\1.1\patchy-1.1.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\oshi-project\oshi-core\1.1\oshi-core-1.1.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\java\dev\jna\jna\4.4.0\jna-4.4.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\java\dev\jna\platform\3.4.0\platform-3.4.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\com\ibm\icu\icu4j\66.1\icu4j-66.1.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\com\mojang\javabridge\1.0.22\javabridge-1.0.22.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\sf\jopt-simple\jopt-simple\5.0.3\jopt-simple-5.0.3.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\io\netty\netty-all\4.1.25.Final\netty-all-4.1.25.Final.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\com\google\guava\guava\21.0\guava-21.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\apache\commons\commons-lang3\3.5\commons-lang3-3.5.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\commons-io\commons-io\2.5\commons-io-2.5.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\commons-codec\commons-codec\1.10\commons-codec-1.10.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\com\mojang\brigadier\1.0.17\brigadier-1.0.17.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\com\mojang\datafixerupper\4.0.26\datafixerupper-4.0.26.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\com\google\code\gson\gson\2.8.0\gson-2.8.0.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\com\mojang\authlib\2.1.28\authlib-2.1.28.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\it\unimi\dsi\fastutil\8.2.1\fastutil-8.2.1.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-api\2.8.1\log4j-api-2.8.1.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\apache\logging\log4j\log4j-core\2.8.1\log4j-core-2.8.1.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl\3.2.2\lwjgl-3.2.2.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl-jemalloc\3.2.2\lwjgl-jemalloc-3.2.2.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl-openal\3.2.2\lwjgl-openal-3.2.2.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl-opengl\3.2.2\lwjgl-opengl-3.2.2.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl-glfw\3.2.2\lwjgl-glfw-3.2.2.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl-stb\3.2.2\lwjgl-stb-3.2.2.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\org\lwjgl\lwjgl-tinyfd\3.2.2\lwjgl-tinyfd-3.2.2.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\libraries\com\mojang\text2speech\1.11.3\text2speech-1.11.3.jar;
            //C:\Users\ennerperez\AppData\Roaming\.minecraft\versions\1.16.5-forge-36.1.4\1.16.5-forge-36.1.4.jar
            //--username mrspectate
            //--assetsDir C:\Users\ennerperez\AppData\Roaming\.minecraft\assets
            //--assetIndex 1.16 --uuid eb94853f206b41908a0ea6d72955eb5a
            //--accessToken eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIzZTE1MDJhMWFlNDVhYmY1OTE5N2NkNjUzNjM0ODUzMSIsInlnZ3QiOiJiZTkzOTU0NWE0MTU0NjNmYTBmMGZiYmFmYmUwNzgwMCIsInNwciI6ImViOTQ4NTNmMjA2YjQxOTA4YTBlYTZkNzI5NTVlYjVhIiwiaXNzIjoiWWdnZHJhc2lsLUF1dGgiLCJleHAiOjE2MjAxNjc3MDEsImlhdCI6MTYxOTk5NDkwMX0.LWkPeK62UnTqm-WmKuDIrHV4m4MrXiX8Kvz02PqgpP0
            //--userType mojang
            //--versionType release

            //--fml.forgeGroup net.minecraftforge
            //--fml.mcpVersion 20210115.111550";

            var profile = _game.Profiles.First();
            var args = new List<string>();
            args.Add($"--gameDir {profile.Value.GameDir}");
            args.Add($"--version {profile.Value.LastVersionId}");
            args.Add("-Xss1M");
            args.Add("-Xmx2G");
            args.Add("-XX:+UnlockExperimentalVMOptions");
            args.Add("-XX:+UseG1GC");
            args.Add("-XX:G1NewSizePercent=20");
            args.Add("-XX:G1ReservePercent=20");
            args.Add("-XX:MaxGCPauseMillis=50");
            args.Add("-XX:G1HeapRegionSize=32M2");
            args.Add("cpw.mods.modlauncher.Launcher");
            args.Add("-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump");
            args.Add("--launchTarget fmlclient");
            args.Add($"--fml.forgeVersion {_server.Versions["Forge"]}"); //36.1.4
            args.Add($"--fml.mcVersion {_server.Versions["Minecraft"]}"); //1.16.5
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = _java.JavaExecutable,
                    Arguments = string.Join(" ", args.ToArray())
                }
            };
            process.Start();
        }
    }
}
