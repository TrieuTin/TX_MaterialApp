using MaterialTracking.Class;
using MaterialTracking.PageViews;
using MTM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.UsersControl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Uc_InventoryExtend : Grid
    {
        InventoryPageGeneral_Type _master;
        
        ObservableCollection<InventoryPageDetail_Type> _details;


        string actionUser = DB.StoreLocal.Instant.Depid;

      
        public Uc_InventoryExtend( InventoryPageGeneral_Type Master,ObservableCollection<InventoryPageDetail_Type>Details )
        {
            InitializeComponent();
            _master = Master;

            _details = Details;
            if (Network.Net.HasNet)
            {

                Load();
                //Set_MainTitle();
                //SetTitle2();
            }
            //expParent.Tapped += (s, e) =>
            //{
            //    //  var itMe = s as Xamarin.CommunityToolkit.UI.Views.Expander;
            //    Task.Delay(1800);
            //    exp_Detail.IsExpanded = true;

            //  //  Debug.WriteLine(itMe.IsExpanded);
            //   // Debug.WriteLine(exp_Detail.IsExpanded);
            //};

        }
        void SetRowCol_SubContent2(int TotalSize,int TotalComp)
        {
            for (int i = 0; i < TotalComp; i++)
            {
                if (i == 0)
                {

                    var G_ColComp = new ColumnDefinition
                    {
                        Width = new GridLength(110, GridUnitType.Absolute)
                    };
                    subContent2.ColumnDefinitions.Add(G_ColComp);
                }
                var G_Row = new RowDefinition
                {
                    Height = GridLength.Auto
                };

                subContent2.RowDefinitions.Add(G_Row);
            }
            for (int i = 0; i < TotalSize; i++)
            {
                var G_ColSize = new ColumnDefinition
                {
                    Width = GridLength.Star
                };
                subContent2.ColumnDefinitions.Add(G_ColSize);
            }

            var G_Col = new ColumnDefinition
            {
                Width = new GridLength(80, GridUnitType.Absolute)
            };
            subContent2.ColumnDefinitions.Add(G_Col);

        }

        void SetTitle2()
        {
            string sql = $"SELECT * FROM App_Cutting_Barcodes_Edit where Barcode='{_master.Barcode}' And ZLBH ='{_master.ZLBH}' And CLBH='{_master.CLBH}'";

            if (DB.SQL.ConnectDB.Connection.FillDataTable(sql).Rows.Count == 0) return;

            //   stackCommand.Children.Add(btn);

            var row = new RowDefinition
            {
                Height = GridLength.Auto
            };
            //Cot dau tien size
            var col = new ColumnDefinition
            {
                Width =new  GridLength(100, GridUnitType.Absolute)
            };

            Title2.RowDefinitions.Add(row);

            Title2.ColumnDefinitions.Add(col);

            Label lbl = new Label
            {
                Text = "Component",
                FontSize = 20,
                Padding = new Thickness(0, 0, 0, 0)
                ,
                TextColor = Color.Black,
                HorizontalTextAlignment = TextAlignment.Start
                
            };
            var f = new Frame
            {
                BackgroundColor = Color.FromHex("#93B1A6"),
                HasShadow = true,
                HeightRequest = 27,
                Margin = new Thickness(0),
                Padding = new Thickness(0)
                
            };

            f.Content =lbl;
            Title2.Children.Add(f, 0, 0);

            var listzlbh = _details.Where(z => z.Zlbh == _master.ZLBH).ToList();

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
                col = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };

                Title2.ColumnDefinitions.Add(col);

                //Them comp vao menu chinh
                f = new Frame
                {
                    BackgroundColor = Color.FromHex("#93B1A6"),
                    HasShadow = true,
                    HeightRequest = 27,
                    Margin = new Thickness(0),
                    Padding = new Thickness(0)
                };

                f.Content = lbl = new Label { HorizontalTextAlignment = TextAlignment.Center, Text = SortSize[i], TextColor = Color.Black, FontSize = 20, Padding = new Thickness(0, 0, 0, 0) };
                Title2.Children.Add(f, i + 1, 0);

            }
            col = new ColumnDefinition
            {
                Width = new GridLength(80, GridUnitType.Absolute)
            };
            Title2.ColumnDefinitions.Add(col);
            f = new Frame
            {
                BackgroundColor = Color.FromHex("#93B1A6"),
                HasShadow = true
                , HeightRequest = 27,
                Margin = new Thickness(0),
                Padding = new Thickness(0)
            };
            f.Content = lbl = new Label { HorizontalTextAlignment = TextAlignment.Center, Text = "Action", TextColor = Color.Black, FontSize = 20, Padding = new Thickness(0, 0, 0, 0) };

            //Them cac nut Action trong sub menu
            Title2.Children.Add(f, SortSize.Count + 1, 0);
        }
        void Set_MainTitle()
        {
            lbl_Ry.Text = _master.ZLBH;

            lbl_art.Text = _master.Art;

            lbl_line.Text = _master.Line;

            lbl_TotalComp.Text = _master.Components;

            lbl_date.Text = _master.ProDate;

            lbl_modelname.Text = _master.ModelName;

            lbl_delay.Text = "Unknown";
            lbl_Delivered .Text= "Unknown";
            lbl_Done.Text = "Unknown";
            lbl_going.Text = "Unknown";
           
        }
        void Load()
        {
            Set_MainTitle();

            SetTitle2();


            int rowCount = 0;

            var listzlbh = _details.Where(z => z.Zlbh == _master.ZLBH).ToList();

            var grpComp = listzlbh.AsEnumerable().GroupBy(c => c.Component).ToList();

            var grpSize = listzlbh.GroupBy(s => s.Size).ToList();

            List<string> SortSize = new List<string>();


            for (int i = 0; i < grpSize.Count; i++)
            {
                if (!SortSize.Contains(grpSize[i].Key)) SortSize.Add(grpSize[i].Key);

            }

            SortSize.Sort();
            //----------------------------------------

            SetRowCol_SubContent2(SortSize.Count, grpComp.Count);
            

            foreach (var comp in grpComp)
            {
                int columnCount = 1;            

                Frame frameDetail1 = new Frame
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),


                    BackgroundColor = Class.Style.Organ,
                    Content = new Label { HorizontalTextAlignment = TextAlignment.Start, Text = comp.Key, TextColor = Color.White, FontSize = 20, Padding = new Thickness(0) }
                };

                subContent2.Children.Add(frameDetail1, 0, rowCount);



                foreach (var size in SortSize)
                {                  
                    var qty = listzlbh.AsEnumerable().Where(sc => sc.Size.Contains(size) && sc.Component.Contains(comp.Key)).ToList();

                    Frame framedetail = new Frame
                    {
                        Margin = new Thickness(0),
                        Padding = new Thickness(0),
                        BackgroundColor = Class.Style.Organ,
                        
                        Content = new Label { HorizontalTextAlignment = TextAlignment.Center, Text = qty[0].Qty, TextColor = Color.White, FontSize = 20, Padding = new Thickness(0) }
                    };

                    subContent2.Children.Add(framedetail, columnCount, rowCount);

                    columnCount++;
                }

                var NutThiHanh = new Button { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, Text = "Execute", FontSize = 18, Padding = new Thickness(0) };

                NutThiHanh.Clicked += (s, e) =>
                {
                    Services.Alert.PopupNotification("Thông Báo!", "Tính năng đang hoàn thành", "OK", Class.Style.Notification);
                };
                Frame framedetai3 = new Frame
                {
                    Margin = new Thickness(0),
                    Padding = new Thickness(0),
                    BackgroundColor = Class.Style.Organ ,
                    Content = NutThiHanh              
                };

                subContent2.Children.Add(framedetai3, columnCount, rowCount);

                rowCount++;

            }
        }

     
    }
   
}