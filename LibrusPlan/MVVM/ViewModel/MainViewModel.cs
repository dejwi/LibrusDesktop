using HttpClientLibrus;
using HttpClientLibrus.LessonStrucs;
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

        private HttpClientLibrus.LibrusPlan _data;

        public HttpClientLibrus.LibrusPlan data
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

        public MainViewModel()
        {
            LimitedPeriods = new List<TimePeriod>(15);
            CurrentDay = ((int)DateTime.Today.DayOfWeek);

            progress = new Progress<int>();
            progress.ProgressChanged += ReportProgress;

            LoginWin = new LoginWindow();
            LoginWin.DataContext = this;
            OpenLoginCommand = new RelayCommand(o => LoginWin.Show());
            LoginCommand = new RelayCommand(o => LoginFromView());
            

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

        private void ReportProgress(object? sender, int e)
        {
            ((MainWindow)Application.Current.MainWindow).LoadProgress.Value = e;
        }

        async Task LoginFromView()
        {
            try
            {
                init(LocalName, Id, Login, Password);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        async void init(string locname, string Class, string libruslogin, string libruspassword)
        {
            //currently used for debug purposes
            try
            {
                acc = await Task.Run(async () => new AccountModel(locname, Class, await LibrusConnection.Connect(libruslogin, libruspassword,progress)));
                data = await Task.Run(() => HttpClientLibrus.LibrusPlan.Retrieve(acc.accountData,progress));
                var x = new List<TimePeriod>();
                for (int i = 0; i < LimitedPeriods.Capacity; i++)
                {
                    x.Add(data.timePeriods[i]);
                }
                LimitedPeriods = x;
            }
            catch (Exception)
            {
            }
            
        }
    }
}
