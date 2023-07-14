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
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        public void CrawlData()
        {
            var startTime = DateTime.Now;
            var targetUserIds = new List<string>();
            var targerCountFollower = new List<InstaUserInformationModel>();
            var followersDictionary = new Dictionary<string, InstaFollowerModel>();
            var excelData = new List<ExcelData>();
            string names = txtTargetUserNames.Text.ToString();
            List<string> targetUserNames = names.Replace(" ", "").Split("\r\n").ToList();
            string sessionId = null;
            string username = txtUsername.Text.ToString();
            string password = txtPassword.Text.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Chưa nhập tài khoản hoặc mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #region Login -> sessionId
            try
            {
                var requestLoginData = new List<KeyValuePair<string, string>>
                {
                    new("username", username),
                    new("password", password),
                    new("verification_code", ""),
                    new("proxy", ""),
                    new("locale", ""),
                    new("timezone", "")
                };
                sessionId = CreateHttpRequest("auth/login", HttpMethod.Post, requestLoginData);
            }
            catch (Exception e)
            {
                File.WriteAllText(@"./log.txt", DateTime.Now.ToString() + e.Message);
                MessageBox.Show("Đăng nhập thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion Login -> sessionId

            #region Get target user id

            foreach (var targetUserName in targetUserNames)
            {
                try
                {
                    var userInfo = GetUserInformation(sessionId, targetUserName);
                    targetUserIds.Add(userInfo.pk);
                    targerCountFollower.Add(userInfo);
                }
                catch (Exception e)
                {
                    File.WriteAllText(@"./log.txt", $"userName: {targetUserName}" + e.Message);
                }
            }
            #endregion Get target user id

            if (targetUserIds.Count <= 0)
            {
                MessageBox.Show("Không tìm thấy thông tin của tên nhập vào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            #region Get all follower

            foreach (var requestGetFollowerData in targetUserIds.Select(id => new List<KeyValuePair<string, string>>
                {
                    new("sessionid", sessionId),
                    new("user_id", id),
                    new("use_cache", "true"),
                    new("amount", targerCountFollower.Where(x => x.pk == id).Select(x => x.follower_count).FirstOrDefault().ToString() ?? "0")
                }))
            {
                try
                {
                    var responseGetFollowers = CreateHttpRequest("user/followers", HttpMethod.Post, requestGetFollowerData);
                    followersDictionary.AddRange(responseGetFollowers.ToObject<Dictionary<string, InstaFollowerModel>>());
                }
                catch (Exception e)
                {
                    File.WriteAllText(@"./log.txt", "userName: " + requestGetFollowerData.ToString() + DateTime.Now.ToString() + e.Message);
                }
            }
            #endregion Get all follower

            #region Get info follower 

            foreach (var follower in followersDictionary)
            {
                try
                {
                    var info = GetUserInformation(sessionId, follower.Value.username);
                    if (info.is_private)
                        continue;
                    var excel = info.GetExcelData();
                    if (string.IsNullOrEmpty(excel.phone_number))
                    {
                        try
                        {
                            // Lấy ra data của 10 bài viết
                            var mediaData = GetMediaByUserId(sessionId, excel.id);
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
                                excel.phone_number = phoneNumb;
                            }

                            if (string.IsNullOrEmpty(excel.phone_number))
                            {
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            File.WriteAllText(@"./log.txt", $"userId: {excel.id} -- {excel.username}" + e.Message);
                        }
                    }

                    excelData.Add(excel);
                }
                catch (Exception e)
                {
                    File.WriteAllText(@"./log.txt", $"userId: {follower.Key} -- {follower.Value.username} " + DateTime.Now.ToString() + e.Message);
                }
            }

            #endregion Get info follower 


            #region Add data to excel
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
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
                File.WriteAllText(@"./log.txt", e.Message);
            }
            var endTime = DateTime.Now;
            var totalTime = GetTime(startTime, endTime);
            MessageBox.Show($"Lấy data thành công! \r\n Tổng thời gian hoàn thành là: {totalTime}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
            #endregion Add data to excel
        }

        private static string CreateHttpRequest(string endpoint, HttpMethod action, IEnumerable<KeyValuePair<string, string>> data)
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromHours(24);
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
        public string GetTime(DateTime lower, DateTime upper)
        {
            string returnString = string.Empty;
            TimeSpan timeSpan = upper - lower;
            if (timeSpan.Days >= 1)
            {
                returnString = timeSpan.Days.ToString() + " ngày";
            }
            else
            {
                if (timeSpan.Hours == 0 && timeSpan.Minutes == 0)
                {
                    returnString = "Less than 1 minute";
                }
                else if (timeSpan.Hours > 0)
                {
                    returnString = timeSpan.Hours.ToString() + " giờ " + timeSpan.Minutes.ToString() + " phút";
                }
                else
                {
                    returnString += timeSpan.Minutes.ToString() + " phút" + timeSpan.Seconds.ToString() + "giây";
                }
            }
            return returnString;
        }
    }
}