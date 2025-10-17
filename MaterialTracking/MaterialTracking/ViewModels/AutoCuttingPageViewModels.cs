using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using MaterialTracking.Views;
namespace MaterialTracking.ViewModels
{

    public class AutoCuttingPageViewModels
    {
        //JobTicket
        //Scan BarCode
        //SuperMaket
        //Cutting
        //Production Tracking
        public Command JobTicketCommand { get; set; }
        public Command ScanBarcodeCommand { get; set; }
        public Command SuperMaketCommand { get; set; }
        public Command CuttingCommand { get; set; }
        public Command ProductionTrackingCommand { get; set; }
        public Command StockCommand { get; set; }

        


        ZXing.Net.Mobile.Forms.ZXingScannerPage Camera;
        private readonly ServicesMessage.IMessageServices messageServices;
        public AutoCuttingPageViewModels()
        {
            this.messageServices = DependencyService.Get<ServicesMessage.IMessageServices>();
            this.JobTicketCommand = new Command(this.JobTicketClickedAsync);
            this.ScanBarcodeCommand = new Command(this.ScanBarcodeClickedAsync);
            this.SuperMaketCommand = new Command(this.SuperMarketView);
            this.StockCommand = new Command(this.Stock_Click);
        }
        private void Stock_Click ()
        {
            Alarm.Sound.Click();
           // this.messageServices.showAsyncNewPage(new Views.NoSaw.CallingDetailView();
        }
        private void SuperMarketView()
        {
            Alarm.Sound.Click();
            this.messageServices.showAsyncNewPage(new Views.SupperMaketView());
        }
        private void JobTicketClickedAsync()
        {
            Alarm.Sound.Click();
            this.messageServices.showAsyncNewPage(new JobTicketViews());
        }
        private void ScanBarcodeClickedAsync()
        {
            Alarm.Sound.Click();
            Camera = new ZXing.Net.Mobile.Forms.ZXingScannerPage();
            Camera.OnScanResult += (result) =>
            {
                Camera.IsScanning = false;
                Device.BeginInvokeOnMainThread(() =>
                {
                   // this.messageServices.ShowAsync("Result", result.Text);
                    //  var text = result.Text;
                    

                });

            };
            this.messageServices.showPushModalAsync(Camera);
        }
    }
}
