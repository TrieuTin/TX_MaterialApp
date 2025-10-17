using MaterialTracking.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PopupRequest : Popup
    {
        ModelPopupRequest vm;
        public PopupRequest(ModelPopupRequest model)
        {
            InitializeComponent();
                       
            vm = model;

            this.IsLightDismissEnabled = vm.Dismiss;

            vm.CurrentPopup = this;

            this.BindingContext = vm;

            //this.Cv.ItemsSource = vm.Data;
        }
        List<PropertyPopupRequest> CollectionResource
        {
            get => (List<PropertyPopupRequest>)Cv.ItemsSource;
            set => Cv.ItemsSource = value;
        }
        ModelPopupRequest Context
        {
            get => this.BindingContext as ModelPopupRequest;
        }
        private void Checked_Change(object sender, CheckedChangedEventArgs e)
        {
            var chck = (CheckBox)sender;

            var temp = new List<PropertyPopupRequest>();

            if (chck.IsChecked)
                foreach (var item in Context.Data)
                {
                    item.Selected = chck.IsChecked;
                    temp.Add(item);
                }
            else 
                foreach (var item in Context.Data)
                {
                    item.Selected = chck.IsChecked;
                    temp.Add(item);
                }
            if (temp.Any())
            {
                Context.Data = CollectionResource = temp;
            }
        }

        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            var Source = Context.Data;

            if (Source != null)
            {
                Cv.BackgroundColor = Class.Style.Blue;
            }
            else
            {                
                Cv.BackgroundColor = Class.Style.Red;
            }
            CollectionResource = Context.Data;
            
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var newtext = e.NewTextValue;

            if(newtext != null)
            {
                Cv.BackgroundColor = Class.Style.Blue;
            }
        }
    }
    public class ModelPopupRequest : BaseViewModel
    {
        private string _ry;

        ICommand tapCommand;

        Popup<object> _currentContext;

        bool _dismiss;

        private List<PropertyPopupRequest> _data;

        public string RY { get => _ry; set => SetProperty(ref _ry, value); }
        private string _warehouse;
        public ICommand SearchCommand { get; }
        public List<PropertyPopupRequest> Data { get => _data; set => SetProperty(ref _data, value); }
        public Popup<object> CurrentPopup { get => _currentContext; set => _currentContext = value; }

        public bool Dismiss { get => _dismiss; set => _dismiss = value; }
        public ModelPopupRequest()
        {
            
            tapCommand = new Command(OnTapped);
                      
            SearchCommand = new Command<string>(OnSearch);
        }

        private void OnSearch(string Obj)
        {
            
            if (!string.IsNullOrEmpty(Obj))
            {
                RY = Obj.Trim();
                if(Data != null )
                {
                    Data.Clear();                   
                }
               
                Data = RequestData(Obj.Trim());
            }
        }

        List<PropertyPopupRequest> RequestData(string ry)
        {
            //string sql = $@"SELECT t3.ywsm, t1.BWBH ,convert(int,t1.Qty)qty
            //                    FROM Cutting_Barcodes t1
            //                    LEFT JOIN App_Cutting_Barcodes_Edit t2 ON t1.BWBH = t2.BWBH and t1.ZLBH=t2.zlbh 
            //                    inner join bwzl t3 on t1.BWBH = t3.bwdh left join App_material_Request t4 on t1.ZLBH = t4.ZLBH
            //                    WHERE t1.ZLBH = '{ry}' and t2.BWBH IS NULL and t4.ZLBH is null";

            var sql = $@"SELECT t3.ywsm, t1.BWBH ,convert(int,t1.Qty)qty
                            FROM (SELECT DISTINCT ZLZLS2.ZLBH,ZLZLS2.BWBH,BWZL.ywsm PARTNAME,CLBH,DFL,XFL,ARTICLE ,ddzl.Pairs Qty
                            FROM ZLZLS2 
                            LEFT JOIN DDZL ON DDZL.DDBH=ZLZLS2.ZLBH
                            LEFT JOIN XXBWFL on xxbwfl.XieXing=DDZL.XieXing and XXBWFL.SheHao=DDZL.SheHao and XXBWFL.BWBH=zlzls2.BWBH
                            LEFT JOIN XXBWFLS on XXBWFLS.FLBH=XXBWFL.FLBH
                            LEFT JOIN BWZL ON BWZL.bwdh=ZLZLS2.BWBH
                            WHERE ZLZLS2.ZLBH='{ry}'
                            and XXBWFLS.DFL='C') t1
                            LEFT JOIN App_Cutting_Barcodes_Edit t2 ON t1.BWBH = t2.BWBH and t1.ZLBH=t2.zlbh 
                            inner join bwzl t3 on t1.BWBH = t3.bwdh left join App_material_Request t4 on t1.ZLBH = t4.ZLBH
                            WHERE t1.ZLBH = '{ry}' and t2.BWBH IS NULL and t4.ZLBH is null";
            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if (ta.Rows.Count > 0)
            {
                _data = new List<PropertyPopupRequest>();
                foreach (System.Data.DataRow item in ta.Rows)
                {
                    _data.Add(new PropertyPopupRequest
                    {
                        Bwbh = item["BWBH"].ToString(),

                        Comp = item["ywsm"].ToString(),

                        Qty = item["Qty"].ToString(),
                        
                        Selected = false

                    });
                }
            }
            else _data = null;
            return _data;
        }
     
        public ICommand TapCommand
        {
            get { return tapCommand; }
            private set { tapCommand = value; OnPropertyChanged("TapCommand"); }
        }

        public string Warehouse { get => _warehouse; set => _warehouse = value; }

        void OnTapped(object s)
        {
            if (Data != null)
            {


                var list = Data.Where(r => r.Selected).ToList();
                int kq = 0;
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        try
                        {
                            string sql = @"INSERT INTO [dbo].[App_Material_Request]
                                           ([ZLBH]
                                           ,[BWBH]
                                           ,[ywsm]
                                           ,[Qty]
                                           ,[UserID]
                                           ,[WareHouse]
                                           ,[Request_DepNo]
                                           ,[Request_Date])
                                     VALUES
                                           (@zlbh
                                           ,@bwbh
                                           ,@comp
                                           ,@qty
                                           ,@User
                                           ,@wh
                                           ,@dep
                                           ,Getdate())";
                            string[] arr = { RY.ToUpper(), item.Bwbh.ToUpper(), item.Comp, item.Qty, DB.StoreLocal.Instant.UserName, Warehouse, DB.StoreLocal.Instant.CurrentDep };

                            if(Warehouse != DB.StoreLocal.Instant.CurrentDep)
                            {
                                kq = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, arr);
                            }
                            else
                            {
                                kq = 0;
                                Services.DisplayToast.Show.Toast("Không thể yêu cầu cho chính mình", Class.Style.Error);
                                CurrentPopup.Dismiss("Err");
                            }
                        }
                        catch (Exception xx)
                        {

                            Services.DisplayToast.Show.Toast(xx.Message, Class.Style.Error);

                            CurrentPopup.Dismiss("Err");
                        }


                    }
                    if (kq != 0)
                    {
                        Services.DisplayToast.Show.Toast("Yêu cầu thành công", Class.Style.ColdKidsSky.Green);
                        CurrentPopup.Dismiss("OK");
                    }
                  
                }
                else
                {
                    CurrentPopup.Dismiss("No");
                }


            }
            else 
                    CurrentPopup.Dismiss("Null");
        }

    }
    public class PropertyPopupRequest
    {
        public string RY { set; get; }
        public string Comp { get; set; }
        public string Bwbh { get; set; }
        public string Qty { get; set; }
        public string DepRQ { get; set; }
        public bool Selected { get; set; }
    }
}