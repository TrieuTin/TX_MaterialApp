using MaterialTracking.Models;
using MaterialTracking.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExpandCtrl : Grid
    {
        public ExpandCtrl(Model_ShowRequest ShowRQ)
        {
            InitializeComponent();

            if (ShowRQ.Data.Any())
            {


                var st = new StackLayout();

                this.lblDep.Text = ShowRQ.Data[0].DepRQ;
                this.lblry.Text = ShowRQ.Data[0].RY.ToUpper();

                foreach (var item in ShowRQ.Data)
                {
                    st.Children.Add(new Label { Text = string.Format("{0}({1})", item.Comp, item.Bwbh), FontSize = 18 });
                    st.Children.Add(new Label { Text = item.Qty, FontSize = 16, FontAttributes = FontAttributes.Italic });
                }

                this.subContent.Children.Add(st);
            }
        }
        
    }
    public class Model_ShowRequest : BaseViewModel
    {
        private List<PropertyPopupRequest> _data;
        private string _ry;
        private bool _showAgain;
        public Model_ShowRequest(string RY, bool showAgain=false)
        {
            _ry = RY;
            _showAgain = showAgain;
            GetRequest();
        }

        public List<PropertyPopupRequest> Data { get => _data; set { _data = value; OnPropertyChanged(nameof(Data)); } }

        List<PropertyPopupRequest> GetRequest()
        {
            string sql;
            if(!_showAgain)
              sql = $@"SELECT * FROM App_material_Request where WareHouse ='{DB.StoreLocal.Instant.CurrentDep}' And ZLBH ='{_ry}'";
            else
              sql = $@"SELECT * FROM App_material_Request where Request_depno='{DB.StoreLocal.Instant.CurrentDep}' And ZLBH ='{_ry}'";

            _data = new List<PropertyPopupRequest>();

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if (ta.Rows.Count > 0)
            {
                foreach (System.Data.DataRow r in ta.Rows)
                {

                    _data.Add(new PropertyPopupRequest
                    {
                        RY = r["ZLBH"].ToString(),
                        Bwbh = r["BWBH"].ToString(),
                        Comp = r["ywsm"].ToString(),
                        Qty = r["qty"].ToString(),
                        DepRQ= !_showAgain? r["Request_Depno"].ToString(): r["Warehouse"].ToString(),
                        Selected = false
                    });
                }
            }
            return _data;
        }

    }
}