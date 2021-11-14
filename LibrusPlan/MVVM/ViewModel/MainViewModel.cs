using LibrusLib;
using LibrusLib.LessonStrucs;
using LibrusPlan.Core;
using LibrusPlan.MVVM.Model;
using LibrusPlan.MVVM.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LibrusPlan.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public static string configPath = "LB/config.json";
        public static string userDataPath = "LB/UserData.json";
        public static string dataPath = "LB/data.json";
        public static string timePeriodPath = "LB/dataPeriod.json";

        private AccountModel _acc;
        public AccountModel acc
        {
            get { return _acc; }
            set { 
                _acc = value;
                OnPropertyChanged();
            }
        }

        private  List<TimePeriod> _LimitedPeriods;
        public List<TimePeriod> LimitedPeriods
        {
            get { return _LimitedPeriods; }
            set { 
                _LimitedPeriods = value;
                OnPropertyChanged();
            }
        }

        private LibrusLib.LibrusPlan _data;
        public LibrusLib.LibrusPlan data
        {
            get { return _data; }
            set { 
                _data = value;
                OnPropertyChanged();
            }
        }

        public string LocalName { get; set; }
        public string Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public RelayCommand OpenLoginCommand { get; set; }
        public RelayCommand LoginCommand { get; set; }

        public int CurrentDay { get; set; }

        private static bool DebugMode = false;

        public LoginWindow LoginWin { get; set; }
        public Progress<int> progress { get; set; }

        private bool _autologinIsChecked;
        public bool autologinIsChecked
        {
            get { return _autologinIsChecked; }
            set
            {
                _autologinIsChecked = value;
                config.AutoLogin = value;
                Task.Run(SaveConfig);
                if (value)
                {    //checked

                    //autologin or local data
                    loadlocalIsChecked = false;
                }
                else
                {
                    //unchecked
                }
                OnPropertyChanged();
            }
        }

        private bool _loadlocalIsChecked;
        public bool loadlocalIsChecked
        {
            get { return _loadlocalIsChecked; }
            set
            {
                _loadlocalIsChecked = value;
                config.LoadLocalOnStart = value;
                Task.Run(SaveConfig);

                if (value)
                {    //checked

                    //autologin or local data
                    autologinIsChecked = false;
                }
                else
                {
                    //unchecked
                }
                OnPropertyChanged();
            }
        }

        private bool _EnglishPolishIsChecked;
        public bool EnglishPolishIsChecked
        {
            get { return _EnglishPolishIsChecked; }
            set
            {
                _EnglishPolishIsChecked = value;
                if (value)
                {
                    //checked | polish
                    config.EnglishOrPolish = "Polish";
                    App.SelectCulture("Polish");
                }
                else
                {
                    //unchecked | english
                    config.EnglishOrPolish = "English";
                    App.SelectCulture("English");
                }
                Task.Run(SaveConfig);
                OnPropertyChanged();
            }
        }

        private ConfigModel _config;

        public ConfigModel config
        {
            get { return _config; }
            set { _config = value;}
        }


        public MainViewModel()
        {
            LimitedPeriods = new List<TimePeriod>(15);
            CurrentDay = ((int)DateTime.Today.DayOfWeek);

            //for progressbar 
            progress = new Progress<int>();
            progress.ProgressChanged += ReportProgress;

            LoginWin = new LoginWindow();
            LoginWin.DataContext = this;

            OpenLoginCommand = new RelayCommand(o => LoginWin.Show());
            LoginCommand = new RelayCommand(o => LoginFromView());

            Task.Run(LoadData);

            if (DebugMode)
            {
                try
                {
                    var x = JsonConvert.DeserializeObject<SavedUserData>(File.ReadAllText("Debug/userdata.json"));
                    init(x.locname, x.Class, x.libusername, x.libpassword);
                }
                catch (Exception)
                {
                }
            }
        }

        //TODO: EnglishOrPolish
        async Task LoadData()
        {
            
            if (Directory.Exists("LB"))
            {
                if (File.Exists(configPath))
                {
                    try
                    {
                        var json = File.ReadAllText(configPath);
                        config = JsonConvert.DeserializeObject<ConfigModel>(json);
                        App.SelectCulture(config.EnglishOrPolish);
                    }
                    catch (Exception)
                    {
                        config = DefConfig();
                        File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
                    }
                }
                else
                {
                    config = DefConfig();
                    File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
                }
                //autologin
                if (config.AutoLogin && File.Exists(userDataPath))
                {
                    try
                    {
                        var json = File.ReadAllText(userDataPath);
                        var x = JsonConvert.DeserializeObject<SavedUserData> (json);
                        init(x.locname, x.Class,x.libusername, x.libpassword);
                        autologinIsChecked = true;
                    }
                    catch (Exception) {autologinIsChecked = false;}
                }
                else
                {
                    autologinIsChecked = false; //just in case
                    //local data
                    if (config.LoadLocalOnStart && File.Exists(dataPath) && File.Exists(timePeriodPath))
                    {
                        try
                        {
                            acc = new AccountModel("local data", "local");
                            Task.Run(() =>
                            {
                                var json = File.ReadAllText(dataPath);
                                data = JsonConvert.DeserializeObject<LibrusLib.LibrusPlan>(json);
                            });
                            Task.Run(() =>
                            {
                                var json = File.ReadAllText(timePeriodPath);
                                LimitedPeriods = JsonConvert.DeserializeObject<List<TimePeriod>>(json);
                            });
                        }
                        catch (Exception) { loadlocalIsChecked = false; }
                    }
                    else 
                        loadlocalIsChecked = false;
                }

                //TODO: EnglishOrPolish
            }
            else
            {
                Directory.CreateDirectory("LB");
                config = DefConfig();
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
            }

            //if(File.Exists())
        }

        //progressbar 
        private void ReportProgress(object? sender, int e)
        {
            ((MainWindow)Application.Current.MainWindow).LoadProgress.Value = e;
        }

        //from LoginWindow
        async Task LoginFromView()
        {
            try
            {
                init(LocalName, Id, Login, Password);

                //save data
                if (!Directory.Exists("LB"))
                    Directory.CreateDirectory("LB");
                var json = JsonConvert.SerializeObject(new SavedUserData
                {
                    locname = LocalName,
                    Class = Id,
                    libusername = Login,
                    libpassword = Password
                });
                File.WriteAllText(userDataPath, json);

                //just reset progressbar in case login pass was wrong
                
            }
            catch (Exception)
            {
            }
            
        }

        //contains saving data
        async void init(string locname, string Class, string libruslogin, string libruspassword)
        {
            //currently used for debug purposes
            try
            {
                acc = await Task.Run(async () => new AccountModel(locname, Class, await LibrusConnection.Connect(libruslogin, libruspassword,progress)));
                data = await Task.Run(() => LibrusLib.LibrusPlan.Retrieve(acc.accountData,progress));
                var x = new List<TimePeriod>();
                for (int i = 0; i < LimitedPeriods.Capacity; i++)
                {
                    x.Add(data.timePeriods[i]);
                }
                LimitedPeriods = x;

                //save data
                if (!Directory.Exists("LB"))
                    Directory.CreateDirectory("LB");
                File.WriteAllTextAsync(dataPath, JsonConvert.SerializeObject(data));
                File.WriteAllTextAsync(timePeriodPath, JsonConvert.SerializeObject(LimitedPeriods));

                
            }
            catch (Exception)
            {
                await Task.Delay(600);
                ((MainWindow)Application.Current.MainWindow).LoadProgress.Value = 0;
            }
            
        }

        private ConfigModel DefConfig()
        {
            return new ConfigModel { AutoLogin = false, EnglishOrPolish = "Polish", LoadLocalOnStart = false };
        }

        async Task SaveConfig()
        {
            if (Directory.Exists("LB"))
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
            else
            {
                Directory.CreateDirectory("LB");
                File.WriteAllText(configPath, JsonConvert.SerializeObject(config));
            }
        }
    }
}
