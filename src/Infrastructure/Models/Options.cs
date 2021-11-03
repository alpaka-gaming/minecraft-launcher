using System;

namespace Infrastructure.Models
{
    public class DownloaderOptions
    {
        public string Versions { get; set; }
    }

    public class MinecraftOptions
    {
        public Version MinVersion { get; set; }
        public bool Betas { get; set; }
    }
}