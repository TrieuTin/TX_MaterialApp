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
    public partial class TrackingTransfer : TabbedPage
    {
        public TrackingTransfer()
        {
            InitializeComponent();

           // this.Children.Add()
        }

        private void TabbedPage_CurrentPageChanged(object sender, EventArgs e)
        {

        }
    }
}