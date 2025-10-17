using MaterialTracking.Models;
using MaterialTracking.UsersControl;
using MTM.Model;
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
    public partial class InvetoryPage : ContentPage
    {
        public InvetoryPage(string ry)
        {
            InitializeComponent();
            BindingContext = new InventoryPage_ModelView(ry);
            Load();
           

        }

        void Load()
        {
            var bind = this.BindingContext as InventoryPage_ModelView;// new InventoryPage_ModelView();


            var data1 = bind.Data1;
            var data2 = bind.Data2;

            foreach (var m in data1)
            {               
                stackContent.Children.Add(new Uc_InventoryExtend(m,data2));                
            }
        }       
    }




    public class InventoryPage_ModelView:BaseViewModel
    {
        private ObservableCollection<InventoryPageGeneral_Type> _data1;
        private ObservableCollection<InventoryPageDetail_Type> _data2;

         public ObservableCollection<InventoryPageGeneral_Type> Data1 { get =>data1() ; set { SetProperty(ref _data1, value); } }
         public ObservableCollection<InventoryPageDetail_Type> Data2 { get =>data2() ; set { SetProperty(ref _data2, value); } }
        private string _ry;

        public InventoryPage_ModelView(string ry)
        {
            _ry = ry;
        }
        private ObservableCollection<InventoryPageDetail_Type> data2()
        {
            var list = new ObservableCollection<InventoryPageDetail_Type>();

            var GenData = _data1 == null ? _data1 = data1() : _data1;

            if(GenData.Any())

                foreach (InventoryPageGeneral_Type Gen in GenData)
                {
                    string sql = $"SELECT zlbh, bwbh, bwzl.ywsm, XXcc,prono, convert(varchar, convert(int,Planqty)) +'/'+ convert(varchar,convert(int, actualqty)) Qty,[action] from App_Cutting_Barcodes_Edit app inner join bwzl on app .BWBH =bwzl.bwdh  where barcode = '{Gen.Barcode}' and zlbh = '{Gen.ZLBH}' and clbh = '{Gen.CLBH}'  and convert(int, PlanQty) !=0 and convert(int,ActualQty)!=0 order by XXCC";
                    //and bwbh = '{Gen.BWBH}' 
                    var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
                     

                    foreach (System.Data.DataRow Detail in ta.Rows)
                    {
                       var targetArr = Detail["Qty"].ToString().Split('/');

                        var act = Detail["Action"].ToString();

                        if (int.Parse(targetArr[1]) > 0 )
                        {
                            list.Add(new InventoryPageDetail_Type
                            {
                                Component = Detail["ywsm"].ToString()
                                ,
                                Zlbh = Detail["ZLBH"].ToString()
                               ,
                                Bwbh = Detail["bwbh"].ToString()
                               ,
                                Qty = Detail["Qty"].ToString()
                                ,
                                Size = Detail["xxcc"].ToString()
                                ,
                                Prono = Detail["Prono"].ToString()
                            });
                        }


                    }
                }

            return list;
        }
        private ObservableCollection<InventoryPageGeneral_Type>  data1()
        {
            //sql Summary
            string sql = "";
            if(string.IsNullOrEmpty( _ry)) sql = @"SELECT distinct t0.Barcode, T0.USERDATE, ry,article,model,T0.clbh,

 lean,psdt prodate,

             Cast((SELECT T2.ywsm + '; '
      
                   FROM(SELECT DISTINCT a.zlbh,
                                          bwzl.ywsm,
                                          CONVERT(VARCHAR, a.userdate, 111)USERDATE
      
                          FROM(SELECT b.barcode,
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
      
                                                  AND a.xxcc = b.xxcc
      
                                                  AND a.bwbh = b.bwbh) a
      
                                LEFT JOIN bwzl
      
                                       ON bwdh = bwbh) T2
      
                   WHERE  T2.zlbh = T0.ry
      
                          AND T2.userdate = T0.userdate
      
                   ORDER  BY T2.ywsm ASC
      
                   FOR xml path('')) AS VARCHAR(800)) 'TotalComp'
FROM(SELECT CONVERT(VARCHAR, CBs.userdate, 111)   USERDATE,
              CBs.zlbh                              RY,
              dd.article,
              xieming                               Model,
              Sum(CBs.actualqty)                    Actual_qty,
              Sum(CBs.planqty)                      Plan_qty,
              Sum(CBs.planqty) - Sum(CBs.actualqty) Remain_qty,
              pdsch.lean,
                CBs.BWBH,
                cbs.CLBH,
              pdsch.psdt, cbs.Barcode
       FROM(SELECT b.barcode,
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
                               AND a.xxcc = b.xxcc
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
                CBs.BWBH,
                cbs.CLBH,
              pdsch.psdt, cbs.Barcode) T0
 WHERE  actual_qty > 0  and t0.PSDT >= getdate() - 7 ";
            else sql = @"SELECT distinct t0.Barcode, T0.USERDATE, ry,article,model,T0.clbh,

 lean,psdt prodate,

             Cast((SELECT T2.ywsm + '; '
      
                   FROM(SELECT DISTINCT a.zlbh,
                                          bwzl.ywsm,
                                          CONVERT(VARCHAR, a.userdate, 111)USERDATE
      
                          FROM(SELECT b.barcode,
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
      
                                                  AND a.xxcc = b.xxcc
      
                                                  AND a.bwbh = b.bwbh) a
      
                                LEFT JOIN bwzl
      
                                       ON bwdh = bwbh) T2
      
                   WHERE  T2.zlbh = T0.ry
      
                          AND T2.userdate = T0.userdate
      
                   ORDER  BY T2.ywsm ASC
      
                   FOR xml path('')) AS VARCHAR(800)) 'TotalComp'
FROM(SELECT CONVERT(VARCHAR, CBs.userdate, 111)   USERDATE,
              CBs.zlbh                              RY,
              dd.article,
              xieming                               Model,
              Sum(CBs.actualqty)                    Actual_qty,
              Sum(CBs.planqty)                      Plan_qty,
              Sum(CBs.planqty) - Sum(CBs.actualqty) Remain_qty,
              pdsch.lean,
                CBs.BWBH,
                cbs.CLBH,
              pdsch.psdt, cbs.Barcode
       FROM(SELECT b.barcode,
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
                               AND a.xxcc = b.xxcc
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
                CBs.BWBH,
                cbs.CLBH,
              pdsch.psdt, cbs.Barcode) T0
 WHERE  actual_qty > 0  and t0.PSDT >= getdate() - 7 and (RY like '%"+_ry+ "%' OR article like '%" + _ry + "%' Or model  like '%" + _ry + "%')";

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            var list = new ObservableCollection<InventoryPageGeneral_Type>();

            foreach (System.Data.DataRow r in ta.Rows)
            {
                var arrcomp = r["TotalComp"].ToString().Split(';');                

                list.Add(new InventoryPageGeneral_Type
                {
                    Art = r["article"].ToString(),
                    //BWBH = r["Bwbh"].ToString(),
                    Barcode = r["barcode"].ToString(),
                    CLBH = r["Clbh"].ToString(),
                    Components= r["TotalComp"].ToString(),
                    Line = r["lean"].ToString(),
                    ModelName = r["model"].ToString(),
                    ProDate = r["prodate"].ToString().Split(' ')[0],
                   // QtyAndTarget = r["Qty"].ToString(),
                    ZLBH = r["ry"].ToString(),
                });
             
            }

            return list;
        }

    }
    public class InventoryPageGeneral_Type
    {
        public string Barcode{ get; set; }
        public string ZLBH { get; set; }
        public string Art { get; set; }
        public string ModelName { get; set; }
        public string Line { get; set; }
        public string ProDate { get; set; }
        public string Target { get; set; }
        public string Actual { get; set; }
        public string Components { get; set; }
        public string OnGoing{ get; set; }
        public string Done{ get; set; }
        public string Delivered{ get; set; }
        public string CLBH{ get; set; }
        public string BWBH{ get; set; }
        public string[] CountComp{ get; set; }
        public string Comp{ get; set; }

        public Color ColorFull { get; set; }
        public Color BgrColor { get; set; }
        public Color BgrColorDelay { get; set; }

       public System.Windows.Input.ICommand ClickCommand { get; set; }

    }
    public class InventoryPageDetail_Type
    {
        public string Prono { set; get; }
        public string Component { set; get; }
        public string Zlbh { set; get; }
        public string Bwbh { get; set; }
        public string Size { get; set; }
        public string Qty { get; set; }
        
       
    }
}