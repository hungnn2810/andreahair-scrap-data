using CrawlData.Model;
using DocumentFormat.OpenXml.Bibliography;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.DevTools;
using ScrappingData.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cookie = OpenQA.Selenium.Cookie;
using NPOI.SS.Formula.PTG;
using OpenQA.Selenium.Support.Extensions;
using NPOI.SS.Formula.Functions;
using Match = System.Text.RegularExpressions.Match;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Ganss.Excel;
using CrawlData.Commons.Utils;
using static NPOI.HSSF.Util.HSSFColor;
using System.Security.AccessControl;
using static System.Windows.Forms.Design.AxImporter;

namespace CrawlData
{
    public partial class Main : Form
    {
        private Thread _myThread;

        public Main()
        {
            InitializeComponent();
        }

        #region Properties
        List<ExcelData> exportList = new List<ExcelData>();
        string cookies = "";
        string cookiesPath = @"C:\Andreahair";
        string username = "";
        string password = "";
        List<string> targetLinks = new List<string>();
        List<string> followerLinks = new List<string>();
        List<ExcelData> excelDatas = new List<ExcelData>();
        int totalScan = 0;
        #endregion Properties

        public async Task MainServiceAsync()
        {
            var startTime = DateTime.Now;
            try
            {
                // Mở trình duyệt
                string chromeDriverPath = @"C:\chromedriver.exe";
                // Create ChromeOptions and set any desired options
                ChromeOptions options = new ChromeOptions();
                //options.AddArguments("--disable-usb-discovery");
                // Create the ChromeDriver instance with the options
                IWebDriver driver = new ChromeDriver(chromeDriverPath, options);
                driver.Navigate().GoToUrl(@"https://www.instagram.com/");

                // Check cookie -> nếu có thì load cookie cho trình duyệt (Check cookie còn sống k - vào được trang chính hay chưa)
                var cookiesExisted = CheckCookies();

                // Login -> Save cookie
                if (!cookiesExisted)
                {
                    Login(driver, username, password);
                }
                else
                {
                    LoadCookies(driver, cookies);
                }

                // Vào tất cả target lấy danh sách follower (link) -> lưu biến toàn cục: List<string> followerLinks
                foreach (var link in targetLinks)
                {
                    GetFollower(driver, link);
                }

                // Pool trình duyệt = 3, chia dữ liệu cho mỗi trình duyệt
                var quantityDivide1 = totalScan / 3;
                var quantityDivide2 = quantityDivide1 * 2;
                var list1 = followerLinks.GetRange(0, quantityDivide1);
                var list2 = followerLinks.GetRange(quantityDivide1 + 1, quantityDivide2);
                var list3 = followerLinks.GetRange(quantityDivide2 + 1, totalScan - 1);
                // Ở mỗi trình duyệt, loop danh sách dữ liệu được cấp phát -> lấy data -> lưu vào 1 biến toàn cục List<Response gì đó>

                // Tìm kiếm sđt trong bài viết
                // Create two tasks.
                Task task1 = Task.Run(() =>
                {
                    NewThread(chromeDriverPath, options, list1);
                });

                Task task2 = Task.Run(() =>
                {
                    NewThread(chromeDriverPath, options, list2);
                });

                Task task3 = Task.Run(() =>
                {
                    NewThread(chromeDriverPath, options, list3);
                });
                // Wait for the tasks to complete.
                Task taskAll = Task.WhenAll(task1, task2);

                // Do something else while the tasks are running.

                // When the tasks are complete, do this.
                await taskAll;

                ExportExcel(excelDatas);

                btnStart.Enabled = true;
            }
            catch (Exception ex)
            {

            }

            var endTime = DateTime.Now;
            var totalTime = GetTime(startTime, endTime);
            MessageBox.Show($"{totalTime}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool CheckCookies()
        {
            cookies = "";
            var isCookiesExisted = File.Exists(cookiesPath);
            string folderPath = @"C:\Andreahair";
            // Check if the folder already exists
            if (!Directory.Exists(folderPath))
            { // Create the folder
                Directory.CreateDirectory(folderPath);
            }
            if (!isCookiesExisted)
            {
                File.Create(cookiesPath).Close();
            }

            cookies = File.ReadAllText(cookiesPath);
            if (string.IsNullOrEmpty(cookies))
            {
                return false;
            }

            return true;
        }

        private void GetFollower(IWebDriver driver, string targetLinks)
        {
            var lastChar = targetLinks[targetLinks.Length - 1].ToString();
            if (lastChar == "/")
            {
                driver.Navigate().GoToUrl($"{targetLinks}followers/");
            }
            else
            {
                driver.Navigate().GoToUrl($"{targetLinks}/followers/");
            }

            var isHaveFollower = CheckExistElement(driver, "._aano", 5);

            if (isHaveFollower)
            {
                try
                {
                    var numberOfFollower = driver.FindElements(By.ClassName("_ac2a"))[1].GetAttribute("title");
                    // Scroll to the element
                    int totalFl = Convert.ToInt32(numberOfFollower.Replace(",", "").Replace(".", ""));
                    int countGet = 0;
                    int countStart = 0;
                    // Get the dispatcher for the main thread.
                    lblTotalFollower.BeginInvoke(new Action(() => lblTotalFollower.Text = totalFl.ToString()), null);

                    var startTime = DateTime.Now;

                    //Lấy ra số lượng hiển thị lúc chưa scroll
                    CheckExistElement(driver, ".x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd", 10);

                    var findStart = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd').length");
                    countStart = Convert.ToInt32(findStart);
                    lblTotalScan.BeginInvoke(new Action(() => lblTotalScan.Text = countGet.ToString()), null);

                    while (countStart != countGet)
                    {
                        var heightBefore = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelector('._aano').scrollHeight");
                        ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelector('._aano').scrollTop = document.querySelector('._aano').scrollHeight");
                        var heightAfter = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelector('._aano').scrollHeight");

                        //So sánh xem chiều dài trước khi scroll và sau khi scroll
                        while ((Convert.ToInt32(heightBefore) + 20) > Convert.ToInt32(heightAfter))
                        {
                            Thread.Sleep(2000);
                            heightAfter = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelector('._aano').scrollHeight");
                            if ((Convert.ToInt32(heightBefore) + 20) == (Convert.ToInt32(heightAfter) + 20))
                            {
                                break;
                            }
                        }
                        countStart = countGet;

                        var find = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd').length");
                        countGet = Convert.ToInt32(find);
                        totalScan += countGet;
                        lblTotalScan.BeginInvoke(new Action(() => lblTotalScan.Text = countGet.ToString()), null);
                    }

                    //var links = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd')");
                    for (int i = 0; i < countGet; i++)
                    {
                        try
                        {
                            var links = ((IJavaScriptExecutor)driver).ExecuteScript($"return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd')[{i}].attributes.href.value");
                            var href = Convert.ToString("https://www.instagram.com" + links.ToString());
                            followerLinks.Add(href);
                        }
                        catch { }
                    }

                    var endTime = DateTime.Now;
                    var total = GetTime(startTime, endTime);
                    lblTimer.BeginInvoke(new Action(() => lblTimer.Text = total.ToString()), null);
                    //MessageBox.Show($"{total}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {

                }
            }
        }
        private void NewThread(string chromeDriverPath, ChromeOptions options, List<string> listDivide)
        {
            IWebDriver driver = new ChromeDriver(chromeDriverPath, options);
            List<ExcelData> dataTemp = new List<ExcelData>();
            driver.Navigate().GoToUrl(@"https://www.instagram.com/");

            // Check cookie -> nếu có thì load cookie cho trình duyệt (Check cookie còn sống k - vào được trang chính hay chưa)
            LoadCookies(driver, cookies);

            foreach (var followerLink in listDivide)
            {
                var phoneNumb = GetPhoneNumberInBio(driver, followerLink);
                Thread.Sleep(500);
                var isPrivate = CheckExistElement(driver, "._aady._aa_s", 1);

                ExcelData excelData = new ExcelData();
                excelData.username = followerLink.Replace("https://www.instagram.com/", "").Replace("/", "");
                excelData.phone_number = phoneNumb;
                excelData.link_ig = followerLink;
                excelData.isPrivate = isPrivate;

                dataTemp.Add(excelData);
            }

            dataTemp.RemoveAll(x => x.isPrivate);
            // Ở mỗi trình duyệt, loop danh sách dữ liệu được cấp phát -> lấy data -> lưu vào 1 biến toàn cục List<Response gì đó>

            // Tìm kiếm sđt trong bài viết
            foreach (var data in dataTemp.Where(x => string.IsNullOrEmpty(x.phone_number)))
            {
                data.phone_number = GetPhoneNumberInPost(driver, data.link_ig);
            }

            excelDatas.AddRange(dataTemp);
        }

        private string GetPhoneNumberInBio(IWebDriver driver, string linkFollower)
        {
            var phoneNumb = "";
            driver.Navigate().GoToUrl(linkFollower);

            var isLoaded = CheckExistElement(driver, ".x78zum5.x1q0g3np.xieb3on", 10);

            if (!isLoaded)
            {
                return phoneNumb; 
            }

            var textContent = ((IJavaScriptExecutor)driver).ExecuteScript($"return document.querySelector('.x7a106z.x972fbf.xcfux6l.x1qhh985.xm0m39n.x9f619.x78zum5.xdt5ytf.x2lah0s.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x1n2onr6.x11njtxf.xwonja6.x1dyjupv.x1onnzdu.xwrz0qm.xgmu61r.x1nbz2ho.xbjc6do').textContent");
            
            phoneNumb = !string.IsNullOrEmpty(textContent.ToString()) ? RegexPhoneNumber(textContent.ToString()) : "";

            return phoneNumb;
        }

        private string GetPhoneNumberInPost(IWebDriver driver, string linkFollower)
        {
            var phoneNumb = "";

            try
            {
                driver.Navigate().GoToUrl(linkFollower);

                var isLoaded = CheckExistElement(driver, ".x78zum5.x1q0g3np.xieb3on", 10);

                if (!isLoaded)
                {
                    return phoneNumb;
                }
                List<string> listPhoneInPost = new List<string>();
                for (int i = 0; i < 10; i++)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript($"document.querySelectorAll('._aagu')[{i}].click()");

                    isLoaded = CheckExistElement(driver, "._ae1k._ae2q._ae2r", 2);

                    if (isLoaded)
                    {
                        var hasComment = CheckExistElement(driver, "._a9zm", 1);

                        if (hasComment)
                        {
                            var textContent = ((IJavaScriptExecutor)driver).ExecuteScript($"return document.querySelector('._aacl._aaco._aacu._aacx._aad7._aade').textContent");

                            var tmpNumb = RegexPhoneNumber(textContent.ToString());
                            if (!string.IsNullOrEmpty(tmpNumb))
                            {
                                listPhoneInPost.Add(tmpNumb);
                            }
                        }
                    }
                }

                phoneNumb = string.Join(",", listPhoneInPost.Distinct());

                return phoneNumb;
            }
            catch
            {
                return phoneNumb;
            }
        }

        private void ExportExcel(List<ExcelData> excelDatas)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                //Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var excelMapper = new ExcelMapper();
                var timeDone = DateTime.Now.ToString("HH.mm_dd-MM-yyyy");
                var fileWinPath = string.Concat(desktopPath, @"/followers_" + timeDone + ".xlsx");
                ExcelHelper file = new ExcelHelper();
                // Create a new workbook with a single sheet
                file.NewFile();
                // Add a new sheet to the workbook
                file.NewSheet();
                // Saving the file in a speicifed path
                file.SaveAs(fileWinPath);
                // Closing the file
                file.Close();

                excelMapper.Save(fileWinPath, excelDatas);
            }
            catch
            {

            }
        }

        private void LoadCookies(IWebDriver driver, string cookiesString)
        {
            var cookies = JsonConvert.DeserializeObject<List<CookiesModel>>(cookiesString);

            foreach (var item in cookies)
            {
                var expiry = DateTimeOffset.FromUnixTimeSeconds(item.expiry);

                Cookie cookie = new Cookie(item.name, item.value, item.domain, item.path, expiry.DateTime, item.secure, item.httpOnly, item.sameSite);

                driver.Manage().Cookies.AddCookie(cookie);
            }

            driver.Navigate().Refresh();

            var isLoad = CheckExistElement(driver, "._aag2", 10);

            if (!isLoad)
            {
                Login(driver, username, password);
            }
            else
            {
                SaveCookies(driver, cookiesPath);
            }
        }

        private void Login(IWebDriver driver, string username, string password)
        {
            var isLoaded = CheckExistElement(driver, "._aa4b._add6._ac4d", 5);

            if (!isLoaded)
            {
                MessageBox.Show("Đăng nhập thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var findUserName = driver.FindElement(By.Name("username"));
            findUserName.SendKeys(username);

            var findPassword = driver.FindElement(By.Name("password"));
            findPassword.SendKeys(password);

            Thread.Sleep(2000);

            ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector('._acan._acap._acas._aj1-').click()");

            // Get the response code.
            Thread.Sleep(2000);
            driver.Navigate().GoToUrl(@"https://www.instagram.com/");

            var isError = CheckExistElement(driver, "._ab2z", 2);

            if (!isError)
            {
                SaveCookies(driver, cookiesPath);
            }
            else
            {
                MessageBox.Show("Đăng nhập thất bại!\r\nSai tài khoản hoặc mật khẩu! ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void SaveCookies(IWebDriver driver, string cookiesPath)
        {
            var cookies = driver.Manage().Cookies.AllCookies;

            // Save the cookies to a file.
            string cookiesString = JsonConvert.SerializeObject(cookies);
            File.WriteAllText(cookiesPath, cookiesString);
        }

        private void btnCrawl_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            username = txtUsername.Text;
            password = txtPassword.Text;
            targetLinks = txtTargetLinks.Text.Split("\r\n").ToList();

            var namePath = CheckNamePath(username);
            if (!cookiesPath.Contains(namePath))
            {
                cookiesPath = $@"{cookiesPath}\{namePath}.json";
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Chưa nhập tài khoản hoặc mật khẩu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (targetLinks.Count < 0)
            {
                MessageBox.Show("Chưa nhập link tài khoản cần lấy dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create a new thread object.
            _myThread = new Thread(new ParameterizedThreadStart(MyThreadProc));

            // In the constructor of the thread object, specify the code that you want to run in the new thread.
            _myThread.Start();
        }

        private void MyThreadProc(object state)
        {
            MainServiceAsync();
        }

        public static string RegexPhoneNumber(string val)
        {
            List<string> regex = new List<string>();
            
            var regexVal = val.Replace(" ", "");

            Regex extractPhoneNumberRegex = new Regex("\\+?(\\s*\\d+)*[0-9][0-9]{7,14}");

            regex = extractPhoneNumberRegex.Matches(regexVal)
                            .Cast<Match>()
                            .Select(m => m.Value).
                            ToList();

            var phoneNumber = string.Join(",", regex.Distinct());

            return phoneNumber;
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

        private string CheckNamePath(string namePath)
        {
            namePath = namePath.Replace(".", "").Replace("/", "").Replace(@"\", "").Replace(@"|", "").Replace(@":", "").Replace(@"<", "").Replace(@">", "").Replace(@"*", "").Replace(@"?", "").Replace(@"""", "");
            return namePath;
        }

        public bool CheckExistElement(IWebDriver driver, string querySelector, double timeWait_Second = 0)
        {
            bool isExist = true;
            try
            {
                int timeStart = Environment.TickCount;
                while ((string)((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('" + querySelector + "').length+''") == "0")
                {
                    if (Environment.TickCount - timeStart > timeWait_Second * 1000)
                    {
                        isExist = false;
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch
            {
                return false;
            }
            return isExist;
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = true;
            //_myThread.Abort();
        }
    }
}