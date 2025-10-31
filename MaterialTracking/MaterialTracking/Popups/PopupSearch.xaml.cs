using MaterialTracking.Class;
using MaterialTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupSearch : Popup
    {
        public PopupSearch(string title="")
        {
            InitializeComponent();
            var vm = new PopupSearchViewModel( );

            vm.CurrentPopup = this;
            vm.Title = title;

            this.BindingContext = vm;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(100); //  wait popup render 
                txtSearch.Focus();     // focus Entry
            });
        }

        private void txtSearch_Completed(object sender, EventArgs e)
        {            

            this.Dismiss(txtSearch.Text);
        }
       
    }
    public class PopupSearchViewModel : BaseViewModel
    {
        //ICommand tapCommand;

        

        Popup<object> _dismiss;
        public Popup<object> CurrentPopup { get => _dismiss; set { _dismiss = value; OnPropertyChanged("CurrentPopup"); } }
        public PopupSearchViewModel()
        {
            TapCommand = new Command(OnTapped);
        }
        public ICommand TapCommand{get;}
        
        public string Holder { get => ConvertLang.Convert.Translate_LYM("Nhập mã", "ဝင်ပါ။"); }
        private void OnTapped(object obj)
        {
            var b = obj as Button;
          
            CurrentPopup.Dismiss(b.AutomationId);
        }
    }
}