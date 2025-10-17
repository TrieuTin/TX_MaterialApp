using MaterialTracking.PageViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class dg_WH_AC : Grid
    {
        public dg_WH_AC()
        {
            InitializeComponent();

           // ThisContext = My_Model;
        }
        //private static readonly BindableProperty DataProperty = BindableProperty.Create(nameof(Data), typeof(string), typeof(dg_WH_AC), default(List<Model_DataGrid>));

        //public string Data
        //{
        //    get => (string)GetValue(DataProperty);
        //    set => SetValue(DataProperty, value);
        //}
         public  Model_Inventory ThisContext
        {
            get => this.BindingContext as Model_Inventory;
            set
            {
                this.BindingContext = value;

                if (ItemSource != null)
                {

                    //Data_search = ItemSource;

                   // btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });


                }

                ItemSource = ThisContext.Data;

                if (ThisContext.Data != null)
                {
                    if (CheckedAll.IsChecked) CheckedAll.IsChecked = false;

                    var lstpicker = ThisContext.Data.Select(x => x.DepName)
                                   .Distinct().OrderBy(x => x)
                                   .ToList();

                    lstpicker.Add("All");

                    lstpicker.Select(x => x.Any()).OrderBy(x => x);

                    DataPickerLean.ItemsSource = lstpicker;
                }
            }
        }
        public List<Model_DataGrid> ItemSource
        {
            get => (List<Model_DataGrid>)gv_Inventory.ItemsSource;
            set
            {
                gv_Inventory.ItemsSource = value;

                ThisContext.CountItems = ItemSource == null ? string.Format("{0} {1}", 0, Class.ConvertLang.Convert.Translate_LYM("Đơn", "ပစ္စည်းများ")) : string.Format("{0}  {1}", ItemSource.Count, Class.ConvertLang.Convert.Translate_LYM("Đơn", "ပစ္စည်းများ"));



               // btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });
            }
        }
        
        protected override void OnParentSet()
        {
            base.OnParentSet();

            //this.TranslationX = this.Width; // Chuẩn bị off-screen bên phải

            Device.BeginInvokeOnMainThread(() =>
            {
                //wait this.TranslateTo(0, 0, 900, Easing.SinIn); // Animation khi control vừa gắn vào


                var animation = new Animation(v => this.TranslationX = v, this.Width, 0, Easing.CubicOut);
                animation.Commit(this, "SlideIn", length: 1300);
            });
        }

        private void On_SelectDepName_Change_Event(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var selectLine = picker.SelectedItem as string;

           // var date = Date_Filter.Date;

            if (selectLine != "All")
            {
                if (ThisContext.Data.Any())
                {
                    var filter = ThisContext.Data.Where(dk => dk.DepName == selectLine).ToList();

                    ItemSource = new List<Model_DataGrid>(filter);
                }
            }
            else
            {
                ItemSource = ThisContext.Data;

                ThisContext.isShowDatepicker = false;
            }

        }

        private void Checked_Change(object sender, CheckedChangedEventArgs e)
        {
            var check = (CheckBox)sender;

            var temp = new List<Model_DataGrid>();

            if (check.IsChecked)
                foreach (var item in ItemSource)
                {

                    item.Sel = true;
                    temp.Add(item);
                }
            else
                foreach (var item in ItemSource)
                {

                    item.Sel = false;
                    temp.Add(item);
                }

            //Data_search = ItemSource = temp;
            ThisContext.Data = ItemSource = temp;
        }
    }
}