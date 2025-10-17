 
/* Này bạn thân ơi số kiếp nhân sinh đã đưa bạn đến code này của tôi
 lúc tôi viết code này thì chỉ có chúa và tôi mới hiểu nó chạy như thế nào
 nhưng giờ thì chỉ có chúa, nếu bạn hack vào đây được thì bạn cũng là cao thủ,
 vậy xin bạn hay command lại nhưng gì bạn hiểu để tôi xem lại cho tôi hiểu theo*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using SQLite;
using MaterialTracking.Class;
using System.Linq;
using MaterialTracking.PageViews;
using System.Threading.Tasks;

namespace MaterialTracking.DB
{
    public class DataLocal
    {
        string DBPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mat.db3");
        private SQLiteConnection _connection;
        private static DataLocal _table;
        private DataLocal()
        {
            _connection = new SQLiteConnection(DBPath);
            _connection.CreateTable<Table_Barcode>();
            _connection.CreateTable<Barcode_Edit_LVY>();
            _connection.CreateTable<BarcodeHistory>();
        
            _connection.CreateTable<AccountUser>();

            _connection.CreateTable<Models.AutoCuttingModel>();
            _connection.CreateTable<Models.OrderModel>();

           // _connection.CreateTable<ComponentTranslate>();
        }
        #region Order
        public List<Models.OrderModel> Get_ItemOrder
        {
            get => _connection.Table<Models.OrderModel>().ToList();
        }

        public async void Insert_OrderStore(System.Data.DataTable table)
        {
            await System.Threading.Tasks.Task.Run(() => Inser_Order_Background(table));
        }
        public void Insert_OrderStore(Models.OrderModel data)
        {
            _connection.Insert(data);
        }
        void Inser_Order_Background(System.Data.DataTable table)
        {
            var datalocal = Get_ItemOrder;

            if (!datalocal.Any()) //the first time inport
            {
                if (table .Rows.Count>0)
                {
                    if (table.Rows.Count > 0)
                    {
                        for (int i = 0; i < table.Rows.Count; i++)
                        {
                            var ps = DateTime.Parse(table.Rows[i]["psdt"].ToString());

                            var temp = new Models.OrderModel
                            {
                                OrderID = table.Rows[i]["Orderid"].ToString(),

                                ZLBH = table.Rows[i]["zlbh"].ToString(),

                                Component = table.Rows[i]["Component"].ToString(),

                                Article = table.Rows[i]["article"].ToString(),

                                ModelName = table.Rows[i]["ModelName"].ToString(),

                                DepNo = table.Rows[i]["depno"].ToString(),

                                BWBH = table.Rows[i]["BWBH"].ToString(),

                                CLBH = table.Rows[i]["CLBH"].ToString(),

                                TarQty = table.Rows[i]["TarQty"].ToString(),

                                UserDate = DateTime.Today.Ticks,

                                PSDT = new DateTime(ps.Year, ps.Month, ps.Day, 0, 0, 0).Ticks

                            };

                            Insert_OrderStore(temp);
                        }

                    }
                }
            }
            else //datalocal is not null
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    var ps = DateTime.Parse(table.Rows[i]["psdt"].ToString());

                    var checkNotExist = datalocal.Any(r => r.OrderID == table.Rows[i]["OrderID"].ToString());

                    if (!checkNotExist)
                    {

                        var temp = new Models.OrderModel
                        {
                            OrderID = table.Rows[i]["Orderid"].ToString(),

                            ZLBH = table.Rows[i]["zlbh"].ToString(),

                            Component = table.Rows[i]["Component"].ToString(),

                            Article = table.Rows[i]["article"].ToString(),

                            ModelName = table.Rows[i]["ModelName"].ToString(),

                            DepNo = table.Rows[i]["depname"].ToString(),

                            BWBH = table.Rows[i]["BWBH"].ToString(),

                            CLBH = table.Rows[i]["CLBH"].ToString(),

                            TarQty = table.Rows[i]["tarqty"].ToString(),

                            UserDate = DateTime.Today.Ticks,

                            PSDT = new DateTime(ps.Year, ps.Month, ps.Day, 0, 0, 0).Ticks

                        };
                        Insert_OrderStore(temp);
                    }

                }
            }

        }
        #endregion
        #region Kho AC
        public List<Models.AutoCuttingModel> Get_Items_Store_AC
        {
            get => _connection.Table<Models.AutoCuttingModel>().ToList();

        }
        public async void Insert_Store_AC(System.Data.DataTable data)
        {

            await System.Threading.Tasks.Task.Run(() => Insert_StoreAC_Background(data));          

        }

        public async void Data_Store_AC_Changed()
        {
            await System.Threading.Tasks.Task.Run(() => Change_Background());          

        }
        void Change_Background()
        {
            string sql = @"SELECT 
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

            var datalocal = DB.DataLocal.Table.Get_Items_Store_AC;

            if (!Class.Network.Net.HasNet) return ;

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if (ta.Rows.Count > 0)
            {
                if (datalocal.Any())
                {
                    foreach (System.Data.DataRow row in ta.Rows)
                    {
                        if (!datalocal.Any(r=>r.RY == row["zlbh"].ToString() && r.Art == row["Article"].ToString() && r.ModelName == row["ModelName"].ToString()
                        && r.BWBH == row["BWBH"].ToString() && r.CLBH == row["CLBH"].ToString() && r.Component == row["Component"].ToString()))
                        {
                            var ps = DateTime.Parse(row["psdt"].ToString());

                            Insert_Store_AC(new Models.AutoCuttingModel
                            {
                                RY = row["zlbh"].ToString(),

                                Component = row["Component"].ToString(),

                                Art = row["article"].ToString(),

                                ModelName = row["ModelName"].ToString(),

                                Line = row["depname"].ToString(),

                                Qty = int.Parse(row["actual"].ToString()),

                                BWBH = row["BWBH"].ToString(),

                                CLBH = row["CLBH"].ToString(),

                                PlanQty = int.Parse(row["PlanQty"].ToString()),

                                UserDate = DateTime.Today.Ticks,

                                PSDT = new DateTime(ps.Year, ps.Month, ps.Day, 0, 0, 0).Ticks
                            });
                        }
                    }
                }
                else
                {
                    Insert_Store_AC(ta);
                }
            }
        }

        void Insert_StoreAC_Background(System.Data.DataTable data)
        {
            if (data != null)
            {
                if (data.Rows.Count > 0)
                {
                    for (int i = 0; i < data.Rows.Count; i++)
                    {
                        var ps = DateTime.Parse(data.Rows[i]["psdt"].ToString());

                        // long psdt = new DateTime(ps.Year, ps.Month, ps.Day, 0, 0, 0).Ticks;

                        var temp = new Models.AutoCuttingModel
                        {
                            RY = data.Rows[i]["zlbh"].ToString(),

                            Component = data.Rows[i]["Component"].ToString(),

                            Art = data.Rows[i]["article"].ToString(),

                            ModelName = data.Rows[i]["ModelName"].ToString(),

                            Line = data.Rows[i]["depname"].ToString(),

                            Qty = int.Parse(data.Rows[i]["actual"].ToString()),

                            BWBH = data.Rows[i]["BWBH"].ToString(),

                            CLBH = data.Rows[i]["CLBH"].ToString(),

                            PlanQty = int.Parse(data.Rows[i]["PlanQty"].ToString()),

                            UserDate = DateTime.Today.Ticks,

                            PSDT = new DateTime(ps.Year, ps.Month, ps.Day, 0, 0, 0).Ticks
                        };



                        _connection.Insert(temp);
                    }

                }
            }
        }
        public bool Insert_Store_AC(Models.AutoCuttingModel data)
        {
            int result = 0;

            if (data != null)
            {
                result += _connection.Insert(data);
                
            }
            return result > 0;

        }
        #endregion
        #region ComponentName
        public string Component_Data_ENGtoVN(string ENG)
        {
            var lst = _connection.Table<ComponentTranslate>().ToList();

            var result= lst.Where(r => r.Eng == ENG).ToList();

            if (result.Any()) return result[0].Vn;
            
            else return ENG;
        }

        public string Component_Data_VNtoENG(string VN)
        {
            var lst = _connection.Table<ComponentTranslate>().ToList();

            var result = lst.Where(r => r.Vn == VN).ToList();

            if (result.Any()) return result[0].Eng;

            else return VN;
        }

        public int InsertComponentName(ComponentTranslate comp)
        {
            return _connection.Insert(comp);
        }
        #endregion
        #region User

        public List<AccountUser> User
        {
            get => _connection.Table<AccountUser>().ToList();
        }
        public  int Insert_User(AccountUser acc)
        {
            var list = User;
            int i = 0;
            if (list.Count > 0)
            {
                foreach (AccountUser item in list)
                {
                    var ji =  _connection.Delete(item);

                }
            }
            i =  _connection.Insert(acc);
            return i;
        }

        #endregion
      
        public static DataLocal Table
        {
            get
            {
                //if (_table == null) return _table = new DataLocal(); else return _table;
                return _table == null ? _table = new DataLocal() : _table;
            }
            set
            {
                if (_table == null) { _table = new DataLocal(); _table = value; } else _table = value;

            }
        }
        #region Table barcode
        public int TableBarcode_Count
        {
            get
            {
                return _connection.Table<Table_Barcode>().Count();
            }
        }
        public int Delete(Table_Barcode table)
        {
            return _connection.Delete(table);
        }
      
        public int Insert_Barcode(Table_Barcode data)
        {
            return _connection.Insert(data);           
        }
        public int Update_Barcode(Table_Barcode data)
        {
            return _connection.Update(data);
        }
        //public int Delete_Barcode(Table_Barcode data)
        //{
        //    var i = _connection.Delete(data);

        //    Debug.WriteLine("Deleted : " + data.Barcode);

        //    return i;
        //}
      
        public int Delete_All
        {
            get
            {
                var list_Local = this.Data;
                int i = 0;
                foreach (DB.Table_Barcode item in list_Local)
                {
                    i += Delete(item);
                }
               
                Debug.WriteLine("Deleted ALl: " + i);
              
                return i;
            }

        }
      
        public List<Table_Barcode> Data
        {
          
            get => _connection.Table<Table_Barcode>().ToList();
        }
        public List<Table_Barcode> Data_Barcode(string barCode ,string ry="")
        {

            var lst =ry!=""? _connection.Table<Table_Barcode>().Where(x=> x.Barcode == barCode && x.ZLBH ==ry).ToList() : _connection.Table<Table_Barcode>().Where(x => x.Barcode == barCode ).ToList() ;
            return lst;

        }
        public List<Table_Barcode> Data_Barcode(string barCode, string ry ,string clbh,string bwbh)
        {

            var lst = _connection.Table<Table_Barcode>().Where(x => x.Barcode == barCode && x.ZLBH == ry && x.CLBH == clbh && x.BWBH == bwbh).ToList();



            return lst;

        }
        
        public List<Table_Barcode> All_Data_Barcode()
        {
            return _connection.Table<Table_Barcode>().ToList();
        }
        public int Barcode_RowCount
        {
            get => _connection.Table<Table_Barcode>().Count();            
        }
        public  List<Table_Barcode> ExistSize(string barcode, string zlbh, string xxcc, string bwbh)
        {
            
            var a = _connection.Table<Table_Barcode>().Where(x => (x.ZLBH == zlbh) && (x.XXCC == xxcc) && (x.BWBH == bwbh) && (x.Barcode == barcode)).ToList();                      
            
            return a;

        }
        public bool Exist_Prono_TableBarcode(string prono, out List<Table_Barcode> list)
        {
            list = _connection.Table<Table_Barcode>().Where(x => (x.Prono == prono)).ToList();

            if (list.Count > 0) return true; else return false;
        }

       



     
        public async Task<int > Save_CuttingEdit(List<DB.Table_Barcode> Ds)
        {
            /* Checking CuttingBarcode and CuttingHistory table
             * if table has any rows in this table (CuttingBarcode), we need updating that row exactly and then we insert into History table
             * if table has'n any rows yet in this table (CuttingBarcode), we need inserting new row and then insert into History table
             */
            int exe = 0;
            if (!Network.Net.HasNet) return exe;
            if (Ds.Count <= 0) return exe;

            //List<DB.Table_Barcode> localData= DB.DataLocal.Table.Data_Barcode(Ds[0].Barcode, Ds[0].ZLBH);

            var listEdited = Ds.Where(r => r.Isfull ==false && r.Target != 0 && r.Qty!=0).ToList();

            if(listEdited.Any())
            {                
                foreach (var item in listEdited)
                {



                    //Sau khi xac nhan se ko xoa dc phieu do nhung van in duoc
                    string updateCutting_bacode = "update Cutting_Barcode set yn = 9 where (barcode=@barc)";

                    string[] arr1 = { item.Barcode };

                    DB.SQL.ConnectDB.Connection.Update_Parameter(updateCutting_bacode, arr1);
                    
                    
                    
                    string existSQL = $"Select Convert(int,ActualQty) qty from App_Cutting_Barcodes_Edit Where prono ='{item.Prono}'";

                    var ExistProno = SQL.ConnectDB.Connection.FillDataTable(existSQL);
                   
                    if(ExistProno.Rows.Count > 0 )// It's exist at least a row
                    {
                        // Begin updating

                        int mainQty = item.Qty;
                     

                        string updateSQL = @"UPDATE [dbo].[App_Cutting_Barcodes_Edit]
                                            SET          
                                                [ActualQty] = @qty
                                                ,[USERID] = @userid
                                                ,[USERDATE] = getdate()
                                                ,[Action]=null
                                            WHERE (ProNo =@prono)";

                        string[] arr = { mainQty.ToString(), item.UserID, item.Prono };
                        try
                        {
                            if (int.Parse(ExistProno.Rows[0]["qty"].ToString()) < mainQty) //San luong tren the thong phai nho hon
                            {
                                DB.SQL.ConnectDB.Connection.Update_Parameter(updateSQL, arr);


                                exe = 1;

                                item.QtySql = item.Qty;

                                item.isEdited = false;

                                if (item.Qty == item.Target)
                                {
                                    item.Isfull = true;

                                }
                                DB.DataLocal.Table.Update_Barcode(item);
                            }
                        }
                        catch (Exception cc)
                        {

                            exe = 0;
                        }

                       
                        
                    }
                   
                    else //It's not exist any row
                    {
                        // Begin inserting
                        string insertSQL = @"INSERT INTO [dbo].[App_Cutting_Barcodes_Edit]
                                               ([Barcode]
                                               ,[ZLBH]
                                               ,[CLBH]
                                               ,[XXCC]
                                               ,[PlanQty]
                                               ,[ActualQty]
                                               ,[USERID]
                                               ,[USERDATE]
                                               ,[YN]
                                               ,[BWBH]
         
                                               ,[ProNo])
                                         VALUES
                                               (@barcode
                                               ,@zLBH
                                               ,@cLBH
                                               ,@xXCC
                                               ,@planQty
                                               ,@actualQty
                                               ,@USERID
                                               ,getdate()
                                               ,1
                                               ,@bWBH
           
                                               ,@proNo)";
                        try
                        {
                            //int mainQty = item.Qty ;
                            
                            //int mainQty = item.TempQty != item.Qty ? item.TempQty + item.Qty : item.Target;

                            DB.SQL.ConnectDB.Connection.Update_Parameter(insertSQL, new string[] 
                            { 
                                item.Barcode,
                                item.ZLBH,item.CLBH,
                                item.XXCC,
                                item.Target.ToString(),
                                item.Qty.ToString(),
                                item.UserID, 
                                item.BWBH, 
                                item.Prono 

                            });

                            exe += 1;


                            item.QtySql = item.Qty;

                            if (item.Qty == item.Target)
                            {
                                item.Isfull = true;



                            }
                            item.isEdited = false;
                            DB.DataLocal.Table.Update_Barcode(item);

                            string del = $@"DELETE App_Material_Request WHERE (BWBH=@bwbh and ZLBH=@zlbh)";

                            string[] arr = { item.BWBH, item.ZLBH };

                            DB.SQL.ConnectDB.Connection.Update_Parameter(del, arr);

                        }
                        catch (Exception xx)
                        {
                            Services.Alert.Msg("ERR_Delete", xx.Message);
                            exe = 0;
                        }
                       
                    }
                    // After updating or inserting has successed we need insert new row and increase times number column by 1 into History table
                    if (exe > 0) 
                    {
                        item.Qty = item.TempQty + item.Qty;
                        Insert_To_History(item); 
                    }
                }
            }
            else { Services.DisplayToast.Show.Toast("Đã nhập", Style.Organ); }
            
            return exe;
        }
        void Insert_To_History(Table_Barcode Row)
        {
            string sql = $"SELECT top(1)* FROM App_Cutting_History_Edit where ProNo='{Row.Prono}' order by times desc";

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            int times = 1;
            if(ta.Rows.Count >0)
            {
                times = int.TryParse(ta.Rows[0]["times"].ToString(), out times) ? times += 1 : 1;
            }

            string sqlInsert = @"INSERT INTO [dbo].[App_Cutting_History_Edit]
                                ([ProNo]
                                ,[XXCC]
                                ,[ActualQty]
                                ,[USERID]
                                ,[USERDATE]
                                ,[YN]
                                ,[Times]
                                )
                           VALUES
                               (@prono
                               , @xxcc
                               , @actualQty
                               , @uSERID
                               , getdate()
                               , 1
                               , @times)";
            DB.SQL.ConnectDB.Connection.Update_Parameter(sqlInsert, new string[] { Row.Prono, Row.XXCC, Row.Qty.ToString(), Row.UserID, times.ToString() });

        }

        #endregion
        //----------------------------------------------------//
        public int Delete_All_LYV
        {
            get
            {
                var list_Local = All_Local_LYV();
                int i = 0;
                foreach (DB.Barcode_Edit_LVY item in list_Local)
                {
                    i += Delete_Item_LYV(item);
                }

                Debug.WriteLine("Deleted ALl: " + i);

                return i;
            }

        }
        public int Delete_Item_LYV(Barcode_Edit_LVY table)
        {
            return _connection.Delete(table);
        }
        public List<Barcode_Edit_LVY> All_Local_LYV()
        {
            return _connection.Table<Barcode_Edit_LVY>().ToList();
        }
        public List<Barcode_Edit_LVY> ExistRow_Local_LYV(string barcode, string Tua,  string bwbh,string clbh, string Gxxcc="")
        {
            if (Gxxcc != "")
            
                return _connection.Table<Barcode_Edit_LVY>().Where(x => (x.Barcode == barcode) && (x.GXXCC == Gxxcc) && (x.CLBH == clbh) && (x.BWBH == bwbh) && (x.Tua == Tua)).ToList();
            
            else

                return _connection.Table<Barcode_Edit_LVY>().Where(x => (x.Barcode == barcode)  && (x.CLBH == clbh) && (x.BWBH == bwbh) && (x.Tua == Tua)).ToList();
        }
        public int Insert_Local_LYV(Barcode_Edit_LVY Data_Insert)
        {
            return _connection.Insert(Data_Insert);
            Debug.WriteLine("Insert : " + Data_Insert.Barcode);
        }
        public int Update_Local_LYV(Barcode_Edit_LVY Data_Update)
        {
            return _connection.Update(Data_Update);
            Debug.WriteLine("Update : " + Data_Update.Barcode);
        }
        public int Delete_Local_LYV(Barcode_Edit_LVY Data_Delete)
        {
            var i = _connection.Delete(Data_Delete);

            Debug.WriteLine("Deleted : " + Data_Delete.Barcode);

            return i;
        }
        //----------------
        public int Delete_History(BarcodeHistory table)
        {

           
            return _connection.Delete(table);
        }

        public int Insert_History(BarcodeHistory data)
        {
            if (Get_History.Count < 10)
            {
                return _connection.Insert(data);

            }
            else
            {
                var a = Get_History.OrderBy(h => h.UDate).FirstOrDefault();//lay cai code cũ nhat de xoa

                Delete_History(a);

                return _connection.Insert(data);

            }
        }
        public int Update_History(BarcodeHistory data)
        {
            var lst = Get_History.Where(x=>x.Barcode == data.Barcode).ToList();
            if (lst.Any())
            {
                lst[0].UDate = data.UDate;
                return _connection.Update(lst[0]);
            }
            else
            {
                return Insert_History(data);
            }

            
        }
        public List<BarcodeHistory> Get_History
        {
            get
            {
                return _connection.Table<BarcodeHistory>().OrderByDescending(h=>h.UDate).ToList();
            }
        }
    }
    #region List of Table
    public class Barcode_Edit_LVY
    {
        [PrimaryKey, AutoIncrement] public int id { get; set; }

        public string Barcode { get; set; }
        public string ZLBHs { get; set; }
        public string Tua{ get; set; }        
        public string CLBH { get; set; }     
        public string GXXCC { get; set; }
        public int Target { get; set; }
        public int LocalQty{ get; set; }    
        public string UserID { get; set; }
        public long UserDate { get; set; }
        public string BWBH { get; set; }                   
        public string Prono { set; get; }

        public bool Isfull { get; set; }
    }
    
    public class Table_Barcode
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Barcode { get; set; }
        public string ZLBH { get; set; }
        public string CLBH { get; set; }
        public string XXCC { get; set; }
        public int Target { get; set; }
        public int Qty { get; set; }
        public int TempQty { get; set; }
        public int QtySql { get; set; }
        
        public string UserID { get; set; }
        public long UserDate { get; set; }
        public int YN { get; set; }
        public string BWBH { get; set; }
        public bool isEdited { set; get; }//cho biet tinh trang co su edited
        public string Action { set; get; }//cho biet tinh trang giao hoac goi
        public bool Isfull{ get; set; }//cho biet tinh trang dang bi ket mang chua update len sql
        public string Prono { set; get; }
    }
   public class AccountUser
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public long uDate { get; set; }
        public string UID { get; set; }
        public string Pwd { get; set; }
        public string Company { get; set; }
      
    }
    #endregion
    public class BarcodeHistory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public long UDate { get; set; }
        public string Barcode { get; set; }
       
    }
    public class ComponentTranslate
    {
        [PrimaryKey, AutoIncrement]
        public string Eng { get; set; }
        public string Vn { get; set; }
        
    }
}

