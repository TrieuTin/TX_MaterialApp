using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SupperMaketView : ContentPage
    {
        public SupperMaketView()
        {
            InitializeComponent();
            var sp = new ViewModels.SupermarketViewModel();
            sp.Navigation = Navigation;

            BindingContext = sp;

            // Services.Alert.ToastMsg("Scan");

            this.lstVw.ItemsSource = (this.BindingContext as ViewModels.SupermarketViewModel).Inventory();


        }
   
        private void picker_Building_SelectedIndexChanged(object sender, EventArgs e)
        {          
            picker_Lean.ItemsSource = (this.BindingContext as ViewModels.SupermarketViewModel).GetLean(picker_Building.SelectedItem.ToString());
        }
       
        private void picker_Lean_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (picker_Lean.SelectedItem != null)
            {

                this.lstVw.ItemsSource = (this.BindingContext as ViewModels.SupermarketViewModel).Inventory(picker_Lean.SelectedItem.ToString(), picker_Date.Date);
            }
        }

        private void picker_Date_DateSelected(object sender, DateChangedEventArgs e)
        {
            if (picker_Lean.SelectedItem != null)
            {

                this.lstVw.ItemsSource = (this.BindingContext as ViewModels.SupermarketViewModel).Inventory(picker_Lean.SelectedItem.ToString(), picker_Date.Date);
            }
        }

        private void Delivery_Invoked(object sender, EventArgs e)
        {

        }

        

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            SearchBar sb = (SearchBar)sender;
            if (sb != null)
            {
                var ry = string.Format("%{0}%",sb.Text);
                this.lstVw.ItemsSource = (this.BindingContext as ViewModels.SupermarketViewModel).Inventory(ry);
            }
        }

        private void Inventory_Invoked(object sender, EventArgs e)
        {
            var swipeItem = sender as SwipeItem;

            // Lấy đối tượng dữ liệu tương ứng với dòng hiện tại đang swipe
            var dataItem = swipeItem.BindingContext as ViewModels.Model_Inventory;

            // Kiểm tra xem đối tượng dữ liệu có null hay không
            if (dataItem != null)
            {                
               // Navigation.PushModalAsync(new Views.AutoCuttingStock(dataItem));
            }
        }
    }
   
}