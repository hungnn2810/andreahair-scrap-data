using Ganss.Excel;
using Instagram.Commons.Utils;
using Instagram.Model;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Cookie = OpenQA.Selenium.Cookie;
using Match = System.Text.RegularExpressions.Match;
using Task = System.Threading.Tasks.Task;

namespace Instagram
{
    public partial class Main : Form
    {
        private Thread _myThread;

        public Main()
        {
            InitializeComponent();
        }

        #region Properties
        List<string> targetLinks = new();
        List<string> followerLinks = new();
        List<ExcelData> excelDatas = new();
        List<LoginModel> listLogin = new();
        TimeSpan timeSpan = new(0, 0, 0);
        int totalScan = 0;
        int totalHasScan = 0;
        int totalPhoneNubmer = 0;
        int totalThread = 0;
        #endregion Properties

        public async void MainServiceAsync()
        {
            try
            {
                #region GetFollower
                using (SemaphoreSlim concurrencySemaphore = new(listLogin.Count))
                {
                    List<Task> tasks = new();
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
                #endregion GetFollower


                #region GetPhoneNumber
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
                        listFollower.Add(followerLinks.GetRange(index * (i - 1), total - (index * (i - 1))));
                    }
                    else
                    {
                        listFollower.Add(followerLinks.GetRange(index * (i - 1), index));
                    }
                }

                // Ở mỗi trình duyệt, loop danh sách dữ liệu được cấp phát -> lấy data -> lưu vào 1 biến toàn cục List<Response gì đó>

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
                #endregion GetPhoneNumber

                var fileName = ExportExcel(excelDatas);

                MessageBox.Show($"File đã xuất có tên {fileName}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnStart.BeginInvoke(new Action(() => btnStart.Enabled = true), null);
                timer1.Stop();
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
            options.AddArgument("--headless");

            //Suppresses ChromeDriver's diagnostics outputs
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            // Create the ChromeDriver instance with the options

            IWebDriver driver = new ChromeDriver(service, options);
            totalThread += 1;
            lblThread.BeginInvoke(new Action(() => lblThread.Text = totalThread.ToString()), null);
            driver.Manage().Cookies.DeleteAllCookies();
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

            Thread.Sleep(2000);
            var lastChar = targetLinks[targetLinks.Length - 1].ToString();
            if (lastChar == "/")
            {
                driver.Navigate().GoToUrl($"{targetLinks}followers/");
            }
            else
            {
                driver.Navigate().GoToUrl($"{targetLinks}/followers/");
            }

            var isNeedReload = CheckExistElement(driver, ".x6s0dn4.xrvj5dj.x1iyjqo2.x5yr21d.x1swvt13.x1pi30zi.x2b8uid", 2);

            if (isNeedReload)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector('.x1i10hfl.xjqpnuy.xa49m3k.xqeqjp1.x2hbi6w.x972fbf.xcfux6l.x1qhh985.xm0m39n.xdl72j9.x2lah0s.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.x2lwn1j.xeuugli.xexx8yu.x18d9i69.x1hl2dhg.xggy1nq.x1ja2u2z.x1t137rt.x1q0g3np.x1lku1pv.x1a2a7pz.x6s0dn4.xjyslct.x1lq5wgf.xgqcy7u.x30kzoy.x9jhf4c.x1ejq31n.xd10rxx.x1sy0etr.x17r0tee.x9f619.x9bdzbf.x1ypdohk.x78zum5.x1i0vuye.x1f6kntn.xwhw2v2.xl56j7k.x17ydfre.x1n2onr6.x2b8uid.xlyipyv.x87ps6o.x14atkfc.x1d5wrs8.xn3w4p2.x5ib6vp.xc73u3c.x1tu34mt.xzloghq').click()");

                Thread.Sleep(2000);

                if (lastChar == "/")
                {
                    driver.Navigate().GoToUrl($"{targetLinks}followers/");
                }
                else
                {
                    driver.Navigate().GoToUrl($"{targetLinks}/followers/");
                }
            }

            var isCanntReload = CheckExistElement(driver, ".x6s0dn4.xrvj5dj.x1iyjqo2.x5yr21d.x1swvt13.x1pi30zi.x2b8uid", 3);

            if (isCanntReload)
            {
                totalThread -= 1;
                lblThread.BeginInvoke(new Action(() => lblThread.Text = totalThread.ToString()), null);
                File.Delete(cookiesPath);
                MessageBox.Show($"Tài khoản: {userName} tạm thời không thể sử dụng được!", "Thông báo", MessageBoxButtons.OK);
                return;
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

                    //Lấy ra số lượng hiển thị lúc chưa scroll
                    CheckExistElement(driver, ".x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd", 10);

                    var findStart = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd').length");
                    countGet = Convert.ToInt32(findStart);
                    lblHasScan.BeginInvoke(new Action(() => lblHasScan.Text = "0/" + countGet.ToString()), null);

                    while (countStart != countGet)
                    {
                        var heightBefore = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelector('._aano').scrollHeight");
                        ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelector('._aano').scrollTop = document.querySelector('._aano').scrollHeight");
                        var heightAfter = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelector('._aano').scrollHeight");

                        //So sánh xem chiều dài trước khi scroll và sau khi scroll
                        while ((Convert.ToInt32(heightAfter) - 30) <= Convert.ToInt32(heightBefore))
                        {
                            Thread.Sleep(2000);
                            heightAfter = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelector('._aano').scrollHeight");
                            if (Convert.ToInt32(heightAfter) == Convert.ToInt32(heightBefore))
                            {
                                break;
                            }
                        }
                        totalScan += (countGet - countStart);
                        countStart = countGet;

                        var find = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd').length");
                        countGet = Convert.ToInt32(find);
                        lblHasScan.BeginInvoke(new Action(() => lblHasScan.Text = "0/" + countGet.ToString()), null);
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

            totalThread -= 1;
            lblThread.BeginInvoke(new Action(() => lblThread.Text = totalThread.ToString()), null);
            driver.Quit();
            driver.Dispose();
        }

        private async void GetPhoneNumber(List<string> listFollower, string userName, string password, string cookiesPath)
        {
            //Mở trình duyệt
            //Create ChromeOptions and set any desired options
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--headless");

            //Suppresses ChromeDriver's diagnostics outputs
            ChromeDriverService service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            // Create the ChromeDriver instance with the options

            IWebDriver driver = new ChromeDriver(service, options);
            driver.Navigate().GoToUrl(@"https://www.instagram.com/");

            totalThread += 1;
            lblThread.BeginInvoke(new Action(() => lblThread.Text = totalThread.ToString()), null);
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
                totalHasScan += 1;
                lblHasScan.BeginInvoke(new Action(() => lblHasScan.Text = totalHasScan.ToString() + $@"/{totalScan}"), null);

                var phoneNumb = GetPhoneNumberInBio(driver, followerLink);
                Thread.Sleep(2000);

                ExcelData excelData = new ExcelData();
                excelData.username = followerLink.Replace("https://www.instagram.com/", "").Replace("/", "");
                excelData.phone_number = phoneNumb;
                excelData.link_ig = followerLink;

                if (!string.IsNullOrEmpty(excelData.phone_number))
                {
                    excelDatas.Add(excelData);
                }
            }

            // Ở mỗi trình duyệt, loop danh sách dữ liệu được cấp phát -> lấy data -> lưu vào 1 biến toàn cục List<Response gì đó>

            totalThread -= 1;
            lblThread.BeginInvoke(new Action(() => lblThread.Text = totalThread.ToString()), null);
            driver.Quit();
            driver.Dispose();
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
                    Thread.Sleep(1000);
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

        private string ExportExcel(List<ExcelData> excelDatas)
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

                return fileWinPath;
            }
            catch
            {
                return "";
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

            var isLoad = CheckExistElement(driver, ".x1iyjqo2.xh8yej3", 10);

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

            isLoaded = CheckExistElement(driver, ".x1iyjqo2.xh8yej3", 5);

            if (isLoaded)
            {
                SaveCookies(driver, cookiesPath);
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
            timeSpan = new();
            if (listLogin.Count == 0)
            {
                MessageBox.Show("Tài khoản nhập vào không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Create a timer and start it.
            timer1.Interval = 1000;
            timer1.Start();


            targetLinks = txtTargetLinks.Text.Split("\r\n").ToList();

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

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Tăng thời gian của TimeSpan thêm 1 giây.
            timeSpan += TimeSpan.FromSeconds(1);

            lblTimer.Text = timeSpan.ToString();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (excelDatas.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var fileName = ExportExcel(excelDatas);
            MessageBox.Show($"File đã xuất có tên {fileName}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void btnAccount_Click(object sender, EventArgs e)
        {
            fLoginAccount account = new fLoginAccount();
            account.StartPosition = FormStartPosition.CenterScreen;
            account.ShowDialog();
        }

        private void Main_Activated(object sender, EventArgs e)
        {
            listLogin = new List<LoginModel>();
            dtgvAccount.Rows.Clear();
            if (File.Exists($@"C:\Andreahair\loginAccount.txt"))
            {
                var data = File.ReadAllText($@"C:\Andreahair\loginAccount.txt");

                var account = data.Replace(" ", "").Split("\r\n");
                string cookiesPath = @"C:\Andreahair";

                foreach (var acc in account)
                {
                    if (!string.IsNullOrEmpty(acc) && acc.Contains("="))
                    {
                        LoginModel loginModel = new();
                        var index = acc.IndexOf("=");
                        var userName = acc.Substring(0, index);
                        var password = acc.Replace(userName, "").Replace("=", "");

                        loginModel.UserName = userName;
                        loginModel.Password = password;
                        loginModel.CookiesPath = $@"{cookiesPath}\{userName}.json";

                        listLogin.Add(loginModel);
                    }
                }

                foreach (var acc in listLogin)
                {
                    int myRowIndex = dtgvAccount.Rows.Add();
                    dtgvAccount.Rows[myRowIndex].Cells["cUserName"].Value = acc.UserName;
                    dtgvAccount.Rows[myRowIndex].Cells["cPassword"].Value = acc.Password;
                }
            }
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}