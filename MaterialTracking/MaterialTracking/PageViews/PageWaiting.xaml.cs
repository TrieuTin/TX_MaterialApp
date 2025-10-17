using MaterialTracking.Class;
using MaterialTracking.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.PageViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageWaiting : ContentPage
    {
        List<Model_DataGrid> Data_search;

        

        public PageWaiting()
        {
            InitializeComponent();

            UpdateData();

            ThisContext.Title = ConvertLang.Convert.Translate_LYM("Đặt Hàng", "အမိန့်ပေးသည်။");


            //searchBar.Text = DB.StoreLocal.Instant.Ry;

            //if(DB.StoreLocal.Instant.Depname == DB.Departments.NoSew)
            //{
            //    btn_execute.Text = SysFont.Brands.Cut;
            //    btn_execute.TextColor = Class.Style.Error;
            //}


            
        }

      

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

        }

        private async Task<int> UpdateData()
        {          
            ThisContext = new Model_Waiting();

            ThisContext.ConfirmCompleted += (s,e)=>
            {
                Confirm_Clicked(s, e);
            };

           // btn_execute.IsEnabled = DB.StoreLocal.Instant.IsMaterial == "NS"?false:true;

            if (ThisContext.Data != ItemSource)
            {
                Data_search  = ThisContext.Data;

                if (Data_search == null) return 0;

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


                //btn_execute.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });

                return ItemSource.Count;
               
            }
            return 0;

        }

        
        Model_Waiting ThisContext
        {
            get => this.BindingContext as Model_Waiting;
            set
            { 
                this.BindingContext = value;

                Data_search = ItemSource;

              
                var lstpicker = Data_search.Select(x => x.ProdDate)
                           .Distinct().OrderBy(x => x)
                           .ToList();

                lstpicker.Add("All");

                lstpicker.Select(x => x.Any()).OrderBy(x => x);

                DatapickerFilter.ItemsSource = lstpicker;
            }
        }
        List<Model_DataGrid> ItemSource
        {
            get => (List<Model_DataGrid>)gv_wait.ItemsSource;
            set { gv_wait.ItemsSource = value; ThisContext.Countitems = string.Format("{0} Đơn", ItemSource.Count);
                btn_execute.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource }); }
        }
        private void Checked_Change(object sender, CheckedChangedEventArgs e)
        {

            var check = (CheckBox)sender;

            var temp = new List<Model_DataGrid>();

            if (check.IsChecked)
                foreach (var item in ItemSource)
                {
                    if (item.ColorStatus != Class.Style.SoftColor.MPink)
                    {
                        item.Sel = true;
                        temp.Add(item);
                    }
                }
            else
                foreach (var item in ItemSource)
                {
                    item.Sel = false;
                    temp.Add(item);
                }

            Data_search = ItemSource = temp;
        }
        private void Search_TextChange(object sender, TextChangedEventArgs e)
        {
            var sb = (SearchBar)sender;

            if (sb != null)
            {
                if (Data_search.Any())
                {
                    var filter = Data_search.Where(dk => dk.RY.Contains(sb.Text.ToUpper()) || dk.Art.Contains(sb.Text.ToUpper()) || dk.Comp.Contains(sb.Text.ToUpper())).ToList();

                    if (filter.Any())
                    {
                        ItemSource = new List<Model_DataGrid>(filter);
                    };
                }

            }
            else
            {
                ItemSource = Data_search;
            }
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                DB.StoreLocal.Instant.Ry = "";
            }

            //btn_execute.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource});
        }
       
        private async void Refresh_Clicked(object sender, EventArgs e)
        {
            ThisContext.IsBusy = true;
            await Task.Delay(100);
            
            var t  = await UpdateData();

            ThisContext.IsBusy = false;
          
        }
      
       
        private void Confirm_Clicked(object sender, EventArgs e)
        {            
            
            if (ThisContext.Is_confirm_completed == false) {  return; }

            var temp = Data_search.Except(ItemSource.Where(r => r.Sel && r.DepName == DB.StoreLocal.Instant.CurrentDep && r.ColorStatus != Class.Style.SoftColor.MPink).ToList()).ToList();            

            ThisContext.Data = Data_search = ItemSource = temp;                       

            btn_execute.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });

         
            ThisContext.Is_confirm_completed = false;
        }
        
        private async void On_FilterChange_Event(object sender, CheckedChangedEventArgs e)
        {
            

            ThisContext.IsBusy = true;
            await Task.Delay(10);

            //var data = ThisContext.Data;
            //bool giongNhau = !ItemSource.Except(data).Any() && !data.Except(ItemSource).Any();
            //if (giongNhau)
            Data_search = ItemSource = ThisContext.Data;


            ThisContext.SelectedProdateIndex = -1;
            ThisContext.IsBusy = false;
            ThisContext.IsFirstTime = true;
        }

        private void On_SelectProDate_Change_Event(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var prodate = picker.SelectedItem as string;

            if (prodate != "All")
            {
                if (Data_search.Any())
                {                   
                    var filter = Data_search.Where(dk => (dk.ProdDate == prodate)).ToList();

                    ItemSource = new List<Model_DataGrid>(filter);
                }
            }
            else ItemSource = Data_search;
        }
    }





    //-----------------------------------------------------------------------
    public class Model_Waiting: BaseViewModel
    {
        public event EventHandler ConfirmCompleted;

        //bool isNoSew = DB.StoreLocal.Instant.Depname == DB.Departments.NoSew;

        private bool _ac_checked = DB.StoreLocal.Instant.Depname == DB.Departments.AutoCutting;
        private bool _ns_checked = DB.StoreLocal.Instant.Depname == DB.Departments.NoSew;
        private bool _print_checked = DB.StoreLocal.Instant.Depname == DB.Departments.Printing;
        private bool _hf_checked = DB.StoreLocal.Instant.Depname == DB.Departments.HighFrequency;
        private bool _em_checked = DB.StoreLocal.Instant.Depname == DB.Departments.Embroidery;
        private bool _st_checked = DB.StoreLocal.Instant.Depname == DB.Departments.Stiching;
        private bool _os_checked = DB.StoreLocal.Instant.Depname == DB.Departments.OutSourcing;

        private bool _ac_enb;
        private bool _ns_enb;
        private bool _print_enb;
        private bool _hf_enb;
        private bool _em_enb;
        private bool _st_enb;
        private bool _os_enb;

        
        public string SearchBar_Holder { get => ConvertLang.Convert.Translate_LYM("Tìm", "ရှာဖွေ"); }
        public string rb_AC_Content { get => ConvertLang.Convert.Translate_LYM("CTĐ", "အော်တို ဖြတ်စက်ရှာဖွေ"); }
        public string rb_ST_Content { get => ConvertLang.Convert.Translate_LYM("May", "စက်ချုပ်လိုင်း"); }
        public string rb_NS_Content { get => ConvertLang.Convert.Translate_LYM("Ép Nóng", "ချုပ်ကြောင်းမပါသောဖြစ်စဥ်"); }
        public string ColumnRY { get => ConvertLang.Convert.Translate_LYM("RY", "အာဝိုင်"); }
        public string ColumnArt { get => ConvertLang.Convert.Translate_LYM("Art", "အာတီကယ်"); }
        public string ColumnComp { get => ConvertLang.Convert.Translate_LYM("Comp", "အစိတ်အပိုင်းများ"); }
        public string ColumnModelName { get => ConvertLang.Convert.Translate_LYM("ModelName", "မော်ဒယ်"); }
        public string ColumnDate { get => ConvertLang.Convert.Translate_LYM("Date", "ရက်စွဲ"); }
        public string ColumnDepID { get => ConvertLang.Convert.Translate_LYM("DepID", "ဌာန"); }






        private List<string> _prodate_data;

        private List<Model_DataGrid> _data;

        private string _selected_prodate;

        private int _selected_prodate_index = -1;
        public string SelectedProdate
        {
            get => _selected_prodate; set => SetProperty(ref _selected_prodate, value);
        }
        public int SelectedProdateIndex
        {
            get => _selected_prodate_index; set => SetProperty(ref _selected_prodate_index, value);
        }

        public ICommand Cmd_Refresh { get; }
        public ICommand Cmd_Confirm { get; }
      
      
        


        public List<Model_DataGrid> Data { get => GetData_Wait(); set => SetProperty(ref _data, value); }



        bool is_confirm_completed = false;

        public Model_Waiting()
        {
            Cmd_Refresh = new Command(Refresh);
            Cmd_Confirm = new Command(Confirm);

            _ac_enb = true;
            _ns_enb = true;
            //_print_enb = true;
            //_hf_enb = true;
            //_em_enb = true;
            _st_enb = true;
            //_os_enb = true;

           

        }
      
        private string _countitems;
        public string Countitems { get => _countitems; set => SetProperty(ref _countitems, value); }
        public bool Is_confirm_completed { get => is_confirm_completed; set => is_confirm_completed = value; }
        public bool Ac_checked { get => _ac_checked; set => SetProperty(ref _ac_checked, value); }
        public bool Ns_checked { get => _ns_checked; set => SetProperty(ref _ns_checked, value); }
        public bool Print_checked { get => _print_checked; set => SetProperty(ref _print_checked, value); }
        public bool Hf_checked { get => _hf_checked; set => SetProperty(ref _hf_checked, value); }
        public bool Em_checked { get => _em_checked; set => SetProperty(ref _em_checked, value); }
        public bool St_checked { get => _st_checked; set => SetProperty(ref _st_checked, value); }
        public bool Os_checked { get => _os_checked; set => SetProperty(ref _os_checked, value); }


        public bool Ac_enb { get => _ac_enb; set => SetProperty(ref _ac_enb, value); }
        public bool Ns_enb { get => _ns_enb; set => SetProperty(ref _ns_enb, value); }
        public bool Print_enb { get => _print_enb; set => SetProperty(ref _print_enb, value); }
        public bool Hf_enb { get => _hf_enb; set => SetProperty(ref _hf_enb, value); }
        public bool Em_enb { get => _em_enb; set => SetProperty(ref _em_enb, value); }
        public bool St_enb { get => _st_enb; set => SetProperty(ref _st_enb, value); }
        public bool Os_enb { get => _os_enb; set => SetProperty(ref _os_enb, value); }

        public List<string> Prodate_data { get => _prodate_data; set => SetProperty(ref _prodate_data, value); }
        

        private async void Confirm(object obj)
        {
            var dgv = _data.Where(r => r.Sel).ToList();

            var dep = DB.StoreLocal.Instant.CurrentDep.ToUpper();           

            if (dgv.Any())
            {   
                //Giao
                var LstDelivery = dgv.Where(r => r.Sel && r.OrderID.Contains(dep)).ToList();
                //Nhan
                var LstReceive = dgv.Where(r => r.Sel && r.DepName == dep).ToList();

                var kq = 0;
                if (LstDelivery.Any())
                {
                    var result = await Services.Alert.PopupYesNo("Hỏi", "Giao/Nhận hàng?", "Đúng", "Không");
                   
                    if ((result as string) == "Đúng")
                    {
                        var m = new Model_Mater();

                        kq = m.Insert_Process_table(LstDelivery);

                        if (kq != 0)
                        {
                            //is_confirm_completed = true;
                            //ConfirmCompleted?.Invoke(this, EventArgs.Empty); Services.DisplayToast.Show.Toast("Đơn hàng đã chuẩn bị", Color.DarkGreen);
                            foreach (var item in LstDelivery)
                            {
                                item.ColorStatus = Class.Style.Primary;
                            }
                        }
                        //else { Services.DisplayToast.Show.Toast("Không thành công", Class.Style.Error); }
                    }                   
                }
                if (LstReceive.Any())
                {
                    var result = await Services.Alert.PopupYesNo("Hỏi", "Giao/Nhận hàng?", "Đúng", "Không");
                    if ((result as string) == "Đúng")
                    {
                        var m = new Model_Mater();
                        kq = m.Insert_Process_table(LstReceive);

                    }
                }
                if (kq != 0) { is_confirm_completed = true; ConfirmCompleted?.Invoke(this, EventArgs.Empty); Services.DisplayToast.Show.Toast("Hoàn thành", Color.DarkGreen); Alarm.Sound.Completed(); }

                else
                    Services.DisplayToast.Show.Toast("Không thành công", Class.Style.Error); Alarm.Sound.Error();
            }
            else
            {
                    Services.DisplayToast.Show.Toast("DGV không bắt được sự kiện check, vui lòng thoát ra vào lại", Class.Style.Error); Alarm.Sound.Error();

            }

         
        }
       
      
        private async void Refresh(object obj)
        {
            IsBusy = true;

            await Task.Delay(500);

            //_data = GetData_Wait();


            //IsBusy = false;

        }
        bool isfirstTime = true;

        public bool IsFirstTime { get => isfirstTime; set => isfirstTime = value; }


        List<Model_DataGrid> GetData_Wait()
        {


            if (!isfirstTime && _data != null)
            {
                return _data;
            }
            isfirstTime = false;

            _data = new List<Model_DataGrid>();
           


            if (!Class.Network.Net.HasNet) return _data;

            string depno = "";

            if (Ac_checked)
                depno += "'AC',"; 

            if (Hf_checked)
                depno += "'HF',";

            if (Ns_checked)
                depno += "'NS',";

            if (Print_checked)
                depno += "'PR',";

            if (Em_checked)
                depno += "'EM',";

        

            if (St_checked)
                depno += "'ST',";

            depno = depno.TrimEnd(',');

            string sql = "";
           
            if (depno != "'ST'")

    //            sql = $@"SELECT App_Material_Process.Status,a.Orderid, K.ZLBH,
    //               K.Component,
			 //      K.CompVN,
    //               K.CLBH,
    //               K.ModelName,
    //               K.ARTICLE,
    //               K.PSDT,
    //               K.BWBH,A.DepNo,
    //              convert(varchar, a.qty  )+'/'+ convert(varchar,k.planqty) TarQty
	   //            FROM ( SELECT 
    //               e.ZLBH,
    //               b.ywsm Component,
			 //      c.CompVN,
    //               e.CLBH,
    //               x.XieMing ModelName,
    //               DDZL.ARTICLE,
    //               PDSCH.PSDT,
    //               e.BWBH,
    //               SUM(CONVERT(INT, e.PlanQty)) planqty,
    //               SUM(CONVERT(INT, e.ActualQty)) Actual
    //        FROM App_Cutting_Barcodes_Edit e
    //            INNER JOIN bwzl b
    //                ON b.bwdh = e.BWBH
    //            INNER JOIN DDZL
    //                ON e.ZLBH = DDZL.DDBH
    //            INNER JOIN xxzl x
    //                ON DDZL.ARTICLE = x.ARTICLE
    //            INNER JOIN PDSCH
    //                ON e.ZLBH = PDSCH.ZLBH
			 //   INNER JOIN ComponentName c
				//    ON b.ywsm = c.CompEng
    //        GROUP BY e.ZLBH,
    //                 b.ywsm,
    //                 x.XieMing,
    //                 DDZL.ARTICLE,
    //                 PDSCH.PSDT,
    //                 e.BWBH,
    //                 e.CLBH
				//     ,c.CompVN) K 
	   //         inner JOIN
	   //         (SELECT *FROM dbo.App_Material_Orders WHERE  DepNo in ( {depno}) )
	   //         A ON k.ZLBH=a.RY AND A.BWBH = k.BWBH AND A.CLBH =k.CLBH
			 //   left JOIN  App_Material_Process ON App_Material_Process.OrderId = A.OrderId 
		  //  WHERE (App_Material_Process.Status=0 or App_Material_Process.Status IS NULL)
				//and year(A.UserDate) = year(getdate())
				//and MONTH(A.UserDate) >= MONTH(GETDATE())-3
			 //   ORDER by   a.UserDate desc";
            sql = $@"SELECT ord.OrderId,
		                        pro.[Status],
		                        Ord.RY ZLBH,
		                        ord.Component,
		                        CN.CompVN,
		                        ord.BWBH,
		                        ord.CLBH,
		                        x.XieMing ModelName,
		                        ord.DepNo ,
		                        d.ARTICLE,   
		                        P.PSDT,
		                        convert(varchar, ord.qty  )+'/'+ convert(varchar,d.Pairs)  TarQty,
		                        ord.UserDate
                        FROM App_Material_Orders ord 
	                        INNER JOIN DDZL D ON D.ZLBH = ord.RY
	                        INNER JOIN  XXZL X ON x.ARTICLE = D.ARTICLE
	                        INNER JOIN PDSCH P ON p.ZLBH = ord.RY
	                        LEFT JOIN ComponentName CN ON ord.Component =cn.CompEng
	                        LEFT JOIN App_Material_Process pro ON ord.OrderId = pro.OrderId 
                        WHERE year(ord.UserDate) = year(getdate())
				                        AND MONTH(ord.UserDate) >= MONTH(GETDATE())-3
				                        AND (ord.OrderId not in (SELECT Orderid FROM App_Material_Process )
				                        OR  pro.[Status]=0)
				                        AND ord.[DepNo]  in ( {depno})
                        ORDER BY ord.UserDate desc";
            else
            {
                string sqlAppend = "";
                if (DB.StoreLocal.Instant.CurrentDep == depno)
                    sqlAppend = "";
                else
                    sqlAppend = $"And SUBSTRING(a.OrderId, 1, 2)='{DB.StoreLocal.Instant.CurrentDep}'";

                sql = $@"SELECT 
		            b.[Status], 
		            a.OrderId,
		            a.RY ZLBH,
		            a.Component,
		            p.CompVN,
		            a.CLBH,
		            a.BWBH,
		            d.ARTICLE,
		            pdsch.PSDT,
		            xxzl.XieMing ModelName, 
		            a.depno,
	
		            convert(varchar, a.qty  )+'/'+ convert(varchar,d.Pairs) TarQty
                FROM App_Material_Orders a left join App_Material_Process
                b on (a.OrderId=b.OrderId) inner join ComponentName p 
                on p.CompEng = a.Component inner join DDZL d 
                on d.DDBH =a.RY inner join pdsch
                on a.RY = pdsch.ZLBH inner join xxzl 
                on xxzl.ARTICLE = d.ARTICLE

                where (b.[Status]=0 or b.[Status] is null)
                and a.depno = {depno} And  year(a.UserDate) = year(getdate()) 
				and month(a.UserDate) >= month(GETDATE())-3 {sqlAppend}";
            }

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            var ListCompENG_VN = new List<(string ENG, string VN)>();

            var compTable = DB.SQL.ConnectDB.Connection.FillDataTable("SELECT * FROM Componentname");

            foreach (System.Data.DataRow item in compTable.Rows)
            {
                (string, string) temp1;

                temp1.Item1 = item["compeng"].ToString();
                temp1.Item2 = item["compvn"].ToString();

                ListCompENG_VN.Add(temp1);
            }

            if (ta.Rows.Count != 0)
            {
                foreach (System.Data.DataRow row in ta.Rows)
                {
                    var tick = long.Parse(row["Orderid"].ToString().Substring(2, row["Orderid"].ToString().Length - 2));

                    var OrderDate = new DateTime(tick);

                    // DB.DataLocal.Table.Insert_OrderStore(ta);

                    var stt = row["Status"].ToString();

                    var compText = row["Component"].ToString();

                    var compvn = row["CompVN"]?.ToString() ?? string.Empty;


                    var newtext = new[] { new string(compText.Where(c => char.IsLetter(c) || c == ' ').ToArray()).Trim() };


                    if (newtext[0] != "")
                    {
                        if (string.IsNullOrEmpty(compvn))
                        {
                            var int_Exist = ListCompENG_VN.FindIndex(x => x.ENG == newtext[0]);
                            if (int_Exist > 0)
                            {

                                var tv = ListCompENG_VN[ListCompENG_VN.FindIndex(x => x.ENG == newtext[0])];

                                if (!string.IsNullOrEmpty(tv.VN))
                                    compText = compText.Replace(newtext[0], tv.VN);

                                else
                                    compText = row["Component"]?.ToString() ?? string.Empty;

                            }
                            else compText = row["Component"].ToString();

                        }
                        else
                            compText = compText.Replace(newtext[0], compvn);

                    }
                    else
                    {
                        compText = "";
                    }

                    _data.Add(new Model_DataGrid
                    {
                        Sel = false,

                        RY = row["Zlbh"].ToString(),

                        Comp = row["Component"].ToString(),

                        CompVN = compText,

                        TarQty = row["TarQty"].ToString(),

                        Art = row["article"].ToString(),

                        Bwbh = row["bwbh"].ToString(),

                        ModelName = row["modelname"].ToString(),

                        ProdDate = OrderDate.ToString("yyyy-MM-dd"),

                        Clbh = row["CLBH"].ToString(),

                        OrderID = row["Orderid"].ToString(),

                        DepName = row["DepNo"].ToString(),

                        ColorStatus = string.IsNullOrEmpty(stt) ? Class.Style.SoftColor.MPink : Class.Style.Primary
                    });
                }
            }
            //_prodate_data = _data.Select(x => x.ProdDate)
            //                       .Distinct().OrderBy(x => x)
            //                       .ToList();
            //_prodate_data.Add("All");


            //_prodate_data.Select(x => x.Any()).OrderBy(x => x);

            return _data;
        }
    }
}