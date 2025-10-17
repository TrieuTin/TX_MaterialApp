using MaterialTracking.Controls;
using MaterialTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupShowRequest : Popup
    {
        public PopupShowRequest( bool isRequested)
        {
            InitializeComponent();

            var model = new DataShowRQ(isRequested);

            var data = model.Items;

            if (data.Any())
            {
                string ry_Reapt="";
                foreach (var item in data)
                {
                    if (ry_Reapt != item.RY)
                    {
                        ry_Reapt = item.RY;
                        var model1 = new Model_ShowRequest(ry_Reapt,isRequested);
                        var expaner = new ExpandCtrl(model1);
                        this.MainGrid.Children.Add(expaner);
                    }
                }
            }
        }
        
    }
    public class DataShowRQ
    {
        private List<PropertyPopupRequest> _items;
        bool _showRequested;
        public DataShowRQ(bool Showrequested)
        {
            _showRequested = Showrequested;
            GetData();
        }

        public List<PropertyPopupRequest> Items { get => _items; set => _items = value; }

        void GetData()
        {
            string sql ="";
            if(!_showRequested)
                sql = $@"SELECT * FROM App_material_Request where WareHouse ='{DB.StoreLocal.Instant.CurrentDep}'";
            else
                sql = $@"SELECT * FROM App_material_Request where Request_depno ='{DB.StoreLocal.Instant.CurrentDep}'";
            _items = new List<PropertyPopupRequest>();

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if (ta.Rows.Count > 0)
            {
                foreach (System.Data.DataRow r in ta.Rows)
                {

                    _items.Add(new PropertyPopupRequest
                    {
                        RY = r["ZLBH"].ToString(),
                        Bwbh = r["BWBH"].ToString(),
                        Comp = r["ywsm"].ToString(),
                        Qty = r["qty"].ToString(),
                        Selected = false
                    });
                }
            }
        }

    }
   
}