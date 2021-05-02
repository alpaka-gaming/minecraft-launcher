using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Model
{
    public class Grid
    {
        [JsonProperty("article_grid")]
        public List<ArticleGrid> ArticleGrid { get; set; }

        [JsonProperty("article_count")]
        public int ArticleCount { get; set; }
    }
}
