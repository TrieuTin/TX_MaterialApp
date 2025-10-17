using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using MaterialTracking.Class;
using System.Windows.Input;

namespace MaterialTracking.ViewModels
{
    public class AutoCuttingScanModelView : ViewModels_Base
    {
        string _barcode = "";

        public string BarCode { get; set; }
        public string Xieming { get; set; }
        public string Article { get; set; }
        public string Shift { get; set; }
        public string WorkDate { get; set; }
        public string CutDie { get; set; }
        public string DMQty { get; set; }
        public string MachineNo { get; set; }
        public string User { get; set; }
        public string YWPM { get; set; }
        public string Layer { get; set; }
        public string Lean { get; set; }
        public string CLBH{ get; set; }

        ICommand _collapsedCmd;

        List<string> _lstRY;
        public ObservableCollection<AutoCuttingScan_GridTitle_Model> DataTitle { get; set; }
        public ObservableCollection<AutoCuttingScan_Data> Data{ get; set; }
        public ObservableCollection<DB.Table_Barcode> Data_Confirm{ get; set; }
        public ObservableCollection<ACS_DataGroup> DataGroup{ get; set; }

        public ICommand CollapsedCmd { get => _collapsedCmd; set { _collapsedCmd = value; OnPropertyChanged("CollapsedCmd"); } }
        
        private GridLength _rowHeight;

        public GridLength RowHeight
        {
            get { return _rowHeight; }
            set
            {
                _rowHeight = value;
                OnPropertyChanged(nameof(RowHeight));
            }
        }

        public AutoCuttingScanModelView(string Barcode,List<string> RYs)
        {
            _barcode = Barcode;
            //_ry = ry;
            _lstRY = RYs;

            Set_Data();

            Set_MainTitle();

            Set_ColumnTitle();

            Set_DataConfirm();

            Set_DataGrp();
            CollapsedCmd = new Command(CollapseRunTime);
        }

        private void CollapseRunTime(object obj)
        {
            if (RowHeight.Value != 200)
                RowHeight = new GridLength(200);
            else RowHeight = new GridLength(0);
        }

        private void Set_DataGrp()
        {            //and CB2.ZLBH ='{_ry}'
            string sql = $"SELECT bc.zlbh,bc.clbh,bc.bwbh,bc.xxcc CC,CONVERT (INT, Isnull(GBb.gqty, 0)) ActualQty,CONVERT(INT, Isnull(GBc.gqty, 0))  RemainQty,Isnull(CB2.qty, 0)                 PlanQty,Isnull(CB2.gxxcc, '')              GXXCC,Isnull(GBa.gqty, 0)                GQty,Isnull(GBa_count.gqty, 1)          GQty_merge FROM(SELECT c1.barcode,c1.zlbh,c1.clbh,c1.xxcc,c1.gxxcc,c1.qty,c1.userid,c1.userdate,c1.yn,c1.bwbh,sum(case when c2.Action = '-' THEN isnull((-1) * c2.actualqty, 0) ELSE isnull(c2.actualqty, 0) END)  ActualQty FROM   cutting_barcodes2 c1 LEFT JOIN app_cutting_barcodes_edit c2 ON c2.barcode = c1.barcode AND c2.zlbh = c1.zlbh AND c2.clbh = c1.clbh AND c2.bwbh = c1.bwbh AND c1.xxcc = c2.xxcc WHERE  c1.barcode = '{_barcode}' GROUP BY c1.Barcode, c1.ZLBH, c1.CLBH, c1.XXCC, c1.GXXCC, c1.Qty, c1.USERID, c1.USERDATE, c1.YN, c1.BWBH) BC LEFT JOIN pdsch PD ON PD.zlbh = BC.zlbh LEFT JOIN ddzls DDS ON BC.xxcc = DDS.cc AND DDS.ddbh = PD.ry LEFT JOIN cutting_barcodes2 CB2 ON CB2.zlbh = BC.zlbh AND CB2.xxcc = BC.xxcc AND CB2.clbh = BC.clbh AND CB2.bwbh = BC.bwbh AND CB2.barcode = BC.barcode LEFT JOIN(SELECT barcode, zlbh,bwbh,gxxcc,Isnull(Sum(qty), 0) GQty FROM   cutting_barcodes2 WHERE  barcode = '{_barcode}' GROUP BY barcode, zlbh, bwbh, gxxcc) GBa ON GBa.barcode = CB2.barcode AND GBa.gxxcc = CB2.gxxcc AND GBa.zlbh = CB2.zlbh AND GBa.bwbh = CB2.bwbh LEFT JOIN(SELECT c1.barcode,  c1.zlbh, c1.bwbh, c1.gxxcc, Isnull(Sum(c2.actualqty), 0) GQty FROM   cutting_barcodes2 c1 LEFT JOIN app_cutting_barcodes_edit c2 ON c2.barcode = c1.barcode AND c2.zlbh = c1.zlbh AND c2.clbh = c1.clbh AND c2.bwbh = c1.bwbh AND c1.xxcc = c2.xxcc  WHERE c1.barcode = '{_barcode}' GROUP BY c1.barcode, c1.zlbh, c1.bwbh, gxxcc)GBb ON GBb.barcode = CB2.barcode AND GBb.gxxcc = CB2.gxxcc AND GBb.zlbh = CB2.zlbh AND GBb.bwbh = CB2.bwbh  LEFT JOIN(SELECT c1.barcode,  c1.zlbh, c1.bwbh, c1.gxxcc, Isnull(Sum(c1.qty), 0) -Isnull(Sum(c2.actualqty), 0)GQty FROM   cutting_barcodes2 c1 LEFT JOIN app_cutting_barcodes_edit c2 ON c2.barcode = c1.barcode AND c2.zlbh = c1.zlbh AND c2.clbh = c1.clbh AND c2.bwbh = c1.bwbh AND c1.xxcc = c2.xxcc WHERE c1.barcode = '{_barcode}' GROUP BY c1.barcode, c1.zlbh, c1.bwbh, gxxcc)GBc ON GBc.barcode = CB2.barcode AND GBc.gxxcc = CB2.gxxcc AND GBc.zlbh = CB2.zlbh AND GBc.bwbh = CB2.bwbh LEFT JOIN(SELECT barcode, zlbh, bwbh, gxxcc, Count(gxxcc) GQty FROM cutting_barcodes2 WHERE barcode = '{_barcode}' GROUP BY barcode, zlbh, gxxcc, bwbh)GBa_count ON GBa_count.barcode = CB2.barcode AND GBa_count.gxxcc = CB2.gxxcc AND GBa_count.zlbh = CB2.zlbh AND GBa_count.bwbh = CB2.bwbh WHERE BC.barcode = '{_barcode}'  ORDER BY bc.zlbh, bc.clbh, bc.bwbh, BC.xxcc";
           
            var da = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
          
            var list =new ObservableCollection<ACS_DataGroup>();
         
            if (da.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in da.Rows)
                {

                    list.Add(new ACS_DataGroup
                    {
                        Bwbh = row["Bwbh"].ToString(),
                        CC= row["CC"].ToString(),
                        Clbh= row["CLBH"].ToString(),
                        Gqty=Math.Abs( Convert.ToInt32( row["GQty"])),
                        Gxxcc= row["GXXCC"].ToString(),
                        Qty= row["planQty"].ToString(),
                        Remain = row["RemainQty"].ToString(),
                        Zlbh= row["zlbh"].ToString(),
                        Gqty_merge= row["Gqty_merge"].ToString(),
                        ActualQty =Math.Abs( Convert.ToInt32( row["ActualQty"]))

                    });
                }
            }
            DataGroup = list;
        }

        void Set_MainTitle()
        {
            string sql = $"select CB.Barcode, CB.WorkDate, CB.Layers, CB.Shift, CB.Article , CB.MachineNo, CB.CutDie, CB.MUSERID,CB.DepID, CB.USERID, CB.USERDATE,Total.Qty, Total.DMQty, xxzl.DAOMH,xxzl.DDMH, xxzl.XieMing,BD.DepName from Cutting_Barcode CB left join xxzl on xxzl.ARTICLE = CB.ARTICLE left join BDepartment BD on BD.ID = CB.DepID left join(select Barcode,convert(int, SUM(Qty)) Qty,convert(int, Sum(DMQty)) DMQty from Cutting_Barcodes group by Barcode )Total on Total.Barcode = CB.Barcode LEFT JOIN CuttingFile CT ON CT.CuttingID = CB.Article  AND CT.BWBH = CB.BWBH WHERE cb.Barcode = '{_barcode}'";

            var data = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            sql = $"SELECT Distinct CLBH,ywpm FROM Cutting_Barcodes3 left join clzl on clzl.cldh = Cutting_Barcodes3.CLBH where Barcode = '{_barcode}'";

            var data2 = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            if (data2 == null)
            {
                Services.Alert.Msg("Cảnh báo", "Không có mạng", "CANCEL");return;
            }
            if (data2.Rows.Count>0)
            {

                YWPM = data2.Rows[0]["Ywpm"].ToString();
                CLBH = data2.Rows[0]["CLBH"].ToString();
            }
            if (data.Rows.Count >0)
            {
                foreach (System.Data.DataRow r in data.Rows)
                {
                    BarCode = r["barcode"].ToString();
                    Xieming = r["XieMing"].ToString();
                    Article = r["Article"].ToString();
                    Shift = r["Shift"].ToString();
                    WorkDate = DateTime.TryParse(r["Workdate"].ToString(), out DateTime d) ? d.ToString("yyyy-MM-dd") : "NO Date";
                    CutDie = r["CutDie"].ToString();
                    DMQty = r["DMqty"].ToString();
                    MachineNo = r["MachineNo"].ToString();
                    User = r["MuserID"].ToString();
                    Layer = r["Layers"].ToString();
                  
                    Lean = r["DepName"].ToString();
                }
            }
        }
       
        void Set_ColumnTitle()
        {
            //Tim xem co bao nhieu size trong barcode nay
            string sql = $"Select distinct case when CHARINDEX('K',XXCC)> 0 then ''+ LTRIM(RTRIM(XXCC)) else LTRIM(RTRIM(XXCC)) end CC 	from Cutting_Barcodes2 where Barcode = '{_barcode}' AND Cutting_Barcodes2.ZLBH in(select distinct c.ZLBH from Cutting_Barcodes2 c where Barcode='{_barcode}' )  order by 1";
            
            var data = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            
            var lst = new ObservableCollection<AutoCuttingScan_GridTitle_Model>();
           
            if (!Network.Net.HasNet)
            {
                Services.Alert.Msg("Cảnh báo", "Không có mạng", "CANCEL"); return;
            }
            if (data.Rows.Count >0)
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    lst.Add(new AutoCuttingScan_GridTitle_Model
                    {
                        Size = data.Rows[i]["CC"].ToString()
                    });  
                }
                DataTitle = lst;
            }
        }
        void Set_Data()
        {
            #region Code SQL
            var sql = $"if object_id(N'TempDB..#Nyx') is not null	begin drop table #Nyx  end Select CB2.ZLBH DDBH, CB2.CLBH,CB2.BWBH, bwzl.ywsm,CB2.XXCC CC, CB2.GXXCC,CB2.Qty,isnull(GBa.GQty, 0) GQty,CB.DMQty into #Nyx from Cutting_Barcodes2 CB2 left join Cutting_Barcodes CB on CB.Barcode = CB2.Barcode and CB.BWBH = CB2.BWBH and CB.CLBH = CB2.CLBH and CB.ZLBH = CB2.ZLBH left join bwzl on bwzl.bwdh = CB2.BWBH Left join(select Barcode, ZLBH, GXXCC, BWBH, CLBH, sum(qty) GQty from Cutting_Barcodes2 group by Barcode, ZLBH, GXXCC, BWBH, CLBH )GBa on GBa.Barcode = CB2.Barcode and GBa.GXXCC = CB2.GXXCC and GBa.ZLBH = CB2.ZLBH and GBa.BWBH = CB2.BWBH and GBa.CLBH = CB2.CLBH where CB2.Barcode = '{_barcode}' Select A.DDBH,a.CLBH,A.BWBH, A.ywsm, A.DMQty, Sum(A.Qty) Total, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '01.0K'),0) ' 01.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '01.5K'),0) ' 01.5K'									, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '02.0K'),0) ' 02.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '02.5K'),0) ' 02.5K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '03.0K'),0) ' 03.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '03.5K'),0) ' 03.5K'		, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '04.0K'),0) ' 04.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '04.5K'),0) ' 04.5K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '05.0K'),0) ' 05.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '05.5K'),0) ' 05.5K'	, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '06.0K'),0) ' 06.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '06.5K'),0) ' 06.5K'											, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '07.0K'),0) ' 07.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '07.5K'),0) ' 07.5K'												, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '08.0K'),0) ' 08.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '08.5K'),0) ' 08.5K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '09.0K'),0) ' 09.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '09.5K'),0) ' 09.5K'									, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '10.0K'),0) ' 10.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '10.5K'),0) ' 10.5K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '11.0K'),0) ' 11.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '11.5K'),0) ' 11.5K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '12.0K'),0) ' 12.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '12.5K'),0) ' 12.5K'	, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '13.0K'),0) ' 13.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '13.5K'),0) ' 13.5K'			, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '01'),0) '01', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '01.5'),0) '01.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '02'),0) '02', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '02.5'),0) '02.5'	, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '03'),0) '03', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '03.5'),0) '03.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '04'),0) '04', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '04.5'),0) '04.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '05'),0) '05', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '05.5'),0) '05.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '06'),0) '06', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '06.5'),0) '06.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '07'),0) '07', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '07.5'),0) '07.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '08'),0) '08', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '08.5'),0) '08.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '09'),0) '09', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '09.5'),0) '09.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '10'),0) '10', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '10.5'),0) '10.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '11'),0) '11', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '11.5'),0) '11.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '12'),0) '12', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '12.5'),0) '12.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '13'),0) '13', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '13.5'),0) '13.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '14'),0) '14', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '14.5'),0) '14.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '15'),0) '15', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '15.5'),0) '15.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '16'),0) '16', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '16.5'),0) '16.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '17'),0) '17', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '17.5'),0) '17.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '18'),0) '18', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '18.5'),0) '18.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '19'),0) '19', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '19.5'),0) '19.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '20'),0) '20', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '20.5'),0) '20.5'	from #Nyx A group by A.DDBH, A.BWBH, A.ywsm, A.CLBH, A.DMQty Order by A.BWBH, A.ywsm, A.DDBH ";


            #endregion
            if (!Network.Net.HasNet)
            {
                Services.Alert.Msg("Cảnh báo", "Không có mạng", "CANCEL"); return;
            }
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            sql = $"select CC sizes from ddzls where ddzls.DDBH in(select distinct c.ZLBH from Cutting_Barcodes2 c where Barcode='{_barcode}' ) group by CC order by sizes";

            var taSize = DB.SQL.ConnectDB.Connection.FillDataTable(sql);  
            
            

            var list = new ObservableCollection<AutoCuttingScan_Data>();

            
            if (ta.Rows.Count >0)
            {

                foreach (System.Data.DataRow DRow in ta.Rows)
                {                    
                                    
                    if (_lstRY.Contains(string.Format("{0}_{1}",DRow["DDBH"].ToString(),DRow["bwbh"].ToString())))
                    {

                        list.Add(new AutoCuttingScan_Data
                        {
                            DDBH = DRow["DDBH"].ToString(),
                            DMQty = double.TryParse(DRow["DMQty"].ToString(), out double dmqty) ? dmqty : 0,
                            Total = double.TryParse(DRow["Total"].ToString(), out double total) ? total : 0,
                            CLBH = DRow["CLBH"].ToString(),
                            Size_01k = ProductSize(" 01.0K", taSize )? DRow[" 01.0K"].ToString():"",
                            Size_015k = ProductSize(" 01.5K", taSize) ? DRow[" 01.5K"].ToString():"",
                            Size_02k = ProductSize(" 02.0K", taSize) ? DRow[" 02.0K"].ToString() : "",
                            Size_025k = ProductSize(" 02.5K", taSize) ? DRow[" 02.5K"].ToString() : "",
                            Size_03k = ProductSize(" 03.0K", taSize) ? DRow[" 03.0K"].ToString() : "",
                            Size_035k = ProductSize(" 03.5K", taSize) ? DRow[" 03.5K"].ToString() : "",
                            Size_04k = ProductSize(" 04.0K", taSize) ? DRow[" 04.0K"].ToString() : "",
                            Size_045k = ProductSize(" 04.5K", taSize) ? DRow[" 04.5K"].ToString() : "",
                            Size_05k = ProductSize(" 05.0K", taSize) ? DRow[" 05.0K"].ToString() : "",
                            Size_055k = ProductSize(" 05.5K", taSize) ? DRow[" 05.5K"].ToString() : "",
                            Size_06k = ProductSize(" 06.0K", taSize) ? DRow[" 06.0K"].ToString() : "",
                            Size_065k = ProductSize(" 06.5K", taSize) ? DRow[" 06.5K"].ToString() : "",
                            Size_07k = ProductSize(" 07.0K", taSize) ? DRow[" 07.0K"].ToString() : "",
                            Size_075k = ProductSize(" 07.5K", taSize) ? DRow[" 07.5K"].ToString() : "",
                            Size_08k = ProductSize(" 08.0K", taSize) ? DRow[" 08.0K"].ToString() : "",
                            Size_085k = ProductSize(" 08.5K", taSize) ? DRow[" 08.5K"].ToString() : "",
                            Size_09k = ProductSize(" 09.0K", taSize) ? DRow[" 09.0K"].ToString() : "",
                            Size_095k = ProductSize(" 09.5K", taSize) ? DRow[" 09.5K"].ToString() : "",
                            Size_10k = ProductSize(" 10.0K", taSize) ? DRow[" 10.0K"].ToString() : "",
                            Size_105k = ProductSize(" 10.5K", taSize) ? DRow[" 10.5K"].ToString() : "",
                            Size_11k = ProductSize(" 11.0K", taSize) ? DRow[" 11.0K"].ToString() : "",
                            Size_115k = ProductSize(" 11.5K", taSize) ? DRow[" 11.5K"].ToString() : "",
                            Size_12k = ProductSize(" 12.0K", taSize) ? DRow[" 12.0K"].ToString() : "",
                            Size_125k = ProductSize(" 12.5K", taSize) ? DRow[" 12.5K"].ToString() : "",
                            Size_13k = ProductSize(" 13.0K", taSize) ? DRow[" 13.0K"].ToString() : "",
                            Size_135k = ProductSize(" 13.5K", taSize) ? DRow[" 13.5K"].ToString() : "",
                            Size_01 = ProductSize("01", taSize) ? DRow["01"].ToString() : "",
                            Size_015 = ProductSize("01.5", taSize) ? DRow["01.5"].ToString() : "",
                            Size_02 = ProductSize("02", taSize) ? DRow["02"].ToString() : "",
                            Size_025 = ProductSize("02.5", taSize) ? DRow["02.5"].ToString() : "",
                            Size_03 = ProductSize("03", taSize) ? DRow["03"].ToString() : "",
                            Size_035 = ProductSize("03.5", taSize) ? DRow["03.5"].ToString() : "",
                            Size_04 = ProductSize("04", taSize) ? DRow["04"].ToString() : "",
                            Size_045 = ProductSize("04.5", taSize) ? DRow["04.5"].ToString() : "",
                            Size_05 = ProductSize("05", taSize) ? DRow["05"].ToString() : "",
                            Size_055 = ProductSize("05.5", taSize) ? DRow["05.5"].ToString() : "",
                            Size_06 = ProductSize("06", taSize) ? DRow["06"].ToString() : "",
                            Size_065 = ProductSize("06.5", taSize) ? DRow["06.5"].ToString() : "",
                            Size_07 = ProductSize("07", taSize) ? DRow["07"].ToString() : "",
                            Size_075 = ProductSize("07.5", taSize) ? DRow["07.5"].ToString() : "",
                            Size_08 = ProductSize("08", taSize) ? DRow["08"].ToString() : "",
                            Size_085 = ProductSize("08.5", taSize) ? DRow["08.5"].ToString() : "",
                            Size_09 = ProductSize("09", taSize) ? DRow["09"].ToString() : "",
                            Size_095 = ProductSize("09.5", taSize) ? DRow["09.5"].ToString() : "",
                            Size_10 = ProductSize("10", taSize) ? DRow["10"].ToString() : "",
                            Size_105 = ProductSize("10.5", taSize) ? DRow["10.5"].ToString() : "",
                            Size_11 = ProductSize("11", taSize) ? DRow["11"].ToString() : "",
                            Size_115 = ProductSize("11.5", taSize) ? DRow["11.5"].ToString() : "",
                            Size_12 = ProductSize("12", taSize) ? DRow["12"].ToString() : "",
                            Size_125 = ProductSize("12.5", taSize) ? DRow["12.5"].ToString() : "",
                            Size_13 = ProductSize("13", taSize) ? DRow["13"].ToString() : "",
                            Size_135 = ProductSize("13.5", taSize) ? DRow["13.5"].ToString() : "",
                            Size_14 = ProductSize("14", taSize) ? DRow["14"].ToString() : "",
                            Size_145 = ProductSize("14.5", taSize) ? DRow["14.5"].ToString() : "",
                            Size_15 = ProductSize("15", taSize) ? DRow["15"].ToString() : "",
                            Size_155 = ProductSize("15.5", taSize) ? DRow["15.5"].ToString() : "",
                            Size_16 = ProductSize("16", taSize) ? DRow["16"].ToString() : "",
                            Size_165 = ProductSize("16.5", taSize) ? DRow["16.5"].ToString() : "",
                            Size_17 = ProductSize("17", taSize) ? DRow["17"].ToString() : "",
                            Size_175 = ProductSize("17.5", taSize) ? DRow["17.5"].ToString() : "",
                            Size_18 = ProductSize("18", taSize) ? DRow["18"].ToString() : "",
                            Size_185 = ProductSize("18.5", taSize) ? DRow["18.5"].ToString() : "",
                            Size_19 = ProductSize("19", taSize) ? DRow["19"].ToString() : "",
                            Size_195 = ProductSize("19.5", taSize) ? DRow["19.5"].ToString() : "",
                            Size_20 = ProductSize("20", taSize) ? DRow["20"].ToString() : "",
                            Size_205 = ProductSize("20.5", taSize) ? DRow["20.5"].ToString() : "",

                            Component = DRow["Ywsm"].ToString(),
                            BWBH = DRow["BWBH"].ToString()


                        });
                    }
                }

                //var GrpDDBH = list.GroupBy(r => r.DDBH).ToList();

                //var listDDBH = new List<AutoCuttingScan_Data>();

                //foreach (var item in GrpDDBH)
                //{
                //    var ddbhDup= list.Where(r => r.DDBH == item.Key).ToList();

                //    foreach (var item1 in ddbhDup)
                //    {
                //        listDDBH.Add(item1);
                //    }

                //}

                //foreach (var DuplicationQTy in listDDBH )
                //{

                //}

                Data = list;
            }
        }
        private bool ProductSize(string size, System.Data.DataTable tablesize)
        {
            var sx = false;
            for (int i = 0; i < tablesize.Rows.Count; i++)
            {
                if (tablesize.Rows[i][0].ToString().Trim() == size.Trim())
                {
                    sx = true;break;
                }
            }
            return sx;
        }
        void Set_DataConfirm()
        {

            //string sql = $"select* FROM App_Cutting_Barcodes_Edit where barcode='{_barcode}'";

            //string sql = $"select distinct Barcode,ZLBH,CLBH,XXCC,PlanQty,(select sum(ActualQty) QTy from App_Cutting_Barcodes_Edit where Barcode = E.Barcode and zlbh = E.ZLBH and BWBH = E.BWBH and XXCC = E.XXCC and Action = '+') -(select ISNULL(sum(ActualQty), 0) subQTy from App_Cutting_Barcodes_Edit where Barcode = E.Barcode and zlbh = E.ZLBH and BWBH = E.BWBH and XXCC = E.XXCC and Action = '-') ActualQty,USERID,(select top 1  USERDATE from App_Cutting_Barcodes_Edit E  where Barcode = Barcode and zlbh = ZLBH and BWBH = BWBH and XXCC = XXCC order by Times desc ) userdate,YN,BWBH,Reason from App_Cutting_Barcodes_Edit E  where Barcode = '{_barcode}' AND ZLBH='{_ry}'";
            string sql = $"SELECT * FROM App_Cutting_Barcodes_Edit where  Barcode = '{_barcode}' ";

            var list = new ObservableCollection<DB.Table_Barcode>();
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            if (ta.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in ta.Rows)
                {
                    DateTime date = Convert.ToDateTime(row["userdate"]);
                    list.Add(new DB.Table_Barcode
                    {
                        Barcode = _barcode,

                        BWBH = row["BWBH"].ToString(),

                        CLBH = row["ClBh"].ToString(),

                        Qty = Convert.ToInt32(row["actualQty"]),

                        Target = Convert.ToInt32(row["planqty"]),

                        UserDate = date.Ticks,

                        XXCC = row["XXCC"].ToString(),
                        
                        UserID = row["UserID"].ToString(),

                        YN = Convert.ToInt32(row["YN"].ToString()),

                        ZLBH = row["ZLBH"].ToString(),
                        
                        Prono= row["prono"].ToString(),                        
                    });



                }
            }
            Data_Confirm = list;
        }
    }
    public class AutoCuttingScan_GridTitle_Model
    {
      public string Size { get; set; }
        
    }
    public class ACS_DataGroup
    {
        public string Zlbh { get; set; }
        public string Clbh { get; set; }
        public string Bwbh { get; set; }
        public string CC { get; set; }
        public string Remain { get; set; }
        public string Qty { get; set; }
        public string Gxxcc { get; set; }
        public int  Gqty { get; set; }
        public int ActualQty { get; set; }
        public string Gqty_merge { get; set; }
        
    }
    public class AutoCuttingScan_Data
    {
        public string DDBH { set; get; }
        public string ZLBH { set; get; }
        public double  DMQty { set; get; }
        public string CLBH { set; get; }
        public string  Component{ set; get; }
        public double  Total { set; get; }
        public string BWBH { get; set; }
        public string Size_01k { set; get; }
        public string Size_015k { set; get; }
        public string Size_02k { set; get; }
        public string Size_025k { set; get; }
        public string Size_03k { set; get; }
        public string Size_035k { set; get; }
        public string Size_04k { set; get; }
        public string Size_045k { set; get; }
        public string Size_05k { set; get; }
        public string Size_055k { set; get; }
        public string Size_06k { set; get; }
        public string Size_065k { set; get; }
        public string Size_07k { set; get; }
        public string Size_075k { set; get; }
        public string Size_08k { set; get; }
        public string Size_085k { set; get; }
        public string Size_09k { set; get; }
        public string Size_095k { set; get; }
        public string Size_10k { set; get; }
        public string Size_105k { set; get; }
        public string Size_11k { set; get; }
        public string Size_115k { set; get; }
        public string Size_12k { set; get; }
        public string Size_125k { set; get; }
        public string Size_13k { set; get; }
        public string Size_135k { set; get; }
        public string Size_01 { set; get; }
        public string Size_015 { set; get; }
        public string Size_02 { set; get; }
        public string Size_025 { set; get; }
        public string Size_03 { set; get; }
        public string Size_035 { set; get; }
        public string Size_04 { set; get; }
        public string Size_045 { set; get; }
        public string Size_05 { set; get; }
        public string Size_055 { set; get; }
        public string Size_06 { set; get; }
        public string Size_065 { set; get; }
        public string Size_07 { set; get; }
        public string Size_075 { set; get; }
        public string Size_08 { set; get; }
        public string Size_085 { set; get; }
        public string Size_09 { set; get; } 
        public string Size_095 { set; get; }
        public string Size_10 { set; get; }
        public string Size_105 { set; get; }
        public string Size_11 { set; get; }
        public string Size_115 { set; get; }
        public string Size_12 { set; get; }
        public string Size_125 { set; get; }
        
        public string Size_13 { set; get; }
        public string Size_135 { set; get; }
        public string Size_14 { set; get; }
        public string Size_145 { set; get; }
        public string Size_15 { set; get; }
        public string Size_155 { set; get; }
        public string Size_16 { set; get; }
        public string Size_165 { set; get; }
        public string Size_17 { set; get; }
        public string Size_175 { set; get; }
        public string Size_18 { set; get; }
        public string Size_185 { set; get; }
        public string Size_19 { set; get; }
        public string Size_195 { set; get; }
        public string Size_20 { set; get; }
        public string Size_205 { set; get; }
    }
}
