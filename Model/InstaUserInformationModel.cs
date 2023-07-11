using LanguageDetection;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CrawlData.Model
{
    public class InstaUserInformationModel
    {
        public string pk { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }
        public bool is_private { get; set; }
        public string profile_pic_url { get; set; }
        public string profile_pic_url_hd { get; set; }
        public bool is_verified { get; set; }
        public int media_count { get; set; }
        public int follower_count { get; set; }
        public int following_count { get; set; }
        public string biography { get; set; }
        public string external_url { get; set; }
        public bool is_business { get; set; }
        public string public_email { get; set; }
        public string contact_phone_number { get; set; }
        public string business_contact_method { get; set; }
        public string business_category_name { get; set; }
        public string category_name { get; set; }

        private static string GetPhoneNumber(string biography, string external_url, string contact_phone_number)
        {
            List<string> phoneNubmers = new List<string>();

            if (!string.IsNullOrEmpty(biography))
            {
                phoneNubmers.AddRange(RegexPhoneNumber(biography));
            }
            if (!string.IsNullOrEmpty(external_url))
            {
                phoneNubmers.AddRange(RegexPhoneNumber(external_url));
            }
            if (!string.IsNullOrEmpty(contact_phone_number))
            {
                phoneNubmers.AddRange(RegexPhoneNumber(contact_phone_number));
            }

            var returnVal = string.Join(",", phoneNubmers);

            return returnVal;
        }

        public static List<string> RegexPhoneNumber(string val)
        {
            List<string> result = new List<string>();
            
            var regexVal = val.Replace(" ", "");

            Regex extractPhoneNumberRegex = new Regex("\\+?(\\s*\\d+)*[0-9][0-9]{7,14}");

            result = extractPhoneNumberRegex.Matches(regexVal)
                            .Cast<Match>()
                            .Select(m => m.Value).
                            ToList();

            return result;
        }

        public static string GetLanguageCode(string val)
        {
            var detector = new LanguageDetector();
            detector.AddAllLanguages();

            return detector.Detect(val);
        }

        public ExcelData GetExcelData()
        {
            return new ExcelData()
            {
                id = pk,
                username = username,
                full_name = full_name,
                phone_number = GetPhoneNumber(biography, external_url, contact_phone_number),
                link_ig = $"https://www.instagram.com/{username}",
                language_code = GetLanguageCode(biography)
            };
        }
    }

    public class ExcelData
    {
        public string id { get; set; }
        public string username { get; set; }
        public string full_name { get; set; }
        public string phone_number { get; set; }
        public string link_ig { get; set; }
        public string language_code { get; set; }
    }
}
