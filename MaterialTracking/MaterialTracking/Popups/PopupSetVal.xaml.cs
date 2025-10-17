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
	public partial class PopupSetVal : Popup
	{
		public PopupSetVal ()
		{
			InitializeComponent ();
		}

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}