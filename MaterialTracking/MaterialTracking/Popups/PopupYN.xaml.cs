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

namespace MaterialTracking.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupYN : Popup
    {
        public PopupYN(BindingViewModel_PopupYN MV)
        {
            InitializeComponent();
            var mv = MV;
            mv.CurrentPopup = this;
            BindingContext = mv;
        }
    }
    public class BindingViewModel_PopupYN : INotifyPropertyChanged
    {
        ICommand tapCommand1;
        ICommand tapCommand2;

        Popup<object> _dismiss;

        string _lblTitle;

        string _lblContent;

        string _btnName1;
        string _btnName2;

        private bool _isbusy;

        private string _title;

        private Xamarin.Forms.Color _color;

        public string Title { get => _title; set { _title = value; OnPropertyChanged("Title"); } }
        
        public bool IsBusy
        {
            get => _isbusy;
            set
            {
                _isbusy = value; OnPropertyChanged("IsBusy");
            }
        }
       
        public bool IsNotBusy => !IsBusy;

        public string LabelNotification { set { _lblTitle = value; OnPropertyChanged(LabelNotification); } get => _lblTitle; }
        public string LabelContent { set { _lblContent = value; OnPropertyChanged(LabelContent); } get => _lblContent; }
        public string ButtonName1 { set { _btnName1 = value; OnPropertyChanged(ButtonName1); } get => _btnName1; }
        public string ButtonName2 { set { _btnName2 = value; OnPropertyChanged(ButtonName2); } get => _btnName2; }
        public Popup<object> CurrentPopup { get => _dismiss; set { _dismiss = value; OnPropertyChanged("CurrentPopup"); } }
        public Color Theme { get => _color; set { _color = value; OnPropertyChanged("Theme"); } }
        public BindingViewModel_PopupYN()
        {
            // configure the TapCommand with a method
            tapCommand1 = new Command(OnTapped_1);
            tapCommand2 = new Command(OnTapped_2);
        }
        public ICommand TapCommand_1
        {
            get { return tapCommand1; }
            set { tapCommand1 = value; OnPropertyChanged("TapCommand_1"); }
        }
        public ICommand TapCommand_2
        {
            get { return tapCommand2; }
            set { tapCommand2 = value; OnPropertyChanged("TapCommand_2"); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        void OnTapped_1(object s)
        {
            CurrentPopup.Dismiss(s);
        }
        void OnTapped_2(object s)
        {
            CurrentPopup.Dismiss(s);
        }

    }
}