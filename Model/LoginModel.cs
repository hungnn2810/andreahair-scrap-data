using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instagram.Model
{
    public class LoginModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string CookiesPath { get; set; }

        public bool IsInUse { get; set; }
    }
}
