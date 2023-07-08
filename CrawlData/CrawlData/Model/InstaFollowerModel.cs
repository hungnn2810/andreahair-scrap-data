using System.Collections.Generic;

namespace CrawlData.Model
{
    public class InstaFollowerModel
    {
        public string pk { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }
        public string profile_pic_url { get; set; }
        public string profile_pic_url_hd { get; set; }
        public bool is_private { get; set; }
        public List<object> stories { get; set; }
    }
}
