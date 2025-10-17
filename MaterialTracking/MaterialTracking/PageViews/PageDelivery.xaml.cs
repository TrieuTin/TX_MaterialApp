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
    public partial class PageDelivery : ContentPage
    {
        List<Model_DataGrid> Data_search;
        public PageDelivery()
        {
            InitializeComponent();
            UpdateData();
            ThisContext.Title = ConvertLang.Convert.Translate_LYM(string.Format("Kho {0}",DB.StoreLocal.Instant.CurrentDep), "စတိုးဆိုင်");
            
            searchBar.Text = DB.StoreLocal.Instant.Ry;

          
        }

      

        private async Task<int> UpdateData()
        {
            ThisContext = new Model_Delivery();

            ThisContext.ConfirmCompleted += (s, e) =>
                {
                    Confirm_Clicked(s,e);
                };

            if (ThisContext.Data != ItemSource)
            {
                Data_search = ThisContext.Data;

                if (Data_search!=null)
                {

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

                }


                  //ItemSource = ThisContext.Data;

                //btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });

              
            }
            return ItemSource.Count;
        }
        Model_Delivery ThisContext
        {
            get => this.BindingContext as Model_Delivery;
            set
            {
                this.BindingContext = value;
               

                Data_search = ItemSource;

              
               // btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource});

                var lstpicker = Data_search.Select(x => x.ProdDate)
                            .Distinct().OrderBy(x => x)
                            .ToList();

                lstpicker.Add("All");

                lstpicker.Select(x => x.Any()).OrderBy(x => x);

                DatePickerFilter.ItemsSource = lstpicker;
            }
        }
        List<Model_DataGrid> ItemSource
        {
            get => (List<Model_DataGrid>)gv_deliverry.ItemsSource;
            set 
            {
                gv_deliverry.ItemsSource = value;
               // ThisContext.Countitems = string.Format("{0} Đơn", ItemSource.Count); 
               // btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });

               

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
                    }
                    else
                    {
                        ItemSource = null;
                    }
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

           // btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });
        }

        private void Confirm_Clicked(object sender, EventArgs e)
        {
            var isconfirm = ThisContext.Is_confirm_completed;

            if (isconfirm == false) { return; }
                     
           var temp = Data_search.Except(ItemSource.Where(r => r.Sel).ToList()).ToList();

           ThisContext.Data = Data_search = ItemSource = temp;

        
            
            btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });

           // ThisContext.Countitems = string.Format("{0} Đơn", ItemSource.Count);

            ThisContext.Is_confirm_completed = false;

            
        }

        private async void Refresh_Clicked(object sender, EventArgs e)
        {
            ThisContext.IsBusy = true;
            await Task.Delay(100);

            ItemSource = ThisContext.Data;

            ThisContext.IsBusy = false;

          
        }

        private async void On_FilterChange_Event(object sender, CheckedChangedEventArgs e)
        {
            if (Data_search !=null)
            {
                //var filter = new List<Model_DataGrid>();

                // if (ThisContext.Hf_checked)                 
                //     foreach (var item in Data_search)                    
                //         if (item.DepName == "HF") filter.Add(item);                                                

                // if (ThisContext.Ns_checked)
                //     foreach (var item in Data_search)
                //         if (item.DepName == "NS") filter.Add(item);

                // if (ThisContext.Print_checked)
                //     foreach (var item in Data_search)
                //         if (item.DepName == "PR") filter.Add(item);

                // if (ThisContext.Em_checked)
                //     foreach (var item in Data_search)
                //         if (item.DepName == "EM") filter.Add(item);

                // if (ThisContext.Os_checked)
                //     foreach (var item in Data_search)
                //         if (item.DepName == "OS") filter.Add(item);

                // if (ThisContext.St_checked)
                //     foreach (var item in Data_search)
                //         if (item.DepName == "ST") filter.Add(item);

                ThisContext.IsBusy = true;
                await Task.Delay(10);
                Data_search = ItemSource = ThisContext.Data;
                //ItemSource = filter;

                ThisContext.SelectedProdateIndex = -1;

                ThisContext.IsBusy = false;
            }

        }

        private void On_SelectProDate_Change_Event(object sender, EventArgs e)
        {
            var picker = sender as Picker;
            var prodate = picker.SelectedItem as string;

            if (prodate != "All")
            {
                if (Data_search != null)
                {
                    var filter = Data_search.Where(dk => dk.ProdDate == prodate).ToList();
                    ItemSource = new List<Model_DataGrid>(filter);
                }
            }
            else
            {
                ItemSource = Data_search;
            }                     
        }        
    }
    public class Model_Delivery: BaseViewModel
    {

        public string SearchBar_Holder { get => ConvertLang.Convert.Translate_LYM("Tìm", "ရှာရန်"); }
        public string rb_AC_Content { get => ConvertLang.Convert.Translate_LYM("CTĐ", "အော်တို ဖြတ်စက်"); }
        public string rb_NS_Content { get => ConvertLang.Convert.Translate_LYM("Ép Nóng", "ချုပ်ကြောင်းမပါသောဖြစ်စဥ်"); }
        public string rb_ST_Content { get => ConvertLang.Convert.Translate_LYM("May", "စက်ချုပ်လိုင်း"); }
        public string Column_RY { get => ConvertLang.Convert.Translate_LYM("RY", "အာဝိုင်"); }
        public string Column_Art { get => ConvertLang.Convert.Translate_LYM("Art", "အာတီကယ်"); }
        public string Column_Comp { get => ConvertLang.Convert.Translate_LYM("Comp", "အစိတ်အပိုင်းများ"); }
        public string Column_Model { get => ConvertLang.Convert.Translate_LYM("Model", "မော်ဒယ်"); }
        public string Column_Qty { get => ConvertLang.Convert.Translate_LYM("Qty", "အရေအတွက်"); }
        public string Column_Date { get => ConvertLang.Convert.Translate_LYM("Date", "ရက်စွဲ"); }


        public event EventHandler ConfirmCompleted;
        public Model_Delivery()
        {
           // Cmd_Confirm = new Command(Update_Confirm);
            Cmd_Combine = new Command(Combine);
            Cmd_Refresh = new Command(Refresh);


            _ac_enb = true;
            _ns_enb = true;
            _print_enb = true;
            _hf_enb = true;
            _em_enb = true;
            _st_enb = true;
            _os_enb = true;

            GetComp();
        }

        private void Refresh(object obj)
        {
            GetData_Prepair();
        }

        bool is_confirm_completed = false;

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

        public bool Is_confirm_completed { get => is_confirm_completed; set => is_confirm_completed = value; }

        List<Button> VN_Comp = new List<Button> 
        { 
            new Button{Text ="Lưỡi gà"},
            new Button{Text ="Lót lưỡi gà"},
            new Button{Text ="Mũi thân"},
            new Button{Text ="Tăng cường thân"},
            new Button{Text ="Đệm"},
            new Button{Text ="Lót lưỡi gà trên-dưới"},
            new Button{Text ="Trang trí thân"},
            new Button{Text ="Bao mũi"},
            new Button{Text ="Thân"},
            new Button{Text ="Hậu"},
            new Button{Text ="Vòng cổ"},
        };
        List<Button> ENG_Comp = new List<Button>
        {
            new Button{Text ="Tongue"},
            new Button{Text ="Tongue lining"},
            new Button{Text ="Quater vamp"},
            new Button{Text ="Quarter reinforcement"},
            new Button{Text ="Eyestay"},
            new Button{Text ="Top tongue lining/  bottom tongue lining"},
            new Button{Text ="Quater deco"},
            new Button{Text ="Toe cap"},
            new Button{Text ="Quater"},
            new Button{Text ="Heel"},
            new Button{Text ="Collar"},
        };

        List<(Button Eng, Button VN)> ListComp;

        void GetComp()
        {
            string sql = @"SELECT * FROM ComponentName order by CompVN";

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if(ta.Rows.Count > 0)
            {
                ListComp = new List<(Button Eng, Button VN)>();

                foreach (System.Data.DataRow item in ta.Rows)
                {
                    (Button eng, Button vn) temp;

                    temp.eng = new Button { Text = item["CompEng"].ToString() };
                    temp.vn = new Button { Text = item["CompVN"].ToString() };

                    ListComp.Add(temp);
                }
            }

        }


        private async void Combine(object obj)
        {
            var dgv = _data.Where(r => r.Sel).ToList();
            if (dgv.Any())
            {               
                var vn_name = await Services.Alert.PopupMenu("Chọn Tên Component", ListComp.Select(x => x.VN).ToList(), Class.Style.Blue);

                if (vn_name is null) return;

                var eng_name = ListComp[ListComp.FindIndex(x => x.VN.Text == vn_name)].Eng.Text;

                var answer =(string) await Services.Alert.PopupYesNo("Hỏi", "Xác nhận?", "Đúng", "Không");


                if (answer  == "Đúng")
                {
                    var m = new Model_Mater();

                    var kq = Func_Combine(dgv, vn_name, eng_name);

                    if (kq != false) 
                    
                        is_confirm_completed = true; ConfirmCompleted?.Invoke(this, EventArgs.Empty); Services.DisplayToast.Show.Toast("Hoàn thành", Color.DarkGreen); Alarm.Sound.Completed(); 
                    
                    if(kq !=true)
                        Services.DisplayToast.Show.Toast("Không thành công", Class.Style.Error); Alarm.Sound.Error();

                    if(kq ==null)
                        Services.DisplayToast.Show.Toast("Không cùng lệnh", Class.Style.Error); Alarm.Sound.Error();

                    
                }
            }
        }
        private async void Update_Confirm(object obj)
        {
            var dgv = _data.Where(r => r.Sel).ToList();

            switch (DB.StoreLocal.Instant.Depname)
            {
                case DB.Departments.NoSew:

                    {
                        if (dgv.Any())
                        {
                            
                            var result = await Services.Alert.PopupYesNo("Hỏi", "Xác nhận?", "Đúng", "Không");

                            if ((result as string) == "Đúng")
                            {
                                var m = new Model_Mater();
                                var kq = m.Update_Process_table(dgv);

                                if (kq != 0) { is_confirm_completed = true; ConfirmCompleted?.Invoke(this, EventArgs.Empty); Services.DisplayToast.Show.Toast("Hoàn thành", Color.DarkGreen); Alarm.Sound.Completed(); }

                                else
                                    Services.DisplayToast.Show.Toast("Không thành công", Class.Style.Error); Alarm.Sound.Error();
                            }
                        }
                        break;
                    }
                case DB.Departments.Lazer:
                case DB.Departments.AutoCutting:
                    {
                        if (dgv.Any())
                        {
                            var result = await Services.Alert.PopupYesNo("Hỏi", "Xác nhận xóa?", "Đúng", "Không");

                            if ((result as string) == "Đúng")
                            {
                                var m = new Model_Mater();

                                var kq = m.AC_DeleteItem(dgv);

                                if (kq != 0) { is_confirm_completed = true; ConfirmCompleted?.Invoke(this, EventArgs.Empty); Services.DisplayToast.Show.Toast("Đã xóa", Color.DarkGreen); }
                                else { Services.DisplayToast.Show.Toast("Không thành công", Class.Style.Error); }
                            }

                        }
                        break;
                    }
                case DB.Departments.HighFrequency:
                    break;
                case DB.Departments.Printing:
                    break;
                case DB.Departments.Embroidery:
                    break;
                case DB.Departments.Stiching:
                    {
                        var result = await Services.Alert.PopupYesNo("Hỏi", "Xác nhận?", "Đúng", "Không");

                        if ((result as string) == "Đúng")
                        {
                            var m = new Model_Mater();
                            var kq = m.Update_Process_table(dgv);

                            if (kq != 0) { is_confirm_completed = true; ConfirmCompleted?.Invoke(this, EventArgs.Empty); Services.DisplayToast.Show.Toast("Hoàn thành", Color.DarkGreen); Alarm.Sound.Completed(); }

                            else
                                Services.DisplayToast.Show.Toast("Không thành công", Class.Style.Error); Alarm.Sound.Error();
                        }
                        break;
                    }
                case DB.Departments.OutSourcing:                    
                default:
                    break;
            }

          
        }

        private List<Model_DataGrid> _data;
        public ICommand Cmd_Refresh { get; }
        
        public ICommand Cmd_Combine { get; }
        bool isNoSew = DB.StoreLocal.Instant.Depname == DB.Departments.NoSew;
        public List<Model_DataGrid> Data { get => GetData_Prepair(); set => SetProperty(ref _data, value); }

        private string _countitems;
        public string Countitems { get => _countitems; set => SetProperty(ref _countitems, value); }
        public bool iNS { get => isNoSew; }
      
        bool? Func_Combine(List<Model_DataGrid> DataGridview,string namevn ,string nameeng)
        {
            int qty=0;
            //Create a new ID of Component
            string ry = DataGridview[0].RY;

            string newID = string.Format("{0}{1}", DB.StoreLocal.Instant.CurrentDep, DateTime.Now.Ticks);

            //Check exist ID in App_Details_Combine table

            bool allSameRY = DataGridview.Select(x => x.RY).Distinct().Count() == 1;

            if (allSameRY)
            {
                foreach (var exist_id in DataGridview)
                {
                    string sql = $@"SELECT * FROM App_Details_Combine where CompID_Old ='{exist_id.OrderID}'";

                    var dt = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                    var tarQty = exist_id.TarQty.Split('/');

                    qty = int.Parse(tarQty[0]);

                    if (dt.Rows.Count > 0) return false;
                    else
                    {

                        if (int.Parse(tarQty[0]) != int.Parse(tarQty[1])) return false;

                        else
                        {
                            sql = @"INSERT INTO [dbo].[App_Details_Combine]
                                        ([CompID_old]
                                        ,[CompID_new])
                                        VALUES
                                        (@oldID
                                        , @newID)";

                            string[] arr = { exist_id.OrderID, newID };

                            var result = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, arr);


                        }
                    }
                }
                string sql2 = @"INSERT INTO [dbo].[App_Combine_Comp]
                               ([CompID]
                               ,[NameComp]
                               ,[CombDate]
                               ,[Qty]
                               ,[Staff]
                               ,[RY]
                               ,[EngName])
                         VALUES
                               (@CompID
                               ,@NameComp
                               ,@CombDate
                               ,@Qty
                               ,@Staff
                               ,@RY
                               ,@EngName)";

                string[] arr2 = { newID, namevn, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), qty.ToString(), DB.StoreLocal.Instant.UserName,ry , nameeng };

                return DB.SQL.ConnectDB.Connection.Update_Parameter(sql2, arr2) > 0;
            }
            else
            {
                return null;
            }


            
        }
        int solanchay = 0;
        List<Model_DataGrid> GetData_Prepair()
        {
            _data = new List<Model_DataGrid>();
            if(solanchay !=2)
            {
                solanchay++;
                return _data;
            }
          

            //string dep = "";
            //if (Ac_checked)
            //    dep += "'AC',";

            //if (Hf_checked)
            //    dep += "'HF',";

            //if (Ns_checked)
            //    dep += "'NS',";

            //if (Print_checked)
            //    dep += "'PR',";

            //if (Em_checked)
            //    dep += "'EM',";

            //dep += "'OS',";

            //if (St_checked)
            //    dep += "'ST',";

            //dep = dep.Substring(0, dep.Length - 1);
            if (DB.StoreLocal.Instant.Depname == DB.Departments.NoSew)
            {

                string sql = $@"SELECT a.Orderid,
		K.ZLBH,
               K.Component,
			   k.CompVN ,
               K.CLBH,
               K.ModelName,
               K.ARTICLE,
               K.PSDT,
               K.BWBH,A.DepNo,
              convert(varchar, a.qty  )+'/'+ convert(varchar,k.planqty) TarQty
	           FROM ( 
			   SELECT 
               e.ZLBH,
               b.ywsm Component,
			   c.CompVN ,
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
			left join ComponentName c
				ON b.ywsm = c.CompEng
        GROUP BY e.ZLBH,
                 b.ywsm,
                 x.XieMing,
                 DDZL.ARTICLE,
                 PDSCH.PSDT,
                 e.BWBH,
                 e.CLBH
				 ,c.CompVN) K 
	        inner JOIN
	        (SELECT *FROM dbo.App_Material_Orders WHERE  DepNo in ( '{DB.StoreLocal.Instant.CurrentDep}'))
	        A ON k.ZLBH=a.RY AND A.BWBH = k.BWBH AND A.CLBH =k.CLBH 
			left JOIN  App_Material_Process p ON p.OrderId = A.OrderId 
		WHERE p.[Status]=1
		and convert(date,p.UserDate) >= convert(date,getdate()-45)
			ORDER by   a.UserDate desc";

                if (!Class.Network.Net.HasNet) return _data;

                var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                if (ta != null)
                {
                    var ListCompENG_VN = new List<(string ENG, string VN)>();

                    var compTable = DB.SQL.ConnectDB.Connection.FillDataTable("SELECT * FROM Componentname");

                    foreach (System.Data.DataRow item in compTable.Rows)
                    {
                        (string, string) temp1;

                        temp1.Item1 = item["compeng"].ToString();
                        temp1.Item2 = item["compvn"].ToString();

                        ListCompENG_VN.Add(temp1);
                    }

                    foreach (System.Data.DataRow row in ta.Rows)
                    {
                        var compText = row["Component"].ToString();

                        var compvn = row["CompVN"]?.ToString() ?? string.Empty;

                        //lay phan text cua component
                        var newtext = new[] { new string((compText?.Where(c => char.IsLetter(c) || c == ' ').ToArray() ?? Array.Empty<char>())).Trim() };

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

                        var ta2 = DB.SQL.ConnectDB.Connection.FillDataTable($"SELECT * FROM App_Details_Combine where CompID_old in ('{row["Orderid"].ToString()}')").Rows.Count > 0;




                        var tick = long.Parse(row["Orderid"].ToString().Substring(2, row["Orderid"].ToString().Length - 2));

                        var DeliveryDate = new DateTime(tick);

                        var vm = new Model_DataGrid();

                        var test = row["tarqty"].ToString();

                        vm.Sel = false;

                        vm.RY = row["Zlbh"].ToString();

                        vm.Comp = row["Component"].ToString();

                        vm.CompVN = compText;

                        vm.TarQty = row["tarqty"].ToString();

                        vm.Art = row["article"].ToString();

                        vm.Bwbh = row["bwbh"].ToString();

                        vm.ModelName = row["modelname"].ToString();

                        vm.ProdDate = DeliveryDate.ToString("yyyy-MM-dd");//DateTime.Parse(row["psdt"].ToString()).ToString("yyyy-MM-dd");

                        vm.Clbh = row["CLBH"].ToString();

                        vm.OrderID = row["orderid"].ToString();

                        vm.DepName = row["DepNo"].ToString();

                        vm.ColorStatus = ta2 ? Class.Style.Red : Class.Style.SoftColor.MGreen;

                        _data.Add(vm);
                    }

                }

                //_prodate_data = _data.Select(x => x.ProdDate)
                //                      .Distinct().OrderBy(x => x)
                //                      .ToList();


                //_prodate_data.Add("All");


                //_prodate_data.Select(x => x.Any()).OrderBy(x => x);
            }
            return _data;
        }
    }
}