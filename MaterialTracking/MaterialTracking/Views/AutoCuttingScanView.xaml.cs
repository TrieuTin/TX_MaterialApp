/*
 * Hi! Coder
 * My name's Tin, when I wrote this code, only God and i knew how it worked.
 * But now, only God knows it!
 * Therefore, if you are trying to optimize this rountine and it fails (most surely), please contact me via 0824.272.373 
 * This thing is not help you, but I promise I will give you spiritual support
 * Good lucky my brothers
 */


using MaterialTracking.Class;
using MaterialTracking.DB;
using MaterialTracking.Models;
using MaterialTracking.UsersControl;
using MaterialTracking.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace MaterialTracking.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AutoCuttingScanView : ContentPage
    {
        ObservableCollection<ViewModels.AutoCuttingScan_Data> _data;

        ObservableCollection<ViewModels.AutoCuttingScan_GridTitle_Model> _dataTitle;

        ObservableCollection<DB.Table_Barcode> _dataConfirm;

        ObservableCollection<ViewModels.ACS_DataGroup> _dataGrp;

        string _barcode = DB.StoreLocal.Instant.Barcode;

        List<string> listRy;
        const string LocalColor = "#19A7CE";

        bool _canEdit;



        AutoCuttingModel classModel;

        Tools.Timer timer;
        //Label lbl_detail;
        Label lbl_Comp;
        Label lbl_RY;
        //Entry tbox;

        UC_Input ToolInput;

        List<UC_Input> ListControls = new List<UC_Input>();

        //Button SubbtnConform;
        protected override bool OnBackButtonPressed()
        {

            Device.BeginInvokeOnMainThread(async () =>
            {
                base.OnBackButtonPressed();

                var navigationPage = Application.Current.MainPage as NavigationPage;
                if (navigationPage != null)
                {
                    await navigationPage.Navigation.PopToRootAsync();
                }
                else
                {                                                                                                                                                                                        
                    // Handle the case where the page is not inside a NavigationPage
                }


            });

            return true;
        }
        AutoCuttingModel DataContext
        {
            get => (BindingContext as AutoCuttingModel);
        }
        void StartForm( string ry = "", bool CanEdit = true)
        {


            if (ry == "")
            {
                // We need to know how many RYs of this BarCode to show and then the User have to select one of list's ry
                string sql = $"SELECT distinct ZLBH ,bwbh FROM  cutting_barcodes2 v where v.Barcode='{DB.StoreLocal.Instant.Barcode}' order by zlbh";

                var table = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                if (table.Rows.Count > 1)
                {
                    listRy = new List<string>();

                    foreach (System.Data.DataRow r in table.Rows)
                    {
                        listRy.Add(string.Format("{0}_{1}", r["ZLBH"].ToString(), r["bwbh"].ToString()));
                    }

                }
                else if (table.Rows.Count == 1)
                {
                    listRy = new List<string> { string.Format("{0}_{1}", table.Rows[0]["zlbh"].ToString(), table.Rows[0]["bwbh"].ToString()) };

                }
                else return;


            }
            else //_ry = ry;
                listRy = new List<string> { ry };

            _canEdit = CanEdit;

           // _barcode = DB.StoreLocal.Instant.Barcode;

            
            classModel = new AutoCuttingModel( listRy);

            BindingContext = classModel;

            btn_Confirm.IsEnabled = _canEdit;

            _dataTitle = classModel.DataTitle;

            _data = classModel.Data;

            _dataConfirm = classModel.Data_Confirm;

            _dataGrp = classModel.DataGroup;

            SetTitle(_dataTitle);

            SetDetail(_dataTitle, _data);

          

            timer = timer == null ? timer = new Tools.Timer(TimeSpan.FromSeconds(1), SyncData_Tick) : timer;
            //timer.Start();

            ScrollCoDinh.Scrolled += (s, e) =>
            {
                ScrollHor.ScrollToAsync(0, e.ScrollY, false);
            };

            ScrollHor.Scrolled += (s, e) =>
            {
                ScrollCoDinh.ScrollToAsync(0, e.ScrollY, false);
            };
        }

        private void SyncData_Tick()
        {
            if (_dataConfirm.Count > 0)
            {


                var getDataConfirm = classModel.Data_Confirm;

                var a = DB.DataLocal.Table.Data_Barcode(_barcode);



                if (getDataConfirm.Any())
                {
                    foreach (var datacfm in getDataConfirm)
                    {
                        try
                        {
                            var rs = a.Where(x => x.Prono == datacfm.Prono).ToList();
                            rs[0].Qty = Math.Abs(rs[0].Qty - rs[0].QtySql) + datacfm.Qty;
                            rs[0].QtySql = datacfm.Qty;
                            DB.DataLocal.Table.Update_Barcode(rs[0]);
                            foreach (var ctrl in ListControls)
                            {
                                var bdctxt = ctrl.BindingContext as InputViewModel;



                                if (bdctxt.Prono == datacfm.Prono)
                                {
                                    bdctxt.Qtyed = rs[0].Qty;
                                    bdctxt.Qtysql =
                                        rs[0].QtySql;
                                    //bdctxt.Temp = rs[0].Target - rs[0].Qty;
                                    bdctxt.Temp = bdctxt.Temp > (rs[0].Target - rs[0].Qty) ? (rs[0].Target - rs[0].Qty) : bdctxt.Temp;
                                    ctrl.BindingContext = bdctxt;
                                    break;
                                }
                            }
                        }
                        catch (Exception)
                        {


                        }

                    }
                }
            }
            //  Generate_Element(a[0]);
        }

        public AutoCuttingScanView(string ry = "", bool CanEdit = true)
        {
            InitializeComponent();
            Column_RY.Text = ConvertLang.Convert.Translate_LYM("RY", "အာဝိုင်");
            Column_Comp.Text = ConvertLang.Convert.Translate_LYM("Comp", "အစိတ်အပိုင်းများ");
            
            StartForm( ry, CanEdit);

           
        }
        //protected override async void OnAppearing()
        //{
        //    base.OnAppearing();

        //    //var i = DB.DataLocal.Table.DeleteAll; //await App.LocalDatabase.DeleteAll();
        //    //                                      //Services.Alert.ToastMsg("Delete " + i);
        //    //var lst = DB.DataLocal.Table.Data; //await App.LocalDatabase.Data();


        //}


        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Xoa het su kien khi page khong ton tai nua

            //if (tbox != null)
            //{

            //    tbox.Completed -= Tbox_Completed;

            //    tbox.Focused -= Tbox_Focused;

            //    tbox.TextChanged -= Tbox_TextChanged;
            //if (SubbtnConform != null)
            //    SubbtnConform.Clicked -= SubbtnConform_Clicked;
            //}
        }
        private void SetTitle(ObservableCollection<ViewModels.AutoCuttingScan_GridTitle_Model> DataTitle)
        {
            if (!_data.Any()) return;

            string sql = $"SELECT dwbh FROM clzl where clzl.cldh='{_data[0].CLBH}'";

            var donvitinh = DB.SQL.ConnectDB.Connection.FillDataTable(sql).Rows[0][0].ToString();

            string hexColor = "#ECE3CE";

            int font_size = 20;

            Color c = Color.FromHex(hexColor);

            var Title = DataTitle;

            if (Title == null)
            {
                return;
            }



            ColumnDefinition col;


            for (int i = 0; i < Title.Count; i++)
            {
                col = new ColumnDefinition
                {
                    Width = new GridLength(185, GridUnitType.Absolute)
                };

                var lbl_size = new Label
                {
                    Text = Title[i].Size,
                    TextColor = c,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = font_size,
                    FontAttributes = FontAttributes.Bold
                     ,
                    WidthRequest = 100

                };


                GridTitle.ColumnDefinitions.Add(col);

                GridData.ColumnDefinitions.Add(col);

                GridTitle.Children.Add(lbl_size, i, 0);

            }

            col = new ColumnDefinition
            {
                Width = new GridLength(100, GridUnitType.Absolute)
            };

            var label = new Label
            {
                Text = ConvertLang.Convert.Translate_LYM("Tổng", "စုစု"),
                TextColor = c,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = font_size,
                FontAttributes = FontAttributes.Bold
            };

            GridTitle.ColumnDefinitions.Add(col);

            GridTitle.Children.Add(label, Title.Count, 0);

            GridData.ColumnDefinitions.Add(col);


            col = new ColumnDefinition
            {
                Width = new GridLength(100, GridUnitType.Absolute)
            };

            label = new Label
            {
                Text = donvitinh,

                TextColor = c,

                HorizontalTextAlignment = TextAlignment.Center,

                VerticalTextAlignment = TextAlignment.Center,

                FontSize = font_size,

                FontAttributes = FontAttributes.Bold
            };

            GridTitle.ColumnDefinitions.Add(col);

            GridData.ColumnDefinitions.Add(col);

            GridTitle.Children.Add(label, Title.Count + 1, 0);
        }
        private void UCInput_ButtonPlusClicked(object sender, EventArgs e)
        {

            foreach (ViewModels.AutoCuttingScan_Data rowData in _data)
            {
                var compo = rowData.Component.ToString();

            }

        }

        void Generate_Element(List<DB.Table_Barcode> a)
        {
            var setOn = false;

            if (a[0].QtySql != a[0].Target)
            {
                setOn = true;
            }
            else
            {
                setOn = false;
            }

            ToolInput = new UC_Input(new InputViewModel
            {
                Temp = a[0].Target - a[0].Qty,

                Qtyed = a[0].Qty,

                Qtysql = a[0].QtySql,

                TotalQty = a[0].Target,

                Prono = a[0].Prono,

                SetOnOFF = setOn,

                BColor = a[0].QtySql != a[0].Target ? Class.Style.Organ : a[0].Target == 0 ? Class.Style.EnableOff : Class.Style.Primary,

                IsFul = a[0].QtySql != a[0].Target ? false : a[0].Target != 0 ? true : false

            });

            ToolInput.ButtonPlusClicked += UCInput_ButtonPlusClicked;

            ListControls.Add(ToolInput);


        }
        void Generate_Element(double val, string prono)
        {
            ToolInput = new UC_Input(new InputViewModel
            {
                Qtyed = 0,

                Temp = (int)val,

                TotalQty = (int)val,

                Prono = prono,

                Qtysql = 0,

                SetOnOFF = val > 0,

                BColor = val == 0 ? Class.Style.EnableOff : Class.Style.Organ,

                IsFul = false

            });

            ToolInput.ButtonPlusClicked += UCInput_ButtonPlusClicked;

            ListControls.Add(ToolInput);
        }



        private void SetDetail(ObservableCollection<ViewModels.AutoCuttingScan_GridTitle_Model> dataTitle, ObservableCollection<ViewModels.AutoCuttingScan_Data> data)
        {
            int font_size = 20;
            int dong = 0;
            Color color = Color.FromHex(LocalColor);

            if (data == null)
            {
                return;
            }

            foreach (ViewModels.AutoCuttingScan_Data rowData in data)
            {

                int col = 0;

                var compo = rowData.Component.ToString().Trim();

                var bwhh = rowData.BWBH.ToString();

                var ddbh = rowData.DDBH;

                var zlbh = rowData.ZLBH;

                var total = rowData.Total.ToString();

                var mqty = rowData.DMQty.ToString();

                var clbh = rowData.CLBH.ToString();

                lbl_Comp = new Label
                {
                    Text = compo,
                    FontSize = font_size,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    BackgroundColor = color,
                    ClassId = "Comp"
                };
                //GridLength(100, GridUnitType.Absolute)
                var row = new RowDefinition
                {
                    Height = new GridLength(150, GridUnitType.Absolute)
                };

                GridData.RowDefinitions.Add(row);

                GridCompRY.RowDefinitions.Add(row);

                GridCompRY.Children.Add(lbl_Comp, 0, dong);


                lbl_RY = new Label
                {
                    Text = ddbh,

                    FontSize = 18,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    BackgroundColor = color,
                    //Margin = new Thickness(2, 1)
                };

                var GridRY = new Grid();

                GridRY.BackgroundColor = Color.FromHex(LocalColor);

                var colRY = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Auto)
                };

                GridRY.ColumnDefinitions.Add(colRY);
            
                GridRY.Children.Add(lbl_RY);

                GridRY.VerticalOptions = LayoutOptions.FillAndExpand;

                GridRY.HorizontalOptions = LayoutOptions.FillAndExpand;
                //TUA
               
                if (ddbh != zlbh)
                {
                    if (string.IsNullOrEmpty(zlbh))
                    {
                        return;
                    }
                    var rowRY = new RowDefinition
                    {
                        Height = new GridLength(1, GridUnitType.Auto)
                    };
                    GridRY.RowDefinitions.Add(rowRY);                    

                    var lbl1 = new Label
                    {
                        Text = zlbh.Split('-')[2],

                        FontSize = 18,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        BackgroundColor = color,
                        Margin = new Thickness(0)
                        //Margin = new Thickness(2, 1)
                    };

                    GridRY.Children.Add(lbl1, 0, 1);
                }
                var btn_Save = new Button();

                var btn_Reset = new Button();

                btn_Save.Text = "Save";

                btn_Save.FontSize = 18;

                btn_Reset.Text = "Reset";

                btn_Reset.FontSize = 18;

                //[0] barcode; [1] ddbh; [2] clbh; [3] size; [4]Bwbh

                btn_Reset.AutomationId = btn_Save.AutomationId = lbl_Comp.AutomationId = lbl_RY.AutomationId = string.Format("{0}_{1}_{2}_{3}_{4}", _barcode, ddbh, clbh, bwhh, rowData.ZLBH);

                btn_Save.VerticalOptions = LayoutOptions.Center;

                btn_Save.HorizontalOptions = LayoutOptions.Center;

                btn_Reset.VerticalOptions = LayoutOptions.Center;

                btn_Reset.HorizontalOptions = LayoutOptions.Center;

                btn_Save.Clicked += (s, e) =>
                {
                    var btn = s as Button;

                    SaveLocal(btn.AutomationId);

                    AddQty_Compo();

                    Alarm.Sound.Click();
                };

                btn_Reset.Clicked += (s, e) =>
                {
                    var btnReset = s as Button;

                    ResetTemp(btn_Reset.AutomationId);

                };

                GridRY.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                GridRY.Children.Add(btn_Reset, 0, ddbh != zlbh ? 2 : 1);

                GridRY.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

                GridRY.Children.Add(btn_Save, 0, ddbh != zlbh ? 3 : 2);

                btn_Reset.Margin = new Thickness(0, -5);

                btn_Save.Margin = new Thickness(0, -5);

                GridCompRY.Children.Add(GridRY, 1, dong);

                var lbl = new Label
                {
                    Text = total,
                    FontSize = font_size,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    BackgroundColor = color,                    
                };

                GridData.Children.Add(lbl, dataTitle.Count, dong);             

                lbl = new Label
                {
                    Text = mqty,
                    FontSize = font_size,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    BackgroundColor = color,                    
                };

                GridData.Children.Add(lbl, dataTitle.Count + 1, dong);
                
                for (int cot = 0; cot < dataTitle.Count; cot++)
                {
                    foreach (var properties in rowData.GetType().GetProperties())
                    {
                        var value = double.TryParse(properties.GetValue(rowData).ToString(), out double val) ? val : 0;

                        var colName = properties.Name;

                        if (dataTitle[cot].Size == CheckSize(colName))
                        {
                            var prono = string.Format("{0}_{1}_{2}_{3}_{4}", _barcode, rowData.DDBH, rowData.CLBH, dataTitle[cot].Size, rowData.BWBH);
                            #region check data local

                            var sz = dataTitle[cot].Size;

                            var a = DB.DataLocal.Table.ExistSize(_barcode, ddbh, sz, bwhh);

                            if (!a.Any() && !_dataConfirm.Any()) //Load New complete
                            {
                                Generate_Element(val, prono);

                                GridData.Children.Add(ToolInput, col, dong);                                

                                ToolInput.HorizontalOptions = LayoutOptions.Fill;

                                ToolInput.VerticalOptions = LayoutOptions.Fill;

                                col++;

                            }
                            else
                            {
                                if (a.Any())
                                {
                                    if (_dataConfirm.Count > 0)//Both have
                                    {
                                        var getDataConfirm = _dataConfirm.AsEnumerable().Where(r => r.Prono == prono).ToList();

                                        if (getDataConfirm.Any())
                                        {
                                            //neu co trong Local va co trong Server thi gan Qtysql = Qty cua Server

                                            foreach (var server in getDataConfirm)
                                            {
                                                foreach (var local in a)
                                                {
                                                    if (server.Prono == local.Prono)
                                                    {
                                                        if (server.Qty > local.Qty)
                                                        {
                                                            local.QtySql = local.Qty = server.Qty;

                                                            local.TempQty = server.Target - server.Qty;

                                                            local.Isfull = server.Target == server.Qty;

                                                            DB.DataLocal.Table.Update_Barcode(local);

                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else//dataconfirm is nothing
                                    {
                                        a[0].Isfull = false;

                                        a[0].QtySql = 0;

                                        a[0].TempQty = a[0].Target - a[0].Qty;

                                        DB.DataLocal.Table.Update_Barcode(a[0]);
                                    }

                                    Generate_Element(a);

                                    GridData.Children.Add(ToolInput, col, dong);

                                    ToolInput.HorizontalOptions = LayoutOptions.Fill;

                                    ToolInput.VerticalOptions = LayoutOptions.Fill;

                                    col++;
                                }
                                else //only dataconfirm
                                {
                                    var getDataConfirm = _dataConfirm.AsEnumerable().Where(r => r.Prono == prono).ToList();

                                    if (getDataConfirm.Any())
                                    {
                                        getDataConfirm[0].QtySql = getDataConfirm[0].Qty;

                                        Generate_Element(getDataConfirm);

                                        GridData.Children.Add(ToolInput, col, dong);

                                        DB.DataLocal.Table.Insert_Barcode(new Table_Barcode
                                        {
                                            ZLBH = getDataConfirm[0].ZLBH,

                                            Prono = getDataConfirm[0].Prono,

                                            UserDate = getDataConfirm[0].UserDate,

                                            Barcode = getDataConfirm[0].Barcode,

                                            BWBH = getDataConfirm[0].BWBH,

                                            CLBH = getDataConfirm[0].CLBH,

                                            UserID = getDataConfirm[0].UserID,

                                            XXCC = getDataConfirm[0].XXCC,

                                            Qty = getDataConfirm[0].Qty,

                                            QtySql = getDataConfirm[0].QtySql,

                                            Target = getDataConfirm[0].Target,

                                            TempQty = getDataConfirm[0].Target - getDataConfirm[0].Qty,

                                            Isfull = getDataConfirm[0].Target == getDataConfirm[0].Qty,

                                            isEdited = getDataConfirm[0].Target != getDataConfirm[0].Qty,

                                            YN = 1

                                        });

                                        ToolInput.HorizontalOptions = LayoutOptions.Fill;

                                        ToolInput.VerticalOptions = LayoutOptions.Fill;

                                        col++;
                                    }
                                    else //Add more 
                                    {
                                        Generate_Element(val, prono);

                                        GridData.Children.Add(ToolInput, col, dong);
                                        
                                        ToolInput.HorizontalOptions = LayoutOptions.Fill;

                                        ToolInput.VerticalOptions = LayoutOptions.Fill;

                                        col++;

                                    }
                                }
                            }

                            #endregion


                            break;
                        }

                    }
                }
                #region Group
                //Group
                dong++;

                if (dong > 0 && dong % 2 != 0)
                {
                    //string Gsz = "";

                    //int merge = 0;

                    //int cotMerge = 0;

                    //row = new RowDefinition
                    //{
                    //    Height =new  GridLength(1 , GridUnitType.Absolute)
                    //};

                    //GridData.RowDefinitions.Add(row);
                    //GridCompRY.RowDefinitions.Add(row);

                    //foreach (ViewModels.ACS_DataGroup asc in _dataGrp)
                    //{
                    //    if (ddbh == asc.Zlbh && bwhh == asc.Bwbh)
                    //    {
                    //        if (Gsz != asc.Gxxcc)
                    //        {
                    //            Gsz = asc.Gxxcc;

                    //            merge = Convert.ToInt32(asc.Gqty_merge);

                    //            int remain = asc.Gqty - asc.ActualQty;

                    //            var lblG = new Label
                    //            {
                    //                Text = string.Format("({0}){1}/{2}", Math.Abs(remain).ToString("D0"), asc.ActualQty.ToString("D0"), asc.Gqty.ToString("D0")),

                    //                HorizontalTextAlignment = TextAlignment.Center
                    //                ,
                    //                VerticalTextAlignment = TextAlignment.Center
                    //                ,
                    //                FontSize = 20,

                    //                Background = Class.Style.Normal,

                    //                Padding = new Thickness(0),

                    //                Margin = new Thickness(0)

                    //            };

                    //            GridData.Children.Add(lblG, cotMerge, dong);

                    //            Grid.SetColumnSpan(lblG, merge);

                    //            cotMerge += merge;
                    //        }
                    //    }
                    //}

                    //Frame frm = new Frame
                    //{
                    //    BackgroundColor = Class.Style.Normal,
                    //};
                    //GridData.Children.Add(frm, cotMerge, dong);

                    //Grid.SetColumnSpan(frm, GridData.ColumnDefinitions.Count-4);

                    //frm = new Frame()
                    //{
                    //    BackgroundColor = Class.Style.Normal
                    //};
                    //GridCompRY.Children.Add(frm, 0, dong);
                    //Grid.SetColumnSpan(frm, 2);
                }

                // dong++;
                #endregion

            }
            Alarm.Sound.Completed();
            AddQty_Compo();

        }
        void SetBinding()
        {
            foreach (Element obj in GridData.Children)
            {
                if (obj is UC_Input inp)
                {
                    var bind = inp.BindingContext as InputViewModel;

                    if (bind.Qtyed == bind.TotalQty)
                    {

                        bind.IsFul = true;

                        bind.BColor = Class.Style.Normal;

                        bind.SetOnOFF = false;

                        inp.BindingContext = bind;
                    }
                }
            }
        }
        void ResetTemp(string prono)
        {
            foreach (Element obj in GridData.Children)
            {
                if (obj is UC_Input inp)
                {
                    var bind = inp.BindingContext as InputViewModel;

                    var pronoUC = bind.Prono;


                    var arrUC = pronoUC.Split('_');

                    var arrPro = prono.Split('_');


                    if (arrPro[0] == arrUC[0] && arrPro[1] == arrUC[1] && arrPro[2] == arrUC[2] && arrPro[3] == arrUC[4])
                    {
                        if (bind.Temp != 0)
                        {
                            bind.SelectedIndex = bind.Temp = 0;
                            
                            inp.BindingContext = bind;
                        }

                    }
                }
            }
        }
        void SaveLocal(string prono)
        {
            int alr = 0;
            foreach (Element obj in GridData.Children)
            {
                if (obj is UC_Input inp)
                {
                    var bind = inp.BindingContext as InputViewModel;

                    var pronoUC = bind.Prono;

                    var arrUC = pronoUC.Split('_');

                    var arrPro = prono.Split('_');

                    if (arrPro.Length >= 4)
                    {
                        if (arrPro[0] == arrUC[0] && arrPro[1] == arrUC[1] && arrPro[2] == arrUC[2] &&
                            arrPro[3] == arrUC[4])
                        {
                            if (bind.Temp != 0)
                            {
                                bind.Qtyed = bind.Qtysql == 0 ?
                                    bind.Temp : bind.Qtyed + bind.Temp > bind.TotalQty ?
                                        bind.TotalQty : bind.Qtyed + bind.Temp;
                            }

                            bind.Temp = 0;

                            bind.SetOnOFF = true;

                            inp.BindingContext = bind;

                            bind.SelectedIndex = bind.Temp;

                            //Update Local
                            var a = new List<Table_Barcode>();

                            var hasVale = DB.DataLocal.Table.Exist_Prono_TableBarcode(pronoUC, out a);

                            if (hasVale)
                            {
                                a[0].Qty = bind.Qtyed;

                                a[0].TempQty = a[0].Target - bind.Qtyed;

                                if (DB.DataLocal.Table.Update_Barcode(a[0]) > 0)
                                {
                                    alr++;
                                }
                            }
                            else
                            {
                                var serverHas = _dataConfirm.Where
                                    (
                                        r => r.Barcode == _barcode
                                        && r.ZLBH == arrUC[1]
                                        && r.CLBH == arrUC[2]
                                        && r.XXCC == arrUC[3]
                                        && r.BWBH == arrUC[4]
                                    ).ToList();

                                var newrow = new DB.Table_Barcode();

                                if (!serverHas.Any())
                                {
                                    newrow.Barcode = _barcode;

                                    newrow.CLBH = arrUC[2];

                                    newrow.BWBH = arrUC[4];

                                    newrow.Qty = bind.Qtyed;

                                    newrow.QtySql = 0;

                                    newrow.Target = bind.TotalQty;

                                    newrow.TempQty = bind.TotalQty - bind.Qtyed;

                                    newrow.UserID = DB.StoreLocal.Instant.UserName;

                                    newrow.UserDate = DateTime.Now.Ticks;

                                    newrow.XXCC = arrUC[3];

                                    newrow.YN = 1;

                                    newrow.ZLBH = arrUC[1];

                                    newrow.Isfull = false;

                                    newrow.isEdited = true;

                                    newrow.Prono = pronoUC;
                                }
                                else
                                {
                                    newrow.Barcode = _barcode;

                                    newrow.CLBH = arrUC[2];

                                    newrow.BWBH = arrUC[4];

                                    newrow.Qty = serverHas[0].Qty;

                                    newrow.QtySql = serverHas[0].Qty;

                                    newrow.Target = bind.TotalQty;

                                    newrow.TempQty = serverHas[0].Target - serverHas[0].Qty;

                                    newrow.UserID = DB.StoreLocal.Instant.UserName;

                                    newrow.UserDate = DateTime.Now.Ticks;

                                    newrow.XXCC = arrUC[3];

                                    newrow.YN = 1;

                                    newrow.ZLBH = arrUC[1];

                                    newrow.Isfull = serverHas[0].Qty != serverHas[0].Target ? false : serverHas[0].Target != 0 ? true : false;

                                    newrow.isEdited = newrow.Isfull ? false : true;

                                    newrow.Prono = pronoUC;
                                }
                                alr = DB.DataLocal.Table.Insert_Barcode(newrow);
                            }
                        }
                    }
                }
            }
        }


        void AddQty_Compo()
        {
            Label lbl_comp = null;

            var pronoComp = "";
            var pronoSave= "";
            var comp = "";

            
            foreach (var element in GridCompRY.Children)
            {
                if (element is Grid subGrid)
                {
                    foreach (var e in subGrid.Children)
                    {
                        if (e is Button btnS)
                        {
                            if (btnS.AutomationId == pronoComp)
                            {
                                pronoSave = btnS.AutomationId;

                                break;
                            }
                        }
                      
                    }
                }
                else if (element is Label lblcomp)
                {
                    lbl_comp = lblcomp;

                   var comp1 = lblcomp.Text.Split('(');

                    comp = comp1[0];
                    pronoComp = element.AutomationId;
                 }

                if(pronoComp == pronoSave)
                    if (Network.Net.HasNet)
                    {
                        string sql = $"SELECT sum(convert(int, planqty)) sumTarget, sum(convert(int,ActualQty)) sumActual  FROM App_Cutting_Barcodes_Edit where Barcode = '{pronoComp.Split('_')[0]}' and ZLBH = '{pronoComp.Split('_')[1]}' and CLBH = '{pronoComp.Split('_')[2]}' and BWBH = '{pronoComp.Split('_')[3]}'";
                    
                        var table = DB.SQL.ConnectDB.Connection.FillDataTable(sql);


                        int actual = 0;
                        int tar = 0;
                        int localActual = 0;

                        if (table.Rows.Count > 0 && !string.IsNullOrEmpty(table.Rows[0]["sumactual"].ToString()))
                        {
                             actual = int.Parse(table.Rows[0]["sumactual"].ToString());
                             tar = int.Parse(table.Rows[0]["sumTarget"].ToString());                      
                        
                        }

                        var list = DB.DataLocal.Table.Data_Barcode(pronoComp.Split('_')[0], pronoComp.Split('_')[1], pronoComp.Split('_')[2], pronoComp.Split('_')[3]);

                        var tong = _data.Where(r => r.DDBH == pronoComp.Split('_')[1]  && r.CLBH == pronoComp.Split('_')[2] && r.BWBH == pronoComp.Split('_')[3] && r.ZLBH.Contains(pronoComp.Split('_')[4])).ToList();
                        
                        if (list.Any())
                        {
                            actual += localActual =list.Sum(r => r.Target - r.Qty);

                            actual = int.Parse(tong[0].Total.ToString()) - actual;
                        }

                        lbl_comp.Text = "";

                        lbl_comp.Text = string.Format("{0} ({1}/{2})", comp, actual, tong[0].Total);
                    }
            }
        }
                   
        string CheckSize(string sizeData)
        {
         
            string size = "";
         


            if (sizeData == "Size_01k") return size = "01.0K";
            if (sizeData == "Size_015k") return size = "01.5K"; 
            if (sizeData == "Size_02k") return size = "02.0K"; 
            if (sizeData == "Size_025k ") return size = "02.5k"; 
           if (sizeData == "Size_03k") return size = "03.0K"; 
            if (sizeData == "Size_035k") return size = "03.5K"; 
            if (sizeData == "Size_04k") return size = "04.0K"; 
           if (sizeData == "Size_045k") return size = "04.5K"; 
           if (sizeData == "Size_05k") return size = "05.0K"; 
            if (sizeData == "Size_055k") return size = "05.5K"; 
           if (sizeData == "Size_06k") return size = "06.0K"; 
            if (sizeData == "Size_065k") return size = "06.5K"; 
           if (sizeData == "Size_07k") return size = "07.0K"; 
            if (sizeData == "Size_075k") return size = "07.5K"; 
            if (sizeData == "Size_08k") return size = "08.0K"; 
           if (sizeData == "Size_085k") return size = "08.5K"; 
           if (sizeData == "Size_09k") return size = "09.0K"; 
           if (sizeData == "Size_095k") return size = "09.5K"; 
           if (sizeData == "Size_10k") return size = "10.0K"; 
           if (sizeData == "Size_105k") return size = "10.5K"; 
           if (sizeData == "Size_11k") return size = "11.0K"; 
            if (sizeData == "Size_115k") return size = "11.5K"; 
           if (sizeData == "Size_12k") return size = "12.0K"; 
           if (sizeData == "Size_125k") return size = "12.5K"; 
            if (sizeData == "Size_13k") return size = "13.0K"; 
           if (sizeData == "Size_135k") return size = "13.5K"; 

           if (sizeData == "Size_01") return size = "01"; 
           if (sizeData == "Size_015") return size = "01.5"; 
            if (sizeData == "Size_02") return size = "02"; 
           if (sizeData == "Size_025") return size = "02.5"; 
            if (sizeData == "Size_03") return size = "03"; 
            if (sizeData == "Size_035") return size = "03.5"; 
            if (sizeData == "Size_04") return size = "04"; 
            if (sizeData == "Size_045") return size = "04.5"; 
            if (sizeData == "Size_05") return  size = "05"; 
            if (sizeData == "Size_055") return size = "05.5"; 
           if (sizeData == "Size_06") return size = "06"; 
            if (sizeData == "Size_065") return size = "06.5"; 
            if (sizeData == "Size_07") return size = "07"; 
           if (sizeData == "Size_075") return size = "07.5"; 
            if (sizeData == "Size_08") return size = "08"; 
            if (sizeData == "Size_085") return size = "08.5"; 
            if (sizeData == "Size_09") return size = "09"; 
            if (sizeData == "Size_095") return size = "09.5"; 
           if (sizeData == "Size_10") return size = "10"; 
            if (sizeData == "Size_105") return size = "10.5"; 
           if (sizeData == "Size_11") return size = "11"; 
            if (sizeData == "Size_115") return size = "11.5"; 
            if (sizeData == "Size_12") return size = "12"; 
            if (sizeData == "Size_125") return size = "12.5"; 

           if (sizeData == "Size_13") return size = "13"; 
           if (sizeData == "Size_135") return size = "13.5"; 
            if (sizeData == "Size_14") return size = "14"; 
            if (sizeData == "Size_145") return size = "14.5"; 
            if (sizeData == "Size_15") return size = "15"; 
           if (sizeData == "ize_155") return size = "15.5"; 
           if (sizeData == "Size_16") return size = "16"; 
           if (sizeData == "Size_165") return size = "16.5"; 
           if (sizeData == "Size_17") return size = "17"; 
            if (sizeData == "Size_175") return size = "17.5"; 
            if (sizeData == "Size_18") return size = "18"; 
           if (sizeData == "Size_185") return size = "18.5"; 
            if (sizeData == "Size_19") return size = "19"; 
            if (sizeData == "Size_195") return size = "19.5"; 
            if (sizeData == "Size_20") return size = "20"; 
            if (sizeData == "Size_205") return size = "20.5"; 
       

            return size;
    
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
       
            if (ListControls.Any())
            {
                DataContext.IsBusy = true;
                await Task.Delay(10);

                var itemConfirm = DB.DataLocal.Table.Data_Barcode(_barcode);

                if (itemConfirm.Any())
                {
                    var ListCFM = new List<Table_Barcode>();

                    foreach (var UC in ListControls)
                    {
                        var pronoUC = UC.AutomationId;

                        foreach (var Local in itemConfirm)
                        {
                            var pronoLocal = Local.Prono;

                            if (pronoLocal == pronoUC)
                            {
                                ListCFM.Add(Local);
                            }
                        }
                    }

                    if (ListCFM.Count > 0)
                    {
                       
                        
                        var kq = await DB.DataLocal.Table.Save_CuttingEdit(ListCFM);

                        DataContext.IsBusy = false;

                        if (kq > 0)
                        {
                            SetBinding();

                            Alarm.Sound.Completed();

                            Services.DisplayToast.Show.Toast("Hoàn thành", Class.Style.Success);

                            await Navigation.PopAsync();

                        }



                    }
                    else Alarm.Sound.Error();

                }
                else Alarm.Sound.Error();
            }
        
        
        }


    }
    public class ConvertBinding : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var txt = value as Entry;
            var arr = txt.AutomationId.Split('_');
            double qty = System.Convert.ToDouble(txt.Text);
            double total = System.Convert.ToDouble(arr[3]);

            return (total - qty).ToString() + "/" + arr[3].ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
            
        }
    }


    public class AutoCuttingModel :BaseViewModel
    {
        public AutoCuttingModel(List<string> RYs)
        {
           
            //_ry = ry;
            _lstRY = RYs;

            Set_Data();

            Set_MainTitle();

            Set_ColumnTitle();

           // Set_DataConfirm();

            Set_DataGrp();

            CollapsedCmd = new Command(CollapseRunTime);
        }
        private void CollapseRunTime(object obj)
        {
            if (RowHeight.Value != 200)
                RowHeight = new GridLength(200);

            else RowHeight = new GridLength(0);
        }
        string _barcode = DB.StoreLocal.Instant.Barcode;

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
        public string CLBH { get; set; }

        ICommand _collapsedCmd;

        List<string> _lstRY;
        public ObservableCollection<ViewModels.AutoCuttingScan_GridTitle_Model> DataTitle { get; set; }
        public ObservableCollection<ViewModels.AutoCuttingScan_Data> Data { get; set; }
        public ObservableCollection<DB.Table_Barcode> Data_Confirm { get=> Set_DataConfirm();}
        public ObservableCollection<ViewModels.ACS_DataGroup> DataGroup { get; set; }

        public ICommand CollapsedCmd { get => _collapsedCmd; set { _collapsedCmd = value; OnPropertyChanged("CollapsedCmd"); } }

        private GridLength _rowHeight =new GridLength(200);

        public GridLength RowHeight
        {
            get { return _rowHeight; }
            set
            {
                _rowHeight = value;
                OnPropertyChanged(nameof(RowHeight));
            }
        }
            
        private void Set_DataGrp()
        {            //and CB2.ZLBH ='{_ry}'
            string sql = $"SELECT bc.zlbh,bc.clbh,bc.bwbh,bc.xxcc CC,CONVERT (INT, Isnull(GBb.gqty, 0)) ActualQty,CONVERT(INT, Isnull(GBc.gqty, 0))  RemainQty,Isnull(CB2.qty, 0)                 PlanQty,Isnull(CB2.gxxcc, '')              GXXCC,Isnull(GBa.gqty, 0)                GQty,Isnull(GBa_count.gqty, 1)          GQty_merge FROM(SELECT c1.barcode,c1.zlbh,c1.clbh,c1.xxcc,c1.gxxcc,c1.qty,c1.userid,c1.userdate,c1.yn,c1.bwbh,sum(case when c2.Action = '-' THEN isnull((-1) * c2.actualqty, 0) ELSE isnull(c2.actualqty, 0) END)  ActualQty FROM   cutting_barcodes2 c1 LEFT JOIN app_cutting_barcodes_edit c2 ON c2.barcode = c1.barcode AND c2.zlbh = c1.zlbh AND c2.clbh = c1.clbh AND c2.bwbh = c1.bwbh AND c1.xxcc = c2.xxcc WHERE  c1.barcode = '{_barcode}' GROUP BY c1.Barcode, c1.ZLBH, c1.CLBH, c1.XXCC, c1.GXXCC, c1.Qty, c1.USERID, c1.USERDATE, c1.YN, c1.BWBH) BC LEFT JOIN pdsch PD ON PD.zlbh = BC.zlbh LEFT JOIN ddzls DDS ON BC.xxcc = DDS.cc AND DDS.ddbh = PD.ry LEFT JOIN cutting_barcodes2 CB2 ON CB2.zlbh = BC.zlbh AND CB2.xxcc = BC.xxcc AND CB2.clbh = BC.clbh AND CB2.bwbh = BC.bwbh AND CB2.barcode = BC.barcode LEFT JOIN(SELECT barcode, zlbh,bwbh,gxxcc,Isnull(Sum(qty), 0) GQty FROM   cutting_barcodes2 WHERE  barcode = '{_barcode}' GROUP BY barcode, zlbh, bwbh, gxxcc) GBa ON GBa.barcode = CB2.barcode AND GBa.gxxcc = CB2.gxxcc AND GBa.zlbh = CB2.zlbh AND GBa.bwbh = CB2.bwbh LEFT JOIN(SELECT c1.barcode,  c1.zlbh, c1.bwbh, c1.gxxcc, Isnull(Sum(c2.actualqty), 0) GQty FROM   cutting_barcodes2 c1 LEFT JOIN app_cutting_barcodes_edit c2 ON c2.barcode = c1.barcode AND c2.zlbh = c1.zlbh AND c2.clbh = c1.clbh AND c2.bwbh = c1.bwbh AND c1.xxcc = c2.xxcc  WHERE c1.barcode = '{_barcode}' GROUP BY c1.barcode, c1.zlbh, c1.bwbh, gxxcc)GBb ON GBb.barcode = CB2.barcode AND GBb.gxxcc = CB2.gxxcc AND GBb.zlbh = CB2.zlbh AND GBb.bwbh = CB2.bwbh  LEFT JOIN(SELECT c1.barcode,  c1.zlbh, c1.bwbh, c1.gxxcc, Isnull(Sum(c1.qty), 0) -Isnull(Sum(c2.actualqty), 0)GQty FROM   cutting_barcodes2 c1 LEFT JOIN app_cutting_barcodes_edit c2 ON c2.barcode = c1.barcode AND c2.zlbh = c1.zlbh AND c2.clbh = c1.clbh AND c2.bwbh = c1.bwbh AND c1.xxcc = c2.xxcc WHERE c1.barcode = '{_barcode}' GROUP BY c1.barcode, c1.zlbh, c1.bwbh, gxxcc)GBc ON GBc.barcode = CB2.barcode AND GBc.gxxcc = CB2.gxxcc AND GBc.zlbh = CB2.zlbh AND GBc.bwbh = CB2.bwbh LEFT JOIN(SELECT barcode, zlbh, bwbh, gxxcc, Count(gxxcc) GQty FROM cutting_barcodes2 WHERE barcode = '{_barcode}' GROUP BY barcode, zlbh, gxxcc, bwbh)GBa_count ON GBa_count.barcode = CB2.barcode AND GBa_count.gxxcc = CB2.gxxcc AND GBa_count.zlbh = CB2.zlbh AND GBa_count.bwbh = CB2.bwbh WHERE BC.barcode = '{_barcode}'  ORDER BY bc.zlbh, bc.clbh, bc.bwbh, BC.xxcc";

            var da = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            var list = new ObservableCollection<ViewModels.ACS_DataGroup>();

            if (da.Rows.Count > 0)
            {
                foreach (System.Data.DataRow row in da.Rows)
                {

                    list.Add(new ViewModels.ACS_DataGroup
                    {
                        Bwbh = row["Bwbh"].ToString(),
                        CC = row["CC"].ToString(),
                        Clbh = row["CLBH"].ToString(),
                        Gqty = Math.Abs(Convert.ToInt32(row["GQty"])),
                        Gxxcc = row["GXXCC"].ToString(),
                        Qty = row["planQty"].ToString(),
                        Remain = row["RemainQty"].ToString(),
                        Zlbh = row["zlbh"].ToString(),
                        Gqty_merge = row["Gqty_merge"].ToString(),
                        ActualQty = Math.Abs(Convert.ToInt32(row["ActualQty"]))

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
                Services.Alert.Msg("Cảnh báo", "Không có mạng", "CANCEL"); return;
            }
            if (data2.Rows.Count > 0)
            {

                YWPM = data2.Rows[0]["Ywpm"].ToString();

                CLBH = data2.Rows[0]["CLBH"].ToString();
            }
            if (data.Rows.Count > 0)
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

            var lst = new ObservableCollection<ViewModels.AutoCuttingScan_GridTitle_Model>();

            if (!Network.Net.HasNet)
            {
                Services.Alert.Msg("Cảnh báo", "Không có mạng", "CANCEL"); return;
            }
            if (data.Rows.Count > 0)
            {
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    lst.Add(new ViewModels.AutoCuttingScan_GridTitle_Model
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
           

            var sql = $@" IF Object_id(N'TempDB..#Nyx') IS NOT NULL
  BEGIN
      DROP TABLE #nyx
  END

SELECT CB2.zlbh            DDBH,
       CB2.clbh,
       CB2.bwbh,
       bwzl.ywsm,
       CB2.xxcc            CC,
       CB2.gxxcc,
       CB2.qty,
       Isnull(GBa.gqty, 0) GQty,
       CB.dmqty, CB.RY TUA
INTO   #nyx
FROM   cutting_barcodes2 CB2
       LEFT JOIN cutting_barcodes CB
              ON CB.barcode = CB2.barcode
                 AND CB.bwbh = CB2.bwbh
                 AND CB.clbh = CB2.clbh
                 AND CB.zlbh = CB2.zlbh AND CB.RY=CB2.RY
       LEFT JOIN bwzl
              ON bwzl.bwdh = CB2.bwbh
       LEFT JOIN(SELECT barcode,
                        zlbh,
                        gxxcc,
                        bwbh,
                        clbh,
                        Sum(qty) GQty
                 FROM   cutting_barcodes2
                 GROUP  BY barcode,
                           zlbh,
                           gxxcc,
                           bwbh,
                           clbh)GBa
              ON GBa.barcode = CB2.barcode
                 AND GBa.gxxcc = CB2.gxxcc
                 AND GBa.zlbh = CB2.zlbh
                 AND GBa.bwbh = CB2.bwbh
                 AND GBa.clbh = CB2.clbh
WHERE  CB2.barcode = '{_barcode}'
 
SELECT A.ddbh,
       a.clbh,
       A.bwbh,
       A.ywsm,
       A.dmqty,
	   A.TUA,
       Sum(A.qty)                                         Total,
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '01.0K'), 0) ' 01.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '01.5K'), 0) ' 01.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '02.0K'), 0) ' 02.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '02.5K'), 0) ' 02.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '03.0K'), 0) ' 03.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '03.5K'), 0) ' 03.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '04.0K'), 0) ' 04.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '04.5K'), 0) ' 04.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '05.0K'), 0) ' 05.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '05.5K'), 0) ' 05.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '06.0K'), 0) ' 06.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '06.5K'), 0) ' 06.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '07.0K'), 0) ' 07.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '07.5K'), 0) ' 07.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '08.0K'), 0) ' 08.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '08.5K'), 0) ' 08.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '09.0K'), 0) ' 09.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '09.5K'), 0) ' 09.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '10.0K'), 0) ' 10.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '10.5K'), 0) ' 10.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '11.0K'), 0) ' 11.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '11.5K'), 0) ' 11.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '12.0K'), 0) ' 12.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '12.5K'), 0) ' 12.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '13.0K'), 0) ' 13.0K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '13.5K'), 0) ' 13.5K',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '01'), 0)    '01',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '01.5'), 0)  '01.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '02'), 0)    '02',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '02.5'), 0)  '02.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '03'), 0)    '03',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '03.5'), 0)  '03.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '04'), 0)    '04',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '04.5'), 0)  '04.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '05'), 0)    '05',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '05.5'), 0)  '05.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '06'), 0)    '06',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '06.5'), 0)  '06.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '07'), 0)    '07',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '07.5'), 0)  '07.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '08'), 0)    '08',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '08.5'), 0)  '08.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '09'), 0)    '09',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '09.5'), 0)  '09.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '10'), 0)    '10',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '10.5'), 0)  '10.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '11'), 0)    '11',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '11.5'), 0)  '11.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '12'), 0)    '12',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '12.5'), 0)  '12.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '13'), 0)    '13',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '13.5'), 0)  '13.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '14'), 0)    '14',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '14.5'), 0)  '14.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '15'), 0)    '15',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '15.5'), 0)  '15.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '16'), 0)    '16',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '16.5'), 0)  '16.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '17'), 0)    '17',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '17.5'), 0)  '17.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '18'), 0)    '18',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '18.5'), 0)  '18.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '19'), 0)    '19',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '19.5'), 0)  '19.5',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh
					  AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '20'), 0)    '20',
       Isnull((SELECT qty
               FROM   #nyx
               WHERE  ddbh = A.ddbh
                      AND clbh = A.clbh
                      AND bwbh = A.bwbh
					  AND TUA =A.TUA
                      AND Rtrim(Ltrim(cc)) = '20.5'), 0)  '20.5'
FROM   #nyx A
GROUP  BY A.ddbh,
          A.bwbh,
          A.ywsm,
          A.clbh,
          A.dmqty,
		  A.TUA
ORDER  BY A.bwbh,
          A.ywsm,
          A.ddbh";
            #endregion
            if (!Network.Net.HasNet)
            {
                Services.Alert.Msg("Cảnh báo", "Không có mạng", "CANCEL"); return;
            }

            var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            var ddbh = ta.Rows[0]["DDBH"].ToString();

            var tua = ta.Rows[0]["tua"].ToString();

            if (ddbh == tua)
            {
                sql = $"if object_id(N'TempDB..#Nyx') is not null	begin drop table #Nyx  end Select CB2.ZLBH DDBH, CB2.CLBH,CB2.BWBH, bwzl.ywsm,CB2.XXCC CC, CB2.GXXCC,CB2.Qty,isnull(GBa.GQty, 0) GQty,CB.DMQty into #Nyx from Cutting_Barcodes2 CB2 left join Cutting_Barcodes CB on CB.Barcode = CB2.Barcode and CB.BWBH = CB2.BWBH and CB.CLBH = CB2.CLBH and CB.ZLBH = CB2.ZLBH left join bwzl on bwzl.bwdh = CB2.BWBH Left join(select Barcode, ZLBH, GXXCC, BWBH, CLBH, sum(qty) GQty from Cutting_Barcodes2 group by Barcode, ZLBH, GXXCC, BWBH, CLBH )GBa on GBa.Barcode = CB2.Barcode and GBa.GXXCC = CB2.GXXCC and GBa.ZLBH = CB2.ZLBH and GBa.BWBH = CB2.BWBH and GBa.CLBH = CB2.CLBH where CB2.Barcode = '{_barcode}' Select A.DDBH,a.CLBH,A.BWBH, A.ywsm, A.DMQty, Sum(A.Qty) Total, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '01.0K'),0) ' 01.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '01.5K'),0) ' 01.5K'									, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '02.0K'),0) ' 02.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '02.5K'),0) ' 02.5K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '03.0K'),0) ' 03.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '03.5K'),0) ' 03.5K'		, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '04.0K'),0) ' 04.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '04.5K'),0) ' 04.5K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '05.0K'),0) ' 05.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '05.5K'),0) ' 05.5K'	, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '06.0K'),0) ' 06.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '06.5K'),0) ' 06.5K'											, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '07.0K'),0) ' 07.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '07.5K'),0) ' 07.5K'												, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '08.0K'),0) ' 08.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '08.5K'),0) ' 08.5K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '09.0K'),0) ' 09.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '09.5K'),0) ' 09.5K'									, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '10.0K'),0) ' 10.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '10.5K'),0) ' 10.5K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '11.0K'),0) ' 11.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '11.5K'),0) ' 11.5K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '12.0K'),0) ' 12.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '12.5K'),0) ' 12.5K'	, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '13.0K'),0) ' 13.0K', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '13.5K'),0) ' 13.5K'			, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '01'),0) '01', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '01.5'),0) '01.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '02'),0) '02', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '02.5'),0) '02.5'	, isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '03'),0) '03', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '03.5'),0) '03.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '04'),0) '04', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '04.5'),0) '04.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '05'),0) '05', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '05.5'),0) '05.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '06'),0) '06', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '06.5'),0) '06.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '07'),0) '07', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '07.5'),0) '07.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '08'),0) '08', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '08.5'),0) '08.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '09'),0) '09', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '09.5'),0) '09.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '10'),0) '10', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '10.5'),0) '10.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '11'),0) '11', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '11.5'),0) '11.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '12'),0) '12', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '12.5'),0) '12.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '13'),0) '13', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '13.5'),0) '13.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '14'),0) '14', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '14.5'),0) '14.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '15'),0) '15', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '15.5'),0) '15.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '16'),0) '16', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '16.5'),0) '16.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '17'),0) '17', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '17.5'),0) '17.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '18'),0) '18', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '18.5'),0) '18.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '19'),0) '19', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '19.5'),0) '19.5', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '20'),0) '20', isnull((Select Qty from #Nyx where DDBH = A.DDBH and CLBH=A.CLBH and BWBH=A.BWBH and Rtrim(Ltrim(CC)) = '20.5'),0) '20.5'	from #Nyx A group by A.DDBH, A.BWBH, A.ywsm, A.CLBH, A.DMQty Order by A.BWBH, A.ywsm, A.DDBH ";

                var ta2 = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                sql = $"select DISTINCT c.XXCC sizes from Cutting_Barcodes2 c where Barcode = '{_barcode}' group by xxcC order by sizes";

                var taSize = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                var list = new ObservableCollection<ViewModels.AutoCuttingScan_Data>();

                if (ta.Rows.Count > 0)
                {

                    foreach (System.Data.DataRow DRow in ta.Rows)
                    {

                        if (_lstRY.Contains(string.Format("{0}_{1}", DRow["DDBH"].ToString(), DRow["bwbh"].ToString())))
                        {

                            list.Add(new ViewModels.AutoCuttingScan_Data
                            {
                                DDBH = DRow["DDBH"].ToString(),
                                ZLBH = DRow["DDBH"].ToString(),
                                DMQty = double.TryParse(DRow["DMQty"].ToString(), out double dmqty) ? dmqty : 0,
                                Total = double.TryParse(DRow["Total"].ToString(), out double total) ? total : 0,
                                CLBH = DRow["CLBH"].ToString(),
                                Size_01k = ProductSize(" 01.0K", taSize) ? DRow[" 01.0K"].ToString() : "",
                                Size_015k = ProductSize(" 01.5K", taSize) ? DRow[" 01.5K"].ToString() : "",
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



                    Data = list;
                }
            }
            else
            {
                sql = $"select DISTINCT c.XXCC sizes from Cutting_Barcodes2 c where Barcode = '{_barcode}' group by xxcC order by sizes";

                var taSize = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                var list = new ObservableCollection<ViewModels.AutoCuttingScan_Data>();

                if (ta.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow DRow in ta.Rows)
                    {

                        if (_lstRY.Contains(string.Format("{0}_{1}", DRow["DDBH"].ToString(), DRow["bwbh"].ToString())))
                        {

                            list.Add(new ViewModels.AutoCuttingScan_Data
                            {
                                DDBH = DRow["DDBH"].ToString(),
                                ZLBH = DRow["tua"].ToString(),
                                DMQty = double.TryParse(DRow["DMQty"].ToString(), out double dmqty) ? dmqty : 0,
                                Total = double.TryParse(DRow["Total"].ToString(), out double total) ? total : 0,
                                CLBH = DRow["CLBH"].ToString(),
                                Size_01k = ProductSize(" 01.0K", taSize) ? DRow[" 01.0K"].ToString() : "",
                                Size_015k = ProductSize(" 01.5K", taSize) ? DRow[" 01.5K"].ToString() : "",
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



                    Data = list;
                }
            }
        }
        private bool ProductSize(string size, System.Data.DataTable tablesize)
        {
            var sx = false;
            for (int i = 0; i < tablesize.Rows.Count; i++)
            {
                if (tablesize.Rows[i][0].ToString().Trim() == size.Trim())
                {
                    sx = true; break;
                }
            }
            return sx;
        }
        ObservableCollection<DB.Table_Barcode> Set_DataConfirm()
        {
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

                        Prono = row["prono"].ToString(),
                    });



                }
            }
            return list;
        }
    }   

}

