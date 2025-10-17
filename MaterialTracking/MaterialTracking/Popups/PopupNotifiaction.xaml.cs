using MaterialTracking.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.PageViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupNotifiaction : Popup
    {
        public PopupNotifiaction(BindingViewModel MV)
        {
            InitializeComponent();
            
            var mv = MV;
            
            mv.CurrentPopup = this;
            
            BindingContext = mv;           
        }      
    }
  
    public class BindingViewModel : BaseViewModel
    {        
        ICommand tapCommand;

        Popup<object> _dismiss;

        string _lblNotifi;

        string _lblContent;

        string _btnName;

        private string _icon;

        private Xamarin.Forms.Color colorNotify;                           

        public string LabelNotification { set { _lblNotifi = value; OnPropertyChanged(LabelNotification); } get=>_lblNotifi; }
        public string LabelContent { set { _lblContent = value; OnPropertyChanged(LabelContent); } get=>_lblContent; }
        public string ButtonName { set { _btnName = value;OnPropertyChanged(ButtonName); } get=>_btnName; }
        public Popup<object> CurrentPopup{ get => _dismiss; set { _dismiss = value; OnPropertyChanged("CurrentPopup"); } }
        public Color ColorNotify { get => colorNotify; set { colorNotify = value; OnPropertyChanged("ColorNotify"); } }
        public string Icon { get => ShowAnimation(); set { _icon = value; OnPropertyChanged("Icon"); } }

        private string ShowAnimation()
        {
            
            if (colorNotify == MaterialTracking.Class.Style.Warning)
            {
                _icon = "Warning.json";
            }
            if(colorNotify == MaterialTracking.Class.Style.Error)
            {
                _icon= "ErrorSad.json";
            }
            if (colorNotify == MaterialTracking.Class.Style.Success)
            {
                _icon = "Success.json";
            }
            if (colorNotify == MaterialTracking.Class.Style.Notification)
            {
                _icon = "ManWorking.json";
            }
            return _icon;
        }

        public BindingViewModel()
        {
            // configure the TapCommand with a method
            tapCommand = new Command(OnTapped);
            
            
        }
        public ICommand TapCommand
        {
            get { return tapCommand; }
            set { tapCommand = value; OnPropertyChanged("TapCommand"); }
        }
        
        void OnTapped(object s)
        {            
             CurrentPopup.Dismiss(null);
        }
        
    }
    
}