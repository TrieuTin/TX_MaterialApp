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

namespace MaterialTracking.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupChoise : Popup
    {
        List<Button> _lstbtns;
        BindingViewModel_Choise vm;
        public PopupChoise(BindingViewModel_Choise b ,List< Button> buttons)
        {
            InitializeComponent();
            
            _lstbtns = buttons;

            vm = b;

             this.IsLightDismissEnabled = vm.DismissEnable;

            CreateUI(_lstbtns);
        }
        void CreateUI(List<Button> Ds_Bnt)
        {           
            vm.CurrentPopup = this;

            Grid g = new Grid();
            var scr = new ScrollView();

            for (int i = 0; i < Ds_Bnt.Count; i++)
            {
                var rowDefini = new RowDefinition();
                
                rowDefini.Height = new GridLength(100);

                g.RowDefinitions.Add(rowDefini);

                Ds_Bnt[i].Command = vm.TapCommand;

                Ds_Bnt[i].CommandParameter = Ds_Bnt[i];

                Ds_Bnt[i].HeightRequest = 50;

                Ds_Bnt[i].BackgroundColor = Class.Style.Notification;

                Ds_Bnt[i].TextColor = Color.White;

                Ds_Bnt[i].FontSize = 24d;

                g.Children.Add(Ds_Bnt[i], 0, i);
            }
            
            g.RowSpacing = 1d;
            

            scr.Content = g;

            if (S_Content.Children.Count > 0) { S_Content.Children.Clear();  }

            S_Content.Children.Add(scr);

            this.BindingContext = vm;

        }

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBar sb = (SearchBar)sender;

            if (sb != null)
            {
                if (_lstbtns.Count > 0)
                {
                    var filter = _lstbtns.Where(dk => dk.Text.Split('(')[0].Contains(sb.Text.ToUpper())).ToList();
                    

                    if (filter.Any()) 
                    {
                        CreateUI(filter);
                    }
                    else
                    {
                        filter.Add(new Button { Text = sb.Text.ToUpper() });

                        CreateUI(filter);
                    }

                }
                    
            }
            //else
            //{

            //    CreateUI(_lstbtns);
            //}
        }
    }
    public class BindingViewModel_Choise : BaseViewModel
    {
        ICommand tapCommand;

        Popup<object> _dismiss;

        string _lblNotifi;

        string _lblContent;

        bool _dismissEnable = true;

        private Xamarin.Forms.Color colorNotify;
        
        public string LabelNotification { set { _lblNotifi = value; OnPropertyChanged(LabelNotification); } get => _lblNotifi; }
       
        public string LabelContent { set { _lblContent = value; OnPropertyChanged(LabelContent); } get => _lblContent; }
      
        public Popup<object> CurrentPopup { get => _dismiss; set { _dismiss = value; OnPropertyChanged("CurrentPopup"); } }
     
        public Color ColorNotify { get => colorNotify; set { colorNotify = value; OnPropertyChanged("ColorNotify"); } }

        public bool DismissEnable { get => _dismissEnable; set { _dismissEnable = value; OnPropertyChanged(nameof(DismissEnable)); } }
        public BindingViewModel_Choise()
        {
            // configure the TapCommand with a method
            tapCommand = new Command(OnTapped);

        }
        public ICommand TapCommand
        {
            get { return tapCommand; }
         private   set { tapCommand = value; OnPropertyChanged("TapCommand"); }
        }


        void OnTapped(object s)
        {
            var b = s as Button;
            CurrentPopup.Dismiss(b.Text);
        }

    }
}