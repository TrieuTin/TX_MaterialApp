using MaterialTracking.Class;
using MaterialTracking.Controls;
using MaterialTracking.DB;
using MaterialTracking.Models;
using MaterialTracking.Popups;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace MaterialTracking.PageViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WareHousePage : ContentPage
    {
        //List<Model_DataGrid> Data_search;

       // Tools.Timer Timer_LoadDataBG;


        Controls.dg_WH_AC dgAC;
        Controls.dg_WH_NS dgNS;
        public WareHousePage()
        {
            InitializeComponent();
            ThisContext = new Model_Inventory();

            ThisContext.Title = ConvertLang.Convert.Translate_LYM("Kho", "ဂိုဒေါင်");

           // ThisContext.Countitems = string.Format("{0} Đơn", ItemSource.Count);

            ThisContext.ConfirmCompleted += (s, e) =>
            {
                Confirm_Clicked(s, e);
            };

            var longPressCommand = new Command( () =>
            {
                Services.Alert.PopupShowRequest(true);

            });
            TouchEffect.SetLongPressCommand(btn_Request, longPressCommand);
        }
        protected override  void OnAppearing()
        {
            base.OnAppearing();


            //if (ThisContext.Data != null)
            //{
            //    var sb = this.searchBar;

            //    if (!string.IsNullOrEmpty(sb.Text))
            //    {

            //        if (ThisContext.Data.Any())
            //        {
            //            if (ThisContext.SelectedDepNameIndex != -1)
            //            {

            //                var filter = ThisContext.Data.Where(dk => dk.RY.Contains(sb.Text.ToUpper()) || dk.Art.Contains(sb.Text.ToUpper()) || dk.Comp.Contains(sb.Text.ToUpper())).ToList();

            //                ItemSource = new List<Model_DataGrid>(filter);
            //            }
            //            else
            //            {
            //                var filter = ThisContext.Data.Where(dk => (dk.RY.Contains(sb.Text.ToUpper()) || dk.Art.Contains(sb.Text.ToUpper()) || dk.Comp.Contains(sb.Text.ToUpper()))).ToList();

            //                ItemSource = new List<Model_DataGrid>(filter);
            //            }

            //        }

            //    }
            //    else
            //    {
            //        if (ItemSource.Count != ThisContext.Data.Count)
            //        {

            //            ItemSource = ThisContext.Data;
            //            ThisContext.SelectedDepNameIndex = -1;
            //        }

            //    }
            //    //ThisContext.Countitems = string.Format("{0} Đơn", ItemSource.Count);

            //    btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });
            //}
        }

      

      

        //public async Task AutoBindingAgain()
        //{
        //    while (1+1==2)
        //    {
        //        Data_search = ItemSource = ThisContext.Data;

        //        btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });
                
        //        await Task.Delay(TimeSpan.FromSeconds(5)); // Cập nhật mỗi 5s
        //    }
        //}
        Model_Inventory ThisContext
        {
            get => this.BindingContext as Model_Inventory;
            set
            {
                this.BindingContext = value;

                //if (ItemSource != null)
                //{

                //    //Data_search = ItemSource;

             //   btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = ItemSource });

                   
                //}
            }
        }
     
        private void Confirm_Clicked(object sender, EventArgs e)
        {
            if (ThisContext.Ac_checked)
            {

                if (!ThisContext.Is_confirm_completed) { return; }

                var temp2 = ThisContext.Data.Except(dgAC.ItemSource.Where(r => r.Sel).ToList()).ToList();

                ThisContext.Data = dgAC.ItemSource = temp2;

                ThisContext.Is_confirm_completed = !true;
            }
            else
            {

            }

        }

        private async void Refresh_Clicked(object sender, EventArgs e)
        {
            await Task.Delay(10);
            ThisContext.IsBusy = true;

            //ThisContext.GetData_Inventory();
            if (ThisContext.Ac_checked)
            {

                //remote control before adding

                Remote_Component_in_Grid();

                dgAC = new Controls.dg_WH_AC();

                dgAC.ThisContext = ThisContext;

                dgAC.TranslationX = this.Width;

                // Add to grid
                mainGrid.Children.Add(dgAC);

                Grid.SetRow(dgAC, 1);

                Grid.SetColumn(dgAC, 0);

                btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = dgAC.ItemSource });
            }
            else if (ThisContext.Ns_checked)
            {

                //remote control before adding

               // Remote_Component_in_Grid();

                //dgNS = new Controls.dg_WH_NS();

                //dgNS.ThisContext = ThisContext;

                //dgNS.TranslationX = this.Width;

                //// Add to grid
                //mainGrid.Children.Add(dgNS);

                //Grid.SetRow(dgNS, 1);

                //Grid.SetColumn(dgNS, 0);

                dgNS.ThisContext = ThisContext;

                btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = dgNS.ItemSource });

            }

            // if (CheckedAll.IsChecked) CheckedAll.IsChecked = false;

            ThisContext.IsBusy = false;
        }
        private void Search_Presss(object sender, EventArgs e)
        {
            var sb = (SearchBar)sender;
            if (ThisContext.Ac_checked)
            {
                if (!string.IsNullOrEmpty(sb.Text))
                {
                    if (dgAC.ThisContext.Data != null && dgAC.ThisContext.Data.Any())
                    {
                        if (ThisContext.SelectedDepNameIndex != -1)
                        {

                            var filter = dgAC.ItemSource.Where(dk => dk.RY.Contains(sb.Text.ToUpper()) || dk.Art.Contains(sb.Text.ToUpper()) || dk.Comp.Contains(sb.Text.ToUpper())).ToList();

                            dgAC.ItemSource = filter;
                        }
                        else
                        {
                            var filter = ThisContext.Data.Where(dk => (dk.RY.Contains(sb.Text.ToUpper()) || dk.Art.Contains(sb.Text.ToUpper()) || dk.Comp.Contains(sb.Text.ToUpper()))).ToList();

                            dgAC.ItemSource = filter;
                        }

                    }
                }

                else
                {
                   dgAC.ItemSource = ThisContext.Data;
                    //ItemSource = Data_search;
                    ThisContext.SelectedDepNameIndex = -1;

                }
                btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = dgAC.ItemSource });
            
            }
            else if (ThisContext.Ns_checked)
            {
                if (!string.IsNullOrEmpty(sb.Text))
                {
                    if (dgNS.ThisContext.Data_WH_NS != null && dgNS.ThisContext.Data_WH_NS.Any())
                    {
                        if (ThisContext.SelectedDepNameIndex != -1)
                        {

                            var filter = dgNS.ItemSource.Where(dk => dk.Ry.Contains(sb.Text.ToUpper()) || dk.Article.Contains(sb.Text.ToUpper()) || dk.PO.Contains(sb.Text.ToUpper())).ToList();

                            dgNS.ItemSource = filter;
                        }
                        else
                        {
                            var filter = ThisContext.Data_WH_NS.Where(dk => (dk.Ry.Contains(sb.Text.ToUpper()) || dk.Article.Contains(sb.Text.ToUpper()) || dk.PO.Contains(sb.Text.ToUpper()))).ToList();

                            dgNS.ItemSource = filter;
                        }

                    }
                }

                else
                {
                    dgAC.ItemSource = ThisContext.Data;
                    //ItemSource = Data_search;
                    ThisContext.SelectedDepNameIndex = -1;

                }
                btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = dgNS.ItemSource });
            }
            

        }
        
        private  void Search_TextChange(object sender, TextChangedEventArgs e)
        {
            var sb = (SearchBar)sender;
            if (string.IsNullOrEmpty(sb.Text))
            {

                if (ThisContext.Ac_checked)
                {
                    dgAC.ItemSource = dgAC.ThisContext.Data;

                    ThisContext.SelectedDepNameIndex = -1;

                    btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = dgAC.ItemSource });

                }
                else if (ThisContext.Ns_checked)
                {
                    dgNS.ItemSource = dgNS.ThisContext.Data_WH_NS;

                    ThisContext.SelectedDepNameIndex = -1;

                    btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = dgNS.ItemSource });

                }
                //Kho Khach
            }
        }     

        private void On_ChangeDate_Event(object sender, DateChangedEventArgs e)
        {
            var date = e.NewDate;
            if (ThisContext.Ac_checked)
            {

                if (ThisContext.Data.Any())
                {
                    var filter = dgAC.ItemSource.Where(dk => dk.ProdDate.Contains(date.ToString("yyyy-MM-dd"))).ToList();

                    dgAC.ItemSource = new List<Model_DataGrid>(filter);


                    var _data = dgAC.ItemSource;
                    _data.Add(new Model_DataGrid
                    {
                        DepName = "All"
                    });

                   dgAC.ThisContext.DepName = _data.Select(x => x.DepName)
                                         .Distinct().OrderBy(x => x)
                                         .ToList();

                    ThisContext.SelectedDepNameIndex = -1;
                }
            }               
        }

        private void LabelIconTapped(object sender, EventArgs e)
        {
            ThisContext.isShowDatepicker = true;
        }
        
        private int _onlyRun4times = 0;
        private async void On_FilterChange_Event(object sender, CheckedChangedEventArgs e)
        {
            var rd = (RadioButton)sender;
            if(_onlyRun4times< 4)
            {
                if (rd.IsChecked) goto continues;
                
                _onlyRun4times++;

                
                return;
            }
           
          continues:  await Task.Delay(10);
            ThisContext.IsBusy = true;

            if (ThisContext.Ac_checked || ThisContext.Lz_checked)
            {
                dgAC = new MaterialTracking.Controls.dg_WH_AC();

                dgAC.ThisContext = ThisContext;

                dgAC.TranslationX = this.Width;

                //remote control before adding
                Remote_Component_in_Grid();

                // Add to grid
                mainGrid.Children.Add(dgAC);

                Grid.SetRow(dgAC, 1);

                Grid.SetColumn(dgAC, 0);


                btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = dgAC.ItemSource });
            }
            else if(ThisContext.Ns_checked)
            {
                //remote control before adding

                Remote_Component_in_Grid();

                dgNS = new Controls.dg_WH_NS();
                dgNS.ThisContext = ThisContext;

                dgNS.TranslationX = this.Width;

                // Add to grid
                mainGrid.Children.Add(dgNS);

                Grid.SetRow(dgNS, 1);

                Grid.SetColumn(dgNS, 0);


                btn_Confirm.SetBinding(Button.CommandParameterProperty, new Binding { Source = dgNS.ItemSource });
            }
            else
            {
                Remote_Component_in_Grid();
            }
            ThisContext.IsBusy = false;
        }
        
        void Remote_Component_in_Grid()
        {
            var controlToRemove = mainGrid.Children.FirstOrDefault(view => Grid.GetRow(view) == 1 && Grid.GetColumn(view) == 0);

            if (controlToRemove != null)
            {
                mainGrid.Children.Remove(controlToRemove);
            }
        }
    }

    public class Model_Inventory: BaseViewModel
    {
        private List<Model_DataGrid> _data;
        private List<Model_WH_NS> _data_wh_ns;

        public event EventHandler ConfirmCompleted;

        bool isNoSew = DB.StoreLocal.Instant.Depname == DB.Departments.NoSew;
        System.Windows.Input.ICommand ClickCommand { get; set; }
        System.Windows.Input.ICommand Command_Click { get; set; }
        
        bool _isshowicon = true;

        bool _isshowdate= false;

        List<string> _depname;
        public string Rb_Label_AC { get => ConvertLang.Convert.Translate_LYM("CTĐ", "အော်တို ဖြတ်စက်"); }
        public string Rb_Label_NS { get => ConvertLang.Convert.Translate_LYM("NS", "ချုပ်ကြောင်းမပါသောဖြစ်စဥ်"); }
        public string Rb_Label_ST { get => ConvertLang.Convert.Translate_LYM("ST", "စက်ချုပ်လိုင်း"); }


        public string Search_Holder { get => ConvertLang.Convert.Translate_LYM("Tìm", "ရှာဖွေ"); }
        
        //this is Binding to WH_AC - WH_LZ

        public string Column_RY { get => ConvertLang.Convert.Translate_LYM("RY", "အာဝိုင်"); }
        public string Column_Art { get => ConvertLang.Convert.Translate_LYM("Art", "အာအာတီကယ်"); }
        public string Column_Comp { get => ConvertLang.Convert.Translate_LYM("Comp", "အစိတ်အပိုင်းများ"); }
        public string Column_Model { get => ConvertLang.Convert.Translate_LYM("ModelName", "မော်ဒယ်"); }
        public string Column_Line { get => ConvertLang.Convert.Translate_LYM("Line", "လိုင်း"); }
        public string Column_Qty { get => ConvertLang.Convert.Translate_LYM("Đặt hàng", "အရေအတွက်"); }

        //This is Binding to WH_NS
        public string Column_PO { get => ConvertLang.Convert.Translate_LYM("PO", "PO"); }
        public string Column_VN { get => ConvertLang.Convert.Translate_LYM("VN", "ဗီယက်နမ်"); }
        public string Column_EN { get => ConvertLang.Convert.Translate_LYM("ENG", "အင်္ဂလိပ်"); }
        public string Column_ORQty { get => ConvertLang.Convert.Translate_LYM("Tổng", "စုစုပေါင်း"); }
        public string Column_PSDT { get => ConvertLang.Convert.Translate_LYM("Ngày SX", "ရက်စွဲ"); }





        private bool _ac_checked= DB.StoreLocal.Instant.Depname == DB.Departments.AutoCutting;
        private bool _ns_checked= DB.StoreLocal.Instant.Depname == DB.Departments.NoSew;
        private bool _print_checked= DB.StoreLocal.Instant.Depname == DB.Departments.Printing;
        private bool _hf_checked= DB.StoreLocal.Instant.Depname == DB.Departments.HighFrequency;
        private bool _em_checked= DB.StoreLocal.Instant.Depname == DB.Departments.Embroidery;
        private bool _st_checked= DB.StoreLocal.Instant.Depname == DB.Departments.Stiching;
        private bool _lz_checked= DB.StoreLocal.Instant.Depname == DB.Departments.Lazer;

        private bool _ac_enb = DB.StoreLocal.Instant.Depname == DB.Departments.AutoCutting;
        private bool _ns_enb = DB.StoreLocal.Instant.Depname == DB.Departments.NoSew;
        private bool _print_enb = DB.StoreLocal.Instant.Depname == DB.Departments.Printing;
        private bool _hf_enb = DB.StoreLocal.Instant.Depname == DB.Departments.HighFrequency;
        private bool _em_enb = DB.StoreLocal.Instant.Depname == DB.Departments.Embroidery;
        private bool _st_enb = DB.StoreLocal.Instant.Depname == DB.Departments.Stiching;
        private bool _lz_enb = DB.StoreLocal.Instant.Depname == DB.Departments.Lazer;
        public ICommand Cmd_Confirm { get; }        
        public ICommand Cmd_Refresh { get; }
        public ICommand RequestCommand { get; }
        public List<Model_DataGrid> Data 
        {
            get => _data = Ac_checked ? DB.StoreLocal.Instant.Data_WareHouse_AC : Lz_checked ? DB.StoreLocal.Instant.Data_WareHouse_Lz : null;
            
                           
            set => SetProperty(ref _data, value); 
        }
         public List<Model_WH_NS> Data_WH_NS 
        {
            get => _data_wh_ns;
            
                            
            set => SetProperty(ref _data_wh_ns, value); 
        }
       
        
        private string _countitems;
        
        public string CountItems { get => _countitems; set => SetProperty(ref _countitems, value); }


        public bool Ac_checked { get => _ac_checked; set => SetProperty(ref _ac_checked, value); }
        public bool Ns_checked { get => _ns_checked; set => SetProperty(ref _ns_checked, value); }
        public bool Print_checked { get => _print_checked; set => SetProperty(ref _print_checked, value); }
        public bool Hf_checked { get => _hf_checked; set => SetProperty(ref _hf_checked, value); }
        public bool Em_checked { get => _em_checked; set => SetProperty(ref _em_checked, value); }
        public bool St_checked { get => _st_checked; set => SetProperty(ref _st_checked, value); }
        public bool Lz_checked { get => _lz_checked; set => SetProperty(ref _lz_checked, value); }

        public bool Ac_enb { get => _ac_enb; set => SetProperty(ref _ac_enb, value); }
        public bool Ns_enb { get => _ns_enb; set => SetProperty(ref _ns_enb, value); }
        public bool Print_enb { get => _print_enb; set => SetProperty(ref _print_enb, value); }
        public bool Hf_enb { get => _hf_enb; set => SetProperty(ref _hf_enb, value); }
        public bool Em_enb { get => _em_enb; set => SetProperty(ref _em_enb, value); }
        public bool St_enb { get => _st_enb; set => SetProperty(ref _st_enb, value); }
        public bool Lz_enb { get => _lz_enb; set => SetProperty(ref _lz_enb, value); }

        private const string server = @"http://192.168.61.92:8000/";
        public Model_Inventory()
        {

           Enable_Invetory();
            
            Cmd_Refresh = new Command(Refresh);

            Cmd_Confirm = new Command(Confirm);

            RequestCommand = new Command(Request);

            GetData_Inventory();
            
        }

        private async void Request(object obj)
        {
            string wh = "";
            if (Ac_checked) wh = "AC";
            if (Lz_checked) wh = "LS";
            if (Ns_checked) wh = "NS";
            if (St_checked) wh = "ST";

            var a = await Services.Alert.PopupRequest(wh);
        }

        void Enable_Invetory()
        {

            //switch (DB.StoreLocal.Instant.Depname)
            //{
            //    case Departments.NoSew:
            //        _ns_checked = true;
            //        break;
            //    case Departments.AutoCutting:
            //        _ac_checked = true;
            //        break;
            //    case Departments.HighFrequency:
            //        break;
            //    case Departments.Printing:
            //        break;
            //    case Departments.Embroidery:
            //        break;
            //    case Departments.Stiching:
            //        _st_checked = true;
            //        break;
            //    case Departments.Lazer:
            //        _lz_checked = true;
            //        break;
            //    default:
            //        break;
            //}
            

            if (DB.StoreLocal.Instant.Depname == DB.Departments.AutoCutting)
            {
                _st_enb = _hf_enb = _em_enb = _ns_enb = _print_enb = false;
                _lz_enb =  _ac_checked = true;

            }

            if (DB.StoreLocal.Instant.Depname == DB.Departments.Lazer)
            {

                _st_enb = _hf_enb = _em_enb = _ns_enb = _print_enb = false;

                _ac_enb = _lz_enb = _lz_checked = true;
            }         

            if(DB.StoreLocal.Instant.Depname != DB.Departments.Lazer && DB.StoreLocal.Instant.Depname != DB.Departments.AutoCutting)
            {
                _lz_enb = _ac_enb = _st_enb = _hf_enb = _em_enb = _ns_enb = _print_enb = true;

               

            }
        }

        bool is_confirm_completed = false;
        public bool Is_confirm_completed { get => is_confirm_completed; set => is_confirm_completed = value; }
        public List<string> DepName { get => _depname; set => SetProperty(ref _depname, value); }
        private string _selected_depname;

        private int _selected_depName_index=-1;
        public string SelectedDepName
        {
            get => _selected_depname; set => SetProperty(ref _selected_depname, value); 
        }
        public int SelectedDepNameIndex
        {
            get => _selected_depName_index;set => SetProperty(ref _selected_depName_index, value);
        }
        public bool isShowIcon { get => _isshowicon; set => SetProperty(ref _isshowicon, value); } 
        public bool isShowDatepicker { get => _isshowdate; set { isShowIcon = !value; SetProperty(ref _isshowdate, value); } }
        #region Funcs
        private async void Refresh(object obj)
        {
            IsBusy = true;

            await Task.Delay(10);

            //SetAwait(10);

            if (Ac_checked)
            {
                var dt = Get_Data_AC();
                if (dt.Count > 0)
                {

                    DB.StoreLocal.Instant.Data_WareHouse_Lz = dt.Where(r => !string.IsNullOrEmpty(r.MachineNO) && r.MachineNO.ToUpper() == "LASER").OrderByDescending(r => r.UserDate).ToList();

                    DB.StoreLocal.Instant.Data_WareHouse_AC = dt.Where(r => !string.IsNullOrEmpty(r.MachineNO) && r.MachineNO.ToUpper() != "LASER").OrderByDescending(r => r.UserDate).ToList();
                }

            }
            else if (Ns_checked)
            {
                _data_wh_ns = Load_Data_NS();
            }
            IsBusy = false;
            

        }
        async void SetAwait(int millisecond)
        {
            IsBusy = true;
            await Task.Delay(millisecond);
        }

        async void OrderItems_in_NS(List<Model_WH_NS> data)
        {
            if (data.Any())
            {
                var result = await Services.Alert.PopupYesNo("Gọi Hàng", "Xác Nhận?", "Đúng", "Không");

                if ((result as string) == "Đúng")
                {
                    //var m = new Model_Mater();

                    var kq = 0;

                    IsBusy = true;

                    kq = NS_Insert_Material_Orders(data, DB.StoreLocal.Instant.CurrentDep.ToUpper());

                    IsBusy = false;

                    if (kq > 0)
                    {
                        is_confirm_completed = true;
                        ConfirmCompleted?.Invoke(this, EventArgs.Empty);
                        Services.DisplayToast.Show.Toast(string.Format("Đơn xác nhận {0} chi tiết", kq), Color.DarkGreen);
                    }
                    else if (kq == 0)
                    {
                        Services.DisplayToast.Show.Toast("Không thành công", Class.Style.Error);
                    }
                    else
                    {
                        Services.DisplayToast.Show.Toast("Đã đăt hàng rồi", Class.Style.Error);
                    }

                }
                else { IsBusy = false; return; }

            }
            else Services.Alert.Msg("Thông báo", "Chưa chọn đơn hàng nào");

        }
        async  void OrderItems_in_AC(List<Model_DataGrid> data)
        {
            if (data.Any())
            {
                var result = await Services.Alert.PopupYesNo("Gọi Hàng", "Xác Nhận?", "Đúng", "Không");

                if ((result as string) == "Đúng")
                {
                    //var m = new Model_Mater();

                    var kq = 0;

                    IsBusy = true;

                    kq = AC_Insert_Material_Orders(data, DB.StoreLocal.Instant.CurrentDep.ToUpper());

                    IsBusy = false;

                    if (kq > 0)
                    {
                        is_confirm_completed = true;
                        ConfirmCompleted?.Invoke(this, EventArgs.Empty);
                        Services.DisplayToast.Show.Toast(string.Format("Đơn xác nhận {0} chi tiết", kq), Color.DarkGreen);
                    }
                    else if (kq == 0)
                    {
                        Services.DisplayToast.Show.Toast("Không thành công", Class.Style.Error);
                    }
                    else
                    {
                        Services.DisplayToast.Show.Toast("Đã đăt hàng rồi", Class.Style.Error);
                    }

                }
                else { IsBusy = false; return; }

            }
            else Services.Alert.Msg("Thông báo", "Chưa chọn đơn hàng nào");

        }
        async void NS_DeliveryItems(List<Model_WH_NS> data)
        {
            if ( _ns_checked && DB.StoreLocal.Instant.Depname == Departments.NoSew)
            {
                if (data.Any())
                {
                    var dataConfirm = data.Where(r => r.Sel).ToList();

                    var lstBtn = new List<Button>();

                    if (DB.StoreLocal.Instant.Depname != Departments.NoSew)

                        lstBtn.Add(new Button
                        {
                            Text = "Nosew (Ép Nóng)"
                        });

                    lstBtn.Add(new Button
                    {
                        Text = "High frequency (Ép Cao tần)"
                    });
                    lstBtn.Add(new Button
                    {
                        Text = "Printing (In)"
                    });
                    lstBtn.Add(new Button
                    {
                        Text = "Embroidery (Thêu)"
                    });
                    lstBtn.Add(new Button
                    {
                        Text = "Stiching (May)"
                    });
                    lstBtn.Add(new Button
                    {
                        Text = "Out sourcing (Gia Công)"
                    });

                    var a = await Services.Alert.PopupMenu("Department", lstBtn, Class.Style.Notification);

                    if (string.IsNullOrEmpty(a))
                    {

                        Services.Alert.Msg("Alert", "Plaease select a deparment");
                        IsBusy = false;
                        return;
                    }
                    else
                    {
                        var _depSelect = "";
                        switch (a)
                        {
                            case "Nosew (Ép Nóng)":
                                _depSelect = "NS"; break;

                            //case "High frequency (Ép Cao tần)":
                            //    _depSelect = "HF"; break;

                            //case "Printing (In)":
                            //    _depSelect = "PR"; break;

                            //case "Embroidery (Thêu)":
                            //    _depSelect = "EM"; break;

                            case "Stiching (May)":
                                _depSelect = "ST"; break;

                            default: //HF
                                _depSelect = "HF"; break;
                        }
                      
                    
                        if ( _depSelect == "ST")
                        {

                            var result = await Services.Alert.PopupYesNo("Hỏi", string.Format("Bạn muốn gửi đến đơn vị {0}?", a.ToString()), "Đặt Hàng", "Không");

                            if ((result as string) == "Đặt Hàng")
                            {
                                var kq = NS_Insert_Material_Orders(dataConfirm, _depSelect);

                                if (kq > 0) { is_confirm_completed = true; ConfirmCompleted?.Invoke(this, EventArgs.Empty); Services.DisplayToast.Show.Toast(string.Format("Đơn xác nhận {0} chi tiết", kq), Color.DarkGreen); }
                                else if (kq == 0)
                                { Services.DisplayToast.Show.Toast("Không thành công", Class.Style.Error); }
                                else Services.DisplayToast.Show.Toast("Đã đặt hàng rồi", Class.Style.Error);
                            }
                            else
                            {
                                IsBusy = false; return;
                            }
                        }
                        else
                        {
                            Services.DisplayToast.Show.Toast(string.Format(" Đơn vị {0} chưa hỗ trợ ", _depSelect), Class.Style.Error);

                        }
                    }
                }
            }
        }
        async void DeliveryItems(List<Model_DataGrid> data)
        {
            if ((_ac_checked && DB.StoreLocal.Instant.Depname == Departments.AutoCutting) || (_lz_checked && DB.StoreLocal.Instant.Depname == Departments.Lazer) || (_ns_checked && DB.StoreLocal.Instant.Depname == Departments.NoSew))
            {
                if (data.Any())
                {
                    var dataConfirm = data.Where(r => r.Sel).ToList();

                    var lstBtn = new List<Button>();

                    if(DB.StoreLocal.Instant.Depname != Departments.NoSew)

                        lstBtn.Add(new Button
                        {
                            Text = "Nosew (Ép Nóng)"
                        });

                    lstBtn.Add(new Button
                    {
                        Text = "High frequency (Ép Cao tần)"
                    });
                    lstBtn.Add(new Button
                    {
                        Text = "Printing (In)"
                    });
                    lstBtn.Add(new Button
                    {
                        Text = "Embroidery (Thêu)"
                    });
                    lstBtn.Add(new Button
                    {
                        Text = "Stiching (May)"
                    });
                    lstBtn.Add(new Button
                    {
                        Text = "Out sourcing (Gia Công)"
                    });

                    var a = await Services.Alert.PopupMenu("Department", lstBtn, Class.Style.Notification);

                    if (string.IsNullOrEmpty(a))
                    {

                        Services.Alert.Msg("Alert", "Plaease select a deparment");
                        IsBusy = false;
                        return;
                    }
                    else
                    {
                        var _depSelect = "";
                        switch (a)
                        {
                            case "Nosew (Ép Nóng)":
                                _depSelect = "NS"; break;

                            //case "High frequency (Ép Cao tần)":
                            //    _depSelect = "HF"; break;

                            //case "Printing (In)":
                            //    _depSelect = "PR"; break;

                            //case "Embroidery (Thêu)":
                            //    _depSelect = "EM"; break;

                            case "Stiching (May)":
                                _depSelect = "ST"; break;

                            default: //HF
                                _depSelect = "HF"; break;
                        }
                        #region  Old Code
                        //------------------------Set Volumn------------------------------------
                        //var list = new List<UsersControl.UC_SetVolum>();

                        //foreach (var item in dataConfirm)
                        //{
                        //    list.Add(new UsersControl.UC_SetVolum
                        //    {
                        //        Ry = item.RY,
                        //        Article = item.Art,
                        //        ModelName = item.ModelName,
                        //        Component = item.Comp,
                        //        Pairs = item.TarQty.Split('/')[0].ToString(),
                        //        Old_pairs = item.TarQty.Split('/')[0].ToString(),


                        //    });

                        //}

                        //var setvolum = await Services.Alert.PopupMenu_SetVolum(list);

                        //var selectitem = setvolum.Where(r => r.CheckedSelect);

                        //List<Model_DataGrid> listConfirm = new List<Model_DataGrid>();

                        //if (setvolum != null)
                        //{
                        //    foreach (var item in setvolum)
                        //    {
                        //        if (item.CheckedSelect)
                        //        {

                        //            foreach (var item2 in dataConfirm)
                        //            {
                        //                if (item2.RY == item.Ry && item2.Comp == item.Component && item2.ModelName == item.ModelName)
                        //                {
                        //                    string pairChange = string.Format("{0}/{1}", item.Pairs, item2.TarQty.Split('/')[1]);

                        //                    item2.TarQty = pairChange;

                        //                    listConfirm.Add(item2);
                        //                    break;
                        //                }
                        //            }
                        //        }
                        //    }
                        //}
                        //----------------------------------------
                        #endregion
                        if (_depSelect == "NS" || _depSelect == "ST")
                        {

                            var result = await Services.Alert.PopupYesNo("Hỏi", string.Format("Bạn muốn gửi đến đơn vị {0}?", a.ToString()), "Đặt Hàng", "Không");

                            if ((result as string) == "Đặt Hàng")
                            {
                                var kq = AC_Insert_Material_Orders(dataConfirm, _depSelect);

                                if (kq > 0) { is_confirm_completed = true; ConfirmCompleted?.Invoke(this, EventArgs.Empty); Services.DisplayToast.Show.Toast(string.Format("Đơn xác nhận {0} chi tiết", kq), Color.DarkGreen); }
                                else if (kq == 0)
                                { Services.DisplayToast.Show.Toast("Không thành công", Class.Style.Error); }
                                else Services.DisplayToast.Show.Toast("Đã đặt hàng rồi", Class.Style.Error);
                            }
                            else
                            {
                                IsBusy = false; return;
                            }
                        }
                        else
                        {
                            Services.DisplayToast.Show.Toast(string.Format(" Đơn vị {0} chưa hỗ trợ ", _depSelect), Class.Style.Error);

                        }
                    }
                }
            }
        }
       
        
        void Confirm_NS(List<Model_WH_NS> data)
        {

            var list_Selected = (data as List<Model_WH_NS>).Where(r => r.Sel).ToList();


            switch (DB.StoreLocal.Instant.Depname)
            {
                case DB.Departments.Stiching:
                    {

                        OrderItems_in_NS(list_Selected);
                        break;
                    }
                case DB.Departments.NoSew:
                    {
                      NS_DeliveryItems(list_Selected);
                        break;

                    }
                default:
                    break;
            }
         }
        
        private async void Confirm(object obj)
        {
            //chua ho tro cac kho khac ngoai cac kho nay
            if (!_ac_checked)
                if (!_lz_checked) 
                    if(!_ns_checked) { Services.DisplayToast.Show.Toast("Chưa thể thực hiện!", Class.Style.Error); return; }

         

            SetAwait(20);

            //neu la kho EP NONG
            if (Ns_checked) { Confirm_NS((obj as List<Model_WH_NS>)); return; }

            //Neu la kho ac va lz
            if (Ac_checked || Lz_checked )
            {
                
                var list_Selected = (obj as List<Model_DataGrid>).Where(r => r.Sel).ToList();
              
               // var dgv = _data.Where(r => r.Sel).ToList().Any() ? _data.Where(r => r.Sel).ToList() : list_Selected.Any() ? list_Selected : null;


                switch (DB.StoreLocal.Instant.Depname)
                {
                    case DB.Departments.Stiching:
                    case DB.Departments.NoSew:
                        {
                            OrderItems_in_AC(list_Selected);

                           
                            break;
                        }
                    
                    case DB.Departments.Lazer:
                    case DB.Departments.AutoCutting:
                        {
                            if ((_ac_checked && DB.StoreLocal.Instant.Depname == Departments.AutoCutting) || (_lz_checked && DB.StoreLocal.Instant.Depname == Departments.Lazer))
                            {
                                if (list_Selected.Any())
                                {
                                    var dataConfirm = list_Selected.Where(r => r.Sel).ToList();

                                    var lstBtn = new List<Button>();

                                    lstBtn.Add(new Button
                                    {
                                        Text = "Nosew (Ép Nóng)"
                                    });
                                    lstBtn.Add(new Button
                                    {
                                        Text = "High frequency (Ép Cao tần)"
                                    });
                                    lstBtn.Add(new Button
                                    {
                                        Text = "Printing (In)"
                                    });
                                    lstBtn.Add(new Button
                                    {
                                        Text = "Embroidery (Thêu)"
                                    });
                                    lstBtn.Add(new Button
                                    {
                                        Text = "Stiching (May)"
                                    });
                                    lstBtn.Add(new Button
                                    {
                                        Text = "Out sourcing (Gia Công)"
                                    });

                                    var a = await Services.Alert.PopupMenu("Department", lstBtn, Class.Style.Notification);

                                    if (string.IsNullOrEmpty(a))
                                    {

                                        Services.Alert.Msg("Alert", "Plaease select a deparment");
                                        IsBusy = false;
                                        return;
                                    }
                                    else
                                    {
                                        var _depSelect = "";
                                        switch (a)
                                        {
                                            case "Nosew (Ép Nóng)":
                                                _depSelect = "NS"; break;

                                            case "Stiching (May)":
                                                _depSelect = "ST"; break;

                                            default: //HF
                                                _depSelect = "HF"; break;
                                        }

                                     
                                        if (_depSelect == "NS" || _depSelect == "ST")
                                        {

                                            var result = await Services.Alert.PopupYesNo("Hỏi", string.Format("Bạn muốn gửi đến đơn vị {0}?", a.ToString()), "Đặt Hàng", "Không");

                                            if ((result as string) == "Đặt Hàng")
                                            {
                                                var kq = AC_Insert_Material_Orders(dataConfirm, _depSelect);

                                                if (kq > 0) { is_confirm_completed = true; ConfirmCompleted?.Invoke(this, EventArgs.Empty); Services.DisplayToast.Show.Toast(string.Format("Đơn xác nhận {0} chi tiết", kq), Color.DarkGreen); }
                                                else if (kq == 0)
                                                { Services.DisplayToast.Show.Toast("Không thành công", Class.Style.Error); }
                                                else Services.DisplayToast.Show.Toast("Đã đặt hàng rồi", Class.Style.Error);
                                            }
                                            else
                                            {
                                                IsBusy = false; return;
                                            }
                                        }
                                        else
                                        {
                                            Services.DisplayToast.Show.Toast(string.Format(" Đơn vị {0} chưa hỗ trợ ", _depSelect), Class.Style.Error);

                                        }
                                    }
                                }
                            }
                            break;
                        }

                    default:
                        {
                            var msg = "Lỗi User";
                            Services.DisplayToast.Show.Toast(msg, Color.Red);
                            break;
                        }
                }


                #region Cap nhap local: Sau khi cap thanh cong - gioi han trong local
                foreach (var item in list_Selected)
                {
                    var editLocalAC = DB.StoreLocal.Instant.Data_WareHouse_AC.Where(r => r.RY == item.RY &&
                                                                                           r.Comp == item.Comp &&
                                                                                           r.Clbh == item.Clbh &&
                                                                                           r.Bwbh == item.Bwbh &&
                                                                                           r.Art == item.Art).ToList();

                    var editLocalLZ = DB.StoreLocal.Instant.Data_WareHouse_Lz.Where(r => r.RY == item.RY &&
                                                                                         r.Comp == item.Comp &&
                                                                                         r.Clbh == item.Clbh &&
                                                                                         r.Bwbh == item.Bwbh &&
                                                                                         r.Art == item.Art).ToList();

                    /*khong biet la datalocal nao duoc cap nhat ac hay lz nen phai tim 2 cai luon*/


                    if (editLocalAC != null && editLocalAC.Count > 0)
                    {
                        DB.StoreLocal.Instant.Data_WareHouse_AC.RemoveAll(r => r.RY == item.RY &&
                             r.Comp == item.Comp &&
                             r.Clbh == item.Clbh &&
                             r.Bwbh == item.Bwbh &&
                             r.Art == item.Art &&
                             r.Sel == true);
                    }
                    if (editLocalLZ != null && editLocalLZ.Count > 0)
                    {
                        DB.StoreLocal.Instant.Data_WareHouse_Lz.RemoveAll(r => r.RY == item.RY &&
                            r.Comp == item.Comp &&
                            r.Clbh == item.Clbh &&
                            r.Bwbh == item.Bwbh &&
                            r.Art == item.Art &&
                            r.Sel == true);
                    }
                }
                #endregion
            }

            IsBusy = false;

        }
        int AC_Insert_Material_Orders(List<Model_DataGrid> dataconfirm,string Dep)
        {
            int kq = 0;

            var data = dataconfirm.Where(r => r.Sel).ToList();

            if (data.Any())
            {
                string prefix_ID = _ac_checked ? "AC" : _ns_checked ? "NS" : _st_checked ? "ST" : _print_checked ? "PR" : _em_checked ? "EM" : _lz_checked ? "LZ":"OS";

                foreach (var item in data)
                {
                    if (item.Sel)
                    {

                        if (item.TarQty.ToString().Split('/')[0] == "0") continue;

                        string id = string.Format("{0}{1}", prefix_ID, DateTime.Now.Ticks.ToString());

                        var checkExist = $@"SELECT isnull(sum(qty),0)Qty  FROM App_Material_Orders where ry='{item.RY}' and Component='{item.Comp}'";

                        var ta = DB.SQL.ConnectDB.Connection.FillDataTable(checkExist);

                        var sqlQty = int.Parse(ta.Rows[0][0].ToString());

                        var localTargetQty = int.Parse(item.TarQty.Split('/')[1]);

                        //san luong chua du thi tiep tuc insert
                        if (sqlQty < localTargetQty) 
                        {
                            string sql = @"INSERT INTO [dbo].[App_Material_Orders]
                                           ([OrderId]
                                           ,[RY]
                                           ,[Component]
                                           ,[BWBH]
                                           ,[CLBH]
                                           ,[UserDate]
                                           ,[UserID]
                                           ,[Qty]
                                           ,[DepNo])
                                     VALUES
                                           (@id
                                           , @ry
                                           , @comp
                                           , @bwbh
                                           , @clbh
                                           , @date
                                           , @userid
                                           , @qty
                                           , @depno)";


                            string[] arr = { id, item.RY, item.Comp, item.Bwbh, item.Clbh, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DB.StoreLocal.Instant.UserName, item.TarQty.ToString().Split('/')[0], Dep.ToUpper() };

                            if (Class.Network.Net.HasNet)
                                kq = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, arr);

                            //neu la don vi CTD hoac la don vi Lazer thi ko can phai vao them Process Table [Status =0] 
                            if (DB.StoreLocal.Instant.Depname == Departments.AutoCutting || DB.StoreLocal.Instant.Depname == Departments.Lazer)
                            {
                                if (kq != 0)
                                {

                                    AC_Insert_Material_Process(item, id);
                                }
                            }

                            //------------------
                            List<Models.OrderDetail> lstDetail = new List<Models.OrderDetail>();

                            var lstInput = Data_inputSize(item.RY, item.Comp);

                            foreach (var e in lstInput)
                            {
                                var orderqty = e.Qty - e.QtySize;


                                lstDetail.Add(new Models.OrderDetail
                                {
                                    Orderid = id,
                                    RY = item.RY,
                                    Component = item.Comp,
                                    XXCC = e.xxcc,
                                    OrderQty = orderqty

                                });
                            }
                            kq = Insert_Order_Detail(lstDetail);
                        }
                        else kq = -1;
                    }                                           
                };
            }
            else
            {
                Alarm.Sound.Error();
                Services.DisplayToast.Show.Toast("Nothing", Class.Style.Error);
            }
            return kq;
        }
        int NS_Insert_Material_Orders(List<Model_WH_NS> dataconfirm, string Dep)
        {
            int kq = 0;

            var data = dataconfirm.Where(r => r.Sel).ToList();

            if (data.Any())
            {
                string prefix_ID = _ac_checked ? "AC" : _ns_checked ? "NS" : _st_checked ? "ST" : _print_checked ? "PR" : _em_checked ? "EM" : _lz_checked ? "LZ" : "OS";

                foreach (var item in data)
                {
                    if (item.Sel)
                    {

                        if (item.Qty.ToString().Split('/')[0] == "0") continue;

                        string id = string.Format("{0}{1}", prefix_ID, DateTime.Now.Ticks.ToString());

                        var checkExist = $@"SELECT isnull(sum(qty),0)Qty  FROM App_Material_Orders where ry='{item.Ry}' and Component='{item.EngName}'";

                        var ta = DB.SQL.ConnectDB.Connection.FillDataTable(checkExist);

                        var sqlQty = int.Parse(ta.Rows[0][0].ToString());

                        var localTargetQty = int.Parse(item.Qty.Split('/')[1]);

                        //san luong chua du thi tiep tuc insert
                        if (sqlQty < localTargetQty)
                        {
                            string sql = @"INSERT INTO [dbo].[App_Material_Orders]
                                           ([OrderId]
                                           ,[RY]
                                           ,[Component]
                                           ,[BWBH]
                                           ,[CLBH]
                                           ,[UserDate]
                                           ,[UserID]
                                           ,[Qty]
                                           ,[DepNo])
                                     VALUES
                                           (@id
                                           , @ry
                                           , @comp
                                           ,@bwbh
                                           ,@clbh
                                           , @date
                                           , @userid
                                           , @qty
                                           , @depno)";


                            string[] arr = { id, item.Ry, item.EngName,".",".",  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DB.StoreLocal.Instant.UserName, item.Qty.Split('/')[0].ToString(), Dep.ToUpper() };

                            if (Class.Network.Net.HasNet)
                                kq = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, arr);

                            //neu la don vi CTD hoac la don vi Lazer thi ko can phai vao them Process Table [Status =0] 
                            if (DB.StoreLocal.Instant.Depname == Departments.NoSew)
                            {
                                if (kq != 0)
                                {

                                    NS_Insert_Material_Process(item, id);
                                }
                            }

                            //------------------
                            if (!_ns_checked)
                            {

                                List<Models.OrderDetail> lstDetail = new List<Models.OrderDetail>();

                                var lstInput = Data_inputSize(item.Ry, item.EngName);

                                foreach (var e in lstInput)
                                {
                                    var orderqty = e.Qty - e.QtySize;


                                    lstDetail.Add(new Models.OrderDetail
                                    {
                                        Orderid = id,
                                        RY = item.Ry,
                                        Component = item.EngName,
                                        XXCC = e.xxcc,
                                        OrderQty = orderqty

                                    });
                                }
                                kq = Insert_Order_Detail(lstDetail);
                            }
                        }
                        else kq = -1;
                    }
                };
            }
            else
            {
                Alarm.Sound.Error();
                Services.DisplayToast.Show.Toast("Nothing", Class.Style.Error);
            }
            return kq;
        }
        int Insert_Order_Detail(List<OrderDetail> lstOrderDetail)
        {
            int kq = 0;

            if (lstOrderDetail.Any())
            {
                foreach (var item in lstOrderDetail)
                {

                    if (item.OrderQty == 0) continue;

                    string sql = @"INSERT INTO [dbo].[App_Order_Detail]
                               ([OrderID]
                               ,[RY]
                               ,[Component]
                               ,[XXCC]
                               ,[OrderQty])
                         VALUES
                               ( @orderid
                               , @RY
                               , @Component
                               , @XXCC
                               , @OrderQty)";

                    kq = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, new string[]
                    {
                        item.Orderid,
                        item.RY,
                        item.Component,
                        item.XXCC,
                        item.OrderQty.ToString()
                    });
                }

            }
            return kq;
        }
        //List<(string XXCC, int Qty)> _NS_Data_InputSize(string Ry)
        //{
        //    string sql $@= "SELECT xxcc,sum(actualqty)qty FROM App_Cutting_Barcodes_Edit edit where edit.ZLBH ='AL2411-358' group by xxcc order by xxcc"
        //}
        List<(string xxcc, int Qty,int Target,int QtySize)> Data_inputSize(string ry , string comp)
        {
            List<(string, int, int, int)> lst = new List<(string, int, int, int)>();            

            string sql = $@"declare 
                            @ry varchar(20)='{ry}'
                            ,@comp varchar(50)='{comp}'
                            SELECT ZLBH,
                                   BWBH,
                                   bwzl.ywsm,
                                   app.XXCC,
                                   CONVERT(VARCHAR, CONVERT(INT, ActualQty)) Qty,
                                   CONVERT(VARCHAR, CONVERT(INT, PlanQty)) [Target],
	                               sum(ISNULL(OrderQty,0)) TONG
                            FROM App_Cutting_Barcodes_Edit app
                                INNER JOIN bwzl
                                    ON app.BWBH = bwzl.bwdh
	                            LEFT JOIN App_Order_Detail aod
		                            ON aod.XXCC = app.XXCC
			                            AND ry = app.ZLBH
			                            AND Component = ywsm
                            WHERE ZLBH = @ry
                                  AND CONVERT(INT, PlanQty) != 0
                                  AND CONVERT(INT, ActualQty) != 0
                                  AND ywsm = @comp
                            GROUP BY ZLBH,
                                   BWBH,
                                   bwzl.ywsm,
                                   app.XXCC,ActualQty,PlanQty";

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if(ta.Rows.Count > 0)
            {                               
                foreach (System.Data.DataRow r in ta.Rows)
                {
                    var a = (
                                r["xxcc"].ToString(),
                                int.Parse(r["Qty"].ToString()),
                                int.Parse(r["target"].ToString()),
                                int.Parse(r["tong"].ToString())
                            );

                    lst.Add(a);
                }
            }
            return lst;
        }
        void AC_Insert_Material_Process(Model_DataGrid item,string orderid, string status="0")
        {
            int kq = 0;

            if (item.Sel)
            {                 
                string sql = $@"INSERT INTO [dbo].[App_Material_Process]
               ([OrderId]
               ,[UserDate]
               ,[UserID]
               ,[Status])
         VALUES
               (@id
               ,@uDate
               ,@uID
               ,{status})";

                string[] arr = { orderid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DB.StoreLocal.Instant.UserName };

                if(Class.Network.Net.HasNet)
                    kq += DB.SQL.ConnectDB.Connection.Update_Parameter(sql, arr);                
            }
        }




        void NS_Insert_Material_Process(Model_WH_NS item, string orderid, string status = "0")
        {
            int kq = 0;

            if (item.Sel)
            {
                string sql = $@"INSERT INTO [dbo].[App_Material_Process]
               ([OrderId]
               ,[UserDate]
               ,[UserID]
               ,[Status])
         VALUES
               (@id
               ,@uDate
               ,@uID
               ,{status})";

                string[] arr = { orderid, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DB.StoreLocal.Instant.UserName };

                if (Class.Network.Net.HasNet)
                    kq += DB.SQL.ConnectDB.Connection.Update_Parameter(sql, arr);
            }
        }


        public void GetData_Inventory()
        {
            //var dt = Load_data_temp_Background();
            var dt =  Get_Data_AC();
            if (dt.Count > 0)
            {

                DB.StoreLocal.Instant.Data_WareHouse_Lz = dt.Where(r => !string.IsNullOrEmpty(r.MachineNO) && r.MachineNO.ToUpper() == "LASER").OrderByDescending(r => r.UserDate).ToList();

                DB.StoreLocal.Instant.Data_WareHouse_AC = dt.Where(r => !string.IsNullOrEmpty(r.MachineNO) && r.MachineNO.ToUpper() != "LASER").OrderByDescending(r => r.UserDate).ToList();
            }

            _data_wh_ns = Load_Data_NS();

        }
        //public List<Model_DataGrid> Get_Data_AC()
        //{
        //    string url = string.Format("{0}Get_data_AC", server);

        //    using (HttpClient client = new HttpClient())
        //    {
        //        // Gọi đồng bộ (blocking)
        //        var response = client.GetAsync(url).Result;

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var json = response.Content.ReadAsStringAsync().Result;
        //            var machines = JsonConvert.DeserializeObject<List<Model_DataGrid>>(json);
        //            return machines;
        //        }

        //        return new List<Model_DataGrid>();
        //    }
        //}


        public List<Model_WH_NS> Load_Data_NS()
        {
                    string sql = @"SELECT 
                    comp.CompID ,
                    comp.RY,
                    comp.NameComp VnName,
                    comp.EngName, 
                    dd.ARTICLE ,
                    dd.KHPO,comp.Qty, 
                    dd.Pairs OrderQty, 
                    pdsch .PSDT
                    ,pdsch.LEAN 
                    FROM App_Combine_Comp comp inner join ddzl dd on dd.DDBH =comp.RY inner join pdsch
                    on pdsch.ry = dd.DDBH
                    where    not exists(SELECT * FROM App_Material_Orders o where o.ry=comp.ry and o.Component=comp.EngName)";

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
            var temp = new List<Model_WH_NS>();
            if (ta.Rows.Count > 0)
            {
                CountItems = string.Format("{0}", ta.Rows.Count);

                foreach (System.Data.DataRow r in ta.Rows)
                {

                    Command_Click = new Command(async (Object obj) =>
                    {
                        var d = obj as Model_WH_NS;

                        IsBusy = true;

                        await Task.Delay(1);

                        // var dep = _ac_checked ? "AC" : _ns_checked ? "NS" : _st_checked ? "ST" : _print_checked ? "PR" : _em_checked ? "EM" : _lz_checked ? "LZ" : "OS";

                        // await Application.Current.MainPage.Navigation.PushAsync(new PageViews.SetValuePage(d, dep));

                        string s = $@"SELECT comp.NameComp ,orders.Component FROM App_Details_Combine dc 
inner join App_Material_Orders orders on dc.CompID_old = orders.OrderId inner join App_Combine_Comp comp on comp.CompID = dc.CompID_new
where CompID_new =  '{d.ID}'";


                        var table = DB.SQL.ConnectDB.Connection.FillDataTable(s);

                        if (table.Rows.Count > 0)
                        {
                            var list =new  List<Model_PopupCombine>();

                            foreach (System.Data.DataRow item in table.Rows)
                            {
                                list.Add(new Model_PopupCombine
                                {
                                    Component = item["Component"].ToString(),
                                    NameComp = item["NameComp"].ToString(),

                                });
                            }

                            Services.Alert.PopupListCombine(list);

                        }


                        

                        IsBusy = false;
                    });


                    temp.Add(new Model_WH_NS
                    {
                        Sel =false,

                        ID = r["CompID"].ToString(),

                        Ry = r["Ry"].ToString(),

                        VnName = r["VnName"].ToString(),

                        EngName = r["EngName"].ToString(),

                        Article = r["article"].ToString(),

                        PO = r["KHPO"].ToString(),

                        Qty = string.Format("{0}/{1}", r["Qty"].ToString(), r["OrderQty"].ToString()),

                        PSDT = DateTime.Parse(r["PSDT"].ToString()).ToString("yyyy-MM-dd"),

                        Lean = r["Lean"].ToString(),

                        Command_Click = Command_Click
                    });

                }
                //_depname = temp.Select(x => x.Lean)
                //         .Distinct().OrderBy(x => x)
                //         .ToList();

                //_depname.Add("All");

                //_depname.Select(x => x.Any())
                //    .OrderBy(x => x);


            }
            return temp;
        }
        public List<Model_DataGrid> Get_Data_AC()
        {
            var temp = new List<Model_DataGrid>();


            string sql = "";
              
            #region new code 
            sql = $@"SELECT k.MachineNo,
       zlbh,
       k.component,
	   ComponentVN,
       k.clbh,
       k.modelname,
       k.article,
       k.psdt,
       k.bwbh,
       k.planqty,
	   
k.USERDATE,
       Isnull(CONVERT(INT, his.qty), 0)- ISNULL(orders.Qty,0) Actual,
	   dep.DepName
FROM   (SELECT DISTINCT 
                        e.zlbh,
                        b.ywsm                       Component,
						c.compvn ComponentVN,
                        e.clbh,
                        x.xieming                    ModelName,
                        d.article,
                        p.psdt,
                        e.bwbh,
                        Sum(CONVERT(INT, e.planqty)) planqty,
						cb.DepID,
						cb.MachineNo,
						e.USERDATE
        FROM   app_cutting_barcodes_edit e
               INNER JOIN bwzl b
                       ON b.bwdh = e.bwbh
			   left join componentname  c on c.compeng= b.ywsm
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
                 cb.DepID,
				 cb.MachineNo,
				 e.USERDATE,c.compeng,c.compvn) k
       LEFT JOIN (SELECT RY,Component,BWBH,CLBH,SUM(Qty)Qty FROM app_material_orders GROUP BY RY,Component,BWBH,CLBH ) orders
              ON orders.bwbh = k.bwbh
                 AND orders.ry = k.zlbh
                 AND orders.clbh = k.clbh
                 AND orders.component = k.component
	   LEFT JOIN dbo.BDepartment dep ON dep.ID = k.DepID
       INNER JOIN (SELECT ry,
                          bwbh,
                          clbh,
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
                                                   + 1, Charindex('_', prono,Charindex('_', prono,
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
                           FROM   app_cutting_history_edit
						   WHERE convert(date,Userdate) between convert(date,getdate()-45) and convert(date,getdate())) E
                   GROUP  BY ry,
                             E.bwbh,
                             E.clbh) his
               ON his.bwbh = K.bwbh
                  AND his.clbh = K.clbh
                  AND his.ry = K.zlbh
WHERE( orders.RY IS NULL
        OR orders.qty < K.planqty )  and  convert(date,K.USERDATE) between convert(date,getdate()-45) and  convert(date,getdate())
order by k.USERDATE desc";
            #endregion


            var ListCompENG_VN = new List<(string ENG, string VN)>();

            var compTable = DB.SQL.ConnectDB.Connection.FillDataTable("SELECT * FROM Componentname");

            foreach (System.Data.DataRow item in compTable.Rows)
            {
                (string, string) temp1;

                temp1.Item1 = item["compeng"].ToString();
                temp1.Item2 = item["compvn"].ToString();

                ListCompENG_VN.Add(temp1);
            }

            if (sql == "") return _data;         

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if (ta .Rows.Count>0)
            {
                CountItems = string.Format("{0} ပစ္စည်းများ", ta.Rows.Count);

                // DB.DataLocal.Table.Insert_Store_AC(ta);

                foreach (System.Data.DataRow row in ta.Rows)
                {
                    ClickCommand = new Command(async (Object obj) =>
                    {
                        var d = obj as Model_DataGrid;
                      
                        IsBusy = true;

                        await Task.Delay(1);

                        var dep = _ac_checked ? "AC" : _ns_checked ? "NS" : _st_checked ? "ST" : _print_checked ? "PR" : _em_checked ? "EM" : _lz_checked ? "LZ" : "OS";

                        await Application.Current.MainPage.Navigation.PushAsync(new PageViews.SetValuePage(d, dep));

                        IsBusy = false;
                    });


                    var compText = row["Component"].ToString();

                    var compvn = row["ComponentVN"]?.ToString() ?? string.Empty;

                    //lay phan text cua component
                    var newtext = new[] { new string((compText?.Where(c => char.IsLetter(c) || c == ' ').ToArray() ?? Array.Empty<char>())).Trim() };

                    if (newtext[0] != "")
                    {
                        if (string.IsNullOrEmpty(compvn))
                        {
                            var int_Exist = ListCompENG_VN.FindIndex(x => x.ENG == newtext[0]);
                            if (int_Exist > 0)
                            {

                                var tv = ListCompENG_VN[ListCompENG_VN.FindIndex(x => x.ENG == newtext[0])];

                                if (!string.IsNullOrEmpty(tv.VN))
                                    compText = compText.Replace(newtext[0], tv.VN);

                                else
                                    compText = row["Component"]?.ToString() ?? string.Empty;

                            }
                            else compText = row["Component"].ToString();

                        }
                        else
                            compText = compText.Replace(newtext[0], compvn);

                    }
                    else
                    {
                        compText = "";
                    }


                    temp.Add(new Model_DataGrid
                    {
                        MachineNO = row["MachineNo"].ToString(),

                        Sel = false,

                        RY = row["Zlbh"].ToString(),

                        Comp = row["Component"].ToString(),

                        CompVN = compText,

                        TarQty = string.Format("{0}/{1}", int.Parse(row["Actual"].ToString()), int.Parse(row["planqty"].ToString())),

                        Art = row["article"].ToString(),

                        Bwbh = row["bwbh"].ToString(),

                        ModelName = row["modelname"].ToString(),

                        DepName = row["depname"].ToString(),

                        ProdDate = DateTime.Parse(row["psdt"].ToString()).ToString("yyyy-MM-dd"),

                        Clbh = row["CLBH"].ToString(),

                        OrderID = "",

                        UserDate = DateTime.Parse(row["UserDate"].ToString()),

                        ClickCommand = ClickCommand
                    }) ;


                }
            }

          //  temp = temp.OrderBy(x => x.DepName).ToList();

            //_depname = temp.Select(x => x.DepName)
            //                   .Distinct().OrderBy(x => x)
            //                   .ToList();

            //_depname.Add("All");

            //_depname.Select(x => x.Any())
            //    .OrderBy(x => x);




            return temp;
        }
       
        #endregion
    }
}