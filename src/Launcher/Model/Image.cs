using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Model
{
    public class Image
    {
        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("imageURL")]
        public string ImageURL { get; set; }

        [JsonProperty("alt")]
        public string Alt { get; set; }
    }
}
