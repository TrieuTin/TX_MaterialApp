using MaterialTracking.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.PageViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageDone : ContentPage
    {
        public PageDone()
        {

            InitializeComponent();

            UpdateData();

        }
        List<Model_DataGrid> Data_search;
        private async Task<int> UpdateData()
        {
            ThisContext = new Model_Done();

            if (ThisContext.Data != ItemSource)
            {
                Data_search = ItemSource = ThisContext.Data;
                               
            }
            return ItemSource.Count;

        }
        

        private void Search_TextChange(object sender, TextChangedEventArgs e)
        {
            var sb = (SearchBar)sender;

            if (sb != null)
            {
                if (Data_search.Any())
                {

                    if (ThisContext.SelectedProdateIndex != -1)
                    {

                        var filter = Data_search.Where(dk => (dk.RY.Contains(sb.Text.ToUpper()) || dk.Art.Contains(sb.Text.ToUpper()) || dk.Comp.Contains(sb.Text.ToUpper())) && dk.ProdDate.Contains(ThisContext.SelectedProdate)).ToList();

                        ItemSource = new List<Model_DataGrid>(filter);
                    }
                    else
                    {
                        var filter = Data_search.Where(dk => dk.RY.Contains(sb.Text.ToUpper()) || dk.Art.Contains(sb.Text.ToUpper()) || dk.Comp.Contains(sb.Text.ToUpper())).ToList();

                        ItemSource = new List<Model_DataGrid>(filter);

                    }
                }

            }
            else
            {
                ItemSource = Data_search;

                ThisContext.SelectedProdateIndex = -1;
            }
        }
        Model_Done ThisContext
        {
            get => this.BindingContext as Model_Done;
            set { this.BindingContext = value; Data_search = ItemSource;
                var lstpicker = Data_search.Select(x => x.ProdDate)
                      .Distinct().OrderBy(x => x)
                      .ToList();

                lstpicker.Add("All");

                lstpicker.Select(x => x.Any()).OrderBy(x => x);

                DataPickerFilter.ItemsSource = lstpicker;
            }
        }
        List<Model_DataGrid> ItemSource
        {
            get => (List<Model_DataGrid>)gvDone.ItemsSource;
            set { gvDone.ItemsSource = value; ThisContext.Countitems = string.Format("{0} Đơn", ItemSource.Count); }
        }

        private async void Refresh_Clicked(object sender, EventArgs e)
        {
            ThisContext.IsBusy = true;
            await Task.Delay(100);
            var a = await UpdateData();
            ThisContext.IsBusy = false;
        }

        private void On_SelectProDate_Change_Event(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var prodate = picker.SelectedItem as string;

            if (prodate != "All")
            {
                if (Data_search.Any())
                {                   
                    var filter = Data_search.Where(dk => (dk.ProdDate == prodate));

                    ItemSource = new List<Model_DataGrid>(filter);
                }
            }
            else ItemSource = Data_search;
        }

        private void On_FilterChange_Event(object sender, CheckedChangedEventArgs e)
        {
            if (Data_search == null) return;

            var filter = new List<Model_DataGrid>();

            if (ThisContext.Hf_checked)
                foreach (var item in Data_search)
                    if (item.DepName == "HF") filter.Add(item);

            if (ThisContext.Ns_checked)
                foreach (var item in Data_search)
                    if (item.DepName == "NS") filter.Add(item);

            if (ThisContext.Print_checked)
                foreach (var item in Data_search)
                    if (item.DepName == "PR") filter.Add(item);

            if (ThisContext.Em_checked)
                foreach (var item in Data_search)
                    if (item.DepName == "EM") filter.Add(item);

            if (ThisContext.Os_checked)
                foreach (var item in Data_search)
                    if (item.DepName == "OS") filter.Add(item);

            if (ThisContext.St_checked)
                foreach (var item in Data_search)
                    if (item.DepName == "ST") filter.Add(item);

            ItemSource = filter;

            ThisContext.SelectedProdateIndex = -1;
        }
    }
    public class Model_Done: BaseViewModel
    {
        public Model_Done()
        {            
            if (DB.StoreLocal.Instant.Depname == DB.Departments.AutoCutting)
            {
                _ns_checked = true;
                _print_checked = true;
                _hf_checked = true;
                _em_checked = true;
                _st_checked = true;
                _os_checked = true;


                _ns_enb = true;
                _print_enb = true;
                _hf_enb = true;
                _em_enb = true;
                _st_enb = true;
                _os_enb = true;
            }
        }
        private string _countitems;

        private bool _ns_checked = DB.StoreLocal.Instant.Depname == DB.Departments.NoSew;
        private bool _print_checked = DB.StoreLocal.Instant.Depname == DB.Departments.Printing;
        private bool _hf_checked = DB.StoreLocal.Instant.Depname == DB.Departments.HighFrequency;
        private bool _em_checked = DB.StoreLocal.Instant.Depname == DB.Departments.Embroidery;
        private bool _st_checked = DB.StoreLocal.Instant.Depname == DB.Departments.Stiching;
        private bool _os_checked = DB.StoreLocal.Instant.Depname == DB.Departments.OutSourcing;

        private bool _ns_enb = DB.StoreLocal.Instant.Depname == DB.Departments.NoSew;
        private bool _print_enb = DB.StoreLocal.Instant.Depname == DB.Departments.Printing;
        private bool _hf_enb = DB.StoreLocal.Instant.Depname == DB.Departments.HighFrequency;
        private bool _em_enb = DB.StoreLocal.Instant.Depname == DB.Departments.Embroidery;
        private bool _st_enb = DB.StoreLocal.Instant.Depname == DB.Departments.Stiching;
        private bool _os_enb = DB.StoreLocal.Instant.Depname == DB.Departments.OutSourcing;

        private string _selected_prodate;

        private int _selected_prodate_index = -1;

        private List<string> _prodate_data;
        public string SelectedProdate
        {
            get => _selected_prodate; set => SetProperty(ref _selected_prodate, value);
        }
        public int SelectedProdateIndex
        {
            get => _selected_prodate_index; set => SetProperty(ref _selected_prodate_index, value);
        }
        public List<string> Prodate_data { get => _prodate_data; set => SetProperty(ref _prodate_data, value); }
        public bool Ns_checked { get => _ns_checked; set => SetProperty(ref _ns_checked, value); }
        private bool _ac_checked = DB.StoreLocal.Instant.Depname == DB.Departments.NoSew;
        public bool Print_checked { get => _print_checked; set => SetProperty(ref _print_checked, value); }
        public bool Hf_checked { get => _hf_checked; set => SetProperty(ref _hf_checked, value); }
        public bool Em_checked { get => _em_checked; set => SetProperty(ref _em_checked, value); }
        public bool St_checked { get => _st_checked; set => SetProperty(ref _st_checked, value); }
        public bool Os_checked { get => _os_checked; set => SetProperty(ref _os_checked, value); }

        private bool _ac_enb = DB.StoreLocal.Instant.Depname == DB.Departments.NoSew;
        public bool Ns_enb { get => _ns_enb; set => SetProperty(ref _ns_enb, value); }
        public bool Print_enb { get => _print_enb; set => SetProperty(ref _print_enb, value); }
        public bool Hf_enb { get => _hf_enb; set => SetProperty(ref _hf_enb, value); }
        public bool Em_enb { get => _em_enb; set => SetProperty(ref _em_enb, value); }
        public bool St_enb { get => _st_enb; set => SetProperty(ref _st_enb, value); }
        public bool Os_enb { get => _os_enb; set => SetProperty(ref _os_enb, value); }

        public string Countitems { get => _countitems; set => SetProperty(ref _countitems, value); }

        private List<Model_DataGrid> _data;
        public List<Model_DataGrid> Data { get => GetData_Done(); set => SetProperty(ref _data, value); }

        private List<Model_DataGrid> GetData_Done()
        {
           

            _data = new List<Model_DataGrid>();

            #region old code

            string sql = $@"SELECT a.Orderid, K.ZLBH,
       K.Component,
       K.CLBH,
       K.ModelName,
       K.ARTICLE,
       K.PSDT,
       K.BWBH,
       convert(varchar, a.Qty )+'/'+ convert(varchar,K.planqty ) TarQty,a.DepNo
	   FROM ( SELECT 
       e.ZLBH,
       b.ywsm Component,
       e.CLBH,
       x.XieMing ModelName,
       DDZL.ARTICLE,
       PDSCH.PSDT,
       e.BWBH,
       SUM(CONVERT(INT, e.PlanQty)) planqty,
       SUM(CONVERT(INT, e.ActualQty)) Actual
FROM App_Cutting_Barcodes_Edit e
    INNER JOIN bwzl b
        ON b.bwdh = e.BWBH
    INNER JOIN DDZL
        ON e.ZLBH = DDZL.DDBH
    INNER JOIN xxzl x
        ON DDZL.ARTICLE = x.ARTICLE
    INNER JOIN PDSCH
        ON e.ZLBH = PDSCH.ZLBH
GROUP BY e.ZLBH,
         b.ywsm,
         x.XieMing,
         DDZL.ARTICLE,
         PDSCH.PSDT,
         e.BWBH,
         e.CLBH) K 
	inner JOIN
	dbo.App_Material_Orders
	A ON k.ZLBH=a.RY AND A.BWBH = k.BWBH AND A.CLBH =k.CLBH
	INNER JOIN App_Material_Process ON App_Material_Process.OrderId = A.OrderId AND App_Material_Process.Status=1";
            #endregion

            sql = $@"SELECT   
               a.Orderid,
			   K.ZLBH,
               K.Component,
               K.CLBH,
               K.ModelName,
               K.ARTICLE,
               K.PSDT,
               K.BWBH,
              convert(varchar, a.qty  )+'/'+ convert(varchar,k.planqty) TarQty
	           FROM ( SELECT 
               e.ZLBH,
               b.ywsm Component,
               e.CLBH,
               x.XieMing ModelName,
               x.ARTICLE,
               PDSCH.PSDT,
               e.BWBH,
               SUM(CONVERT(INT, e.PlanQty)) planqty,
               SUM(CONVERT(INT, e.ActualQty)) Actual
        FROM App_Cutting_Barcodes_Edit e
            INNER JOIN bwzl b
                ON b.bwdh = e.BWBH
            INNER JOIN DDZL
                ON e.ZLBH = DDZL.DDBH
            INNER JOIN xxzl x
                ON DDZL.ARTICLE = x.ARTICLE
            INNER JOIN PDSCH
                ON e.ZLBH = PDSCH.ZLBH
        GROUP BY e.ZLBH,
                 b.ywsm,
                 x.XieMing,
				 x.article,
                 DDZL.ARTICLE,
                 PDSCH.PSDT,
                 e.BWBH,
                 e.CLBH) K 
	        inner JOIN
	        (SELECT *FROM dbo.App_Material_Orders WHERE  DepNo = '{DB.StoreLocal.Instant.CurrentDep}')
	        A ON k.ZLBH=a.RY AND A.BWBH = k.BWBH AND A.CLBH =k.CLBH
			left JOIN  App_Material_Process ON App_Material_Process.OrderId = A.OrderId 
			WHERE App_Material_Process.[Status]=1 and
			convert(date,App_Material_Process.UserDate) >= convert(date,getdate()-60)
			ORDER by a.UserDate";

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if (ta != null)
            {
                foreach (System.Data.DataRow row in ta.Rows)
                {
                    var tick = long.Parse(row["Orderid"].ToString().Substring(2, row["Orderid"].ToString().Length - 2));

                    var OrderDate = new DateTime(tick);

                    var vm = new Model_DataGrid();

                    var test = row["tarqty"].ToString();

                    vm.Sel = false;

                    vm.RY = row["Zlbh"].ToString();

                    vm.Comp = row["Component"].ToString();

                    vm.TarQty = row["tarqty"].ToString();

                    vm.Art = row["article"].ToString();

                    vm.Bwbh = row["bwbh"].ToString();

                    vm.ModelName = row["modelname"].ToString();

                    vm.DepName = row["depno"].ToString();

                    vm.ProdDate = OrderDate.ToString("yyyy-MM-dd");//DateTime.Parse(row["psdt"].ToString()).ToString("yyyy-MM-dd");

                    vm.Clbh = row["CLBH"].ToString();

                    vm.OrderID = row["orderid"].ToString();

                    _data.Add(vm);
                }
                _data.Select(x => x.RY)
                                  .OrderBy(x => x)
                                  .ToList();
            }
            //_prodate_data = _data.Select(x => x.ProdDate)
            //                     .Distinct().OrderBy(x => x)
            //                     .ToList();
            //_prodate_data.Add("All");


           // _prodate_data.Select(x => x.Any()).OrderBy(x => x);

            return _data;

        }
    }
}