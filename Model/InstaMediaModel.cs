using System.Collections.Generic;

namespace CrawlData.Model
{
    public class InstaMediaModel
    {
        public string pk { get; set; }
        public string id { get; set; }
        public string code { get; set; }
        public string taken_at { get; set; }
        public int media_type { get; set; }
        public string product_type { get; set; }
        public string thumbnail_url { get; set; }
        public int comment_count { get; set; }
        public int like_count { get; set; }
        public bool? has_liked { get; set; }
        public string caption_text { get; set; }
        public string video_url { get; set; }
        public int view_count { get; set; }
        public double video_duration { get; set; }
        public string title { get; set; }
    }

    public class Location
    {
        public int pk { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string website { get; set; }
        public string category { get; set; }
        public string hours { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public int lng { get; set; }
        public int lat { get; set; }
        public int external_id { get; set; }
        public string external_id_source { get; set; }
    }

    public class UserTag
    {
        public InstaFollowerModel user { get; set; }
        public int x { get; set; }
        public int y { get; set; }
    }
}
