using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaterialTracking.Models
{
    public class WareHouseModels
    {

    }
    public class OrderModel
    {
        [PrimaryKey]       
        public string OrderID{ get; set; }
        public string ZLBH{ get; set; }
        public string Component{ get; set; }
        public string CLBH{ get; set; }
        public string ModelName{ get; set; }
        public string Article{ get; set; }
        public string BWBH{ get; set; }
        public string DepNo{ get; set; }
        public string TarQty{ get; set; }
        public long PSDT{ get; set; }
        public long UserDate { get; set; }

    }
    public class AutoCuttingModel
    {
        [PrimaryKey, AutoIncrement] 
        public int id { get; set; }
        public string RY { get; set; }
        public string Art{ get; set; }
        public string Component{ get; set; }
        public string ModelName{ get; set; }
        public int Qty{ get; set; }
        public int PlanQty{ get; set; }
        public string Line{ get; set; }
        public string BWBH{ get; set; }
        public string CLBH{ get; set; }
        public long UserDate{ get; set; }
        public long PSDT{ get; set; }
    }

    public class OrderDetail
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }


        public string Orderid { get; set; }
        public string RY { get; set; }
        
        public string Component { get; set; }
        public string XXCC { get; set; }
        
        public int OrderQty { get; set; }
        
        
        
        
        
        
    }
}
