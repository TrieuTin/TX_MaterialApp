using MaterialTracking.Class;
using MaterialTracking.DB;
using MaterialTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.PageViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetValuePage : ContentPage
    {
        Model_DataGrid binding;
        string wh = "";
        string Destination = DB.StoreLocal.Instant.CurrentDep.ToUpper();
        List<(string XXCC, int Qty, int Target)> data;
        List<(string orderid, string xxcc, int pairs)> detail;
        List<Entry> lstEntry = new List<Entry>();

        Label lbl_SumEntry = new Label();

        List<(string XXCC, int Qty)> lstOriginal_Qty = new List<(string XXCC, int Qty)>();

      //  Grid gridData;

        public SetValuePage(Model_DataGrid model ,string Warehouse)
        {
            InitializeComponent();

            binding = model;

            lbl_Comp.Text = binding.Comp;

            lbl_ry.Text = binding.RY;

            wh = Warehouse;
            lbl_Source.Text = Source;

            Load();

            lbl_Target.Text = binding.TarQty;

         
        }
        
        string Source
        {
            get
            {
                switch (wh)
                {
                    case "NS":
                        return ConvertLang.Convert.Translate_LYM("Ép Nóng", "ချုပ်ကြောင်းမပါသောဖြစ်စဥ်");


                    case "AC":
                        return ConvertLang.Convert.Translate_LYM("Chặt Tự Động", "အော်တို ဖြတ်စက်");


                    case "Hf":
                        return "Ép Cao Tầng";
                        
                    case "PR":
                       return  "In Lụa";
                        
                    case "EM":
                       return  "Thêu";
                        
                    case "ST":
                        return ConvertLang.Convert.Translate_LYM("May", "ချုပ်ခြင်း");
                    default:
                        return "Gia Công Ngoài";
                        
                }
            }
        }

        void Load()
        {
            var g = new Grid
            {
                ColumnSpacing = 2,
                RowSpacing = 2,
            };

             data = Data;
             detail = OrderDetail;

            if (data.Count > 0)
            {
                var isAlreadyTitle = false;
                for (int i = 0; i < data.Count + 1; i++)
                {
                    var w = 110;                                                                                                                   

                    g.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(w, GridUnitType.Absolute)
                    });

                    if (isAlreadyTitle == false)
                    {
                        for (int colRight = 0; colRight < 2; colRight++)
                        {
                            var G_Row = new RowDefinition
                            {
                                Height = GridLength.Auto
                            };
                            g.RowDefinitions.Add(G_Row);

                            if (colRight == 0)
                            {
                                var frm_TitleSize = new Frame
                                {
                                    Background = Class.Style.ColdKidsSky.BlueSea,
                                    Margin = new Thickness(0),
                                    Padding = new Thickness(0),
                                    Content = new Label { HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text = "Size", TextColor = Color.White, FontSize = 20, Padding = new Thickness(0) }
                                };
                                g.Children.Add(frm_TitleSize, 0, 0);
                            }

                            if (colRight == 1)
                            {
                                var frm_TitleQty = new Frame
                                {
                                    Background = Class.Style.ColdKidsSky.BlueSea,
                                    Margin = new Thickness(0),
                                    Padding = new Thickness(0),
                                    Content = new Label { HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text =ConvertLang.Convert.Translate_LYM("SL Tổng", "စုစုပေါင်း"),TextColor = Color.White, FontSize = 20, Padding = new Thickness(0) }
                                };
                                g.Children.Add(frm_TitleQty, 0, 1);


                                if (detail.Count == 0)
                                {
                                    G_Row = new RowDefinition
                                    {
                                        Height = GridLength.Auto
                                    };
                                    g.RowDefinitions.Add(G_Row);

                                    var frm_Date = new Frame
                                    {
                                        Background = Class.Style.ColdKidsSky.BlueSea,
                                        Margin = new Thickness(0),
                                        Padding = new Thickness(0),
                                        Content = new Label { HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text = DateTime.Now.ToString("yyyy-MM-dd"), TextColor = Color.White, FontSize = 20, Padding = new Thickness(0) }
                                    };
                                    g.Children.Add(frm_Date, 0, g.RowDefinitions.Count - 1);
                                }                               
                            }
                        }
                        isAlreadyTitle = true;
                    }
                    if (i < data.Count)
                    {
                       


                        var frm_Size = new Frame
                        {
                            Background = Class.Style.ColdKidsSky.BlueSea,
                            Margin = new Thickness(0),
                            Padding = new Thickness(0),
                            Content = new Label { FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text = data[i].XXCC, TextColor = Color.White, FontSize = 20, Padding = new Thickness(0) }
                        };

                        g.Children.Add(frm_Size, i + 1, 0);

                        var frm_Pairs = new Frame
                        {
                            Background = Class.Style.ColdKidsSky.BlueSea,
                            Margin = new Thickness(0),
                            Padding = new Thickness(0),
                            Content = new Label { Margin = new Thickness(5, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment = TextAlignment.Center, Text = data[i].Target.ToString(), TextColor = Color.White, FontSize = 20, Padding = new Thickness(0) }
                        };

                        (string, int) l;

                        l.Item1 = data[i].XXCC;

                        l.Item2 = data[i].Target;

                        lstOriginal_Qty.Add(l);

                        g.Children.Add(frm_Pairs, i + 1, 1);

                        //Them qty 
                        if (detail.Count == 0) //chua them lan nao
                        {
                            Entry entry = new Entry
                            {
                                Margin = new Thickness(5, 0, 0, 0),
                                HorizontalTextAlignment = TextAlignment.Start,
                                VerticalTextAlignment = TextAlignment.Center,
                                Text = data[i].Qty.ToString(),
                                TextColor = Color.Black,
                                FontSize = 20,
                                Keyboard = Keyboard.Numeric,
                                AutomationId = data[i].XXCC
                            };

                            entry.TextChanged += (s, e) =>
                            {
                                var obj = s as Entry;

                                if (int.TryParse(obj.Text, out int result))
                                {
                                    var tar = lstOriginal_Qty.Where(r => r.XXCC == obj.AutomationId).ToList();

                                    obj.Text = result >= tar[0].Qty ? tar[0].Qty.ToString() : result.ToString();
                                }
                                else obj.Text = "0";

                                var sum = lstEntry.Sum(r => int.Parse(!int.TryParse(r.Text, out int re) ? "0" : r.Text));

                                //lbl_Target.Text = string.Format("{0}/{1}", sum, binding.TarQty.Split('/')[1]);

                                lbl_SumEntry.Text = lstEntry.Sum(r => int.Parse(!int.TryParse(r.Text, out int re) ? "0" : r.Text)).ToString();

                            };
                            
                            

                            var frm_PairsInputed = new Frame
                            {

                                Background = Class.Style.ColdKidsSky.Cream,
                                Margin = new Thickness(0),
                                Padding = new Thickness(0),
                                Content = entry
                            };

                            lstEntry.Add(entry);

                            g.Children.Add(frm_PairsInputed, i + 1, g.RowDefinitions.Count - 1);

                            if (i + 1 == data.Count)
                            {
                                var frm_Total = new Frame
                                {
                                    Background = Class.Style.ColdKidsSky.BlueSea,
                                    Margin = new Thickness(0),
                                    Padding = new Thickness(0),
                                    Content = lbl_SumEntry = new Label { FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text = lstEntry.Sum(r => int.Parse(!int.TryParse(r.Text, out int re) ? "0" : r.Text)).ToString(), TextColor = Color.Wheat, FontSize = 20, Padding = new Thickness(0) }
                                };
                                g.Children.Add(frm_Total, data.Count + 1, g.RowDefinitions.Count - 1);
                            }

                        }
                        else //da them roi
                        {
                            

                            var cot = 0;
                            var dong = 0;

                            var distinctOrderid = detail
                                 .Select(ord => ord.orderid)
                                 .Distinct()
                                 .ToList();
                            dong = g.RowDefinitions.Count;
                            for (int h = 0; h < distinctOrderid.Count; h++)
                            {                                
                                var date = new DateTime(long.Parse(distinctOrderid[h].Substring(2)));

                                var frms = new Frame
                                {
                                    Background = Class.Style.ColdKidsSky.BlueSea,
                                    Margin = new Thickness(0),
                                    Padding = new Thickness(0),
                                    Content = new Label { Margin = new Thickness(5, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment = TextAlignment.Center, Text = date.ToString("yyyy-MM-dd"), TextColor = Color.White, FontSize = 18, Padding = new Thickness(0) }
                                };


                                g.Children.Add(frms, cot , dong );


                                foreach (var d in data)
                                {
                                   

                                    var inputed = detail.Where(r => r.orderid == distinctOrderid[h] && d.XXCC == r.xxcc).ToList();

                                    frms = new Frame
                                    {
                                        Background = Class.Style.ColdKidsSky.BlueSea,
                                        Margin = new Thickness(0),
                                        Padding = new Thickness(0),
                                        Content = new Label { Margin = new Thickness(5, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment = TextAlignment.Center, Text = inputed.Count != 0 ? inputed[0].pairs.ToString() : "0", TextColor = Color.White, FontSize = 20, Padding = new Thickness(0) }
                                    };
                                    
                                    g.Children.Add(frms, cot+1, dong);


                                    if (i + 1 == data.Count)
                                    {
                                        var sumdetail = detail.Where(r => r.orderid == distinctOrderid[h]).Sum(r => r.pairs);

                                        var frm_Total = new Frame
                                        {
                                            Background = Class.Style.ColdKidsSky.BlueSea,
                                            Margin = new Thickness(0),
                                            Padding = new Thickness(0),
                                            Content = lbl_SumEntry = new Label { FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text = sumdetail.ToString(), TextColor = Color.Wheat, FontSize = 20, Padding = new Thickness(0) }
                                        };
                                        g.Children.Add(frm_Total, data.Count + 1, dong);
                                    }

                                    cot++;
                                    
                                }

                                cot = 0;
                                dong++;
                            }
                            //Dong Cuoi

                            var qtyInputed = data[i].Qty;

                            var remain = qtyInputed - detail.Where(r => r.xxcc == data[i].XXCC).Sum(r => r.pairs) < 0 ? 0 : qtyInputed - detail.Where(r => r.xxcc == data[i].XXCC).Sum(r => r.pairs);

                           

                            Entry entry = new Entry
                            {
                                Margin = new Thickness(5, 0, 0, 0),
                                HorizontalTextAlignment = TextAlignment.Start,
                                VerticalTextAlignment = TextAlignment.Center,
                                Text = remain.ToString(),
                                TextColor = Color.Black,
                                FontSize = 20,
                                Keyboard = Keyboard.Numeric,
                                AutomationId = data[i].XXCC,
                                BackgroundColor = remain == 0 ? Class.Style.Success : Color.White,
                                IsEnabled = remain != 0
                            };

                            entry.TextChanged += (s, e) =>
                            {
                                var obj = s as Entry;

                                if (int.TryParse(obj.Text, out int result))
                                {
                                    var tar = lstOriginal_Qty.Where(r => r.XXCC == obj.AutomationId).ToList();

                                    obj.Text = result >= tar[0].Qty ? tar[0].Qty.ToString() : result.ToString();
                                }
                                else obj.Text = "0";

                                var sum = lstEntry.Sum(r => int.Parse(!int.TryParse(r.Text, out int re) ? "0" : r.Text));

                               // lbl_Target.Text = string.Format("{0}/{1}", sum, binding.TarQty.Split('/')[1]);

                                lbl_SumEntry.Text = lstEntry.Sum(r => int.Parse(!int.TryParse(r.Text, out int re) ? "0" : r.Text)).ToString();

                            };

                            var frm_PairsInputed = new Frame
                            {

                                Background = Class.Style.ColdKidsSky.Cream,
                                Margin = new Thickness(0),
                                Padding = new Thickness(0),
                                Content = entry
                            };

                            lstEntry.Add(entry);

                            var frm_dateEntry = new Frame
                            {
                                Background = Class.Style.ColdKidsSky.BlueSea,
                                Margin = new Thickness(0),
                                Padding = new Thickness(0),
                                Content = new Label { Margin = new Thickness(5, 0, 0, 0), HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment = TextAlignment.Center, Text = DateTime.Now.ToString("yyyy-MM-dd"), TextColor = Color.White, FontSize = 18, Padding = new Thickness(0) }
                            };

                            g.Children.Add(frm_dateEntry, 0, dong);

                            g.Children.Add(frm_PairsInputed, i + 1, dong);

                            if (i + 1 == data.Count)
                            {
                                var frm_Total = new Frame
                                {
                                    Background = Class.Style.ColdKidsSky.BlueSea,
                                    Margin = new Thickness(0),
                                    Padding = new Thickness(0),
                                    Content = lbl_SumEntry = new Label { FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text = lstEntry.Sum(r => int.Parse(!int.TryParse(r.Text, out int re) ? "0" : r.Text)).ToString(), TextColor = Color.Wheat, FontSize = 20, Padding = new Thickness(0) }
                                };
                                g.Children.Add(frm_Total, data.Count + 1, dong);
                            }

                        }                   
                    }

                    if (i == data.Count)
                    {                       
                        g.ColumnDefinitions.Add(new ColumnDefinition
                        {
                            Width = new GridLength(90, GridUnitType.Absolute)
                        });


                        var frm_TitleTotal = new Frame
                        {
                            Background = Class.Style.ColdKidsSky.BlueSea,
                            Margin = new Thickness(0),
                            Padding = new Thickness(0),
                            Content = new Label { FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text = "Tổng", TextColor = Color.Wheat, FontSize = 20, Padding = new Thickness(0) }
                        };
                        g.Children.Add(frm_TitleTotal, g.ColumnDefinitions.Count - 1, 0);

                        var frm_Total = new Frame
                        {
                            Background = Class.Style.ColdKidsSky.BlueSea,
                            Margin = new Thickness(0),
                            Padding = new Thickness(0),
                            Content = new Label { FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, Text = binding.TarQty.Split('/')[1], TextColor = Color.Wheat, FontSize = 20, Padding = new Thickness(0) }
                        };
                        g.Children.Add(frm_Total, g.ColumnDefinitions.Count - 1, 1);




                    }
                }
            }
            this.Stack_Detail.Children.Add(g);
        }

        List<(string XXCC, int Qty ,int Target)> Data
        {
            get
            {


                List<(string, int, int)> list = new List<(string, int, int)>();

                string sql = $@"SELECT zlbh, bwbh, bwzl.ywsm, XXcc,
convert(varchar, convert(int,actualqty)) Qty, convert(varchar,convert(int, Planqty)) [Target]
 from App_Cutting_Barcodes_Edit app inner join bwzl on app .BWBH =bwzl.bwdh  
where  zlbh = '{binding.RY}'  and convert(int, PlanQty) !=0 
and convert(int,ActualQty)!=0 and ywsm='{binding.Comp}' order by XXCC";

                var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                if (ta.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow row in ta.Rows)
                    {
                        (string, int, int) a;

                        a.Item1 = row["xxcc"].ToString();

                        a.Item2 = int.Parse(row["qty"].ToString());

                        a.Item3 = int.Parse(row["Target"].ToString());

                        list.Add(a);
                    }
                }
                return list;
            }
        }
        List<(string orderid,string xxcc,int pairs)> OrderDetail
        {
            get
            {
                List<(string, string, int) >lst=new List<(string, string, int)>();

                string sql = $@"SELECT OrderID, xxcc,ISNULL(OrderQty,0) OrderQty FROM App_Order_Detail where RY='{binding.RY}' and Component='{binding.Comp}' order  by XXCC";

                var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

                if (ta.Rows.Count > 0)
                {
                    foreach (System.Data.DataRow r in ta.Rows)
                    {
                        (string, string, int) a;
                        a.Item1 = r["Orderid"].ToString();
                        a.Item2 = r["xxcc"].ToString();
                        a.Item3 = int.Parse(r["OrderQty"].ToString());
                        lst.Add(a);
                    }
                }
                return lst;

            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if ((DB.StoreLocal.Instant.Depname == Departments.AutoCutting && wh == "AC") ||(DB.StoreLocal.Instant.Depname == Departments.Lazer && wh == "LZ"))
            {
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
                    return;
                }
                else
                {

                    switch (a)
                    {
                        case "Nosew (Ép Nóng)":
                            Destination = "NS"; break;

                        case "High frequency (Ép Cao tần)":
                            Destination = "HF"; break;

                        case "Printing (In)":
                            Destination = "PR"; break;

                        case "Embroidery (Thêu)":
                            Destination = "EM"; break;

                        case "Stiching (May)":
                            Destination = "ST"; break;

                        default: //OS
                            Destination = "OS (Gia Công)"; break;
                    }
                }
                var result = await Services.Alert.PopupYesNo("Hỏi", string.Format("Bạn muốn gửi đến {0}?", Destination), "Đặt Hàng", "Không");

                if ((result as string) == "Đặt Hàng")
                {
                    Insert_Material_Orders();

                    //Comback previous page
                    await Navigation.PopAsync();
                    return;
                }
            }
           
           
                Insert_Material_Orders();

             await Navigation.PopAsync();
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
           (
             @orderid
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
        void Insert_Material_Orders()
        {

            List<Model_DataGrid> dataconfirm = new List<Model_DataGrid>();

            binding.Sel = true;

            dataconfirm.Add(binding);

            int kq = 0;

            var data = dataconfirm.Where(r => r.Sel).ToList();

            string id = "";

            if (data.Any())
            {               
                string prefix_ID = wh;

                foreach (var item in data)
                {

                    if (item.Sel)
                    {

                        string sumsql = $"SELECT isnull(SUM(OrderQty) ,0) orderqty FROM  App_Order_Detail where ry = '{item.RY}' and Component ='{item.Comp}'";

                        var sumentry = lstEntry.Sum(r => int.Parse(!int.TryParse(r.Text, out int re) ? "0" : r.Text));

                        if (sumentry == 0) continue;

                        var taSum = DB.SQL.ConnectDB.Connection.FillDataTable(sumsql);

                        if (taSum.Rows.Count > 0)
                        {
                            var checkQty = int.Parse(taSum.Rows[0][0].ToString()) + sumentry;


                            if (checkQty > int.Parse(item.TarQty.Split('/')[1]))
                            {
                                Services.DisplayToast.Show.Toast("Đã Nhập", Class.Style.Warning);

                                continue;
                            }
                        }


                       


                        id = string.Format("{0}{1}", prefix_ID, DateTime.Now.Ticks.ToString());

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


                        string[] arr = { id, item.RY, item.Comp, item.Bwbh, item.Clbh, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), DB.StoreLocal.Instant.UserName, sumentry.ToString(), Destination };

                        if (Class.Network.Net.HasNet)
                            kq = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, arr);

                        if (kq != 0)
                        {
                            #region Cap nhap local: Sau khi cap thanh cong - gioi han trong local

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


                            if (editLocalAC != null && editLocalAC.Count >0)
                            {

                                var qty = int.Parse(editLocalAC[0].TarQty.Split('/')[0]) - sumentry < 0 ? 0 : int.Parse(editLocalAC[0].TarQty.Split('/')[1]) - sumentry;

                                if(qty == 0)
                                {
                                    DB.StoreLocal.Instant.Data_WareHouse_AC.RemoveAll(r => r.RY == item.RY &&
                                     r.Comp == item.Comp &&
                                     r.Clbh == item.Clbh &&
                                     r.Bwbh == item.Bwbh &&
                                     r.Art == item.Art);
                                }
                                else editLocalAC[0].TarQty = string.Format("{0}/{1}",qty, editLocalAC[0].TarQty.Split('/')[1]);
                            }


                            if(editLocalLZ != null && editLocalLZ.Count >0)
                            {
                                var qty = int.Parse(editLocalLZ[0].TarQty.Split('/')[0]) - sumentry < 0 ? 0 : int.Parse(editLocalLZ[0].TarQty.Split('/')[1]) - sumentry;

                                if (qty == 0)
                                {
                                    DB.StoreLocal.Instant.Data_WareHouse_Lz.RemoveAll(r => r.RY == item.RY &&
                                     r.Comp == item.Comp &&
                                     r.Clbh == item.Clbh &&
                                     r.Bwbh == item.Bwbh &&
                                     r.Art == item.Art);
                                }
                                else

                                    editLocalLZ[0].TarQty = string.Format("{0}/{1}", qty, editLocalLZ[0].TarQty.Split('/')[1]);
                            }


                            #endregion
                            List<Models.OrderDetail> lstDetail = new List<Models.OrderDetail>();

                            foreach (var entry in lstEntry)
                            {
                                lstDetail.Add(new Models.OrderDetail
                                {
                                    Orderid = id,
                                    RY = binding.RY,
                                    Component = binding.Comp,
                                    XXCC = entry.AutomationId,
                                    OrderQty = int.Parse(entry.Text)

                                });
                            }
                            kq = Insert_Order_Detail(lstDetail);


                            if(kq != 0)
                            {
                                if(wh == DB.StoreLocal.Instant.CurrentDep)
                                {

                                    sql = $@"INSERT INTO [dbo].[App_Material_Process]
                                     VALUES
                                           (@OrderId,
                                           @UserDate,
                                           @UserID,
                                           0 )";

                                    kq = DB.SQL.ConnectDB.Connection.Update_Parameter(sql, 
                                        new string[] { id, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff"),DB.StoreLocal.Instant.UserName });
                                }


                                if(kq!=0)
                                    Services.DisplayToast.Show.Toast("Hoàn thành", Class.Style.Success);



                            }
                        }

                    }
                };
            }
            else
            {
                Alarm.Sound.Error();
                Services.DisplayToast.Show.Toast("Nothing", Class.Style.Error);
            }
           // return id;
        }

        private async void Backbutton_Click(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
   
}