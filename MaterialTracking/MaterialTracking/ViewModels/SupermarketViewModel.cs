using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MaterialTracking.ViewModels
{
    class SupermarketViewModel:ViewModels_Base
    {
        public PI.Mvvm.Commands.DelegateCommand SupperMarketCommand { get; }
        //public PI.Mvvm.Commands.DelegateCommand TestModeCommand { get; }
        //public PI.Mvvm.Commands.DelegateCommand BarCode_Available { get; }
       
        internal INavigation Navigation { get; set; }
        private System.Collections.ObjectModel.ObservableCollection<string> _location;
       
        public System.Collections.ObjectModel.ObservableCollection<string >Location {
            get => _location;
                                      
            set 
            {
                if (SetProperty(ref _location, value))
                    RaisePropertyChanged(nameof(Location));
            }
        }
      
        private string _selectedItem;
        public string SelectBuilding
        {
            get =>_selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    RaisePropertyChanged(nameof(SelectBuilding));
                   
                }
            }
        }

       

        void GetLocation()
        {
            _location = new System.Collections.ObjectModel.ObservableCollection<string>();
            string sql = "select distinct loc from BDepartment where DepType in (2,8) and IsActive=1 and Loc is not null";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            if (ta.Rows.Count > 0)
            {
                foreach (System.Data.DataRow loc in ta.Rows)
                {
                    string l = loc[0].ToString();
                    _location.Add(l);
                }
            }
            
        }
        public SupermarketViewModel()
        {
            //SupperMarketCommand = new PI.Mvvm.Commands.DelegateCommand(Scan);
            //TestModeCommand = new PI.Mvvm.Commands.DelegateCommand(TestScan);
         //   BarCode_Available = new PI.Mvvm.Commands.DelegateCommand(Bar_Available);
            GetLocation();

        }

        //private async void Bar_Available()
        //{
        //    var List = DB.DataLocal.Table.Data;

        //    if (List.Count > 0)
        //    {

        //        List<string> Range = new List<string>();
        //        Dictionary<string, string> serialKey = new Dictionary<string, string>();
        //        foreach (var bc in List)
        //        {
        //            if (!serialKey.ContainsKey(bc.ZLBH))
        //            {
        //                serialKey.Add(bc.ZLBH, bc.Barcode); Range.Add(bc.ZLBH); 
        //            }                                                                          
        //        }

        //        var a = await Services.Alert.MsgAction("RY", Range.ToArray());


        //        switch (a)
        //        {
        //            case "CANCEL":
        //            case null:
        //                {
        //                    return;
        //                }
        //            default: break;

        //        }
        //        if (a != "")
        //        {
        //            string bar = "";
        //            foreach (var item in serialKey)
        //            {
        //                if (item.Key == a) { bar = item.Value; break; }
        //            }
        //            var page = Navigation.PushModalAsync(new Views.AutoCuttingScanView(bar, a));
        //        }
        //    }
        //    else Services.Alert.ToastMsg("Nothing");

        //}

        public ObservableCollection<Model_Inventory> Inventory(string lean, DateTime? date )
        {
            var lst =new ObservableCollection<Model_Inventory>();
            string sql = "";
            sql = $"SELECT T0.* , CASE when Remain_qty = 0 THEN 'Done' WHEN Remain_qty> 0 THEN 'Ongoing' ELSE '' END 'Status', CAST((SELECT T2.ywsm + '; ' FROM(SELECT distinct a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111)USERDATE FROM(SELECT b.Barcode, b.BWBH, b.CLBH, b.GXXCC, b.XXCC, b.ZLBH, isnull(a.PlanQty, b.Qty)PlanQty, isnull(a.ActualQty, 0)ActualQty, b.USERDATE FROM Cutting_Barcodes2 b LEFT join  App_Cutting_Barcodes_Edit a ON a.Barcode = b.Barcode AND a.ZLBH = b.ZLBH AND  a.XXCC = b.XXCC AND a.BWBH = b.BWBH) a LEFT JOIN bwzl ON bwdh = bwbh) T2 WHERE T2.ZLBH = T0.RY AND T2.USERDATE = T0.USERDATE order by T2.ywsm asc FOR XML PATH('')) AS varchar(800) ) 'TotalComp'  ,CAST((SELECT T2.ywsm + '; ' FROM(SELECT a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111)USERDATE, CASE WHEN sum(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) = 0 THEN 'Done' WHEN SUM(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) > 0 THEN 'Ongoing' ELSE '' END 'Status' FROM(SELECT b.Barcode, b.BWBH, b.CLBH, b.GXXCC, b.XXCC, b.ZLBH, isnull(a.PlanQty, b.Qty)PlanQty, isnull(a.ActualQty, 0)ActualQty, b.USERDATE FROM Cutting_Barcodes2 b LEFT join  App_Cutting_Barcodes_Edit a ON a.Barcode = b.Barcode AND a.ZLBH = b.ZLBH AND  a.XXCC = b.XXCC AND a.BWBH = b.BWBH) a LEFT JOIN bwzl ON bwdh = bwbh GROUP BY a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111)HAVING sum(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) > 0) T2 WHERE T2.ZLBH = T0.RY AND T2.USERDATE = T0.USERDATE order by T2.ywsm asc FOR XML PATH('')) AS varchar(800) ) 'Ongoing',CAST((SELECT T2.ywsm + '; ' FROM(SELECT a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111)USERDATE, CASE WHEN sum(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) = 0 THEN 'Done'WHEN SUM(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) > 0 THEN 'Ongoing' ELSE '' END 'Status' FROM(SELECT b.Barcode, b.BWBH, b.CLBH, b.GXXCC, b.XXCC, b.ZLBH, isnull(a.PlanQty, b.Qty)PlanQty, isnull(a.ActualQty, 0)ActualQty, b.USERDATE FROM Cutting_Barcodes2 b LEFT join  App_Cutting_Barcodes_Edit a ON a.Barcode = b.Barcode AND a.ZLBH = b.ZLBH AND  a.XXCC = b.XXCC AND a.BWBH = b.BWBH) a LEFT JOIN bwzl ON bwdh = bwbh GROUP BY a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111) HAVING sum(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) = 0) T2 WHERE T2.ZLBH = T0.RY AND T2.USERDATE = T0.USERDATE order by T2.ywsm asc FOR XML PATH('')) AS varchar(800) ) 'Done' FROM(SELECT convert(varchar, CBs.USERDATE, 111) USERDATE, CBs.ZLBH RY, dd.ARTICLE, XieMing Model, sum(CBs.ActualQty) Actual_qty, sum(CBs.PlanQty) Plan_qty, sum(CBs.PlanQty) - sum(CBs.ActualQty) Remain_qty , PDSCH.LEAN, count(CBs.BWBH) counts, PDSCH.PSDT FROM(SELECT b.Barcode, b.BWBH, b.CLBH, b.GXXCC, b.XXCC, b.ZLBH, isnull(a.PlanQty, b.Qty)PlanQty, isnull(a.ActualQty, 0)ActualQty, b.USERDATE FROM Cutting_Barcodes2 b LEFT join  App_Cutting_Barcodes_Edit a ON a.Barcode = b.Barcode AND a.ZLBH = b.ZLBH AND  a.XXCC = b.XXCC AND a.BWBH = b.BWBH) CBs Left join PDSCH on PDSCH.ZLBH = CBs.ZLBH left join DDZL dd on DD.DDBH = PDSCH.RY left join zlzls2 zl on zl.ZLBH = PDSCH.RY and zl.CLBH = CBs.CLBH and zl.BWBH = CBs.BWBH Left join DE_ORDERM  DE on DE.ORDERNO = PDSCH.RY LEFT JOIN xxzl ON xxzl.ARTICLE = DD.ARTICLE left join Cutting_Barcode CB3 on CBs.Barcode = CB3.Barcode GROUP BY convert(varchar, CBs.USERDATE, 111), CBs.ZLBH, dd.ARTICLE, XieMing, PDSCH.LEAN, PDSCH.PSDT)  T0 WHERE  LEAN = '{lean}' AND T0.USERDATE = '{date.Value.ToString("yyyy/MM/dd")}'";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            if (ta.Rows.Count > 0)
            {
                foreach (System.Data.DataRow item in ta.Rows)
                {
                    long time = Convert.ToDateTime(item["UserDate"]).Ticks;
                    var list = new Model_Inventory();
                    list.Art = item["Article"].ToString();
                    list.RY = item["Ry"].ToString();
                    list.Qty = item["Actual_Qty"].ToString();
                    list.Line = item["Lean"].ToString();
                    list.ModelName = item["Model"].ToString();
                    list.UserDate = new DateTime(time).ToShortDateString();
                    list.OnGoing = item["OnGoing"].ToString();
                    list.TotalComp = item["TotalComp"].ToString();
                    
                    lst.Add(list);
                }
            }
            return lst;
        }
        public ObservableCollection<Model_Inventory> Inventory()
        {
            var lst = new ObservableCollection<Model_Inventory>();
            string sql = "";

            sql = $"SELECT T0.* , CASE when Remain_qty = 0 THEN 'Done' WHEN Remain_qty> 0 THEN 'Ongoing' ELSE '' END 'Status', CAST((SELECT T2.ywsm + '; ' FROM(SELECT distinct a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111)USERDATE FROM(SELECT b.Barcode, b.BWBH, b.CLBH, b.GXXCC, b.XXCC, b.ZLBH, isnull(a.PlanQty, b.Qty)PlanQty, isnull(a.ActualQty, 0)ActualQty, b.USERDATE FROM Cutting_Barcodes2 b LEFT join  App_Cutting_Barcodes_Edit a ON a.Barcode = b.Barcode AND a.ZLBH = b.ZLBH AND  a.XXCC = b.XXCC AND a.BWBH = b.BWBH) a LEFT JOIN bwzl ON bwdh = bwbh) T2 WHERE T2.ZLBH = T0.RY AND T2.USERDATE = T0.USERDATE order by T2.ywsm asc FOR XML PATH('')) AS varchar(800) ) 'TotalComp'  ,CAST((SELECT T2.ywsm + '; ' FROM(SELECT a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111)USERDATE, CASE WHEN sum(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) = 0 THEN 'Done' WHEN SUM(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) > 0 THEN 'Ongoing' ELSE '' END 'Status' FROM(SELECT b.Barcode, b.BWBH, b.CLBH, b.GXXCC, b.XXCC, b.ZLBH, isnull(a.PlanQty, b.Qty)PlanQty, isnull(a.ActualQty, 0)ActualQty, b.USERDATE FROM Cutting_Barcodes2 b LEFT join  App_Cutting_Barcodes_Edit a ON a.Barcode = b.Barcode AND a.ZLBH = b.ZLBH AND  a.XXCC = b.XXCC AND a.BWBH = b.BWBH) a LEFT JOIN bwzl ON bwdh = bwbh GROUP BY a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111)HAVING sum(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) > 0) T2 WHERE T2.ZLBH = T0.RY AND T2.USERDATE = T0.USERDATE order by T2.ywsm asc FOR XML PATH('')) AS varchar(800) ) 'Ongoing',CAST((SELECT T2.ywsm + '; ' FROM(SELECT a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111)USERDATE, CASE WHEN sum(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) = 0 THEN 'Done'WHEN SUM(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) > 0 THEN 'Ongoing' ELSE '' END 'Status' FROM(SELECT b.Barcode, b.BWBH, b.CLBH, b.GXXCC, b.XXCC, b.ZLBH, isnull(a.PlanQty, b.Qty)PlanQty, isnull(a.ActualQty, 0)ActualQty, b.USERDATE FROM Cutting_Barcodes2 b LEFT join  App_Cutting_Barcodes_Edit a ON a.Barcode = b.Barcode AND a.ZLBH = b.ZLBH AND  a.XXCC = b.XXCC AND a.BWBH = b.BWBH) a LEFT JOIN bwzl ON bwdh = bwbh GROUP BY a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111) HAVING sum(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) = 0) T2 WHERE T2.ZLBH = T0.RY AND T2.USERDATE = T0.USERDATE order by T2.ywsm asc FOR XML PATH('')) AS varchar(800) ) 'Done' FROM(SELECT convert(varchar, CBs.USERDATE, 111) USERDATE, CBs.ZLBH RY, dd.ARTICLE, XieMing Model, sum(CBs.ActualQty) Actual_qty, sum(CBs.PlanQty) Plan_qty, sum(CBs.PlanQty) - sum(CBs.ActualQty) Remain_qty , PDSCH.LEAN, count(CBs.BWBH) counts, PDSCH.PSDT FROM(SELECT b.Barcode, b.BWBH, b.CLBH, b.GXXCC, b.XXCC, b.ZLBH, isnull(a.PlanQty, b.Qty)PlanQty, isnull(a.ActualQty, 0)ActualQty, b.USERDATE FROM Cutting_Barcodes2 b LEFT join  App_Cutting_Barcodes_Edit a ON a.Barcode = b.Barcode AND a.ZLBH = b.ZLBH AND  a.XXCC = b.XXCC AND a.BWBH = b.BWBH) CBs Left join PDSCH on PDSCH.ZLBH = CBs.ZLBH left join DDZL dd on DD.DDBH = PDSCH.RY left join zlzls2 zl on zl.ZLBH = PDSCH.RY and zl.CLBH = CBs.CLBH and zl.BWBH = CBs.BWBH Left join DE_ORDERM  DE on DE.ORDERNO = PDSCH.RY LEFT JOIN xxzl ON xxzl.ARTICLE = DD.ARTICLE left join Cutting_Barcode CB3 on CBs.Barcode = CB3.Barcode GROUP BY convert(varchar, CBs.USERDATE, 111), CBs.ZLBH, dd.ARTICLE, XieMing, PDSCH.LEAN, PDSCH.PSDT)  T0 WHERE Actual_qty>0";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            if (ta.Rows.Count > 0)
            {
                foreach (System.Data.DataRow item in ta.Rows)
                {
                    long time = Convert.ToDateTime(item["UserDate"]).Ticks;
                    var list = new Model_Inventory();
                    list.Art = item["Article"].ToString();
                    list.RY = item["Ry"].ToString();
                    list.Qty = (item["Actual_Qty"].ToString());
                    list.Line = item["Lean"].ToString();
                    list.ModelName = item["Model"].ToString();
                    list.UserDate = new DateTime(time).ToShortDateString();
                    list.OnGoing = item["OnGoing"].ToString();
                    list.TotalComp = item["TotalComp"].ToString();

                    lst.Add(list);
                }
            }
            return lst;
        }
        public ObservableCollection<Model_Inventory> Inventory(string ry )
        {
            var lst = new ObservableCollection<Model_Inventory>();
            string sql = "";
         
            sql = $"SELECT T0.* , CASE when Remain_qty = 0 THEN 'Done' WHEN Remain_qty> 0 THEN 'Ongoing' ELSE '' END 'Status', CAST((SELECT T2.ywsm + '; ' FROM(SELECT distinct a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111)USERDATE FROM(SELECT b.Barcode, b.BWBH, b.CLBH, b.GXXCC, b.XXCC, b.ZLBH, isnull(a.PlanQty, b.Qty)PlanQty, isnull(a.ActualQty, 0)ActualQty, b.USERDATE FROM Cutting_Barcodes2 b LEFT join  App_Cutting_Barcodes_Edit a ON a.Barcode = b.Barcode AND a.ZLBH = b.ZLBH AND  a.XXCC = b.XXCC AND a.BWBH = b.BWBH) a LEFT JOIN bwzl ON bwdh = bwbh) T2 WHERE T2.ZLBH = T0.RY AND T2.USERDATE = T0.USERDATE order by T2.ywsm asc FOR XML PATH('')) AS varchar(800) ) 'TotalComp'  ,CAST((SELECT T2.ywsm + '; ' FROM(SELECT a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111)USERDATE, CASE WHEN sum(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) = 0 THEN 'Done' WHEN SUM(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) > 0 THEN 'Ongoing' ELSE '' END 'Status' FROM(SELECT b.Barcode, b.BWBH, b.CLBH, b.GXXCC, b.XXCC, b.ZLBH, isnull(a.PlanQty, b.Qty)PlanQty, isnull(a.ActualQty, 0)ActualQty, b.USERDATE FROM Cutting_Barcodes2 b LEFT join  App_Cutting_Barcodes_Edit a ON a.Barcode = b.Barcode AND a.ZLBH = b.ZLBH AND  a.XXCC = b.XXCC AND a.BWBH = b.BWBH) a LEFT JOIN bwzl ON bwdh = bwbh GROUP BY a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111)HAVING sum(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) > 0) T2 WHERE T2.ZLBH = T0.RY AND T2.USERDATE = T0.USERDATE order by T2.ywsm asc FOR XML PATH('')) AS varchar(800) ) 'Ongoing',CAST((SELECT T2.ywsm + '; ' FROM(SELECT a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111)USERDATE, CASE WHEN sum(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) = 0 THEN 'Done'WHEN SUM(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) > 0 THEN 'Ongoing' ELSE '' END 'Status' FROM(SELECT b.Barcode, b.BWBH, b.CLBH, b.GXXCC, b.XXCC, b.ZLBH, isnull(a.PlanQty, b.Qty)PlanQty, isnull(a.ActualQty, 0)ActualQty, b.USERDATE FROM Cutting_Barcodes2 b LEFT join  App_Cutting_Barcodes_Edit a ON a.Barcode = b.Barcode AND a.ZLBH = b.ZLBH AND  a.XXCC = b.XXCC AND a.BWBH = b.BWBH) a LEFT JOIN bwzl ON bwdh = bwbh GROUP BY a.ZLBH, bwzl.ywsm, CONVERT(VARCHAR, a.USERDATE, 111) HAVING sum(isnull(a.PlanQty, 0) - isnull(a.ActualQty, 0)) = 0) T2 WHERE T2.ZLBH = T0.RY AND T2.USERDATE = T0.USERDATE order by T2.ywsm asc FOR XML PATH('')) AS varchar(800) ) 'Done' FROM(SELECT convert(varchar, CBs.USERDATE, 111) USERDATE, CBs.ZLBH RY, dd.ARTICLE, XieMing Model, sum(CBs.ActualQty) Actual_qty, sum(CBs.PlanQty) Plan_qty, sum(CBs.PlanQty) - sum(CBs.ActualQty) Remain_qty , PDSCH.LEAN, count(CBs.BWBH) counts, PDSCH.PSDT FROM(SELECT b.Barcode, b.BWBH, b.CLBH, b.GXXCC, b.XXCC, b.ZLBH, isnull(a.PlanQty, b.Qty)PlanQty, isnull(a.ActualQty, 0)ActualQty, b.USERDATE FROM Cutting_Barcodes2 b LEFT join  App_Cutting_Barcodes_Edit a ON a.Barcode = b.Barcode AND a.ZLBH = b.ZLBH AND  a.XXCC = b.XXCC AND a.BWBH = b.BWBH) CBs Left join PDSCH on PDSCH.ZLBH = CBs.ZLBH left join DDZL dd on DD.DDBH = PDSCH.RY left join zlzls2 zl on zl.ZLBH = PDSCH.RY and zl.CLBH = CBs.CLBH and zl.BWBH = CBs.BWBH Left join DE_ORDERM  DE on DE.ORDERNO = PDSCH.RY LEFT JOIN xxzl ON xxzl.ARTICLE = DD.ARTICLE left join Cutting_Barcode CB3 on CBs.Barcode = CB3.Barcode GROUP BY convert(varchar, CBs.USERDATE, 111), CBs.ZLBH, dd.ARTICLE, XieMing, PDSCH.LEAN, PDSCH.PSDT)  T0 WHERE ry like '{ry}'";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            if (ta.Rows.Count > 0)
            {
                foreach (System.Data.DataRow item in ta.Rows)
                {
                    long time = Convert.ToDateTime(item["UserDate"]).Ticks;
                    var list = new Model_Inventory();
                    list.Art = item["Article"].ToString();
                    list.RY = item["Ry"].ToString();
                    list.Qty = (item["Actual_Qty"].ToString());
                    list.Line = item["Lean"].ToString();
                    list.ModelName = item["Model"].ToString();
                    list.UserDate = new DateTime(time).ToShortDateString();
                    list.OnGoing = item["OnGoing"].ToString();
                    list.TotalComp = item["TotalComp"].ToString();

                    lst.Add(list);
                }
            }
            return lst;
        }
        public System.Collections.ObjectModel.ObservableCollection<string> GetLean(string b)
        {
            var _lean = new System.Collections.ObjectModel.ObservableCollection<string>();
            string sql = $"select leanname from BDepartment where DepType in (2,8) and IsActive=1 and Loc is not null and Loc='{b}'";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            if (ta.Rows.Count > 0)
            {
                foreach (System.Data.DataRow lean in ta.Rows)
                {
                    _lean.Add(lean[0].ToString());
                }
            }
            return _lean;
        }
        //private  void TestScan()
        //{
        //    Navigation.PushModalAsync(new Views.AutoCuttingScanView("20230700805"));
        //}
        
        //private async void Scan()
        //{
        //   var Camera = new ZXing.Net.Mobile.Forms.ZXingScannerPage();
        //    Camera.OnScanResult += (result) =>
        //    {
        //        Camera.IsScanning = false;
        //        Device.BeginInvokeOnMainThread(() =>
        //        {
        //            Navigation.PopModalAsync();

        //            if (result.Text != "")
        //            {

        //                Navigation.PushModalAsync(new Views.AutoCuttingScanView(result.Text));
        //            }

        //        });


        //    };
        //    await Navigation.PushModalAsync(Camera);
        //}

    
    }
    public class Model_Inventory
    {
        public string UserDate { get; set; } 
        public string RY{ get; set; } 
        public string Art{ get; set; } 
        public string ModelName { get; set; } 
        public string Qty{ get; set; } 
        public string Line{ get; set; } 
        public string TotalComp{ get; set; } 
        public string OnGoing{ get; set; } 
        public string Done{ get; set; } 
        public string Delivered{ get; set; } 
        public Color Delay{ get; set; } 
        public DateTime ProDate{ get; set; } 
        public  string Unit { get; set; }
    }
   
}
