using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ganss.Excel;
using Instagram.Commons.Utils;
using Instagram.Model;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

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

        private List<string> _targetLinks = new();
        private List<FollowerLink> _followerLinks = new();
        private List<ExcelData> _excelData = new();
        private List<LoginModel> _listLogin = new();
        private TimeSpan _timeSpan = new(0, 0, 0);
        private int _totalScan;
        private int _totalHasScan;
        private int _totalThread;

        private const string binaryPath = "chromedriver.exe";

        #endregion Properties

        private void MainServiceAsync()
        {
            try
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
                timer1.Stop();
            }
            catch (Exception ex)
            {
                timer1.Stop();
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

        private void GetFollower(string targetLinks, string userName, string password, string cookiesPath)
        {
            //Mở trình duyệt
            var options = new ChromeOptions(
            );
            //options.AddArgument("--headless");

            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            IWebDriver driver = new ChromeDriver(service, options);
            _totalThread += 1;
            lblThread.BeginInvoke(new Action(() => lblThread.Text = _totalThread.ToString()), null);
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
            var lastChar = targetLinks[^1].ToString();
            driver.Navigate().GoToUrl(lastChar == "/" ? $"{targetLinks}followers/" : $"{targetLinks}/followers/");

            SaveCookies(driver, cookiesPath);
            var isNeedReload = CheckExistElement(driver, ".x6s0dn4.xrvj5dj.x1iyjqo2.x5yr21d.x1swvt13.x1pi30zi.x2b8uid", 2);

            if (isNeedReload)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("document.querySelector('.x1i10hfl.xjqpnuy.xa49m3k.xqeqjp1.x2hbi6w.x972fbf.xcfux6l.x1qhh985.xm0m39n.xdl72j9.x2lah0s.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.x2lwn1j.xeuugli.xexx8yu.x18d9i69.x1hl2dhg.xggy1nq.x1ja2u2z.x1t137rt.x1q0g3np.x1lku1pv.x1a2a7pz.x6s0dn4.xjyslct.x1lq5wgf.xgqcy7u.x30kzoy.x9jhf4c.x1ejq31n.xd10rxx.x1sy0etr.x17r0tee.x9f619.x9bdzbf.x1ypdohk.x78zum5.x1i0vuye.x1f6kntn.xwhw2v2.xl56j7k.x17ydfre.x1n2onr6.x2b8uid.xlyipyv.x87ps6o.x14atkfc.x1d5wrs8.xn3w4p2.x5ib6vp.xc73u3c.x1tu34mt.xzloghq').click()");

                Thread.Sleep(2000);

                driver.Navigate().GoToUrl(lastChar == "/" ? $"{targetLinks}followers/" : $"{targetLinks}/followers/");
            }

            var isCannotReload = CheckExistElement(driver, ".x6s0dn4.xrvj5dj.x1iyjqo2.x5yr21d.x1swvt13.x1pi30zi.x2b8uid", 3);

            if (isCannotReload)
            {
                _totalThread -= 1;
                lblThread.BeginInvoke(new Action(() => lblThread.Text = _totalThread.ToString()), null);
                File.Delete(cookiesPath);
                MessageBox.Show($@"Tài khoản: {userName} tạm thời không thể sử dụng được!", "Thông báo", MessageBoxButtons.OK);
            }

            var isHaveFollower = CheckExistElement(driver, "._aano", 5);

            #region Scroll

            if (isHaveFollower)
            {
                try
                {
                    var numberOfFollower = driver.FindElements(By.ClassName("_ac2a"))[1].GetAttribute("title");
                    // Scroll to the element
                    var totalFl = Convert.ToInt32(numberOfFollower.Replace(",", "").Replace(".", ""));
                    var countGet = 0;
                    var countStart = 0;
                    // Get the dispatcher for the main thread.

                    //Lấy ra số lượng hiển thị lúc chưa scroll
                    CheckExistElement(driver, ".x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd", 10);

                    var findStart = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd').length");
                    countGet = Convert.ToInt32(findStart);
                    var get = countGet;
                    lblHasScan.BeginInvoke(new Action(() => lblHasScan.Text = "0/" + get), null);

                    while (countStart != countGet)
                    {
                        ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelector('._aano').scrollTop = document.querySelector('._aano').scrollHeight");
                        Thread.Sleep(2000);
                        //Check is loading
                        if (CheckExistElement(driver, ".x9f619.xjbqb8w.x78zum5.x168nmei.x13lgxp2.x5pf9jr.xo71vjh.xw7yly9.x1uhb9sk.x1plvlek.xryxfnj.x1c4vz4f.x2lah0s.xdt5ytf.xqjyukv.x6s0dn4.x1oa3qoh.x1nhvcw1", 1))
                        {
                            Thread.Sleep(3000);
                        }

                        _totalScan += (countGet - countStart);
                        lblHasScan.BeginInvoke(new Action(() => lblHasScan.Text = "0/" + _totalScan.ToString()), null);
                        countStart = countGet;

                        var find = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd').length");
                        countGet = Convert.ToInt32(find);
                        while (countStart == countGet)
                        {
                            // Is Loading
                            if (CheckExistElement(driver, ".x9f619.xjbqb8w.x78zum5.x168nmei.x13lgxp2.x5pf9jr.xo71vjh.xw7yly9.x1uhb9sk.x1plvlek.xryxfnj.x1c4vz4f.x2lah0s.xdt5ytf.xqjyukv.x6s0dn4.x1oa3qoh.x1nhvcw1", 1)) continue;

                            ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelector('._aano').scrollTop = document.querySelector('._aano').scrollHeight");
                            if (CheckExistElement(driver, ".x9f619.xjbqb8w.x78zum5.x168nmei.x13lgxp2.x5pf9jr.xo71vjh.xw7yly9.x1uhb9sk.x1plvlek.xryxfnj.x1c4vz4f.x2lah0s.xdt5ytf.xqjyukv.x6s0dn4.x1oa3qoh.x1nhvcw1", 1))
                            {
                                Thread.Sleep(2000);
                            }
                            else
                            {
                                var anotherfind = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd').length");
                                countGet = Convert.ToInt32(anotherfind);
                                if (countGet == countStart)
                                {
                                    break;
                                }
                            }
                        }
                    }


                    //var links = ((IJavaScriptExecutor)driver).ExecuteScript("return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd')");
                    var ownerName = targetLinks.Replace("https://www.instagram.com/", "").Replace("/", "");

                    for (var i = 0; i < countGet; i++)
                    {
                        try
                        {
                            var links = ((IJavaScriptExecutor)driver).ExecuteScript($"return document.querySelectorAll('.x1i10hfl.xjbqb8w.x6umtig.x1b1mbwd.xaqea5y.xav7gou.x9f619.x1ypdohk.xt0psk2.xe8uvvx.xdj266r.x11i5rnm.xat24cr.x1mh8g0r.xexx8yu.x4uap5.x18d9i69.xkhd6sd.x16tdsg8.x1hl2dhg.xggy1nq.x1a2a7pz.notranslate._a6hd')[{i}].attributes.href.value");
                            var href = Convert.ToString("https://www.instagram.com" + links);
                            FollowerLink tempLink = new()
                            {
                                ownername = ownerName,
                                followerLink = href
                            };
                            _followerLinks.Add(tempLink);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                catch
                {
                    _totalThread -= 1;
                    lblThread.BeginInvoke(new Action(() => lblThread.Text = _totalThread.ToString()), null);
                    driver.Close();
                    driver.Quit();
                    driver.Dispose();
                }
            }

            _totalThread -= 1;
            lblThread.BeginInvoke(new Action(() => lblThread.Text = _totalThread.ToString()), null);
            driver.Close();
            driver.Quit();
            driver.Dispose();

            #endregion
        }

        private void GetPhoneNumber(List<FollowerLink> listFollower, string userName, string password, string cookiesPath)
        {
            var options = new ChromeOptions();
          //  options.AddArguments("--headless");

            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            IWebDriver driver = new ChromeDriver(service, options);
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

            foreach (var data in listFollower)
            {
                var phoneNumb = GetPhoneNumberInBio(driver, data.followerLink);
                Thread.Sleep(2000);

                var excelData = new ExcelData
                {
                    ownername = data.ownername.Replace("https://www.instagram.com/", "").Replace("/", ""),
                    username = data.followerLink.Replace("https://www.instagram.com/", "").Replace("/", ""),
                    phone_number = phoneNumb,
                    link_ig = data.followerLink
                };

                _excelData.Add(excelData);

                _totalHasScan += 1;
                lblHasScan.BeginInvoke(new Action(() => lblHasScan.Text = _totalHasScan + $@"/{_totalScan}"), null);
            }

            // Ở mỗi trình duyệt, loop danh sách dữ liệu được cấp phát -> lấy data -> lưu vào 1 biến toàn cục List<Response gì đó>

            _totalThread -= 1;
            lblThread.BeginInvoke(new Action(() => lblThread.Text = _totalThread.ToString()), null);
            driver.Close();
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
                var isLoaded = CheckExistElement(driver, ".x78zum5.x1q0g3np.xieb3on", 10);

                if (!isLoaded)
                {
                    return phoneNumb;
                }

                var listPhoneInPost = new List<string>();
                for (var i = 0; i < 30; i++)
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

                    if (i >= 10 && listPhoneInPost.Count >= 1)
                    {
                        break;
                    }

                    Thread.Sleep(1000);
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
                var fileWinPath = string.Concat(desktopPath, @"\followers_" + timeDone + ".xlsx");
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
            MessageBox.Show(cookiesString);
            var cookies = JsonConvert.DeserializeObject<List<CookiesModel>>(cookiesString);

            if (cookies != null)
                foreach (var cookie in from item in cookies let expiry = DateTimeOffset.FromUnixTimeSeconds(item.expiry) select new Cookie(item.name, item.value, item.domain, item.path, expiry.DateTime, item.secure, item.httpOnly, item.sameSite))
                {
                    driver.Manage().Cookies.AddCookie(cookie);
                }

            Thread.Sleep(1000);
            driver.Navigate().Refresh();

            var isLoad = CheckExistElement(driver, ".xl5mz7h.xhuyl8g", 10);
            var notifyExist = CheckExistElement(driver, "._a9-v", 3);
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

            // Create a timer and start it.
            timer1.Interval = 1000;
            timer1.Start();


            _targetLinks = txtTargetLinks.Text.Split("\r\n").ToList();

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
            _targetLinks = new List<string>();
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

            var extractPhoneNumberRegex = new Regex("\\+?(\\s*\\d+)*[0-9][0-9]{7,14}");

            var regex = extractPhoneNumberRegex.Matches(regexVal)
                .Select(m => m.Value).ToList();

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
            if (MessageBox.Show(@"Bạn đang thoát khỏi ứng dụng!\r\nDữ liệu hiện tại sẽ không được lưu trữ, bạn có muốn tiếp tục không.", @"Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}