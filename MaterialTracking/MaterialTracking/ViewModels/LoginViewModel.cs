using MaterialTracking.Class;
using MaterialTracking.PageViews;
using PI.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MaterialTracking.ViewModels
{
    /*CREATE TABLE App_Users (
    UserID NVARCHAR(50) PRIMARY KEY,
    FullName NVARCHAR(50),
    Pwd NVARCHAR(50) not null,
    LastLogin smallDATETIME,
    DepID NVARCHAR(20),
    Lock NVARCHAR(1)
);
*/

    public class LoginViewModel : ViewModels_Base
    {
        //private  Services.Msg.IMessageService Alert;
        public PI.Mvvm.Commands.DelegateCommand LoginCommand { get; }
        public PI.Mvvm.Commands.DelegateCommand CreateAccCommand { get; }
        private string _id;
        private string _pwd;
        private bool _faclvl;
        private bool _faclyv;
        private bool _faclhg;
        private bool _faclym;
        private string _pathClip = "ms-appx:///Landscape.mp4";
        private Xamarin.Forms.Aspect _aspect = Aspect.AspectFill;
        
        internal Xamarin.Forms.INavigation Navigation { get; set; }

        /*ac = "LVL"; }
                if (_faclhg) { ac = "LHG"; }
                if (_faclyv) { ac = "LYV"; }
                if (_factest) { ac = "Debug";*/
        public bool FacLVL
        {
            get 
            {               
                return  _faclvl; 
            }
            set { if (SetProperty(ref _faclvl, value)) { LoginCommand.RaiseCanExecuteChanged(); } }
        }
        public bool FacLYV
        {
            get
            {               
                return     _faclyv; 
            }
            set { if (SetProperty(ref _faclyv, value)) { LoginCommand.RaiseCanExecuteChanged(); } }
        }

        public bool FacHG
        {
            get
            {                              
                return _faclhg; 
            }
            set { if (SetProperty(ref _faclhg, value)) { LoginCommand.RaiseCanExecuteChanged(); } }
        }
        public bool FacLYM
        {
            get => _faclym;
            set { if (SetProperty(ref _faclym, value)) { LoginCommand.RaiseCanExecuteChanged(); } }
        }

        public string Id
        {
            get 
            {
                

                return _id;
            }
            set
            {
                if (SetProperty(ref _id, value))
                {
                    
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public string Pwd
        {
            get
            {
               

                return _pwd;
            }
            set
            {
                if (SetProperty(ref _pwd, value))
                {
                    LoginCommand.RaiseCanExecuteChanged();
                }

            }

        }

        public string PathClip
        {
            get => _pathClip;
            set
            {
                if (SetProperty(ref _pathClip, value))
                {
                    LoginCommand.RaiseCanExecuteChanged();
                }

            }
        }

        public Aspect Aspected
        {
            get => _aspect;
            set
            => SetProperty(ref _aspect, value);
        }                                                                    
       

        public LoginViewModel()
        {

            this.LoginCommand = new PI.Mvvm.Commands.DelegateCommand(Login, CanLogin);
           // this.CreateAccCommand= new PI.Mvvm.Commands.DelegateCommand(CreateAccount);

        }

        public string Version
        {
            get => string.Format("Version: {0}", DB.StoreLocal.Instant.Version);
        }
        public string Serial
        {
            get => string.Format("Serial: {0}", DB.StoreLocal.Instant.Serial.ToUpper());
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(Pwd) && !string.IsNullOrEmpty(Id);
        }

        private async void Login()
        {
           
            if (!Network.Net.HasNet)
            {
                Services.Alert.Msg("Cảnh Báo!", "Không có mạng");
                return;
            }

            try
            {

                string ac = "";

               

                if (_faclvl) { ac = "LVL"; DB.StoreLocal.Instant.Myfac = DB.MyFactory.LVL; }
                if (_faclhg) { ac = "LHG"; DB.StoreLocal.Instant.Myfac = DB.MyFactory.LHG; }
                if (_faclyv) { ac = "LYV"; DB.StoreLocal.Instant.Myfac = DB.MyFactory.LYV; }
                if (_faclym) { ac = "LYM"; DB.StoreLocal.Instant.Myfac = DB.MyFactory.LYM; }//MYAnma

             



                if (string.IsNullOrEmpty(ac)) return;

                DB.StoreLocal.Instant.Factory = ac;

                DB.StoreLocal.Instant.UserName = Id;//Store factory 



                IsBusy = true;
                await Task.Delay(500);
                bool? successLogin = DB.SQL.ConnectDB.Connection.CheckUser(Id, Pwd);
                if (successLogin == true)
                {
                    if (!CheckVersion()) { Services.DisplayToast.Show.Toast("Version nãy đã cũ vui lòng liên hệ IT để lên phiên bản mới", Class.Style.Red); IsBusy = false; return; }

                    var u = DB.DataLocal.Table.User;

                    if (u.Count == 0 || u[0].UID != Id)
                    {
                        DB.DataLocal.Table.Insert_User(new DB.AccountUser
                        {
                            UID = Id,
                            Pwd = Pwd,
                            Company = ac,
                            uDate = DateTime.Now.Ticks
                        });
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {

                        Application.Current.MainPage = new NavigationPage(new Master());

                        // remove Login UI from Stack
                        var navigationStack = Application.Current.MainPage.Navigation.NavigationStack;
                        if (navigationStack.Count > 1)
                        {
                            for (int i = 0; i < navigationStack.Count - 1; i++)
                            {
                                var page = navigationStack[i];
                                Application.Current.MainPage.Navigation.RemovePage(page);
                            }
                        }

                        //Disable Back button 
                        NavigationPage.SetHasBackButton(Application.Current.MainPage, false);

                    });



                }
                else if (successLogin == false)
                {

                    Services.Alert.Msg("Nguyên nhân", "Sai tên hoặc mật khẩu!");

                }
                else
                {
                    if (!Network.Net.HasNet)
                    {                        
                        Services.Alert.Msg("Cảnh Báo!", "Không có mạng");
                    }
                    else
                    {
                        Services.Alert.Msg("Cảnh Báo!", "Tài khoảng  bị khóa");

                    }
                }
                IsBusy = false;
            }
            catch (Exception errmsg)
            {

                Services.Alert.Msg("Login", errmsg.Message);
            }

        }

        bool CheckVersion()
        {
            var sql = $"SELECT * FROM AppVersion where Device ='{DB.StoreLocal.Instant.Serial}' and AppName ='Material'";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);




            if (ta.Rows.Count > 0)
            {
                var allow = ta.Rows[0]["Allow"].ToString();
                if (allow == "True")
                {

                    UpdateVersion("1");
                    return true;
                }

                else
                {
                    return false;
                }


            }
            else
            {
                InsertVersion("1"); return true;

            }
        }
        void InsertVersion(string allow)
        {
            var sql = "INSERT INTO [dbo].[AppVersion]([AppName],[Ver],[UserID],[VerDate] ,[device],[Allow])VALUES(@name,@Ver,@UserID,@VerDate,@Device,@Allow)";
            string[] arr = 
                { 
                    "Material",
                    Xamarin.Essentials.AppInfo.VersionString,
                    DB.StoreLocal.Instant.UserName,
                    DB.StoreLocal.Instant.ServerDate.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    DB.StoreLocal.Instant.Serial,
                    allow
                };

            DB.SQL.ConnectDB.Connection.Update_Parameter(sql, arr);

        }
        void UpdateVersion(string allow)
        {
            string update_Sql = "Update AppVersion set UserID=@UserID, Ver=@Ver , VerDate =@VerDate,Allow =@Allow where (Device = @Dvice) and appName='Material'";

            string[] arr = { DB.StoreLocal.Instant.UserName, DB.StoreLocal.Instant.Version, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), allow, DB.StoreLocal.Instant.Serial };

            DB.SQL.ConnectDB.Connection.Update_Parameter(update_Sql, arr);
        }
    }
    public class ViewModels_Base : BindableBase
    {
        private bool _isbusy;
        private string _title;

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public bool IsBusy
        {
            get => _isbusy;
            set
            {
                if (SetProperty(ref _isbusy, value))
                {
                    RaisePropertyChanged(nameof(IsNotBusy));
                }
            }
        }
        public bool IsNotBusy => !IsBusy;

    }
}
