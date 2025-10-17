using MTM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.DB
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManagerDBLocalView : ContentPage
    {
        public ManagerDBLocalView()
        {
            InitializeComponent();

            BindingContext = new ManagerDataLocal();
        }

        private void DataGrid_ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            var answer = Services.Alert.MsgSelect("", "Delete this row");
            
            //var bc = BindingContext as Table_Barcode;
            //DB.DataLocal.Table.Delete_Barcode(bc);
        }
    }
    public class ManagerDataLocal : Model_Base
    {
     

        List<Barcode_Edit_LVY> _data;

     
        public List<Barcode_Edit_LVY> Data { get => GetData(); set { SetProperty(ref _data, value); } }
        public PI.Mvvm.Commands.DelegateCommand RefreshCommand { get; }

        public ManagerDataLocal()
        {
            this.RefreshCommand = new PI.Mvvm.Commands.DelegateCommand(Refresh);
        }

        private Table_Barcode _selected;


        public Table_Barcode Selected { get => _selected; set => SetProperty(ref _selected, value); }
        private async void Refresh()
        {
            IsBusy = true;
            await Task.Delay(1000);
            Data = GetData();
            IsBusy = false;
        }

        private List<Barcode_Edit_LVY> GetData()
        {

            _data= DB.DataLocal.Table. All_Local_LYV();
            Title = _data.Count.ToString();
            if (_data.Count > 0)
            {              
                return _data;
            }
            else return null;

        }
    }
}