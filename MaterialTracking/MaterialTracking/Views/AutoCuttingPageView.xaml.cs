using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AutoCuttingPageView : ContentPage
    {
        ZXing.Net.Mobile.Forms.ZXingScannerPage Camera;
        public AutoCuttingPageView()
        {
            InitializeComponent();
        }

        private async void Btn_Scan(object sender, EventArgs e)
        {
            Camera = new ZXing.Net.Mobile.Forms.ZXingScannerPage();
            Camera.OnScanResult += (result) =>
            {
                Camera.IsScanning = false;
                Device.BeginInvokeOnMainThread(() =>
                {

                    Navigation.PopModalAsync();
                    Services.Alert.Msg("Result", result.Text);
                    //  var text = result.Text;

                });

            };
            await Navigation.PushModalAsync(Camera);
        }
    }
}