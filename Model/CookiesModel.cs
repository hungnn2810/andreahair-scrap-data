using System;

namespace Instagram.Model
{
    public class CookiesModel
    {
        public string name { get; set; }
        public string value { get; set; }
        public string domain { get; set; }
        public string path { get; set; }
        public bool secure { get; set; }
        public bool httpOnly { get; set; }
        public string sameSite { get; set; }
        public long expiry { get; set; }
    }
}
