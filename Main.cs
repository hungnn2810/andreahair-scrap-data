using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Media.Capture;
using Ganss.Excel;
using Instagram.Commons.Utils;
using Instagram.Model;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using DocumentFormat.OpenXml.Bibliography;
using OpenQA.Selenium.Firefox;
using Org.BouncyCastle.Asn1.X509;
using OpenQA.Selenium.Support.UI;

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

        private List<TargetLinkModel> _targetLinks = new();
        private List<FollowerLink> _followerLinks = new();
        private List<FollowerLink> _linkHasGot = new();
        private List<ExcelData> _excelData = new();
        private List<LoginModel> _listLogin = new();
        private TimeSpan _timeSpan = new(0, 0, 0);
        private int _totalScan;
        private int _totalHasScan;
        private int _totalThread;
        private bool isContinue;

        #endregion Properties

        private void MainServiceAsync()
        {
            try
            {
                if (!isContinue)
                {
                    #region GetFollower

                    using (SemaphoreSlim concurrencySemaphore = new(_listLogin.Count))
                    {
                        List<Task> tasks = new();
                        foreach (var link in _targetLinks)
                        {
                            try
                            {
                                concurrencySemaphore.Wait();
                                foreach (var login in _listLogin.Where(login => !login.IsInUse))
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
                            catch
                            {
                                // ignored
                            }
                        }

                        Task.WaitAll(tasks.ToArray());
                    }

                    #endregion GetFollower
                }


                #region GetPhoneNumber

                var listFollower = new List<List<FollowerLink>>();
                var total = _followerLinks.Count;
                var numberOfAccount = _listLogin.Count;

                for (var i = 1; i <= numberOfAccount; i++)
                {
                    var index = total / numberOfAccount;
                    switch (i)
                    {
                        case 1:
                            listFollower.Add(_followerLinks.GetRange(0, index));
                            break;
                        case 2 when i == numberOfAccount:
                            listFollower.Add(_followerLinks.GetRange(index, total - index));
                            break;
                        default:
                            {
                                listFollower.Add(i == numberOfAccount ? _followerLinks.GetRange(index * (i - 1), total - index * (i - 1)) : _followerLinks.GetRange(index * (i - 1), index));

                                break;
                            }
                    }
                }

                // Ở mỗi trình duyệt, loop danh sách dữ liệu được cấp phát -> lấy data -> lưu vào 1 biến toàn cục List<Response gì đó>

                using (var concurrencySemaphore = new SemaphoreSlim(listFollower.Count))
                {
                    var tasks = new List<Task>();
                    foreach (var link in listFollower)
                    {
                        concurrencySemaphore.Wait();

                        foreach (var login in _listLogin.Where(login => !login.IsInUse))
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

                    Task.WaitAll(tasks.ToArray());
                }

                #endregion GetPhoneNumber

                var fileName = ExportExcel(_excelData);

                MessageBox.Show($@"File đã xuất có tên {fileName}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnStart.BeginInvoke(new Action(() => btnStart.Enabled = true), null);

                if(_totalScan != _totalHasScan)
                    btnContinue.BeginInvoke(new Action(() => btnContinue.Enabled = true), null);

                timer1.Stop();
            }
            catch (Exception ex)
            {
                timer1.Stop();
                MessageBox.Show(ex.Message);
                btnStart.BeginInvoke(new Action(() => btnStart.Enabled = true), null);
                btnContinue.BeginInvoke(new Action(() => btnContinue.Enabled = true), null);

            }
        }

        private static string CheckCookies(string cookiesPath)
        {
            var cookies = "";
            var isCookiesExisted = File.Exists(cookiesPath);
            const string folderPath = @"C:\Andreahair";
            // Check if the folder already exists
            if (!Directory.Exists(folderPath))
            {
                // Create the folder
                Directory.CreateDirectory(folderPath);
            }

            if (!isCookiesExisted)
            {
                File.Create(cookiesPath).Close();
            }

            cookies = File.ReadAllText(cookiesPath);
            return cookies;
        }

        private void GetFollower(TargetLinkModel targetLinks, string userName, string password, string cookiesPath)
        {
            //Mở trình duyệt
            //var options = new ChromeOptions();
            var options = new FirefoxOptions();
            options.AddArgument("--headless");

            //var service = ChromeDriverService.CreateDefaultService();
            var service = FirefoxDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            IWebDriver driver = new FirefoxDriver(service, options, TimeSpan.FromMinutes(5));
            _totalThread += 1;
            lblThread.BeginInvoke(new Action(() => lblThread.Text = _totalThread.ToString()), null);
            driver.Manage().Cookies.DeleteAllCookies();
            driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromMinutes(5);
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
            driver.Navigate().GoToUrl(targetLinks.TargetLinks);

            SaveCookies(driver, cookiesPath);

            var isCannotReload = CheckExistElement(driver, ".x6s0dn4.xrvj5dj.x1iyjqo2.x5yr21d.x1swvt13.x1pi30zi.x2b8uid", 3);

            if (isCannotReload)
            {
                _totalThread -= 1;
                lblThread.BeginInvoke(new Action(() => lblThread.Text = _totalThread.ToString()), null);
                File.Delete(cookiesPath);
                driver.Quit();
                driver.Dispose();
                MessageBox.Show($@"Tài khoản: {userName} tạm thời không thể sử dụng được!", "Thông báo", MessageBoxButtons.OK); 
                return;
            }

            int totalFollowing = 0;
            if (CheckExistElement(driver, "._ac2a", 30))
            {
                var numbOfFollowing = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll(\"._ac2a\")[2].innerText");
                totalFollowing = Convert.ToInt32(numbOfFollowing.ToString().Replace(",", "").Replace(".", ""));
            }

            int timeWait = 60000;
            if (totalFollowing < 2000)
            {
                timeWait = 20000;
            }
            else if (totalFollowing < 3000)
            {
                timeWait = 30000;
            }
            else if (totalFollowing < 4000)
            {
                timeWait = 40000;
            }
            else if (totalFollowing >= 5000 && totalFollowing <= 10000)
            {
                timeWait = 90000;
            }
            else if (totalFollowing > 10000)
            {
                timeWait = 18000000;
            }


            var excuteSrcipt = GetAllUserByScript(targetLinks.UserId, timeWait.ToString());

            var tmp = ((IJavaScriptExecutor)driver).ExecuteAsyncScript($"{excuteSrcipt}");
            var following = tmp.ToJson().ToString().Replace("[", "").Replace("]", "").Replace(@"""", "");

            var listFollowing = following.Split(",");

            var ownerName = GetNameInLink(targetLinks.TargetLinks);

            foreach (var followName in listFollowing)
            {
                try
                {
                    var href = Convert.ToString("https://www.instagram.com/" + followName);
                    FollowerLink tempLink = new()
                    {
                        ownername = ownerName,
                        followerLink = href
                    };
                    _followerLinks.Add(tempLink);
                    _totalScan += 1;
                    lblHasScan.BeginInvoke(new Action(() => lblHasScan.Text = "0/" + _totalScan.ToString()), null);
                }
                catch
                {
                    // ignored
                }
            }

            #region Scroll

            _totalThread -= 1;
            lblThread.BeginInvoke(new Action(() => lblThread.Text = _totalThread.ToString()), null);
            driver.Close();
            driver.Quit();
            driver.Dispose();

            #endregion
        }

        private void GetPhoneNumber(List<FollowerLink> listFollower, string userName, string password, string cookiesPath)
        {
            var options = new FirefoxOptions();
            options.AddArgument("--headless");

            var service = FirefoxDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            IWebDriver driver = new FirefoxDriver(service, options, TimeSpan.FromMinutes(5));
            driver.Navigate().GoToUrl(@"https://www.instagram.com/");

            _totalThread += 1;
            lblThread.BeginInvoke(new Action(() => lblThread.Text = _totalThread.ToString()), null);

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
            SaveCookies(driver, cookiesPath);


            foreach (var data in listFollower)
            {
                var phoneNumb = GetPhoneNumberInBio(driver, data.followerLink);

                var isNeedReload = CheckExistElement(driver, ".x6s0dn4.xrvj5dj.x1iyjqo2.x5yr21d.x1swvt13.x1pi30zi.x2b8uid", 0);
                if (isNeedReload)
                {
                    //((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector('.x1i10hfl.xjqpnuy.xa49m3k.xqeqjp1.x2hbi6w.x972fbf.xcfux6l.x1qhh985.xm0m39n.xdl72j9.x2lah0s.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.x2lwn1j.xeuugli.xexx8yu.x18d9i69.x1hl2dhg.xggy1nq.x1ja2u2z.x1t137rt.x1q0g3np.x1lku1pv.x1a2a7pz.x6s0dn4.xjyslct.x1lq5wgf.xgqcy7u.x30kzoy.x9jhf4c.x1ejq31n.xd10rxx.x1sy0etr.x17r0tee.x9f619.x9bdzbf.x1ypdohk.x78zum5.x1i0vuye.x1f6kntn.xwhw2v2.xl56j7k.x17ydfre.x1n2onr6.x2b8uid.xlyipyv.x87ps6o.x14atkfc.x1d5wrs8.xn3w4p2.x5ib6vp.xc73u3c.x1tu34mt.xzloghq').click()");
                    MessageBox.Show($@"Tài khoản {userName} hiện tại không thể dùng để crawl!", "Thông báo!");
                    break;
                }

                var checkLinkDirection = CheckExistElement(driver, "._aa_y._aa_z._aa_- .x3nfvp2 a", 1);
                var linkLienKet = "";
                if (checkLinkDirection)
                {
                    var href = ((IJavaScriptExecutor)driver).ExecuteScript($"return document.querySelector('._aa_y._aa_z._aa_- .x3nfvp2 a').href");
                    linkLienKet = href.ToString();
                }

                Thread.Sleep(1000);

                var excelData = new ExcelData
                {
                    ownername = data.ownername.Replace("https://www.instagram.com/", "").Replace("/", ""),
                    username = data.followerLink.Replace("https://www.instagram.com/", "").Replace("/", ""),
                    phone_number = phoneNumb,
                    link_ig = data.followerLink,
                    link_lienket = linkLienKet ?? ""
                };

                _excelData.Add(excelData);
                _linkHasGot.Add(new FollowerLink()
                {
                    followerLink = excelData.link_ig,
                    ownername = excelData.ownername,
                });

                _totalHasScan += 1;
                lblHasScan.BeginInvoke(new Action(() => lblHasScan.Text = _totalHasScan + $@"/{_totalScan}"), null);
            }

            _totalThread -= 1;
            lblThread.BeginInvoke(new Action(() => lblThread.Text = _totalThread.ToString()), null);
            driver.Close();
            driver.Quit();
            driver.Dispose();
        }

        private string GetPhoneNumberInBio(IWebDriver driver, string linkFollower)
        {
            Thread.Sleep(2000);
            driver.Navigate().GoToUrl(linkFollower);

            var phoneNumb = "";
            var isLoaded = CheckExistElement(driver, "._aarf", 5);

            if (!isLoaded)
            {
                return phoneNumb;
            }

            var textContent = ((IJavaScriptExecutor)driver).ExecuteScript($"return document.querySelector('.x7a106z.x972fbf.xcfux6l.x1qhh985.xm0m39n.x9f619.x78zum5.xdt5ytf.x2lah0s.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x1n2onr6.x11njtxf.xwonja6.x1dyjupv.x1onnzdu.xwrz0qm.xgmu61r.x1nbz2ho.xbjc6do').textContent");

            phoneNumb = !string.IsNullOrEmpty(textContent.ToString()) ? RegexPhoneNumber(textContent.ToString()) : "";


            if (!string.IsNullOrEmpty(phoneNumb)) return phoneNumb;
            var isPrivate = CheckExistElement(driver, "._aady._aa_s", 1);

            if (!isPrivate)
            {
                phoneNumb = GetPhoneNumberInPost(driver);
            }

            return phoneNumb;
        }

        private static string GetPhoneNumberInPost(IWebDriver driver)
        {
            var phoneNumb = "";

            try
            {
                var isLoaded = CheckExistElement(driver, "._aarf", 5);

                if (!isLoaded)
                {
                    return phoneNumb;
                }

                var listPhoneInPost = new List<string>();
                for (var i = 0; i < 10; i++)
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

                    //if (i >= 10 && listPhoneInPost.Count >= 1)
                    //{
                    //    break;
                    //}

                    Thread.Sleep(1500);
                }

                phoneNumb = string.Join(",", listPhoneInPost.Distinct());

                return phoneNumb;
            }
            catch
            {
                return phoneNumb;
            }
        }

        private static string ExportExcel(IEnumerable<ExcelData> excelDatas)
        {
            try
            {
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                //Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                var excelMapper = new ExcelMapper();
                var timeDone = DateTime.Now.ToString("HH.mm_dd-MM-yyyy");
                var fileWinPath = string.Concat(desktopPath, @"\following_" + timeDone + ".xlsx");
                var file = new ExcelHelper();
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
            var datetime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            if (cookies != null)
                foreach (var cookie in from item in cookies let expiry = datetime.AddSeconds(item.expiry) select new Cookie(item.name, item.value, item.domain, item.path, expiry, item.secure, item.httpOnly, item.sameSite))
                {
                    driver.Manage().Cookies.AddCookie(cookie);
                }

            Thread.Sleep(1000);
            driver.Navigate().Refresh();

            var isLoad = CheckExistElement(driver, ".xl5mz7h.xhuyl8g", 5);
            var notifyExist = CheckExistElement(driver, "._a9-v", 5);
            if (isLoad || notifyExist)
            {
                SaveCookies(driver, cookiesPath);
            }
            else
            {
                Login(driver, userName, password, cookiesPath);
            }
        }

        private static void Login(IWebDriver driver, string username, string password, string cookiesPath)
        {
            var isLoaded = CheckExistElement(driver, "._aa4b._add6._ac4d", 5);

            if (!isLoaded)
            {
                MessageBox.Show($@"Đăng nhập tài khoản {username} thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            isLoaded = CheckExistElement(driver, ".xl5mz7h.xhuyl8g", 5);
            var notifyExist = CheckExistElement(driver, "._a9-v", 3);
            if (isLoaded || notifyExist)
            {
                SaveCookies(driver, cookiesPath);
            }
            else
            {
                MessageBox.Show($@"Đăng nhập tài khoản {username} thất bại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void SaveCookies(IWebDriver driver, string cookiesPath)
        {
            Thread.Sleep(3000);
            var cookies = driver.Manage().Cookies.AllCookies;

            // Save the cookies to a file.
            var cookiesString = JsonConvert.SerializeObject(cookies);
            File.WriteAllText(cookiesPath, cookiesString);
        }

        private void btnCrawl_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            btnContinue.Enabled = false;

            RefreshDefault();

            if (_listLogin.Count == 0)
            {
                MessageBox.Show(@"Tài khoản nhập vào không hợp lệ!", @"Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnStart.Enabled = true;
                return;
            }

            if (string.IsNullOrEmpty(txtTargetLinks.Text))
            {
                MessageBox.Show(@"Chưa nhập link instagram!", @"Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnStart.Enabled = true;
                return;
            }

            var links = txtTargetLinks.Text.Replace(" ", "").Split("\r\n").ToList();

            foreach (var item in links)
            {
                if (string.IsNullOrEmpty(item) || !item.Contains("=")) continue;

                var linkModel = new TargetLinkModel();
                var index = item.IndexOf("=", StringComparison.Ordinal);
                var targetlink = item[..index];
                var userId = item.Replace(targetlink, "").Replace("=", "");

                linkModel.TargetLinks = targetlink;
                linkModel.UserId = userId;
                _targetLinks.Add(linkModel);
            }

            if (_targetLinks.Count == 0)
            {
                MessageBox.Show(@"Link nhập vào không hợp lệ!", @"Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnStart.Enabled = true;
                return;
            }

            // Create a timer and start it.
            timer1.Interval = 1000;
            timer1.Start();

            // Create a new thread object.
            _myThread = new Thread(MyThreadProc);

            // In the constructor of the thread object, specify the code that you want to run in the new thread.
            _myThread.Start();
        }

        private void RefreshDefault()
        {
            lblHasScan.Text = @"0/0";
            lblTimer.Text = @"Đang chờ";
            lblThread.Text = @"Đang chờ";
            _totalScan = 0;
            _totalHasScan = 0;
            _totalThread = 0;
            _targetLinks = new List<TargetLinkModel>();
            _followerLinks = new List<FollowerLink>();
            _excelData = new List<ExcelData>();
            _timeSpan = new TimeSpan(0, 0, 0);
        }


        private void MyThreadProc(object state)
        {
            MainServiceAsync();
        }

        private static string RegexPhoneNumber(string val)
        {
            var regexVal = val.Replace(" ", "");

            var extractPhoneNumberRegex = new Regex(@"\+?\d{1,4}?[-.\s]?\(?\d{1,3}?\)?[-.\s]?\d{1,4}[-.\s]?\d{1,4}[-.\s]?\d{1,9}");

            var regex = extractPhoneNumberRegex.Matches(regexVal)
                .Select(m => m.Value).ToList();
            
            var removeIsNotPhoneNumber = new List<string>();
            foreach (var item in regex)
            {
                if (item.Length < 9)
                {
                    removeIsNotPhoneNumber.Add(item);
                }
            }

            if(removeIsNotPhoneNumber.Count > 0)
            {
                foreach (var item in removeIsNotPhoneNumber)
                {
                    regex.Remove(item);
                }
            }

            var phoneNumber = string.Join(",", regex.Distinct());

            return phoneNumber;
        }

        private static bool CheckExistElement(IWebDriver driver, string querySelector, double timeWaitSecond = 0)
        {
            var isExist = true;
            try
            {
                var timeStart = Environment.TickCount;
                while ((string)((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('" + querySelector + "').length+''") == "0")
                {
                    if (Environment.TickCount - timeStart > timeWaitSecond * 1000)
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
            _timeSpan += TimeSpan.FromSeconds(1);

            lblTimer.Text = _timeSpan.ToString();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (_excelData.Count == 0)
            {
                MessageBox.Show(@"Không có dữ liệu để xuất file!", @"Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var fileName = ExportExcel(_excelData);
            MessageBox.Show($@"File đã xuất có tên {fileName}!", @"Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        private void btnAccount_Click(object sender, EventArgs e)
        {
            var account = new fLoginAccount();
            account.StartPosition = FormStartPosition.CenterScreen;
            account.ShowDialog();
        }

        private void Main_Activated(object sender, EventArgs e)
        {
            _listLogin = new List<LoginModel>();
            dtgvAccount.Rows.Clear();
            if (!File.Exists($@"C:\Andreahair\loginAccount.txt")) return;

            var data = File.ReadAllText($@"C:\Andreahair\loginAccount.txt");

            var account = data.Replace(" ", "").Split("\r\n");
            const string cookiesPath = @"C:\Andreahair";

            foreach (var acc in account)
            {
                if (string.IsNullOrEmpty(acc) || !acc.Contains("=")) continue;
                LoginModel loginModel = new();
                var index = acc.IndexOf("=", StringComparison.Ordinal);
                var userName = acc[..index];
                var password = acc.Replace(userName, "").Replace("=", "");

                loginModel.UserName = userName;
                loginModel.Password = password;
                loginModel.CookiesPath = $@"{cookiesPath}\{userName}.json";

                _listLogin.Add(loginModel);
            }

            foreach (var acc in _listLogin)
            {
                var myRowIndex = dtgvAccount.Rows.Add();
                dtgvAccount.Rows[myRowIndex].Cells["cUserName"].Value = acc.UserName;
                dtgvAccount.Rows[myRowIndex].Cells["cPassword"].Value = acc.Password;
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn đang thoát khỏi ứng dụng!\r\nDữ liệu hiện tại sẽ không được lưu trữ, bạn có muốn tiếp tục không.", @"Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        public string GetAllUserByScript(string userId, string timeWait)
        {
            string result = "";

            result += "options = {\r\n    userId: " + userId + ",\r\n    list: 1 \r\n};";
            result +=
                "let config = {\r\n  followers: {\r\n    hash: 'c76146de99bb02f6415203be841dd25a',\r\n    path: 'edge_followed_by'\r\n  },\r\n  following: {\r\n    hash: 'd04b0a864b4b54837c0d870b0e77e076',\r\n    path: 'edge_follow'\r\n  }\r\n};\r\n\r\nvar allUsers = [];\r\n\r\nfunction getUsernames(data) {\r\n    var userBatch = data.map(element => element.node.username);\r\n    allUsers.push(...userBatch);\r\n}\r\n\r\nasync function makeNextRequest(nextCurser, listConfig) {\r\n    var params = {\r\n        \"id\": options.userId,\r\n        \"include_reel\": true,\r\n        \"fetch_mutual\": true,\r\n        \"first\": 50\r\n    };\r\n    if (nextCurser) {\r\n        params.after = nextCurser;\r\n    }\r\n    var requestUrl = `https://www.instagram.com/graphql/query/?query_hash=` + listConfig.hash + `&variables=` + encodeURIComponent(JSON.stringify(params));\r\n\r\n    var xhr = new XMLHttpRequest();\r\n    xhr.onload = function(e) {\r\n        var res = JSON.parse(xhr.response);\r\n\r\n        var userData = res.data.user[listConfig.path].edges;\r\n        getUsernames(userData);\r\n\r\n        var curser = \"\";\r\n        try {\r\n            curser = res.data.user[listConfig.path].page_info.end_cursor;\r\n        } catch {\r\n\r\n        }\r\n        var users = [];\r\n        if (curser) {\r\n            makeNextRequest(curser, listConfig);\r\n        } else {\r\n            var printString =\"\"\r\n            allUsers.forEach(item => printString = printString + item + \"\\n\");    }\r\n    }\r\n\r\n    xhr.open(\"GET\", requestUrl);\r\n    xhr.send();\r\n}\r\n";
            result += "makeNextRequest(\"\", config.following);\r\n  \r\nconst myFunction = () => {\r\n    return new Promise((resolve, reject) => {\r\n      setTimeout(() => {\r\n        resolve(allUsers);\r\n      }, " + timeWait + ");\r\n    });\r\n  };\r\n  var callback = arguments[arguments.length - 1]; \r\n  myFunction().then((value) => {\r\n     callback(value);\r\n  });";

            return result;
        }

        public string GetNameInLink(string targetLinks)
        {
            string name = "";

            name = targetLinks.Replace("https://www.instagram.com/", "").Replace("/", "");

            return name;
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            isContinue = true;
            btnStart.Enabled = false;

            // Create a timer and start it.
            timer1.Interval = 1000;
            timer1.Start();

            foreach (var item in _linkHasGot)
            {
                _followerLinks.Remove(item);
            }

            _linkHasGot = new List<FollowerLink>();

            // Create a new thread object.
            _myThread = new Thread(MyThreadProc);

            // In the constructor of the thread object, specify the code that you want to run in the new thread.
            _myThread.Start();
        }
    }
}