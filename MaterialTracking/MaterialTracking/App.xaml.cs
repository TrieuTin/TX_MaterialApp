using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace MaterialTracking
{
    public partial class App : Application
    {
        //private static DB.Local _database;
      
        //internal static DB.Local LocalDatabase
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (_database == null) _database = new DB.Local();
        //        }
        //        catch (Exception)
        //        {
        //            Services.Alert.Msg("Thông Báo", "Vui lòng khởi động app lại");

        //        }

        //        return _database;
        //    }
        //}
        public App()
        {
            // Xamarin.Forms.DataGrid.DataGridComponent.Init();
            Xamarin.Forms.DataGrid.DataGridComponent.Init();
            Device.SetFlags(new[] { "Shapes_Experimental", "MediaElement_Experimental" });
            InitializeComponent();
            DependencyService.Register<ServicesMessage.IMessageServices, ServicesMessage.MessageServices>();
            //MainPage = new MainPage();
            MainPage = new NavigationPage(new Views.LoginView());
        }

        protected  override void OnStart()
        {
            //if (DB.StoreLocal.Instant.Myfac == DB.MyFactory.LVL)
            //{
            //    if (Class.Network.Net.HasNet)
            //    {
            //        var ver = DB.StoreLocal.Instant.Version.Split(' ');
            //        // var context = Android.App.Application.Context;
            //        Class.Helper.Instant.CheckForUpdateAsync(ver[1]);

            //    }
            //    else Services.DisplayToast.Show.Toast("Không có mạng", Class.Style.Error);
            //}
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
