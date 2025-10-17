using MaterialTracking.UsersControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Effects;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupSetVolum : Popup
    {
        SetvolumViewModel vmpup;
        Button btn;
        public PopupSetVolum(SetvolumViewModel vm)
        {
            InitializeComponent();
            
            vmpup = vm;

            this.IsLightDismissEnabled = vmpup.DismissEnable;

            CreateUI();
        }

        private void CreateUI()
        {
            vmpup.CurrentPopup = this;

            var stackLabel = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };

            btn = new Button
            {
                Command = vmpup.TapCommand,

                CommandParameter = vmpup.UcRecentl,
                BackgroundColor = Color.DodgerBlue,//MyStyle.MyKolor.BlackBlue,
                WidthRequest = 400,
                TextColor = Color.White,
                Text = string.Format("{0} ({1})", "Chọn", vmpup.UcRecentl.Count(r => r.CheckedSelect))
            };
            CheckBox checkAll;
            if (vmpup.UcRecentl.Count != 0)
            {

                checkAll = new CheckBox
                {
                    IsChecked = vmpup.UcRecentl[0].CheckedSelect,
                    Color = Color.DarkGreen
                };
            }
            else
            {
                checkAll = new CheckBox
                {
                    IsChecked = false,
                    Color = Color.DarkGreen
                };
            }
            checkAll.CheckedChanged += (s, e) =>
            {
                var checkbox = s as CheckBox;

                if (checkbox.IsChecked)
                {
                    for (int i = 0; i < vmpup.UcRecentl.Count; i++)
                    {
                        vmpup.UcRecentl[i].CheckedSelect = checkbox.IsChecked;
                    }
                }
                else
                {
                    for (int i = 0; i < vmpup.UcRecentl.Count; i++)
                    {
                        vmpup.UcRecentl[i].CheckedSelect = checkbox.IsChecked;
                    }

                }
            };
            Label lblTitle = new Label
            {
                Text = "Chỉnh Sản Lượng Trước Khi Giao",
                FontSize =20,
                TextColor= Color.Black
                ,FontAttributes = FontAttributes.Bold
                
            };
            stackLabel.Children.Add(checkAll);
            stackLabel.Children.Add(lblTitle);

            this.Flex_Container.Children.Add(stackLabel);

            if (vmpup.UcRecentl.Any())
            {
              //  TouchEffect.SetLongPressCommand(btn, vmpup.LongPressCommand);

                TouchEffect.SetLongPressCommandParameter(btn, vmpup.UcRecentl);

                if (vmpup.UcRecentl != null)

                    for (int i = 0; i < vmpup.UcRecentl.Count; i++)
                    {
                        this.Flex_Container.Children.Add(vmpup.UcRecentl[i]);


                        vmpup.UcRecentl[i].CheckedEvent += (s, e) =>
                        {
                            btn.Text = string.Format("{0} ({1})", "OK", vmpup.UcRecentl.Count(r => r.CheckedSelect));

                        };
                    }

                this.Flex_Container.Children.Add(btn);

                this.BindingContext = vmpup;
            }
        }
    }
    public class SetvolumViewModel : INotifyPropertyChanged
    {
        #region Property change
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        ICommand tapCommand;
       // ICommand _longPressCommand;
        public ICommand TapCommand
        {
            get { return tapCommand; }
            private set { tapCommand = value; OnPropertyChanged("TapCommand"); }
        }
        //public ICommand LongPressCommand
        //{
        //    get { return _longPressCommand; }
        //    private set { _longPressCommand = value; OnPropertyChanged("LongPressCommand"); }
        //}


        private List<UC_SetVolum> _uc_Volum;

        Popup<object> _dismiss;

        bool _dismissEnable = true;

        public SetvolumViewModel()
        {
            tapCommand = new Command(OnTapped);
           // _longPressCommand = new Command(OnLongPress);
        }



        public Popup<object> CurrentPopup { get => _dismiss; set { _dismiss = value; OnPropertyChanged("CurrentPopup"); } }

        public bool DismissEnable { get => _dismissEnable; set { _dismissEnable = value; OnPropertyChanged(nameof(DismissEnable)); } }
        public List<UC_SetVolum> UcRecentl { get => _uc_Volum; set { _uc_Volum = value; OnPropertyChanged(nameof(UcRecentl)); } }

        void OnTapped(object s)
        {
            var lstTrue = (s as List<UC_SetVolum>).Where(r => r.CheckedSelect ).ToList();
            //return object text
            CurrentPopup.Dismiss(lstTrue);
        }
        //private void OnLongPress(object obj)
        //{
        //    var lstTrue = (obj as List<UC_SetVolum>).Where(r => !r.CheckedSelect).ToList();
        //    CurrentPopup.Dismiss(lstTrue);
        //}
    }
}