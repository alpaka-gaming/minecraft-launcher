using System;

namespace Infrastructure.Models
{
    public class Game
    {
        public Game(string name, string uuid, bool isBeta = false)
        {
            Name = name;
            UUID = uuid;
            IsBeta = isBeta;
        }

        public string Name { get; private set; }
        public string UUID { get; private set; }
        public bool IsBeta { get; private set; }

        public Guid Key => Guid.Parse(UUID);
        public Version Version => Version.Parse(Name);
    }
}