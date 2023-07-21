using CrawlData.Commons.Utils;
using CrawlData.Model;
using Ganss.Excel;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ScrappingData.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using Cookie = OpenQA.Selenium.Cookie;
using Match = System.Text.RegularExpressions.Match;
using Task = System.Threading.Tasks.Task;

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
        List<string> targetLinks = new List<string>();
        List<string> followerLinks = new List<string>();
        List<ExcelData> excelDatas = new List<ExcelData>();
        List<LoginModel> listLogin = new List<LoginModel>();
        TimeSpan timeSpan = new TimeSpan(0, 0, 0);
        int totalScan = 0;
        int totalCanScan = 0;
        #endregion Properties

        public async void MainServiceAsync()
        {
            var startTime = DateTime.Now;
            try
            {
                using (SemaphoreSlim concurrencySemaphore = new SemaphoreSlim(listLogin.Count))
                {
                    List<Task> tasks = new List<Task>();
                    foreach (var link in targetLinks)
                    {
                        concurrencySemaphore.Wait();

                        foreach (var login in listLogin)
                        {
                            if (!login.IsInUse)
                            {
                                login.IsInUse = true;

                                var t = Task.Factory.StartNew(() =>
                                {
                                    try
                                    {
                                        GetFollower(link, login.UserName, login.Password, login.CookiesPath);
                                    }
                                    finally
                                    {
                                        login.IsInUse = false;
                                        concurrencySemaphore.Release();
                                    }
                                });

                                tasks.Add(t);

                                break;
                            }
                        }
                    }

                    Task.WaitAll(tasks.ToArray());
                }

                var listFollower = new List<List<string>>();
                var total = followerLinks.Count;
                var numberOfAccount = listLogin.Count;

                for (int i = 1; i <= numberOfAccount; i++)
                {
                    var index = total / numberOfAccount;
                    if (i == 1)
                    {
                        listFollower.Add(followerLinks.GetRange(0, index));
                    }
                    else if (i == 2 && i == numberOfAccount)
                    {
                        listFollower.Add(followerLinks.GetRange(index, total - index));
                    }
                    else if (i == numberOfAccount)
                    {
                        listFollower.Add(followerLinks.GetRange(index * numberOfAccount, total - (index * numberOfAccount)));
                    }
                    else
                    {
                        listFollower.Add(followerLinks.GetRange(index * i, index));
                    }
                }

                //// Ở mỗi trình duyệt, loop danh sách dữ liệu được cấp phát -> lấy data -> lưu vào 1 biến toàn cục List<Response gì đó>

                using (SemaphoreSlim concurrencySemaphore = new SemaphoreSlim(listFollower.Count))
                {
                    List<Task> tasks = new List<Task>();
                    foreach (var link in listFollower)
                    {
                        concurrencySemaphore.Wait();

                        foreach (var login in listLogin)
                        {
                            if (!login.IsInUse)
                            {
                                login.IsInUse = true;

                                var t = Task.Factory.StartNew(() =>
                                {
                                    try
                                    {
                                        GetPhoneNumber(link, login.UserName, login.Password, login.CookiesPath);
                                    }
                                    finally
                                    {
                                        login.IsInUse = false;
                                        concurrencySemaphore.Release();
                                    }
                                });

                                tasks.Add(t);

                                break;
                            }
                        }
                    }

                    Task.WaitAll(tasks.ToArray());
                }

                ExportExcel(excelDatas);

                var endTime = DateTime.Now;
                var totalTime = GetTime(startTime, endTime);
                MessageBox.Show($"{totalTime}", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnStart.BeginInvoke(new Action(() => btnStart.Enabled = true), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lấy thông tin thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string CheckCookies(string cookiesPath)
        {
            var cookies = "";
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
                return cookies;
            }

            return cookies;
        }

        private void GetFollower(string targetLinks, string userName, string password, string cookiesPath)
        {
            //Mở trình duyệt
            //Create ChromeOptions and set any desired options
            ChromeOptions options = new ChromeOptions();
            //options.AddArguments("--disable-usb-discovery");
            // Create the ChromeDriver instance with the options
            IWebDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(@"https://www.instagram.com/");

            // Check cookie -> nếu có thì load cookie cho trình duyệt (Check cookie còn sống k - vào được trang chính hay chưa)
            var cookiesExisted = CheckCookies(cookiesPath);

            // Login -> Save cookie
            if (string.IsNullOrEmpty(cookiesExisted))
            {
                Login(driver, userName, password, cookiesPath);
            }
            else
            {
                LoadCookies(driver, cookiesExisted, userName, password, cookiesPath);
            }

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
                    totalCanScan += totalFl;
                    int countGet = 0;
                    int countStart = 0;
                    // Get the dispatcher for the main thread.
                    lblTotalFollower.BeginInvoke(new Action(() => lblTotalFollower.Text = totalCanScan.ToString()), null);

                    //Lấy ra số lượng hiển thị lúc chưa scroll
                    CheckExistElement(driver, ".x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd", 10);

                    var findStart = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd').length");
                    countGet = Convert.ToInt32(findStart);
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
                        totalScan += (countGet - countStart);
                        countStart = countGet;

                        var find = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd').length");
                        countGet = Convert.ToInt32(find);
                        lblTotalScan.BeginInvoke(new Action(() => lblTotalScan.Text = totalScan.ToString()), null);
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
                }
                catch
                {

                }
            }

            driver.Quit();
        }

        private async void GetPhoneNumber(List<string> listFollower, string userName, string password, string cookiesPath)
        {
            List<ExcelData> dataTemp = new List<ExcelData>();
            //Mở trình duyệt
            //Create ChromeOptions and set any desired options
            ChromeOptions options = new ChromeOptions();
            //options.AddArguments("--disable-usb-discovery");
            // Create the ChromeDriver instance with the options
            IWebDriver driver = new ChromeDriver(options);
            driver.Navigate().GoToUrl(@"https://www.instagram.com/");

            // Check cookie -> nếu có thì load cookie cho trình duyệt (Check cookie còn sống k - vào được trang chính hay chưa)
            var cookiesExisted = CheckCookies(cookiesPath);

            // Login -> Save cookie
            if (string.IsNullOrEmpty(cookiesExisted))
            {
                Login(driver, userName, password, cookiesPath);
            }
            else
            {
                LoadCookies(driver, cookiesExisted, userName, password, cookiesPath);
            }
            // Check cookie -> nếu có thì load cookie cho trình duyệt (Check cookie còn sống k - vào được trang chính hay chưa)
            LoadCookies(driver, cookiesExisted, userName, password, cookiesPath);

            foreach (var followerLink in listFollower)
            {
                var phoneNumb = GetPhoneNumberInBio(driver, followerLink);
                Thread.Sleep(2000);

                ExcelData excelData = new ExcelData();
                excelData.username = followerLink.Replace("https://www.instagram.com/", "").Replace("/", "");
                excelData.phone_number = phoneNumb;
                excelData.link_ig = followerLink;

                dataTemp.Add(excelData);
            }

            dataTemp.RemoveAll(x => string.IsNullOrEmpty(x.phone_number));
            // Ở mỗi trình duyệt, loop danh sách dữ liệu được cấp phát -> lấy data -> lưu vào 1 biến toàn cục List<Response gì đó>

            excelDatas.AddRange(dataTemp);

            driver.Quit();
        }

        private string GetPhoneNumberInBio(IWebDriver driver, string linkFollower)
        {
            driver.Navigate().GoToUrl(linkFollower);

            var phoneNumb = "";
            var isLoaded = CheckExistElement(driver, ".x78zum5.x1q0g3np.xieb3on", 10);

            if (!isLoaded)
            {
                return phoneNumb;
            }

            var textContent = ((IJavaScriptExecutor)driver).ExecuteScript($"return document.querySelector('.x7a106z.x972fbf.xcfux6l.x1qhh985.xm0m39n.x9f619.x78zum5.xdt5ytf.x2lah0s.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x1n2onr6.x11njtxf.xwonja6.x1dyjupv.x1onnzdu.xwrz0qm.xgmu61r.x1nbz2ho.xbjc6do').textContent");

            phoneNumb = !string.IsNullOrEmpty(textContent.ToString()) ? RegexPhoneNumber(textContent.ToString()) : "";


            if (string.IsNullOrEmpty(phoneNumb))
            {
                var isPrivate = CheckExistElement(driver, "._aady._aa_s", 1);

                if (!isPrivate)
                {
                    phoneNumb = GetPhoneNumberInPost(driver);
                }
            }

            return phoneNumb;
        }

        private string GetPhoneNumberInPost(IWebDriver driver)
        {
            var phoneNumb = "";

            try
            {
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

        private void LoadCookies(IWebDriver driver, string cookiesString, string userName, string password, string cookiesPath)
        {
            var cookies = JsonConvert.DeserializeObject<List<CookiesModel>>(cookiesString);

            foreach (var item in cookies)
            {
                var expiry = DateTimeOffset.FromUnixTimeSeconds(item.expiry);

                Cookie cookie = new Cookie(item.name, item.value, item.domain, item.path, expiry.DateTime, item.secure, item.httpOnly, item.sameSite);

                driver.Manage().Cookies.AddCookie(cookie);
            }

            driver.Navigate().Refresh();

            var isLoad = CheckExistElement(driver, ".xvb8j5.x1vjfegm", 10);

            if (!isLoad)
            {
                Login(driver, userName, password, cookiesPath);
            }
            else
            {
                SaveCookies(driver, cookiesPath);
            }
        }

        private void Login(IWebDriver driver, string username, string password, string cookiesPath)
        {
            var isLoaded = CheckExistElement(driver, "._aa4b._add6._ac4d", 5);

            if (!isLoaded)
            {
                MessageBox.Show("Đăng nhập thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var findUserName = driver.FindElement(By.Name("username"));
            findUserName.SendKeys(username);

            Thread.Sleep(2000);

            var findPassword = driver.FindElement(By.Name("password"));
            findPassword.SendKeys(password);

            Thread.Sleep(2000);

            ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector('._acan._acap._acas._aj1-').click()");

            // Get the response code.
            Thread.Sleep(2000);

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
            var username1 = txtUsername1.Text;
            var username2 = txtUsername2.Text;
            var username3 = txtUsername3.Text;
            var password1 = txtPassword1.Text;
            var password2 = txtPassword2.Text;
            var password3 = txtPassword3.Text;
            listLogin = new List<LoginModel>();
            string cookiesPath = @"C:\Andreahair";

            // Create a timer and start it.
            timer1.Interval = 1000;
            timer1.Start();

            if (!string.IsNullOrEmpty(username1))
            {
                LoginModel login1 = new LoginModel();
                login1.UserName = username1;
                login1.Password = password1;
                login1.CookiesPath = $@"{cookiesPath}\{username1}.json";

                listLogin.Add(login1);
            }
            if (!string.IsNullOrEmpty(username2))
            {
                LoginModel login2 = new LoginModel();
                login2.UserName = username2;
                login2.Password = password2;
                login2.CookiesPath = $@"{cookiesPath}\{username2}.json";

                listLogin.Add(login2);
            }
            if (!string.IsNullOrEmpty(username3))
            {
                LoginModel login3 = new LoginModel();
                login3.UserName = username3;
                login3.Password = password3;
                login3.CookiesPath = $@"{cookiesPath}\{username3}.json";

                listLogin.Add(login3);
            }

            targetLinks = txtTargetLinks.Text.Split("\r\n").ToList();

            var namePath = CheckNamePath(username1);
            if (!cookiesPath.Contains(namePath))
            {
                cookiesPath = $@"{cookiesPath}\{namePath}.json";
            }

            if (string.IsNullOrEmpty(username1) || string.IsNullOrEmpty(password1))
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
            _myThread.

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Tăng thời gian của TimeSpan thêm 1 giây.
            timeSpan += TimeSpan.FromSeconds(1);

            lblTimer.Text = timeSpan.ToString();
        }
    }
}