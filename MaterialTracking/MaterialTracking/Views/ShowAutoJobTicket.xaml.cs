using MTM.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowAutoJobTicket : ContentPage
    {
        public ShowAutoJobTicket(string manchinName)
        {
            InitializeComponent();
            BindingContext = new AutoJobTicket_Model(manchinName);
       
        }

        private async void DataGrid_ItemSelected(object sender, SelectionChangedEventArgs e)
        {
           
           
            var dataitem = BindingContext as AutoJobTicket_Model;

            if(dataitem != null) 
                await Navigation.PushModalAsync(new Views.AutoCuttingScanView( dataitem.Selected.Ry, false));

        }
    }
    public class AutoJobTicket_Model : Model_Base
    {
        List<AutoJobTicket_DataType> _data;
        public List<AutoJobTicket_DataType> Data { get => GetData(); set { SetProperty(ref _data, value); } }
        public PI.Mvvm.Commands.DelegateCommand RefreshCommand { get; }

        string _machineNO;

        public AutoJobTicket_Model(string MachineNO)
        {
            this.RefreshCommand = new PI.Mvvm.Commands.DelegateCommand(Refresh);
            Title = _machineNO = MachineNO;
            
        }

        private AutoJobTicket_DataType _selected;


        public AutoJobTicket_DataType Selected { get => _selected; set => SetProperty(ref _selected, value); }
        private async void Refresh()
        {
            IsBusy = true;
            await Task.Delay(500);
            Data = GetData();
            IsBusy = false;
        }

        private List<AutoJobTicket_DataType> GetData()
        {
            string sql = $"SELECT CB3.machineno Machine,CBs.barcode ProNo,pdsch.lean,cb3.article,xieming ModelName, CBs.clbh, CBs.zlbh RY,bwzl.ywsm Component,dd.pairs TargetQty,isnull(edit.qty, 0)       Supermaket,CB3.workdate ProductionDate FROM cutting_barcodes CBs LEFT JOIN cutting_barcode CB3 ON CBs.barcode = CB3.barcode LEFT JOIN pdsch ON pdsch.zlbh = CBs.zlbh LEFT JOIN ddzl dd ON DD.ddbh = pdsch.ry LEFT JOIN zlzls2 zl ON zl.zlbh = pdsch.ry AND zl.clbh = CBs.clbh AND zl.bwbh = CBs.bwbh LEFT JOIN de_orderm DE ON DE.orderno = pdsch.ry LEFT JOIN xxzl ON xxzl.article = CB3.article LEFT JOIN bwzl ON bwdh = CBs.bwbh left join(SELECT zlbh, clbh, bwbh, convert(int, sum(ActualQty)) qty FROM App_Cutting_Barcodes_Edit group by zlbh, clbh, bwbh) edit on edit.ZLBH = CBs.zlbh and edit.CLBH = CBs.clbh and edit.BWBH = CBs.bwbh WHERE  1 = 1 AND CB3.machineno = '{_machineNO}'and CB3.workdate >getdate()-7";

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            _data = new List<AutoJobTicket_DataType>();

            if (ta.Rows.Count > 0)
            {

                foreach (System.Data.DataRow r in ta.Rows)
                {
                    try
                    {
                        var prodate = DateTime.Parse(r["productiondate"].ToString());

                        var model = new AutoJobTicket_DataType();
                        model.ProNo = r["prono"].ToString();
                        model.Line = r["lean"].ToString();

                        
                        model.Instock =string.Format("{0}/{1}", r["Supermaket"].ToString(),r["targetqty"].ToString());
                        model.CLBH = r["clbh"].ToString();

                        model.Art = r["article"].ToString();


                        model.Ry = r["ry"].ToString();
                        model.Component = r["component"].ToString();
                        model.Machine = r["machine"].ToString();
                        model.ModelName = r["Modelname"].ToString();

                        model.ProductionDate = prodate.ToString("yyyy-MM-dd");




                        _data.Add(model);
                        Debug.WriteLine(_data.Count);
                    }
                    catch (Exception xx)
                    {

                        
                    }
                    
                }
                return _data;
            }
            else return null;

        }
    }

    public class AutoJobTicket_DataType
    {
        
        
        public string Machine { get; set; }
        public string ProNo { get; set; }
        public string Line { get; set; }
        public string Art { get; set; }

        public string ModelName { get; set; }
        public string Ry { get; set; }

        public string CLBH { get; set; }
        public string Component { get; set; }
     
       
        public string Instock { get; set; }
        public string ProductionDate { get; set; }
        
      

    }
}