using MaterialTracking.Controls;
using MaterialTracking.PageViews;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaterialTracking.DB
{

   public  enum Departments
    {
        NoSew,
        AutoCutting,
        HighFrequency,
        Printing,
        Embroidery,
        Stiching,
        Lazer,
        OutSourcing
    }
    public class StoreLocal
    {
        public StoreLocal() { }
        private static StoreLocal _instant;
        private string _userName="86029";
        private string _fac= "LVL";
        private string _pass;
       // private string _unit;
        private string _lang;
        private string _depid;
        private string _ismaterial;
        private string _barcode= "20240700138";//20240700179 20240700138 20240800164
        private string _ry;

        private List<Model_DataGrid> _data_warehouse_ac;
        private List<Model_DataGrid> _data_warehouse_lz;
        private List<Model_WH_NS> _data_WH_NS;


        private List<Model_DataGrid> _data_raw;
        
        private List<InventoryPageGeneral_Type> _data_warehouse_general;

        //Chi su dung cho nha may LYV
        private string _machine = "";

        private Departments _depname;
        
        public List<Model_DataGrid>Data_WareHouse_AC { get => _data_warehouse_ac; set => _data_warehouse_ac = value; }
        public List<Model_DataGrid>Data_WareHouse_Lz { get => _data_warehouse_lz; set => _data_warehouse_lz = value; }
        public List<Model_WH_NS> Data_WH_NS { get => _data_WH_NS; set => _data_WH_NS= value; }
        public List<InventoryPageGeneral_Type> Data_Warehouse_General { get => _data_warehouse_general; set => _data_warehouse_general= value; }

        public List<Model_DataGrid> Data_raw { get => _data_raw; set => _data_raw = value; }


        private List<Model_DataGrid> _nosew_inventory;
        private List<Model_DataGrid> _autocutting_inventory;
        public string Lang { get => _lang; set => _lang = value; }
        public string Factory { get => _fac; set => _fac = value; }       
        public string UserName { get => _userName; set => _userName = value; }
        public string Password { get => _pass; set => _pass = value; }
        public string Depid { get => _depid; set => _depid = value; }
        public string Ry { get => _ry; set => _ry = value; }
        
      //  public string Unit { get => _unit; set => _unit = value; }
        public string CurrentDep { get => _ismaterial; set => _ismaterial = value; }
        public string Barcode 
        { 
            get => _barcode; 
             
            set => _barcode = value; 
        }

        public List<Model_DataGrid> NoSew_Inventory { get => _nosew_inventory; set => _nosew_inventory = value; }
        public List<Model_DataGrid> AutoCutting_Inventory { get => _autocutting_inventory; set => _autocutting_inventory = value; }
        public Departments Depname 
        {
          
            get
            {
                switch (CurrentDep)
                {
                    case "NS":                        
                            return _depname = Departments.NoSew;
                        
                    case "AC":                        
                            return _depname = Departments.AutoCutting;
                        
                    case "HF":                        
                            return _depname = Departments.HighFrequency;
                        
                    case "PR":                        
                            return _depname = Departments.Printing;
                        
                    case "EM":
                        return _depname = Departments.Embroidery;

                    case "ST":
                        return _depname = Departments.Stiching; 

                    case "LS":
                        return _depname = Departments.Lazer;

                    default: //OS
                        return _depname = Departments.OutSourcing;
                }
            }
        }

        private MyFactory _myfac = MyFactory.LVL;

        public MyFactory Myfac { get => _myfac; set => _myfac = value; }

        public string Serial { get => Android.Provider.Settings.Secure.GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId); }
        public static StoreLocal Instant
        {
            get { if (_instant == null) return _instant = new StoreLocal(); else return _instant; }
            set
            {
                if (_instant == null)
                {
                    _instant = new StoreLocal();
                    _instant = value;
                }
                else _instant = value;
            }
        }
        public string Version { get => Xamarin.Essentials.AppInfo.VersionString; }
      
        
        
        public DateTime ServerDate
        {
            get
            {
                //var dt = DB.SQL.ConnectDB.Connection.FillDataTable("select getdate() today").Rows[0][0].ToString().Split(' ');
                var dt = DB.SQL.ConnectDB.Connection.FillDataTable("SELECT CONVERT(varchar(19), GETDATE(), 120)  AS FormattedDate").Rows[0][0].ToString().Split(' ');

                var d = dt[0].Split('-');
                var t = dt[1].Split(':');

                var year = int.Parse(d[0]);
                var month = int.Parse(d[1]);
                var day = int.Parse(d[2]);

                var hh = int.Parse(t[0]);
                var mm = int.Parse(t[1]);
                var ss = int.Parse(t[2]);
                var fff = DateTime.Now.Millisecond;
                var date = new DateTime(year, month, day, hh, mm, ss, fff);

                return date;
            }
        }

        public string Machine { get => _machine; set => _machine = value; } //Only use for LYV Fac
    }
    public enum MyFactory
    {
        LVL,
        LYV,
        LHG,
        LYM,
        LYF,
        Debug
    }
}
