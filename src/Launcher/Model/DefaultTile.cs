using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Model
{
    public class DefaultTile
    {
        [JsonProperty("sub_header")]
        public string SubHeader { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("tile_size")]
        public string TileSize { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

}
