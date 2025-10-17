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
    public partial class Inventory_PageView : ContentPage
    {
        List<InventoryPageGeneral_Type> data;
        public Inventory_PageView()
        {
            InitializeComponent();

            ThisContext = new Inventory_ViewModel();

            data = ItemSource;           

            ConvertLable();
        }
        void ConvertLable()
        {
            lbl_Title.Text = ConvertLang.Convert.Translate_LYM("Kho", "စတိုးဆိုင်");
            rd_ctd.Content= ConvertLang.Convert.Translate_LYM("CTD", "အော်တို ဖြတ်စက်");
            


            lbl_Tong.Text = data.Count.ToString();
        }
        Inventory_ViewModel ThisContext
        {
            get => BindingContext as Inventory_ViewModel;
            set=> BindingContext = value;                              
            
        }


        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBar sb = (SearchBar)sender;

            if (!string.IsNullOrEmpty(sb.Text))
            {

                //if (data.Where(dk => dk.ZLBH.Contains(sb.Text.ToUpper())).ToList().Any())
                //{

                //}

                //this.BindingContext = new Inventory_ViewModel(data.Where(dk =>
                //    dk.ZLBH.Contains(sb.Text.ToUpper())
                //    || dk.Art.Contains(sb.Text.ToUpper())
                //    || dk.ModelName.Contains(sb.Text.ToUpper())
                //    || dk.Barcode == (sb.Text)).ToList());

                ItemSource = data.Where(dk =>
                    dk.ZLBH.Contains(sb.Text.ToUpper())
                    || dk.Art.Contains(sb.Text.ToUpper())
                    || dk.ModelName.Contains(sb.Text.ToUpper())
                    || dk.Barcode == (sb.Text)).ToList();
            }
            else
            {
                //this.BindingContext = new Inventory_ViewModel();

                ThisContext.Data = data;
            }


        }
        List<InventoryPageGeneral_Type> ItemSource
        {
            get => (List<InventoryPageGeneral_Type>)LvData.ItemsSource;
            set
            {
                LvData.ItemsSource = value;
                lbl_Tong.Text = string.Format("{0}", value.Count);
            }


        }
        private void SwipeItem_Invoked(object sender, EventArgs e)
        {
            Services.DisplayToast.Show.Toast("Chưa hỗ trợ", Class.Style.Red, 2);
        }

        private async void On_FilterChange_Event(object sender, CheckedChangedEventArgs e)
        {
            ThisContext.IsBusy = true;
            await Task.Delay(10);
            ItemSource = data = ThisContext.Data;

            ThisContext.IsBusy = false;
        }
    }
    //----------------------------------------------------------------------------------------
    public class Inventory_ViewModel : BaseViewModel
    {
        private List<InventoryPageGeneral_Type> _data;

        private bool _ac_checked;
        private bool _lz_checked;


        public string Label_RY { get => ConvertLang.Convert.Translate_LYM("RY", "အာဝိုင်"); }
        public string Label_Art { get => ConvertLang.Convert.Translate_LYM("Art", "အာတီကယ်"); }
        public string Label_Model { get => ConvertLang.Convert.Translate_LYM("Model", "မော်ဒယ်"); }
        public string Label_Line { get => ConvertLang.Convert.Translate_LYM("Line", "လိုင်း"); }
        public string Label_ProDate { get => ConvertLang.Convert.Translate_LYM("ProDate", "ထုတ်လုပ်သော နေ့စွဲ"); }
        public string Label_Target { get => ConvertLang.Convert.Translate_LYM("Target", "တားဂက် အရေတွက်"); }
        public string Label_Actual { get => ConvertLang.Convert.Translate_LYM("Actual", "အမှန်တကယ် အရေတွက်"); }
        public string Label_Comp { get => ConvertLang.Convert.Translate_LYM("Comp", "အစိတ်အပိုင်းများ"); }
        public string Label_OG { get => ConvertLang.Convert.Translate_LYM("OnGoing", "လုပ်ဆောင်နေဆဲ"); }
        public string Delivered { get => ConvertLang.Convert.Translate_LYM("Delivered", "လွှဲပြီး"); }
        public string Delayed { get => ConvertLang.Convert.Translate_LYM("Delayed", "နှောင့်နှေးနေသည်"); }
        public string SearchBar_Holder { get => ConvertLang.Convert.Translate_LYM("Tìm", "ရှာဖွေ"); }


        public List<InventoryPageGeneral_Type> Data { get => DB.StoreLocal.Instant.Data_Warehouse_General;
            set => DB.StoreLocal.Instant.Data_Warehouse_General = value;
        }

        

        System.Windows.Input.ICommand ClickCommand{ get; set; }
        public bool Lz_checked { get => _lz_checked; set => SetProperty(ref _lz_checked, value); }
        public bool Ac_checked { get => _ac_checked; set => SetProperty(ref _ac_checked, value); }

        private List<InventoryPageGeneral_Type >data()
        {
            //sql Summary
            string sql = "";
            #region New SQL LVL
            sql = @"IF OBJECT_ID('tempdb.dbo.#TempBarcode', 'U') IS NOT NULL
  DROP TABLE #TempBarcode; 
SELECT DISTINCT t0.barcode,
                T0.userdate,
                ry,
                article,
                model,
                T0.clbh,
				T0.BWBH,
                lean,
                psdt                                     prodate,
                Cast((SELECT T2.ywsm + '; '
                      FROM  (SELECT DISTINCT a.zlbh,
                                             bwzl.ywsm,
                                             CONVERT(VARCHAR, a.userdate, 111)
                                             USERDATE
                             FROM  (SELECT b.barcode,
                                           b.bwbh,
                                           b.clbh,
                                           b.gxxcc,
                                           b.xxcc,
                                           b.zlbh,
                                           Isnull(a.planqty, b.qty)PlanQty,
                                           Isnull(a.actualqty, 0)  ActualQty,
                                           b.userdate
                                    FROM   cutting_barcodes2 b
                                           LEFT JOIN app_cutting_barcodes_edit a
                                                  ON a.barcode = b.barcode
                                                     AND a.zlbh = b.zlbh
                                                     AND a.xxcc = LTRIM(RTRIM(b.XXCC))
                                                     AND a.bwbh = b.bwbh) a
                                   LEFT JOIN bwzl
                                          ON bwdh = bwbh) T2
                      WHERE  T2.zlbh = T0.ry
                             AND T2.userdate = T0.userdate
                      ORDER  BY T2.ywsm ASC
                      FOR xml path('')) AS VARCHAR(800)) 'TotalComp',
                Cast((SELECT T2.ywsm + '; '
                      FROM  (SELECT DISTINCT a.zlbh,
                                             bwzl.ywsm,
                                             CONVERT(VARCHAR, a.userdate, 111)
                                             USERDATE
                             FROM  (SELECT b.barcode,
                                           b.bwbh,
                                           b.clbh,
                                           b.gxxcc,
                                           b.xxcc,
                                           b.zlbh,
                                           Isnull(a.planqty, b.qty)PlanQty,
                                           Isnull(a.actualqty, 0)  ActualQty,
                                           b.userdate
                                    FROM   cutting_barcodes2 b
                                           LEFT JOIN app_cutting_barcodes_edit a
                                                  ON a.barcode = b.barcode
AND a.zlbh = b.zlbh
AND a.xxcc = LTRIM(RTRIM(b.XXCC)) AND a.bwbh = b.bwbh
                                    WHERE  Isnull(a.actualqty, 0) <
                                           Isnull(a.planqty, b.qty)) a
                                   LEFT JOIN bwzl
                                          ON bwdh = bwbh) T2
                      WHERE  T2.zlbh = T0.ry
                             AND T2.userdate = T0.userdate
                      ORDER  BY T2.ywsm ASC
                      FOR xml path('')) AS VARCHAR(800)) 'ongoing',
                CONVERT(INT, actual_qty)                 Actual_qty,
                CONVERT(INT, plan_qty)                   Plan_qty
into #TempBarcode
FROM  (SELECT CONVERT(VARCHAR, CBs.userdate, 111)   USERDATE,
              CBs.zlbh                              RY,
              dd.article,
              xieming                               Model,
              Sum(CBs.actualqty)                    Actual_qty,
              Sum(CBs.planqty)                      Plan_qty,
              Sum(CBs.planqty) - Sum(CBs.actualqty) Remain_qty,
              pdsch.lean,
              CBs.bwbh,
              cbs.clbh,
              pdsch.psdt,
              cbs.barcode
       FROM  (SELECT b.barcode,
                     b.bwbh,
                     b.clbh,
                     b.gxxcc,
                     b.xxcc,
                     b.zlbh,
                     Isnull(a.planqty, b.qty)PlanQty,
                     Isnull(a.actualqty, 0)  ActualQty,
                     b.userdate
              FROM   cutting_barcodes2 b
                     LEFT JOIN app_cutting_barcodes_edit a
                            ON a.barcode = b.barcode
                               AND a.zlbh = b.zlbh
                               AND a.xxcc = LTRIM(RTRIM(b.XXCC))
                               AND a.bwbh = b.bwbh) CBs
             LEFT JOIN pdsch
                    ON pdsch.zlbh = CBs.zlbh
             LEFT JOIN ddzl dd
                    ON DD.ddbh = pdsch.ry
             LEFT JOIN zlzls2 zl
                    ON zl.zlbh = pdsch.ry
                       AND zl.clbh = CBs.clbh
                       AND zl.bwbh = CBs.bwbh
             LEFT JOIN de_orderm DE
                    ON DE.orderno = pdsch.ry
             LEFT JOIN xxzl
                    ON xxzl.article = DD.article
             LEFT JOIN cutting_barcode CB3
                    ON CBs.barcode = CB3.barcode
       GROUP  BY CONVERT(VARCHAR, CBs.userdate, 111),
                 CBs.zlbh,
                 dd.article,
                 xieming,
                 pdsch.lean,
                 CBs.bwbh,
                 cbs.clbh,
                 pdsch.psdt,
                 cbs.barcode) T0
WHERE  actual_qty > 0
       AND t0.psdt >= Getdate() - 45
order by ry
select A.Barcode,A.USERDATE,A.RY,a.ARTICLE,A.Model,a.Bwbh,a.CLBH,A.LEAN
,A.prodate,A.TotalComp,A.ongoing,A.Actual_qty,A.Plan_qty
from #TempBarcode  A 
ORDER BY A.USERDATE DESC";
            #endregion
            #region new Code LVL
           
            string isAC = "";
            
            if (_ac_checked)
            {
                isAC = "WHERE a.MachineNo != 'LASER'";
            }
            if (_lz_checked)
            {
                isAC = "WHERE a.MachineNo ='LASER'";
            }
            var fc = DB.StoreLocal.Instant.Myfac;
            if (fc== DB.MyFactory.LYM)
            {
                isAC = "";
            }
            sql = $@"IF OBJECT_ID('tempdb.dbo.#TempBarcode', 'U') IS NOT NULL
  DROP TABLE #TempBarcode; 
SELECT DISTINCT  t0.barcode,
                T0.userdate,
				T0.MachineNo,
                ry,
                article,
                model,
                T0.clbh,
				T0.BWBH,
                lean,
                psdt                                     prodate,
                Cast((SELECT T2.ywsm + '; '
                      FROM  (SELECT DISTINCT a.zlbh,
                                             bwzl.ywsm,
                                             CONVERT(VARCHAR, a.userdate, 111)
                                             USERDATE
                             FROM  (SELECT b.barcode,
                                           b.bwbh,
                                           b.clbh,
                                           b.gxxcc,
                                           b.xxcc,
                                           b.zlbh,
                                           Isnull(a.planqty, b.qty)PlanQty,
                                           Isnull(a.actualqty, 0)  ActualQty,
                                           b.userdate
                                    FROM   cutting_barcodes2 b
                                           LEFT JOIN app_cutting_barcodes_edit a
                                                  ON a.barcode = b.barcode
                                                     AND a.zlbh = b.zlbh
                                                     AND a.xxcc = LTRIM(RTRIM(b.XXCC))
                                                     AND a.bwbh = b.bwbh) a
                                   LEFT JOIN bwzl
                                          ON bwdh = bwbh) T2
                      WHERE  T2.zlbh = T0.ry
                             AND T2.userdate = T0.userdate
                      ORDER  BY T2.ywsm ASC
                      FOR xml path('')) AS VARCHAR(800)) 'TotalComp',
                Cast((SELECT T2.ywsm + '; '
                      FROM  (SELECT DISTINCT a.zlbh,
                                             bwzl.ywsm,
                                             CONVERT(VARCHAR, a.userdate, 111)
                                             USERDATE
                             FROM  (SELECT b.barcode,
                                           b.bwbh,
                                           b.clbh,
                                           b.gxxcc,
                                           b.xxcc,
                                           b.zlbh,
                                           Isnull(a.planqty, b.qty)PlanQty,
                                           Isnull(a.actualqty, 0)  ActualQty,
                                           b.userdate
                                    FROM   cutting_barcodes2 b
                                           LEFT JOIN app_cutting_barcodes_edit a
                                                  ON a.barcode = b.barcode
AND a.zlbh = b.zlbh
AND a.xxcc = LTRIM(RTRIM(b.XXCC)) AND a.bwbh = b.bwbh
                                    WHERE  Isnull(a.actualqty, 0) <
                                           Isnull(a.planqty, b.qty)) a
                                   LEFT JOIN bwzl
                                          ON bwdh = bwbh) T2
                      WHERE  T2.zlbh = T0.ry
                             AND T2.userdate = T0.userdate
                      ORDER  BY T2.ywsm ASC
                      FOR xml path('')) AS VARCHAR(800)) 'ongoing',
                CONVERT(INT, actual_qty)                 Actual_qty,
                CONVERT(INT, plan_qty)                   Plan_qty
into #TempBarcode
FROM  (SELECT CONVERT(VARCHAR, CBs.userdate, 111)   USERDATE,
              CBs.zlbh                              RY,
              dd.article,
              xieming                               Model,
              Sum(CBs.actualqty)                    Actual_qty,
              Sum(CBs.planqty)                      Plan_qty,
              Sum(CBs.planqty) - Sum(CBs.actualqty) Remain_qty,
              pdsch.lean,
              CBs.bwbh,
              cbs.clbh,
              pdsch.psdt,
              cbs.barcode,
			  CB3.MachineNo
       FROM  (SELECT b.barcode,
                     b.bwbh,
                     b.clbh,
                     b.gxxcc,
                     b.xxcc,
                     b.zlbh,
					 
                     Isnull(a.planqty, b.qty)PlanQty,
                     Isnull(a.actualqty, 0)  ActualQty,
                     b.userdate
					 
              FROM   cutting_barcodes2 b
                     LEFT JOIN app_cutting_barcodes_edit a
                            ON a.barcode = b.barcode
                               AND a.zlbh = b.zlbh
                               AND a.xxcc = LTRIM(RTRIM(b.XXCC))
                               AND a.bwbh = b.bwbh) CBs
             LEFT JOIN pdsch
                    ON pdsch.zlbh = CBs.zlbh
             LEFT JOIN ddzl dd
                    ON DD.ddbh = pdsch.ry
             LEFT JOIN zlzls2 zl
                    ON zl.zlbh = pdsch.ry
                       AND zl.clbh = CBs.clbh
                       AND zl.bwbh = CBs.bwbh
             LEFT JOIN de_orderm DE
                    ON DE.orderno = pdsch.ry
             LEFT JOIN xxzl
                    ON xxzl.article = DD.article
             LEFT JOIN cutting_barcode CB3
                    ON CBs.barcode = CB3.barcode
       GROUP  BY CONVERT(VARCHAR, CBs.userdate, 111),
                 CBs.zlbh,
                 dd.article,
                 xieming,
                 pdsch.lean,
                 CBs.bwbh,
                 cbs.clbh,
                 pdsch.psdt,
                 cbs.barcode,
				 CB3.MachineNo) T0
WHERE  actual_qty > 0
       AND t0.psdt >= Getdate() +1
order by ry

select a.MachineNo, A.Barcode,A.USERDATE,A.RY,a.ARTICLE,A.Model,a.Bwbh,a.CLBH,A.LEAN
,A.prodate,A.TotalComp,A.ongoing,A.Actual_qty,A.Plan_qty
from #TempBarcode  A 
  {isAC}
ORDER BY A.USERDATE DESC";
            #endregion
            //if (DB.StoreLocal.Instant.Factory.Equals("LYV"))
            if (DB.StoreLocal.Instant.Myfac == DB.MyFactory.LYV)
            {
                sql = $@"IF OBJECT_ID('tempdb.dbo.#TempBarcode_0', 'U') IS NOT NULL
  DROP TABLE #TempBarcode_0; 

SELECT 
AP.USERDATE, Get_ZLBH.ZLBH ZLBH_S, AP.ActualQty, AP.PlanQty, AP.BWBH, AP.CLBH,
       AP.Barcode 
INTO #TempBarcode_0
  FROM  app_cutting_barcodes_groups_edit AP
LEFT JOIN  (SELECT distinct Barcode, ZLBH
FROM Cutting_Barcodes aa  
) Get_ZLBH ON Get_ZLBH.Barcode = AP.Barcode  AND Get_ZLBH.ZLBH = AP.ZLBH
      
IF OBJECT_ID('tempdb.dbo.#TempBarcode', 'U') IS NOT NULL
  DROP TABLE #TempBarcode; 
SELECT DISTINCT 
--t0.barcode,
                T0.userdate,

                zlbh_s zlbh,
                article,
                model,
    --            T0.clbh,
				--bwzl.ywsm BWBH,
                lean,
                psdt                                     prodate,
                Cast((SELECT T2.ywsm + '; '
                      FROM  (SELECT DISTINCT 
                                             bwzl.ywsm,
                                             CONVERT(VARCHAR, a.userdate, 111)
                                             USERDATE
                             FROM  (SELECT * FROM #TempBarcode_0 ) a
                                   LEFT JOIN bwzl
                                          ON bwdh = bwbh) T2
                      WHERE   T2.userdate = T0.userdate
                      ORDER  BY T2.ywsm ASC
                      FOR xml path('')) AS VARCHAR(800)) 'TotalComp',
                Cast((SELECT T2.ywsm + '; '
                      FROM  (SELECT DISTINCT 
                                             bwzl.ywsm,
                                             CONVERT(VARCHAR, a.userdate, 111)
                                             USERDATE
                             FROM  (SELECT * FROM  #TempBarcode_0 WHERE  ActualQty<PlanQty  ) a
                                   LEFT JOIN bwzl
                                          ON bwdh = bwbh) T2
                      WHERE  
                              T2.userdate = T0.userdate
                      ORDER  BY T2.ywsm ASC
                      FOR xml path('')) AS VARCHAR(800)) 'ongoing',
                CONVERT(INT, actual_qty)                 Actual_qty,
                CONVERT(INT, plan_qty)                   Plan_qty
into #TempBarcode
FROM  (SELECT CONVERT(VARCHAR, CBs.userdate, 111)   USERDATE,
          
              CBS.ZLBH_S,
              cb.article,
              xieming                               Model,
              Sum(CBs.actualqty)                    Actual_qty,
              Sum(CBs.planqty)                      Plan_qty,
              Sum(CBs.planqty) - Sum(CBs.actualqty) Remain_qty,
              b.DepName lean,
              --CBs.bwbh,
              --cbs.clbh,
              cb.WorkDate psdt
			  --, cbs.barcode
       FROM  (SELECT * FROM #TempBarcode_0 ) CBs
             LEFT JOIN Cutting_Barcode cb ON cb.Barcode = CBs.Barcode
             LEFT JOIN BDepartment b ON b.ID = cb.DepID
             LEFT JOIN xxzl ON xxzl.Article = cb.Article
GROUP  BY CONVERT(VARCHAR, CBs.userdate, 111),
               
                 CBS.ZLBH_S,
                 cb.article,
                 xieming,
                 b.DepName,
                 --CBs.bwbh,
                 --cbs.clbh,
                 cb.WorkDate
                 --cbs.barcode
				 ) T0
                 
                 --LEFT JOIN bwzl ON bwzl.bwdh=T0.bwbh
WHERE  actual_qty > 0
       AND t0.psdt >= Getdate() - 45 

	   order by zlbh
SELECT * FROM  #TempBarcode";
               

                var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                var list = new List<InventoryPageGeneral_Type>();

                int OddEven = 0;

                foreach (System.Data.DataRow r in ta.Rows)
                {
                    OddEven++;

                    var arrcomp = r["TotalComp"].ToString().Split(';');

                    var date = r["prodate"].ToString();

                    DateTime prodate = DateTime.Parse(date).AddDays(-10);

                    ClickCommand = new Command(async (Object obj) =>
                    {
                        var item = obj as InventoryPageGeneral_Type;

                        IsBusy = true;

                        await Task.Delay(100);

                        await Application.Current.MainPage.Navigation.PushAsync(new PageViews.InventoryDetail_PageView(item, list));

                        IsBusy = false;


                    });

                    list.Add(new InventoryPageGeneral_Type
                    {
                        Art = r["article"].ToString(),
                                             
                        Components = r["TotalComp"].ToString(),

                        CountComp = r["TotalComp"].ToString().Split(';'),

             

                        Line = r["lean"].ToString(),

                        ModelName = r["model"].ToString(),

                        ProDate = r["prodate"].ToString().Split(' ')[0],
            

                        ZLBH = r["zlbh"].ToString(),

                        Target = r["plan_qty"].ToString(),

                        Actual = r["actual_qty"].ToString(),

                        ColorFull = int.Parse(r["actual_qty"].ToString()) < int.Parse(r["plan_qty"].ToString()) ? Class.Style.Error : Class.Style.Success

                            ,
                        BgrColor = OddEven % 2 == 0 ? Color.FromHex("#c1d8c3") : Color.FromHex("#F5E8B7"),

                        OnGoing = int.Parse(r["plan_qty"].ToString()) == int.Parse(r["actual_qty"].ToString()) ? "" : r["Ongoing"].ToString(),

                        BgrColorDelay = DateTime.Now >= prodate ? Class.Style.Error : OddEven % 2 == 0 ? Color.FromHex("#c1d8c3") : Color.FromHex("#F5E8B7"),


                        ClickCommand = ClickCommand
                    });

                }
                var brcode = DB.StoreLocal.Instant.Barcode;
                var sortedList = list.OrderByDescending(item => item.Barcode == brcode || item.Target != item.Actual).ThenBy(item => item.Barcode).ToList();

                _data = new List<InventoryPageGeneral_Type>(sortedList);
            }
            else 
            {
                var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                var list = new List<InventoryPageGeneral_Type>();

                int OddEven = 0;

                foreach (System.Data.DataRow r in ta.Rows)
                {
                    OddEven++;

                    var arrcomp = r["TotalComp"].ToString().Split(';');

                    var date = r["prodate"].ToString();

                    DateTime prodate = DateTime.Parse(date).AddDays(-10);

                    ClickCommand = new Command(async (Object obj) =>
                    {
                        var item = obj as InventoryPageGeneral_Type;

                        IsBusy = true;

                        await Task.Delay(10);
                        //if (!DB.StoreLocal.Instant.Factory. Equals("LYV"))
                        if (DB.StoreLocal.Instant.Myfac != DB.MyFactory.LYV)
                        {

                            await Application.Current.MainPage.Navigation.PushAsync(new PageViews.InventoryDetail_PageView(item, list));
                        }
                        else Services.DisplayToast.Show.Toast("Chưa hoàn thiện", Class.Style.Error);

                        IsBusy = false;


                    });

                    list.Add(new InventoryPageGeneral_Type
                    {
                        Art = r["article"].ToString(),
                        //BWBH = r["Bwbh"].ToString(),
                        Barcode = r["barcode"].ToString(),

                        CLBH = r["Clbh"].ToString(),

                        Components = r["TotalComp"].ToString(),

                        CountComp = r["TotalComp"].ToString().Split(';'),

                        BWBH = r["BWBH"].ToString(),

                        Line = r["lean"].ToString(),

                        ModelName = r["model"].ToString(),

                        ProDate = r["prodate"].ToString().Split(' ')[0],
                        // QtyAndTarget = r["Qty"].ToString(),

                        ZLBH = r["ry"].ToString(),

                        Target = r["plan_qty"].ToString(),

                        Actual = r["actual_qty"].ToString(),

                        ColorFull = int.Parse(r["actual_qty"].ToString()) < int.Parse(r["plan_qty"].ToString()) ? Class.Style.Error : Class.Style.Success

                            ,
                        BgrColor = OddEven % 2 == 0 ? Color.FromHex("#c1d8c3") : Color.FromHex("#F5E8B7"),

                        OnGoing = int.Parse(r["plan_qty"].ToString()) == int.Parse(r["actual_qty"].ToString()) ? "" : r["Ongoing"].ToString(),

                        BgrColorDelay = DateTime.Now >= prodate ? Class.Style.Error : OddEven % 2 == 0 ? Color.FromHex("#c1d8c3") : Color.FromHex("#F5E8B7"),


                        ClickCommand = ClickCommand
                    });

                }
                var brcode = DB.StoreLocal.Instant.Barcode;
                var sortedList = list.OrderByDescending(item => item.Barcode == brcode || item.Target != item.Actual).ThenBy(item => item.Barcode).ToList();

                DB.StoreLocal.Instant.Data_Warehouse_General = new List<InventoryPageGeneral_Type>(sortedList);

            }
           
            return DB.StoreLocal.Instant.Data_Warehouse_General;
        }

        public Inventory_ViewModel(List<InventoryPageGeneral_Type> Doc=null)
        {
            _lz_checked = true;
            //var obj = new List<InventoryPageGeneral_Type>();

            //if (Doc!= null)
            //{
            //    foreach (var item in Doc)
            //    {
            //        obj.Add(item);
            //    }
            //    Data = obj;
            //}
            //else
            //{
            //IsBusy = true;

            //Task.Delay(10);
            // if(DB.StoreLocal.Instant.Data_Warehouse_General is null)
            data();
            

            //IsBusy = false;
            //}            
        }      
    }
}