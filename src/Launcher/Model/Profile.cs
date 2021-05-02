using System;

namespace Launcher.Models
{
    public class Profile
    {
        public DateTime Created { get; set; }
        public string GameDir { get; set; }
        public string Icon { get; set; }
        public DateTime LastUsed { get; set; }
        public string LastVersionId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}