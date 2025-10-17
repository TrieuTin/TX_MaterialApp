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
    public partial class PopupComponent_Combine : Popup
    {
        
        public PopupComponent_Combine(List<Model_PopupCombine> listComp)
        {
            InitializeComponent();
            gv_Inventory.ItemsSource = listComp;
            
        }

        //void StartBinding()
        //{
        //    var listView = new ListView
        //    {
        //        ItemsSource = md,
        //        RowHeight = 50,
        //        SeparatorVisibility = SeparatorVisibility.Default,
        //        ItemTemplate = new DataTemplate(() =>
        //        {
        //            var grid = new Grid
        //            {
        //                Padding = 5,
        //                ColumnDefinitions =
        //                {
        //                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                      
        //                }
        //            };

        //            var nameLabel = new Label { VerticalTextAlignment = TextAlignment.Center };
        //            nameLabel.SetBinding(Label.TextProperty, "Component");
        //            grid.Children.Add(nameLabel, 0, 0);


        //            return new ViewCell { View = grid };
        //        })
        //    };

        //    // Đặt ListView vào trang
            
        //    this.Container.Children.Add(listView);
        //    Grid.SetColumn(listView, 0);
        //    Grid.SetRow(listView, 0);
        //}


    };

    public class Model_PopupCombine
    {
        public string NameComp { set; get; }        
        public string Component { set; get; }        
    }
}