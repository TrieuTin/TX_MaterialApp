using MTM.Model;
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
    public partial class InvetorySummeryView : ContentPage
    {
        public InvetorySummeryView()
        {
            InitializeComponent();
            BindingContext = new Invetory_Model();
        }

        private async void DataGrid_ItemSelected(object sender, SelectionChangedEventArgs e)
        {

            string[] arr_choise = { "Xem Kho", "Giao" };
            
            var ac =await Services.Alert.MsgAction("Chọn lựa", arr_choise);

            if(ac == arr_choise[0])
            {
                var bd = BindingContext as Invetory_Model;

                if (bd != null)
                {
                    await Navigation.PushAsync(new InventoryDetailView(bd.Selected.Barcode, bd.Selected.ZLBH, bd.Selected.CLBH, bd.Selected.BWBH));
                }
            }
            else if(ac == arr_choise[1])
            {
                Services.DisplayToast.Show.Toast("Đã Giao", Color.Khaki);
            }

           
                
        }
    }
    public class Invetory_Model : Model_Base
    {
        List<Inventory_DataType> _data;
        public List< Inventory_DataType >Data { get => GetData(); set { SetProperty(ref _data, value); } }
        public PI.Mvvm.Commands.DelegateCommand RefreshCommand { get; }
        
        public Invetory_Model()
        {
            this.RefreshCommand = new PI.Mvvm.Commands.DelegateCommand(Refresh);
        }

        private Inventory_DataType _selected;


        public Inventory_DataType Selected { get => _selected; set => SetProperty(ref _selected, value); }
        private async void Refresh()
        {
            IsBusy = true;
            await Task.Delay(1000);
            Data = GetData();
            IsBusy= false;
        }

        private List<Inventory_DataType> GetData()
        {
            string sql = "SELECT barcode, e.zlbh, clbh, BWBH,convert(date,p.PSDT) prodate, count(xxcc) sizes from app_cutting_barcodes_edit e inner join pdsch p on e.ZLBH = p.ry group by barcode, e.zlbh, clbh, bwbh ,p.PSDT";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            _data = new List<Inventory_DataType>();
            if (ta.Rows.Count > 0)
            {

                foreach (System.Data.DataRow r in ta.Rows)
                {
                    var prodate = DateTime.Parse(r["prodate"].ToString());

                    _data.Add(new Inventory_DataType
                    {
                        Barcode = r["barcode"].ToString(),

                        CLBH = r["clbh"].ToString(),

                        BWBH = r["bwbh"].ToString(),

                        Sizes = int.Parse(r["sizes"].ToString()),

                        ZLBH = r["zlbh"].ToString(),

                        ProDate = prodate.ToString("yyyy-MM-dd")
                    });


                }
                return _data;
            }
            else return null;

        }
    }
   
    public class Inventory_DataType
    {
      public   string Barcode { get; set; }
        public string ZLBH { get; set; }
        public  string CLBH { get; set; }
        public  string BWBH { get; set; }
        public  int Sizes { get; set; }
        public  string ProDate { get; set; }
        
    }
}