using MaterialTracking.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Table =System.Data;
namespace MaterialTracking.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AutoCuttingJobTicketTrackingSummaryView : ContentPage
    {
      
        public AutoCuttingJobTicketTrackingSummaryView(string MachineNO)
        {
            InitializeComponent();
            //this.BindingContext = new ViewModels.AutoCuttingJobTicketTrackingViewModel(MachineNO);

           BindingContext = new ViewModels.AutoCuttingJobTicketTrackingViewModel(MachineNO);
            lbl_Machine.Text = MachineNO;
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
           var selectedItem = e.SelectedItem as  Model_AutoCuttingJob;
         


            if (selectedItem != null)
            {
                lblProno.Text = "Prono: " + selectedItem.ProNo;
                lblRY.Text = "RY: " + selectedItem.Ry;
                lblArt.Text = "Art: " + selectedItem.Art;
                lblMatID.Text = "MatID: " + selectedItem.MatID;
                lblModel.Text = "Model: " + selectedItem.ModelName;
                lblLine.Text = "Line: " + selectedItem.Line;

            }
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //var listView = (ListView)sender;
            //var selectedItem = e.Item;
            //var startingIndex = listView.ItemsSource.IndexOf(selectedItem);

            //for (int i = startingIndex; i < listView.ItemsSource.Count; i++)
            //{
            //    var currentItem = listView.ItemsSource[i];
            //    do something with currentItem
            //}
        }

        private void OnDetails(object sender, EventArgs e)
        {
            var menuitem = ((MenuItem)sender);
            var modelPick = menuitem.CommandParameter as Model_AutoCuttingJob;
            var prono = modelPick.ProNo;
            var ddbh = modelPick.Ry;
            Navigation.PushModalAsync(new Views.AutoCuttingScanView(ddbh,false));
        }

        private void SwipeItem_Invoked(object sender, EventArgs e)
        {
            var swipeItem = sender as SwipeItem;

            // Lấy đối tượng dữ liệu tương ứng với dòng hiện tại đang swipe
            var dataItem = swipeItem.BindingContext as Model_AutoCuttingJob;
            
            // Kiểm tra xem đối tượng dữ liệu có null hay không
            if (dataItem != null)
            {
                Navigation.PushModalAsync(new Views.AutoCuttingScanView(dataItem.Ry, false));
            }
        }
    }
   
}