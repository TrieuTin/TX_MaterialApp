using MaterialTracking.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.PageViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryDetail_PageView : ContentPage
    {
        Grid Title2 = new Grid();

        List<InventoryPageDetail_Type> _details;

        InventoryPageGeneral_Type _master;

        int _howmany_Done = 0;

        //InventoryPageGeneral_Type item_Inventory;
        public InventoryDetail_PageView(InventoryPageGeneral_Type item , List<InventoryPageGeneral_Type>ListMater)
        {
            InitializeComponent();             

            var modeldata = ListMater;

           // item_Inventory = masterData;

            var data = ListMater.Where(r => r.Barcode == item.Barcode && r.ZLBH == item.ZLBH && r.CLBH == item.CLBH && item.BWBH == r.BWBH && r.ModelName == item.ModelName && r.Comp ==item.Comp).ToList();

            if (data.Any())
            {               
                int totalComp = ListMater[0].Components.Split(';').Length;

                ListMater[0].Components = totalComp.ToString();

                ListMater[0].OnGoing = ListMater[0].OnGoing.Split(';').Length.ToString();

                _master = item;

                _master.BWBH = data[0].BWBH;

                Load(item.ZLBH,item.Barcode,item.BWBH,item.CLBH);

                Stack_Detail.Children.Add(Title2);

                ListMater[0].Done = _howmany_Done.ToString();

                ItemSource = data;

            }
        }
        List<InventoryPageGeneral_Type> ItemSource
        {
            get => (List<InventoryPageGeneral_Type>) LvData.ItemsSource;
            set
            {
                LvData.ItemsSource = value;                
            }


        }

        void SetRowCol_SubContent2(int TotalSize, int TotalComp)
        {
            Title2.ColumnSpacing = 2;
            Title2.RowSpacing = 2;
            for (int i = 0; i < TotalComp; i++)
            {
                if (i == 0)
                {

                    var G_ColComp = new ColumnDefinition
                    {
                        Width = new GridLength(110, GridUnitType.Absolute)
                    };
                    Title2.ColumnDefinitions.Add(G_ColComp);
                }
                var G_Row = new RowDefinition
                {
                    Height = GridLength.Auto
                };

                Title2.RowDefinitions.Add(G_Row);
            }
            for (int i = 0; i < TotalSize; i++)
            {
                var G_ColSize = new ColumnDefinition
                {
                    Width = GridLength.Star
                };
                Title2.ColumnDefinitions.Add(G_ColSize);
            }

            var G_Col = new ColumnDefinition
            {
                Width = new GridLength(80, GridUnitType.Absolute)
            };
            Title2.ColumnDefinitions.Add(G_Col);

        }
        void Load(string zlbh,string barcode,string bwbh,string clbh)
        {            
            SetTitle2(zlbh,barcode);


            int rowCount = 2;

            var listzlbh = _details.Where(z => z.Zlbh == zlbh).ToList();

            var grpComp = listzlbh.AsEnumerable().GroupBy(c => c.Component).ToList();

            var grpSize = listzlbh.GroupBy(s => s.Size).ToList();

            List<string> SortSize = new List<string>();


            for (int i = 0; i < grpSize.Count; i++)
            {
                if (!SortSize.Contains(grpSize[i].Key)) 
                    SortSize.Add(grpSize[i].Key);

            }

            SortSize.Sort();

            _howmany_Done = grpComp.Count;
            //----------------------------------------

            SetRowCol_SubContent2(SortSize.Count, grpComp.Count);


            foreach (var comp in grpComp)
            {
                int columnCount = 1;

                var color = Class.Style.Success;
              
                foreach (var size in SortSize)
                {
                  
                    var qty = listzlbh.AsEnumerable().Where(sc => sc.Size.Contains(size) && sc.Component.Contains(comp.Key)).ToList();
                  
                    var qtyedandtarget = qty[0].Qty.Split('/');
                 
                    if (qtyedandtarget[0] != qtyedandtarget[1])
                    {
                        color = Class.Style.Red;

                        _howmany_Done--;

                        break;
                    }
                }

                 Frame frameDetail1 = new Frame
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),


                    BackgroundColor = color,

                    Content = new Label { HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment= TextAlignment.Center, Text = comp.Key, TextColor = Color.White, FontSize = 20, Padding = new Thickness(0) }
                };

                Title2.Children.Add(frameDetail1, 0, rowCount);



                foreach (var size in SortSize)
                {
                    var qty = listzlbh.AsEnumerable().Where(sc => sc.Size.Contains(size) && sc.Component.Contains(comp.Key)).ToList();

                    var qtyedandtarget = qty[0].Qty.Split('/');

                    Frame framedetail = new Frame
                    {
                        Margin = new Thickness(0),
                        Padding = new Thickness(0),
                        BackgroundColor = qtyedandtarget[0] != qtyedandtarget[1]? Class.Style.Red:Class.Style.Success ,

                        Content = new Label { HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text = qtyedandtarget[0], TextColor = Color.White, FontSize = 20, Padding = new Thickness(0)}
                    };

                    Title2.Children.Add(framedetail, columnCount, rowCount);

                    columnCount++;
                }

                bool enableBtn = false;

                if (DB.StoreLocal.Instant.Depname == DB.Departments.NoSew)
                {
                    string checksql = $"SELECT Orderid FROM App_Material_Orders where RY ='{zlbh}' and Component='{comp.Key}' and BWBH='{bwbh}' and CLBH='{clbh}'";

                    var ta = DB.SQL.ConnectDB.Connection.FillDataTable(checksql);

                    if (ta.Rows.Count ==0)
                    {
                        enableBtn = true;
                    }
                    else
                    {
                        checksql = $"SELECT * FROM  App_Material_Process where orderid='{ta.Rows[0][0].ToString()}'";

                        ta = DB.SQL.ConnectDB.Connection.FillDataTable(checksql);

                        if (ta.Rows.Count > 0)
                        {
                            var status = ta.Rows[0]["Status"].ToString();
                            
                            if (status == "False")
                                enableBtn = true;
                        }
                    }
                }
                else
                {
                    string checksql = $"SELECT Orderid FROM App_Material_Orders where RY ='{zlbh}' and Component='{comp.Key}' and BWBH='{bwbh}' and CLBH='{clbh}'";

                    var ta = DB.SQL.ConnectDB.Connection.FillDataTable(checksql);

                    if (ta.Rows.Count == 0)
                    {
                        enableBtn = false;
                    }
                    else
                    {
                        checksql = $"SELECT * FROM  App_Material_Process where orderid='{ta.Rows[0][0].ToString()}'";

                        ta = DB.SQL.ConnectDB.Connection.FillDataTable(checksql);

                        if (ta.Rows.Count == 0)
                        {
                            enableBtn = true;
                        }
                    }
                }

                var btn = new Button { BackgroundColor = Class.Style.Primary, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Text = SysFont.Solid.Cart_Plus, FontFamily = "aws900", FontSize = 24, Padding = new Thickness(0), IsEnabled = enableBtn };

                btn.Clicked +=async (s, e) =>
                {
                    //Services.Alert.PopupNotification("Thông Báo!", "Tính năng đang hoàn thành", "OK", Class.Style.Notification);
                   
                    if (DB.StoreLocal.Instant.Depname == DB.Departments.NoSew)
                    {
                        
                    }


                    var _btn = (Button)s;

                    List<Model_DataGrid> tempItem = new List<Model_DataGrid>();

                    var m = new Model_Mater();

                    var kq = 0;
                   

                   
                    if (DB.StoreLocal.Instant.Depname == DB.Departments.NoSew) //NS
                    {
                       var result = await Services.Alert.PopupYesNo("Hỏi", "Bạn gọi hàng?", "Đúng", "Không");
                        if ((result as string) == "Đúng")
                        {

                            tempItem.Add(new Model_DataGrid
                            {
                                Art = _master.Art,
                                Bwbh = _master.BWBH,
                                Clbh = _master.CLBH,
                                Comp = comp.Key,
                                ModelName = _master.ModelName,
                                RY = _master.ZLBH,
                                Sel = true,
                                TarQty = string.Format("{0}/{1}", 0, _master.Actual),
                                OrderID = string.Format("{0}{1}", DB.StoreLocal.Instant.CurrentDep.ToUpper(),     DateTime.Now.Ticks.ToString()),
                                

                            });

                            kq = m.Insert_Order_table(tempItem);

                            if (kq != 0) { Services.DisplayToast.Show.Toast(string.Format("Đơn xác nhận {0} chi tiết", kq), Color.DarkGreen); } else { Services.DisplayToast.Show.Toast("Đã đặt hàng trước đó", Class.Style.Error);  }

                            _btn.IsEnabled = false;

                        }
                       
                    }
                    else //AC
                    {
                        var result = await Services.Alert.PopupYesNo("Hỏi", "Đã chuẩn bị hàng?", "Đúng", "Không");
                        if ((result as string) == "Đúng")
                        {
                            tempItem.Add(new Model_DataGrid
                            {
                                Art = _master.Art,
                                Bwbh = _master.BWBH,
                                Clbh = _master.CLBH,
                                Comp = comp.Key,
                                ModelName = _master.ModelName,
                                RY = _master.ZLBH,
                                Sel = true,

                                //ProdDate=

                            });

                            kq = m.Insert_Process_table(tempItem);

                            _btn.IsEnabled = false;

                            if (kq != 0) { Services.DisplayToast.Show.Toast(string.Format("Hàng chuẩn bị {0} chi tiết", kq), Color.DarkGreen);  } else { Services.DisplayToast.Show.Toast("Đã chuẩn bị", Class.Style.Error);  }
                        }
                        
                    }


                };
                Frame framedetai3 = new Frame
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                    BackgroundColor = Class.Style.Red,
                    Content = btn
                };

                Title2.Children.Add(framedetai3, columnCount, rowCount);

                rowCount++;

            }
        }
        void SetTitle2(string ZLBH, string Barcode)
        {
            string sql = $"SELECT zlbh, bwbh, bwzl.ywsm, XXcc,prono, convert(varchar, convert(int,actualqty)) +'/'+ convert(varchar,convert(int, Planqty)) Qty,[action] from App_Cutting_Barcodes_Edit app inner join bwzl on app .BWBH =bwzl.bwdh  where  zlbh = '{ZLBH}'  and convert(int, PlanQty) !=0 and convert(int,ActualQty)!=0 and Barcode='{Barcode}' order by XXCC";

            if(DB.StoreLocal.Instant.Myfac == DB.MyFactory.LYV)
            {
                sql = $@"SELECT zlbh,
       bwbh,
       bwzl.ywsm,
       GXXCC,
       prono,
       CONVERT(VARCHAR, CONVERT(INT, actualqty))
       + '/' + CONVERT(VARCHAR, CONVERT(INT, planqty)) Qty
      
FROM   App_Cutting_Barcodes_Groups_Edit app
       INNER JOIN bwzl
               ON app .bwbh = bwzl.bwdh
WHERE  zlbh = '{_master.ZLBH}'
       AND CONVERT(INT, planqty) != 0
       AND CONVERT(INT, actualqty) != 0
ORDER  BY gxxcc  ";
            }

            var title = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if (title.Rows.Count == 0) return;
            
            _details = new List<InventoryPageDetail_Type>();
            
            foreach (System.Data.DataRow Detail in title.Rows)
            {
                var targetArr = Detail["Qty"].ToString().Split('/');               

                if (int.Parse(targetArr[1]) > 0)
                {
                    var pageD = new InventoryPageDetail_Type();

                    pageD.Component = Detail["ywsm"].ToString();

                    pageD.Zlbh = Detail["ZLBH"].ToString();

                    pageD.Bwbh = Detail["bwbh"].ToString(); ;
                       
                     pageD.Qty = Detail["Qty"].ToString();
                        
                     pageD.Size =DB.StoreLocal.Instant.Myfac != DB.MyFactory.LYV ? Detail["xxcc"].ToString() : Detail["Gxxcc"].ToString();
                        
                     pageD.Prono = Detail["Prono"].ToString();

                    _details.Add(pageD);
                }


            }

            //   stackCommand.Children.Add(btn);

          
            //Cot dau tien size
           
            Title2.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(50, GridUnitType.Absolute)
            });

            Title2.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(230, GridUnitType.Absolute)
            });

          
            var f = new Frame
            {
                BackgroundColor = Color.FromHex("#93B1A6"),
                HasShadow = true,
                HeightRequest = 27,
                Margin = new Thickness(0),
                Padding = new Thickness(0)
                ,
                Content = new Label
                {
                    Text = "Comp/Size",
                    FontSize = 24,
                    FontAttributes = FontAttributes.Bold,
                    Padding = new Thickness(0, 0, 0, 0)
                ,
                    TextColor = Color.Black,
                    HorizontalTextAlignment = TextAlignment.Start
                    ,VerticalTextAlignment= TextAlignment.Center

                }
            };

            
            Title2.Children.Add(f, 0, 0);

           



            var listzlbh = _details.Where(z => z.Zlbh == ZLBH).ToList();

            var grpComp = listzlbh.AsEnumerable().GroupBy(c => c.Component).ToList();

            var grpSize = listzlbh.GroupBy(s => s.Size).ToList();

            List<string> SortSize = new List<string>();


            for (int i = 0; i < grpSize.Count; i++)
            {
                if (!SortSize.Contains(grpSize[i].Key)) SortSize.Add(grpSize[i].Key);

            }
            SortSize.Sort();


            for (int i = 0; i < SortSize.Count; i++)
            {
               

                Title2.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                });

                //Them comp vao menu chinh
                f = new Frame
                {
                    BackgroundColor = Color.FromHex("#93B1A6"),
                    HasShadow = true,
                    HeightRequest = 27,
                    Margin = new Thickness(0),
                    Padding = new Thickness(0)
                };

                f.Content =  new Label { HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text = SortSize[i], FontAttributes = FontAttributes.Bold, TextColor = Color.Black, FontSize = 24, Padding = new Thickness(0, 0, 0, 0) };
                Title2.Children.Add(f, i + 1, 0);

            }
         
            Title2.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(80, GridUnitType.Absolute)
            });
            f = new Frame
            {
                BackgroundColor = Color.FromHex("#93B1A6"),
                HasShadow = true
                ,
                HeightRequest = 27,
                Margin = new Thickness(0),
                Padding = new Thickness(0)
            };

            f.Content = new Label 
            { 
                HorizontalTextAlignment = TextAlignment.Center, 
                VerticalTextAlignment= TextAlignment.Center, 
                Text = "Action", 
                TextColor = Color.Black,
                FontSize = 20, 
                Padding = new Thickness(0, 0, 0, 0) 
            };

            //Them cac nut Action trong sub menu
            Title2.Children.Add(f, SortSize.Count + 1, 0);
            Grid.SetRowSpan(f, 2);


            //------------------Target


            Title2.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50, GridUnitType.Absolute) });

           

           
            f = new Frame
            {
                BackgroundColor = Color.FromHex("#FFD93D"),
                HasShadow = true,
                HeightRequest = 27,
                Margin = new Thickness(0),
                Padding = new Thickness(0)
                ,Content = new Label
                {
                    Text = "Target",
                    FontSize = 24,
                    Padding = new Thickness(0, 0, 0, 0),
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.Black,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center
                }
            };

            
            Title2.Children.Add(f, 0, 1);

            for (int i = 0; i < SortSize.Count; i++)
            {
                var List_target = listzlbh.Where(r => r.Size == SortSize[i]).ToList();

                var target = List_target[0].Qty.Split('/');

                f = new Frame
                {
                    BackgroundColor = Color.FromHex("#FFD93D"),
                    HasShadow = true,
                    HeightRequest = 27,
                    Margin = new Thickness(0),
                    Padding = new Thickness(0)
                };

                f.Content = new Label {FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center,Text = target[1], TextColor = Color.Black, FontSize = 24, Padding = new Thickness(0, 0, 0, 0) };
                Title2.Children.Add(f, i + 1, 1);
            }
        }

        private void LvData_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {


            //var firstVisibleItem = e.Item as InventoryPageGeneral_Type;

            //Services.DisplayToast.Show.Toast(string.Format("{0}{1}", e.ItemIndex, firstVisibleItem.ZLBH), Class.Style.Normal);


            //if (firstVisibleItem != null && firstVisibleItem.ZLBH == _master.ZLBH && firstVisibleItem.Comp == _master.Comp)
            //{

            //    LvData.ScrollTo(firstVisibleItem, ScrollToPosition.Start, true);


            //}

            // Lấy item đang xuất hiện
          
        }
    }

   
}