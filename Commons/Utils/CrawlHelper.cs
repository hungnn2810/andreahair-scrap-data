using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace Instagram.Commons.Utils
{
    public class CrawlHelper
    {
        public bool CheckExistElement(ChromeDriver driver, string querySelector, double timeWait_Second = 0)
        {
            var isExist = true;
            try
            {
                var timeStart = Environment.TickCount;
                while ((string)driver.ExecuteScript("return document.querySelectorAll('" + querySelector + "').length+''") == "0")
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
    }
}
