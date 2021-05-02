using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Model
{
    public class ArticleGrid
    {
        [JsonProperty("default_tile")]
        public DefaultTile DefaultTile { get; set; }

        [JsonProperty("articleLang")]
        public string ArticleLang { get; set; }

        [JsonProperty("primary_category")]
        public string PrimaryCategory { get; set; }

        [JsonProperty("categories")]
        public List<string> Categories { get; set; }

        [JsonProperty("article_url")]
        public string ArticleUrl { get; set; }

        [JsonProperty("publish_date")]
        public string PublishDate { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("preferred_tile")]
        public PreferredTile PreferredTile { get; set; }
    }
}
