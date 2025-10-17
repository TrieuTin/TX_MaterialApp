using System;
using System.Collections.Generic;
using System.Text;

namespace MaterialTracking.Class
{
    class ServerData
    {
        private static ServerData data;

        public static ServerData Data
        {
            get { if (data != null) return data; else return data = new ServerData(); }
            set { if (data != null) data = value; else { data = new ServerData(); data = value; } }
        }
        public System.Data.DataTable AutoCuttingView(string MachineNO)
        {
            string sql = $"select CB3.MachineNo Machine,CBs.Barcode ProNo,PDSCH.LEAN,cb3.Article,XieMing ModelName,CBs.CLBH,CBs.ZLBH RY ,bwzl.ywsm Component , dd.Pairs TargetQty, dd.Pairs TotalProcessQty, dd.Pairs Supermaket, CB3.WorkDate ProductionDate from Cutting_Barcodes CBs left join Cutting_Barcode CB3 on CBs.Barcode = CB3.Barcode Left join PDSCH on PDSCH.ZLBH = CBs.ZLBH left join DDZL dd on DD.DDBH = PDSCH.RY left join zlzls2 zl on zl.ZLBH = PDSCH.RY and zl.CLBH = CBs.CLBH and zl.BWBH = CBs.BWBH Left join DE_ORDERM DE on DE.ORDERNO = PDSCH.RY LEFT JOIN xxzl ON xxzl.ARTICLE = CB3.ARTICLE LEFT JOIN bwzl ON bwdh = CBs.BWBH  where 1 = 1 and CB3.MachineNo = '{MachineNO}' group by CB3.MachineNo,CBs.Barcode,CBs.ZLBH,cb3.Article,XieMing,CBs.CLBH,PDSCH.LEAN,bwzl.ywsm ,dd.Pairs,CB3.WorkDate";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            return ta;
        }
        public System.Data.DataTable MergeProNo(string MachineNO)
        {
            string sql = $"select CB3.MachineNo Machine,CBs.Barcode ProNo , COUNT(*) MergeRow from Cutting_Barcodes CBs left join Cutting_Barcode CB3 on CBs.Barcode = CB3.Barcode where 1 = 1 and CB3.MachineNo = '{MachineNO}' GROUP BY CB3.MachineNo,CBs.Barcode ";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            return ta;
        }
        public System.Data.DataTable MergeRy(string MachineNO)
        {
            string sql = $"select CB3.MachineNo Machine, CBs.Barcode ProNo, CBs.ZLBH , COUNT(*)MergeRow from Cutting_Barcodes CBs left join Cutting_Barcode CB3 on CBs.Barcode = CB3.Barcode where 1 = 1 and CB3.MachineNo = '{MachineNO}' GROUP BY CB3.MachineNo,CBs.Barcode ,CBs.ZLBH";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            return ta;
        }
    }
}
