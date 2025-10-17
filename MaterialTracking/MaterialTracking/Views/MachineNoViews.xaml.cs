using MaterialTracking.DB;
using MaterialTracking.Models;
using MaterialTracking.PageViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MaterialTracking.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MachineNoViews : ContentPage
    {
        public MachineNoViews()
        {
            InitializeComponent();
            var vm = new MachineViewModel();
           

            BindingContext = vm;
        }

        private  void Button_Clicked(object sender, EventArgs e)
        {

            Alarm.Sound.Click();
            //var txt = (sender as Button).Text;
        
            //await Navigation.PushAsync(new ShowAutoJobTicket(txt));

        }
      
    }


    public class MachineViewModel : BaseViewModel
    {
        public ICommand Cmd_Machine_click { get; }
        public MachineViewModel()
        {
            Cmd_Machine_click = new Command(Button_Clicked);

            GetListMachine();
           
        }

        private async void Button_Clicked(object obj)
        {
          
            var txt = (obj as string);

            await Application.Current.MainPage. Navigation.PushAsync(new ShowAutoJobTicket(txt));
        }

        private ObservableCollection<MachineNoModels> machineCollection;
        public ObservableCollection<MachineNoModels> MachineCollection
        {
            get
              => machineCollection;

            set
            {
                //  machineCollection = value;
                SetProperty(ref machineCollection, value);
            }
        }
        public void GetListMachine()
        {
            ObservableCollection<MachineNoModels> listmachine = new ObservableCollection<MachineNoModels>();

            string sql = "select * from cutting_machine";
            var ta = MaterialTracking.DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            for (int i = 0; i < ta.Rows.Count; i++)
            {

                listmachine.Add(new MachineNoModels { MachineName = ta.Rows[i]["MachineName"].ToString(), MachineNo = ta.Rows[i]["MachineID"].ToString() });

            }

            MachineCollection = listmachine;

        }
    }


   
   
        
}