using MaterialTracking.Class;
using MaterialTracking.DB;
using MaterialTracking.Models;
using MaterialTracking.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tools;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;
using kolor = MaterialTracking.Class.Style;
using Lottie.Forms;

namespace MaterialTracking.PageViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    
    public partial class Master : ContentPage
    {
        //List<Model_DataGrid> Data_search;
        Timer Timer_Notifies;

        //bool isNoSew = DB.StoreLocal.Instant.IsMaterial.ToUpper() == "NS";
        public Master()
        {
            InitializeComponent();
            Timer_Notifies = Timer_Notifies == null ? Timer_Notifies = new Tools.Timer(TimeSpan.FromSeconds(3), Notification_Tick) : Timer_Notifies;

            if (!Timer_Notifies.IsRunning) Timer_Notifies.Start();

            ThisContext = new Model_Mater();

          

            LabelConvert();

            Notification_Tick();

            //Co Yeu Cau hay khong
            if ((ThisContext as Model_Mater).HasRQ) Services.Alert.PopupShowRequest();

        }

        private void LabelConvert()
        {
            lbl_machine.Text = ConvertLang.Convert.Translate_LYM("Máy", "ဖြတ်စက်");
            lbl_input.Text = ConvertLang.Convert.Translate_LYM("Nhập code", "လက်ထည့်ပါ။");
            lbl_Scan.Text = ConvertLang.Convert.Translate_LYM("Quét", "စကင်န်");
            lbl_store.Text = ConvertLang.Convert.Translate_LYM("Kho", "စတိုးဆိုင်");
            lbl_TB.Text = ConvertLang.Convert.Translate_LYM("Thông Báo", "သတိပေးချက်");
            lbl_Ver.Text = ConvertLang.Convert.Translate_LYM(string.Format("Version: {0}", DB.StoreLocal.Instant.Version), "ဗားရှင်း");
        }

        private void Notification_Tick()
        {
            try
            {
                var notify = ThisContext.Notification();

                ThisContext.Isplay = notify > 0;

                if (ThisContext.Isplay) 
                    btn_bell.PlayAnimation();
                

                ThisContext.BellColor = ThisContext.Isplay ? Class.Style.SoftColor.MPink : Class.Style.ColdKidsSky.BlueSea;

                lbl_TB.Text = string.Format($"{ConvertLang.Convert.Translate_LYM("TB", "သတိပေးချက်")}({0}{1})", notify >= 1000 ? int.Parse(notify.ToString().Substring(0, notify.ToString().Length - 3)) : notify, notify >= 1000 ? "K+" : "");
            }
            catch (Exception)
            {

                
            }
          
        }
        Model_Mater ThisContext
        {
            get => BindingContext as Model_Mater;
            set
            {
                BindingContext = value;
                //Data_search = ItemSource;

                // btn_execute.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource }); ;
            }
        }
        #region oldcode 
        //List<Model_DataGrid> ItemSource
        //{
        //   // get => (List<Model_DataGrid>)gv.ItemsSource;
        //    //set => gv.ItemsSource =  value; 
        //}

        //private void Checked_Change(object sender, CheckedChangedEventArgs e)
        //{

        //    var check = (CheckBox)sender;

        //    if (true)
        //    {
        //        var temp = new List<Model_DataGrid>();

        //        if (check.IsChecked)
        //            foreach (var item in ItemSource)
        //            {
        //                item.Sel = true;
        //                temp.Add(item);
        //            }
        //        else
        //            foreach (var item in ItemSource)
        //            {
        //                item.Sel = false;
        //                temp.Add(item);
        //            }

        //      Data_search  = ItemSource = temp;


        //    }



        //}
        //private void UpdateData()
        //{
        //    ThisContext = new Model_Mater();

        //    Data_search = ItemSource = ThisContext.Data;

        //  //  btn_execute.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });

        // //   ThisContext.Countitems = string.Format("{0} Đơn", ItemSource.Count);

        //  //  if (checkedAll.IsChecked == true) checkedAll.IsChecked = false;
        //}
        //private void Order_Clicked(object sender, EventArgs e)
        //{
        //    UpdateData();


        //}
        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    if(DB.StoreLocal.Instant.Myfac == MyFactory.LHG)
        //        ThisContext.Timer_load.Stop();

        //}
        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    //var navigationStack = Application.Current.MainPage.Navigation.NavigationStack;
        //    //Services.DisplayToast.Show.Toast(navigationStack.Count.ToString(), Class.Style.Purple.DPurple);

        //    ThisContext = new Model_Mater();
        //}


        //private void btn_Refresh(object sender, EventArgs e)
        //{
        //    UpdateData();
        //}



        //private void Press_Search(object sender, EventArgs e)
        //{
        //    var sb = (SearchBar)sender;

        //    if (sb != null)
        //    {
        //        if (Data_search.Any())
        //        {
        //            var filter = Data_search.Where(dk => dk.RY.Contains(sb.Text.ToUpper())).ToList();

        //            if (filter.Any())
        //            {
        //                ItemSource = (filter);
        //            };
        //        }
        //       // btn_execute.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });
        //    }
        //    //else
        //    //{
        //    //    ItemSource = Data_search;
        //    //}

        //   // btn_execute.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });

        //    ThisContext.Countitems = string.Format("{0} Đơn", ItemSource.Count);
        //}

        #endregion

        

    }

    //--------------------------------------------------------------------------------------
    public class Model_DataGrid
    {
        public string MachineNO { set; get; }
        public bool Sel { get; set; }
        public string OrderID { get; set; }
        public string RY { get; set; }
        public string Art { get; set; }
        public string Comp { get; set; }
        public string CompVN { get; set; }
        public string ModelName { set; get; }
        public string TarQty { get; set; }
        public string Bwbh { get; set; }
        public string Clbh { get; set; }
        public string ProdDate { set; get; }
        public DateTime UserDate{ set; get; }
        public string DepName { get; set; }
        
        public Color ColorStatus { get; set; }
        public System.Windows.Input.ICommand ClickCommand { get; set; }
    }
    //--------------------------------------------------------------------------------------
    public class BarcodeModel
    {
        public string Result { get; set; }
    }
    //--------------------------------------------------------------------------------------
    public class Model_Mater : BaseViewModel
    {
        private BarcodeModel _barcode;

      
       
        bool isNoSew = DB.StoreLocal.Instant.Depname == Departments.NoSew;
        public BarcodeModel Barcode
        {
            get => _barcode; 
            set { SetProperty(ref _barcode, value); }
        }

        Tools.Timer _timer;
       


        public ICommand Cmd_Scan { get; }
        public ICommand ShowRQCommand { get; }
        public ICommand Cmd_orders { get; }
        public ICommand Cmd_Inventory { get; }
        public ICommand Cmd_Notify { get; }

        public ICommand Cmd_Bell{ get; }      
        public ICommand Cmd_Del{ get; }      
        public ICommand Cmd_History { get; }
   
        public ICommand Cmd_Refresh { get; }
        public ICommand Cmd_Machine { get; }
        public ICommand Cmd_DataLocal_Click { get; }
        public ICommand Cmd_Handle_input { get; }

        

  
        public Timer Timer_load { get => _timer; set => _timer= value;  }



        private string _countitems;
       
        public string Countitems { get => _countitems; set =>SetProperty(ref _countitems , value); }

        Color _bellColor = Class.Style.ColdKidsSky.BlueSea;
        bool _isplay;
        public Color BellColor { get => _bellColor; set => SetProperty(ref _bellColor, value); }
        public bool Isplay { get => _isplay;  set => SetProperty(ref _isplay, value); }
        public  Model_Mater()
        {
            Barcode = new BarcodeModel();            

        
            //Timer_load = Timer_load == null ? Timer_load = new Tools.Timer(TimeSpan.FromSeconds(2), Listening_Tick) : Timer_load;

            //if (!Timer_load.IsRunning) Timer_load.Start();

            switch (DB.StoreLocal.Instant.Depname)
            {
                case Departments.NoSew:                     
                    Title = ConvertLang.Convert.Translate_LYM("Ép Nóng", "ချုပ်ကြောင်းမပါသောဖြစ်စဥ်");
                    break;
                case Departments.AutoCutting:
                    Title = ConvertLang.Convert.Translate_LYM("CTĐộng", "အော်တို ဖြတ်စက်");
                    break;
                case Departments.HighFrequency:
                    Title = "Ép Cao Tầng";
                    break;
                case Departments.Printing:
                    Title = "In Lụa";
                    break;
                case Departments.Embroidery:
                    Title = "Thêu";
                    break;
                case Departments.Stiching:
                    Title = ConvertLang.Convert.Translate_LYM("May", "စက်ချုပ်လိုင်း");
                    break;
                case Departments.Lazer:
                    Title = ConvertLang.Convert.Translate_LYM( "Laser","Laser");
                    break;                            
                default:
                    Title = ConvertLang.Convert.Translate_LYM("Người dùng", "User");
                    break;
            }

            ShowRQCommand = new Command(Show_RQ);

            Cmd_Scan = new Command(ScanBarcode);
            
            Cmd_Inventory = new Command(Inventory);

            Cmd_Notify = new Command(Complete);
            
            Cmd_Bell = new Command(ShowNotify);
            
            Cmd_Del = new Command(DeleteDatalocal);
            
            Cmd_History= new Command(LoadHistory);
            
            Cmd_orders= new Command(Check_Order);

            Cmd_Refresh = new Command(Refresh);

            Cmd_Machine = new Command(Open_PageMachine);

            Cmd_DataLocal_Click = new Command(Open_Datalocal_Click);

            Cmd_Handle_input = new Command(Input_Code);
                                   
        }

        private void Show_RQ(object obj)
        {
            if (HasRQ) Services.Alert.PopupShowRequest();
        }

        public bool HasRQ
        {
            get
            {
                string sql = $"SELECT top(1)* FROM App_material_Request where WareHouse ='{DB.StoreLocal.Instant.CurrentDep}'";
                return DB.SQL.ConnectDB.Connection.FillDataTable(sql).Rows.Count > 0;
            }
        }

        private async void Open_Datalocal_Click(object obj)
        {
            IsBusy = true;

            await Task.Delay(500);
            await Application.Current.MainPage.Navigation.PushAsync(new ManagerDBLocalView());
            IsBusy = false;

        }

        void Set_Notify(int num)
        {
            if (num != 0)
            {
                BellColor = kolor.Red;
                
            }
            else  BellColor = kolor.ColdKidsSky.BlueSea; 
          
        }
        public int Notification ()
        {
            var dn = DB.StoreLocal.Instant.Depname;
            int temp = 0;
            System.Data.DataTable ta;
            switch (dn)
            {
                case Departments.Stiching:
                    {
                        ta = DB.SQL.ConnectDB.Connection.FillDataTable(@"SELECT count( orderid ) tong FROM App_Material_Orders o 
where o.OrderId  in
(SELECT  d.orderid FROM App_Material_process d where d.[Status]=0) and o.DepNo='ST'");
                        if (ta.Rows.Count > 0)
                        {
                            if (int.Parse(ta.Rows[0][0].ToString()) > 0)
                                temp = int.Parse(ta.Rows[0][0].ToString());
                            else temp = 0;
                        }
                        return temp;
                    }
                case Departments.NoSew:
                    {
                        ta = DB.SQL.ConnectDB.Connection.FillDataTable(@"SELECT count( orderid ) tong FROM App_Material_Orders o 
where o.OrderId  in
(SELECT  d.orderid FROM App_Material_process d where d.[Status]=1) and o.DepNo='NS'");
                        if(ta !=null)
                            if (ta.Rows.Count > 0)
                            {
                                if (int.Parse(ta.Rows[0][0].ToString()) > 0)
                                    temp = int.Parse(ta.Rows[0][0].ToString());
                                else temp = 0;
                            }
                        return temp;
                    }
                case Departments.AutoCutting:
                    {

                        ta = DB.SQL.ConnectDB.Connection.FillDataTable(@"SELECT count( orderid ) tong FROM 
                                                                                App_Material_Orders o 

                                                where o.OrderId not in (SELECT  d.orderid FROM App_Material_process d) and O.OrderId like 'AC%'");
                        if (ta!=null )
                        {
                            if (ta.Rows.Count > 0)
                                if (int.Parse(ta.Rows[0][0].ToString()) > 0)
                                    temp = int.Parse(ta.Rows[0][0].ToString());
                                else temp = 0;
                        }
                        return temp;
                    }
                case Departments.Lazer:
                    {
                        ta = DB.SQL.ConnectDB.Connection.FillDataTable(@"SELECT count( orderid ) tong FROM 
                                                                                App_Material_Orders o 

                                                where o.OrderId not in (SELECT  d.orderid FROM App_Material_process d) and O.OrderId like 'LZ%' ");
                        if(ta !=null)
                            if (ta.Rows.Count > 0)
                            {
                                if (int.Parse(ta.Rows[0][0].ToString()) > 0)
                                    temp = int.Parse(ta.Rows[0][0].ToString());
                                else temp = 0;
                            }
                        return temp;
                    }
                //case Departments.HighFrequency:
                //    break;
                //case Departments.Printing:
                //    break;
                //case Departments.Embroidery:
                //    break;
                //    break;
                //    break;
                //case Departments.OutSourcing:
                //    break;
                default:
                    return 0;
                    
            }
            return temp;
        }
      void Listening_Tick()
        {

            // var fac = DB.StoreLocal.Instant.Factory;
            //   if (fac != "LYV")
            if (DB.StoreLocal.Instant.Myfac != MyFactory.LYV)
            {

                switch (DB.StoreLocal.Instant.Depname)
                {
                    case Departments.NoSew:


                        break;
                    case Departments.AutoCutting:
                        var ta = DB.SQL.ConnectDB.Connection.FillDataTable(@"SELECT count( orderid ) tong FROM 
                                                                                App_Material_Orders o 

                                                where o.OrderId not in (SELECT  d.orderid FROM App_Material_process d)");
                        if(ta.Rows.Count > 0)
                        {
                            if (int.Parse(ta.Rows[0][0].ToString()) > 0)
                                Set_Notify(int.Parse(ta.Rows[0][0].ToString()));
                        }

                        break;
                    case Departments.HighFrequency:
                        break;
                    case Departments.Printing:
                        break;
                    case Departments.Embroidery:
                        break;
                    case Departments.Stiching:
                        break;
                    case Departments.Lazer:
                        break;
                    case Departments.OutSourcing:
                        break;
                    default:
                        break;
                }

                if (!isNoSew) //autocutting
                {
                    string sql = @"SELECT orderid FROM App_Material_Orders o 
where o.OrderId not in (SELECT  d.orderid FROM App_Material_process d)";

                    var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                    var countRow = ta.Rows.Count;

                    Set_Notify(countRow);
                }
                else
                {

                    string sql = @"SELECT * FROM(
SELECT Row_number() OVER(partition BY OrderId ORDER BY Status DESC) ROWNUMBER ,*
FROM App_Material_process 
) A WHERE A.ROWNUMBER=1 AND A.Status=0";

                    var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                    var countRow = ta.Rows.Count;


                    Set_Notify(countRow);
                }
            }
        }

      

        private List<Model_DataGrid> _data;
        public List<Model_DataGrid> Data { get => _data; set => SetProperty(ref _data, value); }
       

        private  void Refresh()
        {
         
            Listening_Tick();
           
        }
        private async void Open_PageMachine(object obj)
        {
            IsBusy = true;

            await Task.Delay(500);
            await Application.Current.MainPage.Navigation.PushAsync(new MachineNoViews());
            IsBusy = false;
        }

        public List<Model_DataGrid>Get_Data( )
        {
         
            _data = new List<Model_DataGrid>();
            string sql = "";

            
            sql = @"SELECT 
       zlbh,
       k.component,
       k.clbh,
       k.modelname,
       k.article,
       k.psdt,
       k.bwbh,
       k.planqty,
       Isnull(CONVERT(INT, his.qty), 0)- ISNULL(orders.Qty,0) Actual,
	   dep.DepName
FROM   (SELECT DISTINCT 
                        e.zlbh,
                        b.ywsm                       Component,
                        e.clbh,
                        x.xieming                    ModelName,
                        d.article,
                        p.psdt,
                        e.bwbh,
                        Sum(CONVERT(INT, e.planqty)) planqty,
						cb.DepID
        FROM   app_cutting_barcodes_edit e
               INNER JOIN bwzl b
                       ON b.bwdh = e.bwbh
               INNER JOIN ddzl d
                       ON e.zlbh = d.ddbh
               INNER JOIN xxzl x
                       ON d.article = x.article
               INNER JOIN pdsch p
                       ON e.zlbh = p.zlbh
               LEFT JOIN Cutting_Barcode cb ON cb.Barcode=e.Barcode
        WHERE  p.psdt > Dateadd(day, -Day(Dateadd(month, -1,
                                          CONVERT(DATE, Getdate())))
                        ,
                                         Dateadd(month, -1,
                                         CONVERT(DATE, Getdate())))
        GROUP  BY e.zlbh,
                  b.ywsm,
                  x.xieming,
                  d.article,
                  p.psdt,
                  e.bwbh,
                  e.clbh,
                 cb.DepID) k
       LEFT JOIN (SELECT RY,Component,BWBH,CLBH,SUM(Qty)Qty FROM app_material_orders GROUP BY RY,Component,BWBH,CLBH ) orders
              ON orders.bwbh = k.bwbh
                 AND orders.ry = k.zlbh
                 AND orders.clbh = k.clbh
                 AND orders.component = k.component
	   LEFT JOIN dbo.BDepartment dep ON dep.ID = k.DepID
       INNER JOIN (SELECT ry,
                          E.bwbh,
                          E.clbh,
                          Sum(E.actualqty) Qty
                   FROM   (SELECT actualqty,
                                  Substring(prono, Charindex('_', prono) + 1,
                                  Charindex('_', prono,
                                  Charindex('_', prono) + 1) -
                                  Charindex('_', prono)
                                  - 1)
                                          RY,
                                  Substring(prono, Charindex('_', prono,
                                                   Charindex('_',
                                                   prono) + 1
                                                   )
                                                   + 1, Charindex('_', prono,
                                                        Charindex('_', prono,
                                                        Charindex('_',
                                                        prono) + 1)+ 1) -Charindex('_', prono, Charindex('_', prono) +1)- 1) AS CLBH,
                                  Substring(prono, Charindex('_', prono,
                                                   Charindex('_',
                                                   prono,
                                                   Charindex(
                                                   '_',
                                                   prono, Charindex('_',
                                                   prono) + 1) + 1) + 1)
                                                   + 1, Len(prono))
                                          BWBH
                           FROM   app_cutting_history_edit) E
                   GROUP  BY ry,
                             E.bwbh,
                             E.clbh) his
               ON his.bwbh = K.bwbh
                  AND his.clbh = K.clbh
                  AND his.ry = K.zlbh
WHERE( orders.RY IS NULL
        OR orders.qty < K.planqty)
order by ZLBH";
            if (!Class.Network.Net.HasNet) return _data;

           // var datalocal = DB.DataLocal.Table.Get_Items_Store_AC;


            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if (ta != null)
            {
                // string.Format("{0} Đơn", ItemSource.Count);
                Countitems = string.Format("{0} Đơn", ta.Rows.Count);

                DB.DataLocal.Table.Insert_Store_AC(ta);

                foreach (System.Data.DataRow row in ta.Rows)
                {
                    _data.Add(new Model_DataGrid
                    {
                        Sel = false,

                        RY = row["Zlbh"].ToString(),

                        Comp = row["Component"].ToString(),

                        TarQty = string.Format("{0}/{1}", int.Parse(row["Actual"].ToString()), int.Parse(row["planqty"].ToString())),

                        Art = row["article"].ToString(),

                        Bwbh = row["bwbh"].ToString(),

                        ModelName = row["modelname"].ToString(),

                        DepName = row["depname"].ToString(),

                        ProdDate = DateTime.Parse(row["psdt"].ToString()).ToString("yyyy-MM-dd"),

                        Clbh = row["CLBH"].ToString(),

                        OrderID = ""

                    });
                }
            }
           

            return _data;
        }

        public List<Model_DataGrid>Get_Delivery()
        {
           var d= new List<Model_DataGrid>();

            string sql = @"SELECT a.Orderid, K.ZLBH,
       K.Component,
       K.CLBH,
       K.ModelName,
       K.ARTICLE,
       K.PSDT,
       K.BWBH,
      convert(varchar, K.planqty )+'/'+ convert(varchar,K.Actual ) TarQty
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
	INNER JOIN App_Material_Process ON App_Material_Process.OrderId = A.OrderId AND App_Material_Process.Status=0";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if (ta != null)
            {
                foreach (System.Data.DataRow row in ta.Rows)
                {

                  
                    var vm = new Model_DataGrid();


                    var test = row["tarqty"].ToString();

                    vm.Sel = false;

                    vm.RY = row["Zlbh"].ToString();

                    vm.Comp = row["Component"].ToString();

                    vm.TarQty = row["tarqty"].ToString();

                    vm.Art = row["article"].ToString();

                    vm.Bwbh = row["bwbh"].ToString();

                    vm.ModelName = row["modelname"].ToString();

                    vm.ProdDate = DateTime.Parse(row["psdt"].ToString()).ToString("yyyy-MM-dd");

                    vm.Clbh = row["CLBH"].ToString();

                    vm.OrderID = row["orderid"].ToString();

                    d.Add(vm);
                }
                d.Select(x => x.RY)
                                   .OrderBy(x => x)
                                   .ToList();
            }

            return d;
        }
       
        public int Insert_Order_table(List<Model_DataGrid> dataSource)
        {
            int kq = 0;          
            
            var data = dataSource.Where(r => r.Sel).ToList();

            string id = "";

            if (data.Any())
            {
                            
                foreach (var item in data)
                {
                    if (item.Sel)
                    {
                        id = string.Format("{0}{1}", DB.StoreLocal.Instant.CurrentDep.ToUpper(), DateTime.Now.Ticks.ToString());

                        string checksql = $"SELECT * FROM App_Material_Orders where RY ='{item.RY}' and Component='{item.Comp}' and BWBH='{item.Bwbh}' and CLBH='{item.Clbh}'";

                        var ta = DB.SQL.ConnectDB.Connection.FillDataTable(checksql);

                        if (ta.Rows.Count > 0) 
                        {
                            var orderid = ta.Rows[0]["orderid"].ToString();

                            checksql = $"SELECT * FROM App_Material_Process where orderid='{orderid}'";

                            if (DB.SQL.ConnectDB.Connection.FillDataTable(checksql).Rows.Count > 0)
                            {
                                data[0].OrderID = orderid;
                                Update_Process_table(data); 
                                return 1;
                            }

                            return 0;
                           
                        };



                        string sql = @"INSERT INTO [dbo].[App_Material_Orders]
           ([OrderId]
           ,[RY]
           ,[Component]
           ,[BWBH]
           ,[CLBH]
           ,[UserDate]
           ,[UserID]
           ,[Qty]
           ,[DepNo])
     VALUES
           (@id
           , @ry
           , @comp
           , @bwbh
           , @clbh
           , @date
           , @userid
           , @qty
           , @depno)";


                        string[] arr = { id, item.RY, item.Comp, item.Bwbh, item.Clbh, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DB.StoreLocal.Instant.UserName, item.TarQty.ToString().Split('/')[1], DB.StoreLocal.Instant.CurrentDep.ToUpper() };

                        kq += DB.SQL.ConnectDB.Connection.Update_Parameter(sql, arr);

                        if (kq != 0)
                        {
                            Alarm.Sound.Completed();
                        }
                        else
                        {
                            Alarm.Sound.Error();
                        }
                    }
                } 
            }
            else
            {
                Alarm.Sound.Error();
                Services.DisplayToast.Show.Toast("Nothing",Class.Style.Error);
            }
            return kq;
        }
        public int NS_DeleteItem(List<Model_DataGrid> dgv)
        {
            int k = 0;

            foreach (var item in dgv)
            {

                string s = $@"SELECT Orderid
                                FROM App_Material_Orders
                                WHERE RY='{item.RY}'
	                                and Component='{item.Comp}'
	                                and BWBH ='{item.Bwbh}'
	                                and CLBH='{item.Clbh}'
	                                and Depno='NS'
	                                and OrderId NOT IN (SELECT OrderId FROM App_Material_Process);";
                var ta = DB.SQL.ConnectDB.Connection.FillDataTable(s);

                if(ta.Rows.Count > 0)
                {
                    s = $@"Delete App_Material_Orders where (OrderID=@orderID)";

                    k = DB.SQL.ConnectDB.Connection.Update_Parameter(s, new string[] { ta.Rows[0][0].ToString() });
                }

            }

            return k;

        }
        public int AC_DeleteItem(List<Model_DataGrid> dgv)
        {
            int k = 0;

            foreach (var item in dgv)
            {

                string s = $@"SELECT Orderid
                                FROM App_Material_Orders
                                WHERE RY='{item.RY}'
	                                and Component='{item.Comp}'
	                                and BWBH ='{item.Bwbh}'
	                                and CLBH='{item.Clbh}'
	                                and Depno='NS'
	                                and OrderId IN (SELECT OrderId FROM App_Material_Process where [Status]=0);";
                var ta = DB.SQL.ConnectDB.Connection.FillDataTable(s);
              
                if (ta.Rows.Count > 0)
                {
                    var orderid = ta.Rows[0][0].ToString();

                    //var dep = orderid.Substring(0, 2).Trim();

                    s = $@"Delete App_Material_Process where (OrderID=@orderID)";                    
                    k = DB.SQL.ConnectDB.Connection.Update_Parameter(s, new string[] { orderid });

                    //if (dep == "AC")
                    //{
                    //    s = @"delete App_Material_Orders where (OrderID=@orderID)";
                    //    k = DB.SQL.ConnectDB.Connection.Update_Parameter(s, new string[] { orderid });
                    //}
                }

            }

            return k;

        }
        public  int Insert_Process_table(List<Model_DataGrid> dataSource ,string orderid="")
        {
            int kq = 0;

            var data = dataSource.Where(r => r.Sel).ToList();

            if (data.Any())
            {
               
                foreach (var item in data)
                {
                  

                    if (item.Sel)
                    {

                        var CheckExist = $@"SELECT app1.depno,
                                               app1.orderid orderid1,
                                               app2.orderid orderid2
                                        FROM   app_material_orders app1
                                               LEFT JOIN app_material_process app2
                                                      ON app1.orderid = app2.orderid
                                        WHERE  app1.orderid = '{item.OrderID}'";

                        var table = DB.SQL.ConnectDB.Connection.FillDataTable(CheckExist);

                        if (table.Rows.Count > 0)
                        {
                            item.OrderID = table.Rows[0]["orderid2"].ToString();
                        }

                        if (!string.IsNullOrEmpty( item.OrderID ))
                        {
                            if (DB.StoreLocal.Instant.CurrentDep.ToUpper() == item.DepName)
                            {

                                string updatesql = @"UPDATE [dbo].[App_Material_Process] set
   
                                                  [UserDate] = @date
                                                  ,[UserID] = @user
                                                  ,[Status] = 1
                                             WHERE (OrderID=@id)";

                                string[] arr = { DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DB.StoreLocal.Instant.UserName, item.OrderID };

                                kq = DB.SQL.ConnectDB.Connection.Update_Parameter(updatesql, arr);

                                //----Insert Inventory
                                if (kq != 0)
                                {

                                    var prono = DateTime.Now.Ticks.ToString();
                                    var ry = item.RY;
                                    var comp = item.Comp;
                                    var bwbh = item.Bwbh;
                                    var clbh = item.Clbh;
                                    var modelname = item.ModelName;
                                    var art = item.Art;
                                    var plan = item.TarQty.Split('/')[1];
                                    var qty = item.TarQty.Split('/')[0];
                                    var psdt = item.ProdDate;

                                    //string sql = $@"SELECT * FROM App_Material_Inventory Where ZLBH ='{ry}' and Component ='{comp}' and DepName ='{DB.StoreLocal.Instant.CurrentDep}'";

                                    // var inventory = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                                    // if (inventory.Rows.Count == 0)
                                    // {

                                    // }
                                    // else
                                    // {
                                    //     sql = $@"Update App_Material_Inventory set ActualQty =convert(int,ActualQty ) + {qty} where (RY =@ry and Component =@comp)";
                                    //     kq = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, new string[] { ry, comp });

                                    //     if (kq > 0)
                                    //     {


                                    //         sql = $@"SELECT xxcc, sum(OrderQty) qty FROM App_Order_Detail where ry ='{ry}'and Component='{comp}' group by xxcc";

                                    //         inventory = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                                    //         if (inventory.Rows.Count > 0)
                                    //         {
                                    //             foreach (System.Data.DataRow r in inventory.Rows)
                                    //             {
                                    //                 sql = @"Update App_Material_Inventory_Size Set qty= @qty Where (prono =@prono and xxcc =@xxcc)";

                                    //                 kq = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, new string[] { r["Qty"].ToString(), prono, r["xxcc"].ToString() });
                                    //             }
                                    //         }
                                    //     }
                                    // }
                                    var sql = @"Insert Into App_Material_Inventory Values (
                                                                                            @Prono, 
	                                                                                        @ZLBH  ,
	                                                                                        @Component ,
	                                                                                        @CLBH ,
	                                                                                        @ModelName ,
	                                                                                        @Article ,
	                                                                                        @BWBH ,
	                                                                                        @PlanQty ,
	                                                                                        @ActualQty,
	                                                                                        @PSDT ,
                                                                                            @ImpDate,
	                                                                                        @Importer,
                                                                                            @DepName )";


                                    arr = new string[] { prono, ry, comp, clbh, modelname, art, bwbh, plan, qty, psdt, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DB.StoreLocal.Instant.UserName, DB.StoreLocal.Instant.CurrentDep.ToUpper() };

                                    kq = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, arr);

                                    if (kq > 0)
                                    {
                                        sql = $@"SELECT xxcc, OrderQty FROM App_Order_Detail where ry ='{ry}'and Component='{comp}' And orderid ='{item.OrderID}' ";

                                       var inventory = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                                        if (inventory.Rows.Count > 0)
                                        {
                                            foreach (System.Data.DataRow r in inventory.Rows)
                                            {
                                                sql = @"INSERT INTO [dbo].[App_Material_Inventory_Size]
           
                                                                 VALUES
                                                                       (@Prono, 
                                                                       @XXCC, 
                                                                       @Qty)";
                                                kq = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, new string[] { prono, r["xxcc"].ToString(), r["OrderQty"].ToString() });

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else //insert process
                        {
                            if (DB.StoreLocal.Instant.CurrentDep.ToUpper() != item.DepName)
                            {

                                string Insertsql = @"INSERT INTO [dbo].[App_Material_Process]                                       
                                 VALUES
                                       (@id
                                       ,@uDate
                                       ,@uID
                                       ,@sta)";



                                string[] arr = { table.Rows[0]["orderid1"].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DB.StoreLocal.Instant.UserName, "0" };

                                kq = DB.SQL.ConnectDB.Connection.Update_Parameter(Insertsql, arr);

                                if (kq != 0)
                                {
                                    Alarm.Sound.Completed();
                                }
                                else
                                {
                                    Alarm.Sound.Error();
                                }
                            }
                        }
                    }
                }
            }
            return kq;
        
        }
       
        public int Update_Process_table(List<Model_DataGrid> dataSource)
        {
            int kq = 0;

            string dep="";

            switch (DB.StoreLocal.Instant.Depname)  
            {
                case Departments.NoSew:
                    dep = "NS";
                    break;
                case Departments.AutoCutting:
                    dep = "AC";
                    break;
                case Departments.HighFrequency:
                    dep = "HF";
                    break;
                case Departments.Printing:
                    dep = "Pr";
                    break;
                case Departments.Embroidery:
                    dep = "EM";
                    break;
                case Departments.Stiching:
                    dep = "ST";
                    break;
                case Departments.OutSourcing:
                    dep = "OS";
                    break;
                default:
                    break;
            }

            if(dataSource.Any())

                foreach (var item in dataSource)
                {
                    if(item.DepName ==dep)

                        if (item.Sel)
                        {
                            string sql = $"SELECT * FROM App_Material_Orders where orderid='{item.OrderID}'";

                            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                            var depno = ta.Rows[0]["DepNo"].ToString();

                           

                            if (DB.StoreLocal.Instant.CurrentDep.ToUpper() == depno)
                            {
                                sql = @"UPDATE [dbo].[App_Material_Process] set
   
                                                  [UserDate] = @date
                                                  ,[UserID] = @user
                                                  ,[Status] = 1
                                             WHERE (OrderID=@id)";

                                string[] arr = { DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DB.StoreLocal.Instant.UserName, item.OrderID };

                                kq = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, arr);

                                
                            }
                          
                        }
                }
            return kq;
        }
       
        private void Check_Order(object obj)
        {
            var dgv = obj as List<Model_DataGrid>;
            if (dgv != null && dgv.Any())
            {

                if (isNoSew)
                {

                    var kq = Insert_Order_table(dgv);

                    if (kq != 0) Services.DisplayToast.Show.Toast(string.Format("Đơn xác nhận {0} chi tiết", kq), Color.DarkGreen);
                }
                else
                {

                    var kq = Insert_Process_table(dgv);

                    if (kq != 0) Services.DisplayToast.Show.Toast(string.Format("Đơn chuẩn bị {0} chi tiết", kq), Color.DarkGreen);
                }
                Listening_Tick();
            }
        }

        private async void LoadHistory(object obj)
        {
            if (Network.Net.HasNet)
            {
                if (DB.StoreLocal.Instant.Myfac == MyFactory.LYV)
                {
                   

                    var lst = DB.DataLocal.Table.Get_History;

                    var listbutton = new List<Button>();

                    if (lst.Any())
                    {
                        foreach (var item in lst)
                        {
                            listbutton.Add(new Button { Text = item.Barcode });
                        }
                        var a = await Services.Alert.PopupMenu("Scaned", listbutton, kolor.Notification);

                        if (a != null)
                        {
                            DB.StoreLocal.Instant.Barcode = a;
                            await Application.Current.MainPage.Navigation.PushAsync(new Views.AutoCuttingScanView_LYV());
                        }
                    }
                    else
                    {
                        Services.DisplayToast.Show.Toast("Không tìm thấy", kolor.Red);
                    }               
                   
                }
                else
                {
                    if (!isNoSew)
                    {
                        var sql = $@"SELECT top 50 * FROM (
                                SELECT  zlbh, barcode,max(USERDATE) Udate
                                FROM App_Cutting_Barcodes_Edit
								where Userid='{DB.StoreLocal.Instant.UserName}'
                                Group by zlbh, barcode ) aa 
								
                                order by Udate desc";


                        var table = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                        var listbutton = new List<Button>();

                        if (table.Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow bar in table.Rows)
                            {

                                listbutton.Add(new Button { Text = string.Format("{0}({1})", bar["zlbh"].ToString(), bar["Barcode"].ToString()) });
                            }
                        }

                        var a = await Services.Alert.PopupMenu("Scaned", listbutton,kolor.Notification);

                        if (a != null)
                        {
                            a = a.Split('(')[1].Substring(0, a.Split('(')[1].Length - 1);

                            await Application.Current.MainPage.Navigation.PushAsync(new Views.AutoCuttingScanView(a));
                        }

                    }
                    else
                    {
                        //Services.Alert.Msg("Cảnh báo", "Chưa hoàn thành"); 
                        var sql = $@"SELECT ry,Component ,max(o.UserDate) uDate
								FROM App_Material_Orders o left join App_Material_Process p  
								On o.OrderId = p.OrderId and p.[Status] !=1
								where o.Userid='{DB.StoreLocal.Instant.UserName}' 
								
									and convert(date,o.userdate)=convert(date,getdate())
									
								Group by  ry,Component
								order by uDate desc";


                        var table = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                        var listbutton = new List<Button>();

                        if (table.Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow bar in table.Rows)
                            {

                                listbutton.Add(new Button { Text = string.Format("{0}({1})", bar["ry"].ToString(), bar["Component"].ToString()) });
                            }
                        }

                        IsBusy = true;
                        await Task.Delay(500);
                        var a = await Services.Alert.PopupMenu("History", listbutton, kolor.Notification);

                        if (a != null)
                        {

                            DB.StoreLocal.Instant.Ry = a.Split('(')[0].Substring(0, a.Split('(')[0].Length );

                            if (!Class.Network.Net.HasNet) { IsBusy = false; return; }
                            
                            var pageTabbed = new PageViews.TrackingTransfer();

                            pageTabbed.CurrentPage = pageTabbed.Children[0];
                            await Application.Current.MainPage.Navigation.PushAsync(pageTabbed);
                        }
                        IsBusy = false;

                    }
                }
            }
        }

        private async void Complete(object obj)
        {
            IsBusy = true;
            await Task.Delay(500);
         
            if (isNoSew)
            {


                string sql = @"SELECT b.OrderId,b.RY,b.Component FROM(
SELECT Row_number() OVER(partition BY OrderId ORDER BY Status DESC) ROWNUMBER ,*
FROM App_Material_process 
) A 
inner JOIN dbo.App_Material_Orders B ON  A.OrderId=B.OrderId
WHERE A.ROWNUMBER=1 AND A.Status=0 AND LEFT(A.OrderId,2)='NS'";

                var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                List<Button> lst = new List<Button>();

                if (ta.Rows.Count > 0)

                    for (int i = 0; i < ta.Rows.Count; i++)
                    {
                        var button = new Button
                        {
                            Text = string.Format("{0}|{1}|{2}", ta.Rows[i]["Orderid"].ToString(), ta.Rows[i]["RY"].ToString(), ta.Rows[i]["Component"].ToString())
                           
                        };
                        lst.Add(button);
                    }

                var a = await Services.Alert.PopupMenu("Get", lst, kolor.Notification);

                if (a != null)
                {

                    var arr = a.Split('|');

                    sql = @"INSERT INTO [dbo].[App_Material_Process]
           ([OrderId]
           ,[UserDate]
           ,[UserID]
           ,[Status])
     VALUES
           (@id
           ,@uDate
           ,@uID
           ,@sta)";



                    string[] array = { arr[0].ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DB.StoreLocal.Instant.UserName, "1" };

                    int kq = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, array);

                    Services.DisplayToast.Show.Toast(string.Format("Đơn hàng {0} Đã xác nhận", a.Split('|')[0].ToString()), Color.DarkGreen);
                }

            }
           // var yn = await Services.Alert.PopupYesNo("Xác nhận", "Bạn đã nhận hàng", "Ok", "No");


            

            IsBusy = false;
        }

        List<Button> ListButton(string sql)
        {
           

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            List<Button> lst = new List<Button>();

            if (ta.Rows.Count > 1)
            {
                foreach (System.Data.DataRow r in ta.Rows)
                {
                    if (lst.Any(b => b.Text != r["Ry"].ToString()))
                    {

                        var button = new Button
                        {
                            Text = string.Format("{0}", r["RY"])
                        };

                        lst.Add(button);
                    }
                }
            }
            return lst;
        }
        async void Data_NoSew()
        {
            string s = @"SELECT * FROM(
SELECT Row_number() OVER(partition BY App_Material_Process.OrderId ORDER BY App_Material_Process.[Status] DESC) ROWNUMBER ,App_Material_Orders.RY,
App_Material_Process.[Status]
FROM App_Material_process inner join 
App_Material_Orders on App_Material_Orders.OrderId = App_Material_Process.OrderId) A
WHERE A.ROWNUMBER=1 AND A.[Status]=0";



            List<Button> lst = ListButton(s);

            if (lst.Any())
            {               
                var a = await Services.Alert.PopupMenu("Select", lst, kolor.Notification);

                DB.StoreLocal.Instant.Ry = a;

                if (!Class.Network.Net.HasNet) return;

                var pageTabbed = new PageViews.TrackingTransfer();

                pageTabbed.CurrentPage = pageTabbed.Children[0];

                await Application.Current.MainPage.Navigation.PushAsync(pageTabbed);
            }
            else
            {
                if (!Class.Network.Net.HasNet) return;
                var pageTabbed = new PageViews.TrackingTransfer();

                pageTabbed.CurrentPage = pageTabbed.Children[0];

                await Application.Current.MainPage.Navigation.PushAsync(pageTabbed);
            }
        }
        async void Data_AC()
        {
            string s = @"SELECT Ry FROM App_Material_Orders o 
where o.OrderId not in (SELECT  d.orderid FROM App_Material_process d)";
          

            List<Button> lst = ListButton(s);

            if (lst.Any())
            {               
                var a = await Services.Alert.PopupMenu("Select", lst, kolor.Notification);

                DB.StoreLocal.Instant.Ry = a;
                if (!Class.Network.Net.HasNet) return;
                var pageTabbed = new PageViews.TrackingTransfer();


                pageTabbed.CurrentPage = pageTabbed.Children[0];

                //Services.DisplayPage.Instant.PushAsync(pageTabbed);
                await Application.Current.MainPage.Navigation.PushAsync(pageTabbed);
            }
            else
            {
                if (!Class.Network.Net.HasNet) return;
                var pageTabbed = new PageViews.TrackingTransfer();


                pageTabbed.CurrentPage = pageTabbed.Children[0];

                //Services.DisplayPage.Instant.PushAsync(pageTabbed);
                await Application.Current.MainPage.Navigation.PushAsync(pageTabbed);
            }
        }

   
        private async  void ShowNotify(object obj)
        {

            IsBusy = true;

            await Task.Delay(10);

            if (DB.StoreLocal.Instant.Myfac == MyFactory.LYV) { Services.Alert.Msg("Message!", "It's not support"); return; }

            try
            {

                if (isNoSew)
                {


                    Data_NoSew();

                  

                }
                else//AC
                {
                    Data_AC();
                }
                //isbusy
            }
            catch (Exception cc)
            {
                IsBusy = false;
                throw;
            }
            finally { IsBusy = false; }
        }                    
        #region Function

        private void DeleteDatalocal(object obj)
        {
            var cout = DB.DataLocal.Table.Delete_All;

            Services.DisplayToast.Show.Toast($"Da xoa {cout}", kolor.Primary);
        }
        private async void Inventory()
        {            
            IsBusy = true;

            await Task.Delay(10);

            await Application.Current.MainPage.Navigation.PushAsync(new PageViews.Inventory_PageView());

            IsBusy = false;
        }
       
        private async void Input_Code()
        {
            if (DB.StoreLocal.Instant.Depname == Departments.AutoCutting || DB.StoreLocal.Instant.Depname == Departments.Lazer) 
            {
                //Neu khong phai la Laser hoac la autocutting thi khong dc nhap tay

                var a = await Services.Alert.PopuSearch("QCode");

                if (a != null)
                {
                    var code = (a as string).Trim();

                    if (string.IsNullOrEmpty(code)) return;

                    try
                    {
                        if (!Network.Net.HasNet) { Services.DisplayToast.Show.Toast("Không có mạng", kolor.Error); return; }

                        DB.StoreLocal.Instant.Barcode = Barcode.Result = code;

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            if (DB.StoreLocal.Instant.Myfac == MyFactory.LYV)
                            {

                                await Application.Current.MainPage.Navigation.PushAsync(new Views.AutoCuttingScanView_LYV());

                            }
                            else
                            {
                                await Application.Current.MainPage.Navigation.PushAsync(new Views.AutoCuttingScanView());

                            }                        
                        });
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
        }

        private async void ScanBarcode()
        {            
            if (DB.StoreLocal.Instant.Depname == Departments.AutoCutting || DB.StoreLocal.Instant.Depname == Departments.Lazer)
            {
                try
                {
                    if (!Network.Net.HasNet) { Services.DisplayToast.Show.Toast(ConvertLang.Convert.Translate_LYM("Không có mạng", "ကွန်ရက်ပြင်ပ"), kolor.Error); return; }
                    var scannerPage = new ZXingScannerPage();

                    scannerPage.OnScanResult += (result) =>
                    {
                        Barcode.Result = result.Text;

                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            DB.StoreLocal.Instant.Barcode = Barcode.Result;
                        // Services.Alert.Msg("CODE", DB.StoreLocal.Instant.Barcode);

                            if (DB.StoreLocal.Instant.Myfac == MyFactory.LYV)
                            {
                                await Application.Current.MainPage.Navigation.PushAsync(new Views.AutoCuttingScanView_LYV());
                            }
                            else
                            {
                                await Application.Current.MainPage.Navigation.PushAsync(new Views.AutoCuttingScanView());
                            }
                            var navigationStack = Application.Current.MainPage.Navigation.NavigationStack;

                            if (navigationStack.Count > 1)
                            {
                                Application.Current.MainPage.Navigation.RemovePage(navigationStack[1]);
                            }
                        });
                    };

                    await Application.Current.MainPage.Navigation.PushAsync(scannerPage);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            //chi co Don vi AC va LZ moi co the scan
            else
            {
                Services.Alert.Msg(ConvertLang.Convert.Translate_LYM("Cảnh báo", "သတိပေးချက်"),ConvertLang.Convert.Translate_LYM("Không có quyền scan", "သင့်တွင် စကင်ဖတ်ရန် ခွင့်ပြုချက် မရှိပါ။"));
            }
        }
        #endregion
    }
}