using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.UsersControl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UC_InventoryList : ListView
    {
        public UC_InventoryList()
        {
            InitializeComponent();
            
        }

        private void Item_Tapped(object sender, SelectedItemChangedEventArgs e)
        {

        }
       
    }
}