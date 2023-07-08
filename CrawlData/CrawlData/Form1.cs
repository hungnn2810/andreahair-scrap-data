using CrawlData.Model;
using Ganss.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;

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
            var followersDictionary = new Dictionary<string, InstaFollowerModel>();
            var excelData = new List<ExcelData>();
            string[] TargetUserNames = { "andreahair_lily" };
            #region Login -> sessionId

            var requestLoginData = new List<KeyValuePair<string, string>>
            {
                new("username", "duong.test01"),
                new("password", "@Hello123"),
                new("verification_code", ""),
                new("proxy", ""),
                new("locale", ""),
                new("timezone", "")
            };
            var sessionId = CreateHttpRequest("auth/login", HttpMethod.Post, requestLoginData);

            #endregion Login -> sessionId

            #region Get target user id

            foreach (var targetUserName in TargetUserNames)
            {
                var userInfo = GetUserInformation(sessionId, targetUserName);
                targetUserIds.Add(userInfo.pk);
            }

            #endregion Get target user id

            if (targetUserIds.Count <= 0)
            {
                MessageBox.Show("Không tìm thấy thông tin của tên nhập vào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            foreach (var requestGetFollowerData in targetUserIds.Select(id => new List<KeyValuePair<string, string>>
                {
                    new("sessionid", sessionId),
                    new("user_id", id),
                    new("use_cache", "true"),
                    new("amount", "5")
                }))
            {
                var responseGetFollowers = CreateHttpRequest("user/followers", HttpMethod.Post, requestGetFollowerData);
                followersDictionary.AddRange(responseGetFollowers.ToObject<Dictionary<string, InstaFollowerModel>>());
            }

            foreach (var follower in followersDictionary)
            {
                var followerInfo = GetUserInformation(sessionId, follower.Value.username);
                if (followerInfo.is_private)
                    continue;
                excelData.Add(followerInfo.GetExcelData());
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var excelMapper = new ExcelMapper();

            if (rdoWin.Checked)
            {
                var fileMacPath = string.Concat(desktopPath, @"\followers.xlsx");
                excelMapper.Save(fileMacPath, excelData);
            }
            else
            {
                var fileMacPath = string.Concat(desktopPath, @"/followers.xlsx");
                excelMapper.Save(fileMacPath, excelData);
            }

            MessageBox.Show("Lấy data thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
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

        private void btnCrawl_Click(object sender, EventArgs e)
        {
            CrawlData();
        }
    }
}