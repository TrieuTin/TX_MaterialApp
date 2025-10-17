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
    public partial class dg_WH_NS : Grid
    {
        public dg_WH_NS()
        {
            InitializeComponent();
            
        }
        public Model_Inventory ThisContext
        {
            get => this.BindingContext as Model_Inventory;
            set
            {
                this.BindingContext = value;
          
                ItemSource = ThisContext.Data_WH_NS;

                if (ThisContext.Data_WH_NS != null)
                {
                    if (CheckedAll.IsChecked) CheckedAll.IsChecked = false;

                    var lstpicker = ThisContext.Data_WH_NS.Select(x => x.Lean)
                                   .Distinct().OrderBy(x => x)
                                   .ToList();

                    lstpicker.Add("All");

                    lstpicker.Select(x => x.Any()).OrderBy(x => x);

                    DataPickerLean.ItemsSource = lstpicker;
                }
            }
        }
        public List<Model_WH_NS> ItemSource
        {
            get => (List<Model_WH_NS>)gv_Inventory.ItemsSource;
            set
            {
                gv_Inventory.ItemsSource = value;

                ThisContext.CountItems = ItemSource == null ? string.Format("{0} {1}", 0, Class.ConvertLang.Convert.Translate_LYM("Đơn", "ပစ္စည်းများ")) : string.Format("{0}  {1}", ItemSource.Count, Class.ConvertLang.Convert.Translate_LYM("Đơn", "ပစ္စည်းများ"));
              
            }
        }

        private void On_SelectDepName_Change_Event(object sender, EventArgs e)
        {

        }

        private void Checked_Change(object sender, CheckedChangedEventArgs e)
        {

        }
        protected override void OnParentSet()
        {
            base.OnParentSet();

            Device.BeginInvokeOnMainThread(() =>
            {               
                var animation = new Animation(v => this.TranslationX = v, this.Width, 0, Easing.CubicOut);
                animation.Commit(this, "SlideIn", length: 1300);
            });
        }
    }
    public class Model_WH_NS
    {
        public bool Sel { get; set; }
        public string ID { get; set; }
        public string Ry { get; set; }
        public string VnName { get; set; }
        public string EngName { get; set; }
        public string Article { get; set; }
        public string PO { get; set; }
        public string Qty { get; set; }
        //public string OrQty { get; set; }
        public string PSDT { get; set; }
        public string Lean { get; set; }
        public System.Windows.Input.ICommand Command_Click { get; set; }
    }
}