using HttpClientLibrus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrusPlan.MVVM.Model
{
    public class AccountModel
    {
        public string locname { get; set; }
        public string Class { get; set; }
        public LibrusData accountData { get; set; }
        public AccountModel(string locname,string Class, LibrusData accountData)
        {
            this.locname = locname;
            this.accountData = accountData;
            this.Class = Class;
        }
    }
}
