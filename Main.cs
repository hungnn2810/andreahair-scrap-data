using CrawlData.Commons.Utils;
using CrawlData.Model;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using Ganss.Excel;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrawlData
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        public async void CrawlData()
        {
            #region Properties
            var targetUserIds = new List<string>();
            var targerCountFollower = new List<InstaUserInformationModel>();
            var followersDictionary = new Dictionary<string, InstaFollowerModel>();
            var excelData = new List<ExcelData>();
            string names = txtTargetUserNames.Text.ToString();
            List<string> targetUserNames = names.Replace(" ", "").Split("\r\n").ToList();
            string sessionId = null;
            string username = txtUsername.Text.ToString();
            string password = txtPassword.Text.ToString();
            CancellationToken cancellationToken = new CancellationToken();
            #endregion Properties

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Chưa nhập tài khoản hoặc mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnCrawl.Enabled = true;
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
                sessionId = await CreateHttpRequest("auth/login", HttpMethod.Post, requestLoginData, cancellationToken);
            }
            catch (Exception e)
            {
                File.WriteAllText(@"./log.txt", DateTime.Now.ToString() + e.Message);
                MessageBox.Show("Đăng nhập thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnCrawl.Enabled = true;
                return;
            }
            #endregion Login -> sessionId

            #region Get target user id
            foreach (var targetUserName in targetUserNames)
            {
                try
                {
                    var userInfo = await GetUserInformation(sessionId, targetUserName, cancellationToken);
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
                btnCrawl.Enabled = true;
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
                    var responseGetFollowers = await CreateHttpRequest("user/followers", HttpMethod.Post, requestGetFollowerData, cancellationToken);
                    followersDictionary.AddRange(responseGetFollowers.ToObject<Dictionary<string, InstaFollowerModel>>());
                }
                catch (Exception e)
                {
                    File.WriteAllText(@"./log.txt", "userName: " + requestGetFollowerData.ToString() + DateTime.Now.ToString() + e.Message);
                }
            }
            #endregion Get all follower

            #region Get info follower 
            var tasks = new List<Task<Tuple<InstaUserInformationModel, bool>>>();

            foreach (var itemTask in followersDictionary)
            {
                tasks.Add(GetUserInformationTuple(sessionId, itemTask.Value.username, cancellationToken));
            }

            foreach (var task in await Task.WhenAll(tasks))
            {
                var data = task.Item1;
                try
                {
                    if (task.Item2)
                    {
                        if (data.is_private)
                            continue;
                        var excel = data.GetExcelData();
                        excelData.Add(excel);
                    }
                }
                catch (Exception e)
                {
                    File.WriteAllText(@"./log.txt", $"userId: {data.pk} -- {data.username} " + DateTime.Now.ToString() + e.Message);
                }
            }
            #endregion Get info follower 

            #region Get phone number follower by medias
            var excelPhoneNumb = new List<ExcelData>();
            var taskGetPhoneNumbers = new List<Task<Tuple<List<InstaMediaModel>, bool>>>();

            // Executed parallel
            foreach (var excel in excelData)
            {
                if (string.IsNullOrEmpty(excel.phone_number))
                {
                    taskGetPhoneNumbers.Add(GetMediaByUserId(sessionId, excel.id, cancellationToken));
                }
            }

            // When all done doing this
            foreach (var task in await Task.WhenAll(taskGetPhoneNumbers))
            {
                var mediaData = task.Item1;
                List<string> tempPhoneNumber = new List<string>();
                var tmpExcel = new ExcelData();

                // Lấy ra sđt có trong 10 bài viết
                foreach (var media in mediaData)
                {
                    var phoneNumber = RegexPhoneNumber(mediaData);
                    if (phoneNumber.Count > 0)
                    {
                        tempPhoneNumber.AddRange(phoneNumber);
                        tmpExcel.id = media.pk;
                    }
                }

                string phoneNumb = null;
                if (tempPhoneNumber.Count > 0)
                {
                    var phoneVal = tempPhoneNumber.Distinct().ToList();
                    phoneNumb = string.Join(",", phoneVal);

                    tmpExcel.phone_number = phoneNumb;
                    excelPhoneNumb.Add(tmpExcel);
                }
            }

            foreach(var item in excelData)
            {
                var phoneNumb = excelPhoneNumb.Where(x => x.id == item.id).Select(x => x.phone_number).FirstOrDefault();
                if (!string.IsNullOrEmpty(phoneNumb))
                {
                    item.phone_number = $"{phoneNumb},{item.phone_number}";
                }
            }
            #endregion Get phone number follower by medias

            #region Add data to excel
            try
            {
                excelData.RemoveAll(x => string.IsNullOrEmpty(x.phone_number));

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

            MessageBox.Show("Lấy data thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnCrawl.Enabled = true;
            return;
            #endregion Add data to excel
        }

        private static async Task<string> CreateHttpRequest(string endpoint, HttpMethod action, IEnumerable<KeyValuePair<string, string>> data, CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromHours(24);

            var httpRequest = new HttpRequestMessage(action, $"http://localhost:8000/{endpoint}");
            httpRequest.Headers.Add("accept", "application/json");

            var body = new FormUrlEncodedContent(data);
            httpRequest.Content = body;

            var httpResponse = await httpClient.SendAsync(httpRequest, cancellationToken);
            httpResponse.EnsureSuccessStatusCode();

            return await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        }

        private static async Task<InstaUserInformationModel> GetUserInformation(string sessionId, string userName, CancellationToken cancellationToken)
        {
            var getUserInfoBody = new List<KeyValuePair<string, string>>
            {
                new("sessionid", sessionId),
                new("username", userName),
                new("use_cache", "true")
            };
            var responseGetUserInfo = await CreateHttpRequest("user/info_by_username", HttpMethod.Post, getUserInfoBody, cancellationToken);

            return responseGetUserInfo.ToObject<InstaUserInformationModel>();
        }

        private static async Task<Tuple<InstaUserInformationModel, bool>> GetUserInformationTuple(string sessionId, string userName, CancellationToken cancellationToken)
        {
            var getUserInfoBody = new List<KeyValuePair<string, string>>
            {
                new("sessionid", sessionId),
                new("username", userName),
                new("use_cache", "true")
            };
            var responseGetUserInfo = await CreateHttpRequest("user/info_by_username", HttpMethod.Post, getUserInfoBody, cancellationToken);

            return Tuple.Create(responseGetUserInfo.ToObject<InstaUserInformationModel>(), true);
        }

        private static async Task<Tuple<List<InstaMediaModel>, bool>> GetMediaByUserId(string sessionId, string userid, CancellationToken cancellationToken)
        {
            var getMediaBody = new List<KeyValuePair<string, string>>
            {
                new("sessionid", sessionId),
                new("user_id", userid),
                new("amount", "10")
            };
            var responseGetUserInfo = await CreateHttpRequest("media/user_medias", HttpMethod.Post, getMediaBody, cancellationToken);

            return Tuple.Create(responseGetUserInfo.ToObject<List<InstaMediaModel>>(), true);
        }

        private void btnCrawl_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bắt đầu cào dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnCrawl.Enabled = false;
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
                                .Cast<System.Text.RegularExpressions.Match>()
                                .Select(m => m.Value).
                                ToList();

                result.AddRange(tmp);
            }

            return result;
        }
    }
}