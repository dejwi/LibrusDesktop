using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpClientLibrus
{
    public class LibrusData
    {
        public string username, password;
        public CookieContainer cookieSession;

        public LibrusData(string username, string password, CookieContainer cookieSession)
        {
            this.username = username;
            this.password = password;
            this.cookieSession = cookieSession;
        }
    }
}
