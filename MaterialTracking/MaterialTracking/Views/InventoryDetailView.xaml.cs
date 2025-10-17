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
    public partial class InventoryDetailView : ContentPage
    {
        public InventoryDetailView(string code, string zlbh , string clbh, string bwbh)
        {
            InitializeComponent();

            var vm = new Invetory_Detail_Model();
          
            vm.Barcode = code;
          
            vm.Bwbh = bwbh;
          
            vm.Clbh = clbh;
          
            vm.Zlbh = zlbh;
          
            BindingContext = vm;
            

        }

        private async void DataGrid_ItemSelected(object sender, SelectionChangedEventArgs e)
        {
            var ac = await Services.Alert.MsgAction("Action", new string[] { "Call", "Delivery" }, "Argree");

            switch (ac)
            {
                case "Delivery":
                    var color = Color.FromHex("#4D96FF");
                    Services.DisplayToast.Show.Toast(ac, color);
                    break;
                case "Call":
                    color = Color.FromHex("#6BCB77");
                    Services.DisplayToast.Show.Toast(ac, color);
                    break;

                default:
                    break;
            }

          
          
        }
        
    }
    public class Invetory_Detail_Model: Model_Base
    {
        private string _barcode;
        private string _zlbh;
        private string _clbh;
        private string _bwbh;
        
        List<Inventory_Detail_DataType> _data;

        public string Barcode { get => _barcode; set { SetProperty(ref _barcode, value); } }
        public string Zlbh { get => _zlbh; set { SetProperty(ref _zlbh, value); } }
        public string Clbh{ get => _clbh; set { SetProperty(ref _clbh, value); } }
        public string Bwbh { get => _bwbh; set { SetProperty(ref _bwbh, value); } }

        public List<Inventory_Detail_DataType> Data { get => GetData(); set { SetProperty(ref _data, value); } }
        public PI.Mvvm.Commands.DelegateCommand RefreshCommand { get; }

        public Invetory_Detail_Model()
        {
            this.RefreshCommand = new PI.Mvvm.Commands.DelegateCommand(Refresh);
        }

        private Inventory_Detail_DataType _selected;


        public Inventory_Detail_DataType Selected { get => _selected; set => SetProperty(ref _selected, value); }
        private async void Refresh()
        {
            IsBusy = true;
            await Task.Delay(1000);
            Data = GetData();
            IsBusy = false;
        }

        private List<Inventory_Detail_DataType> GetData()
        {
            string sql = $"SELECT XXcc ,convert(int, Planqty)Planqty ,convert(int, actualqty ) actualqty , userid ,userdate from App_Cutting_Barcodes_Edit where barcode = '{Barcode}' and zlbh = '{Zlbh}' and clbh = '{Clbh}' and bwbh = '{Bwbh}'";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            _data = new List<Inventory_Detail_DataType>();
            if (ta.Rows.Count > 0)
            {

                foreach (System.Data.DataRow r in ta.Rows)
                {
                    //_data.Add(new Inventory_Detail_DataType
                    //{
                    //    ActualQty = int.Parse(r["actualqty"].ToString()),
                    //    PlanQty = int.Parse(r["Planqty"].ToString()),
                    //    Size = r["XXcc"].ToString(),
                    //    Userdate = DateTime.Parse(r["userdate"].ToString())
                    //    ,Userid=r["userid"].ToString()
                    //});

                    var idd = new Inventory_Detail_DataType();
                                        
                    idd.Size = r["XXcc"].ToString();
                    idd.Userid = r["userid"].ToString();
                    idd.Userdate = DateTime.Parse(r["userdate"].ToString());

                    //idd.PlanQty = int.Parse(r["Planqty"].ToString());
                    idd.PlanQty = int.Parse(r["Planqty"].ToString());

                    idd.ActualQty = int.Parse(r["actualqty"].ToString());
                    
                    
                    

                    _data.Add(idd);
                }
                return _data;
            }
            else return null;

        }
    }
    
    public class Inventory_Detail_DataType
    {
        public string Size   { get; set; }
        public int PlanQty{ get; set; }
        public int ActualQty{ get; set; }
        public string Userid { get; set; }
        public DateTime Userdate { get; set; }

    }
}