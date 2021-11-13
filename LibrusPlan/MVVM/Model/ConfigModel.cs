using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrusPlan.MVVM.Model
{
    public class ConfigModel
    {
        public enum LangFilter {English,Polish}
        public Enum EnglishOrPolish { get; set; }
        public bool LoadLocalOnStart { get; set; }
        public bool AutoLogin { get; set; }
    }
}
