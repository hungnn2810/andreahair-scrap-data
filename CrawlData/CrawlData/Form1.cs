using CrawlData.Commons.Utils;
using CrawlData.Model;
using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CrawlData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void CrawlData()
        {
            var targetUserIds = new List<string>();
            var targerCountFollower = new List<InstaUserInformationModel>(); 
            var followersDictionary = new Dictionary<string, InstaFollowerModel>();
            var excelData = new List<ExcelData>();
            string names = txtTargetUserNames.Text.ToString();
            List<string> TargetUserNames = names.Replace(" ", "").Split("\n\r").ToList();
            string sessionId = null;

            #region Login -> sessionId
            try
            {
                var requestLoginData = new List<KeyValuePair<string, string>>
                {
                    new("username", "duong.test01"),
                    new("password", "@Hello123"),
                    new("verification_code", ""),
                    new("proxy", ""),
                    new("locale", ""),
                    new("timezone", "")
                };
                sessionId = CreateHttpRequest("auth/login", HttpMethod.Post, requestLoginData);
            }
            catch(Exception e)
            {
                File.WriteAllText(@"C:\Users\Admin\Desktop\ConsoleApp1\log.txt", e.Message);
                MessageBox.Show("Đăng nhập thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion Login -> sessionId

            #region Get target user id
            try
            {
                foreach (var targetUserName in TargetUserNames)
                {
                    var userInfo = GetUserInformation(sessionId, targetUserName);
                    targetUserIds.Add(userInfo.pk);
                    targerCountFollower.Add(userInfo);
                }
            }
            catch(Exception e)
            {
                File.WriteAllText(@"C:\Users\Admin\Desktop\ConsoleApp1\log.txt", e.Message);
                MessageBox.Show("Lấy thông tin người dùng thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion Get target user id

            if (targetUserIds.Count <= 0)
            {
                MessageBox.Show("Không tìm thấy thông tin của tên nhập vào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #region Get all follower
            try
            {
                foreach (var requestGetFollowerData in targetUserIds.Select(id => new List<KeyValuePair<string, string>>
                {
                    new("sessionid", sessionId),
                    new("user_id", id),
                    new("use_cache", "true"),
                    new("amount", /*targerCountFollower.Where(x => x.pk == id).Select(x => x.follower_count).ToString() ?? "0"*/ "100")
                }))
                {
                    var responseGetFollowers = CreateHttpRequest("user/followers", HttpMethod.Post, requestGetFollowerData);
                    followersDictionary.AddRange(responseGetFollowers.ToObject<Dictionary<string, InstaFollowerModel>>());
                }
            }
            catch (Exception e)
            {
                File.WriteAllText(@"C:\Users\Admin\Desktop\ConsoleApp1\log.txt", e.Message);
                MessageBox.Show("Lấy followers thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion Get all follower

            #region Get info follower 
            try
            {
                foreach (var follower in followersDictionary)
                {
                    var followerInfo = GetUserInformation(sessionId, follower.Value.username);
                    if (followerInfo.is_private)
                        continue;
                    excelData.Add(followerInfo.GetExcelData());
                }
            }
            catch (Exception e)
            {
                File.WriteAllText(@"C:\Users\Admin\Desktop\ConsoleApp1\log.txt", e.Message);
                MessageBox.Show("Lấy thông tin của follower thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion Get info follower 

            #region Get phone number in posts
            try
            {
                foreach (var data in excelData)
                {
                    if (string.IsNullOrEmpty(data.phone_number))
                    {
                        // Lấy ra data của 10 bài viết
                        var mediaData = GetMediaByUserId(sessionId, data.id);
                        List<string> tempPhoneNumber = new List<string>();

                        // Lấy ra sđt có trong 10 bài viết
                        foreach (var media in mediaData)
                        {
                            var phoneNumber = RegexPhoneNumber(mediaData);
                            if (phoneNumber.Count > 0)
                            {
                                tempPhoneNumber.AddRange(phoneNumber);
                            }
                        }

                        string phoneNumb = null;
                        if (tempPhoneNumber.Count > 0)
                        {
                            var phoneVal = tempPhoneNumber.Distinct().ToList();
                            phoneNumb = string.Join(",", phoneVal);
                        }

                        if (!string.IsNullOrEmpty(phoneNumb))
                        {
                            data.phone_number = phoneNumb;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                File.WriteAllText(@"C:\Users\Admin\Desktop\ConsoleApp1\log.txt", e.Message);
                MessageBox.Show("Lấy số điện thoại trong bài viết thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion Get phone number in posts

            excelData.RemoveAll(x => string.IsNullOrEmpty(x.phone_number));

            #region Add data to excel
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1900, 1, 1)).TotalMilliseconds;
                var excelMapper = new ExcelMapper();
                var fileWinPath = string.Concat(desktopPath, @"/followers_" + unixTimestamp.ToString() + ".xlsx");
                ExcelHelper file = new ExcelHelper();
                // Create a new workbook with a single sheet
                file.NewFile();
                // Add a new sheet to the workbook
                file.NewSheet();
                // Saving the file in a speicifed path
                file.SaveAs(fileWinPath);
                // Closing the file
                file.Close();

                excelMapper.Save(fileWinPath, excelData);
            }
            catch (Exception e)
            {
                File.WriteAllText(@"C:\Users\Admin\Desktop\ConsoleApp1\log.txt", e.Message);
                MessageBox.Show("Đẩy dữ liệu vào excel thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Lấy data thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
            #endregion Add data to excel
        }

        private static string CreateHttpRequest(string endpoint, HttpMethod action, IEnumerable<KeyValuePair<string, string>> data)
        {
            var httpClient = new HttpClient();

            var httpRequest = new HttpRequestMessage(action, $"http://localhost:8000/{endpoint}");
            httpRequest.Headers.Add("accept", "application/json");

            var body = new FormUrlEncodedContent(data);
            httpRequest.Content = body;

            var httpResponse = httpClient.Send(httpRequest);
            httpResponse.EnsureSuccessStatusCode();

            return httpResponse.Content.ReadAsStringAsync().Result;
        }

        private static InstaUserInformationModel GetUserInformation(string sessionId, string userName)
        {
            var getUserInfoBody = new List<KeyValuePair<string, string>>
            {
                new("sessionid", sessionId),
                new("username", userName),
                new("use_cache", "true")
            };
            var responseGetUserInfo = CreateHttpRequest("user/info_by_username", HttpMethod.Post, getUserInfoBody);


            return responseGetUserInfo.ToObject<InstaUserInformationModel>();
        }

        private static List<InstaMediaModel> GetMediaByUserId(string sessionId, string userid)
        {
            var getMediaBody = new List<KeyValuePair<string, string>>
            {
                new("sessionid", sessionId),
                new("user_id", userid),
                new("amount", "10")
            };
            var responseGetUserInfo = CreateHttpRequest("media/user_medias", HttpMethod.Post, getMediaBody);

            return responseGetUserInfo.ToObject<List<InstaMediaModel>>();
        }


        private void btnCrawl_Click(object sender, EventArgs e)
        {
            CrawlData();
        }

        public static List<string> RegexPhoneNumber(List<InstaMediaModel> val)
        {
            List<string> result = new List<string>();

            foreach (var item in val)
            {
                var regexVal = item.caption_text.Replace(" ", "");
                List<string> tmp = new List<string>();

                Regex extractPhoneNumberRegex = new Regex("\\+?(\\s*\\d+)*[0-9][0-9]{7,14}");

                tmp = extractPhoneNumberRegex.Matches(regexVal)
                                .Cast<Match>()
                                .Select(m => m.Value).
                                ToList();

                result.AddRange(tmp);
            }

            return result;
        }
    }
}