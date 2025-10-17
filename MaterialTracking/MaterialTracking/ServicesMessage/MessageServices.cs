using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace MaterialTracking.ServicesMessage
{
    public class MessageServices:IMessageServices
    {
        public async Task ShowAsync(string mess)
        {
            await MaterialTracking.App.Current.MainPage.DisplayAlert("ERROR", mess, "OK");
        }

        public async Task ShowAsync(string mess, string title)
        {
            await MaterialTracking.App.Current.MainPage.DisplayAlert(title, mess, "OK");
        }

        public async Task ShowAsync(string mess, string title, string accept, string cancel)
        {
            await MaterialTracking.App.Current.MainPage.DisplayAlert(title, mess, accept, cancel);
        }
        public async void showAsyncNewPage(NavigationPage page)
        {
            NavigationPage.SetHasNavigationBar(page, false);
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
        public async void showAsyncNewPage(NavigationPage page, bool HasNavigationBar)
        {
            NavigationPage.SetHasNavigationBar(page, HasNavigationBar);
            //GradientStopCollection gradientStops = new GradientStopCollection();

            //gradientStops.Add(new GradientStop() { Color = Color.FromHex("#713d74"), Offset = (float)0.0 });
            //gradientStops.Add(new GradientStop() { Color = Color.FromHex("#221e60"), Offset = (float)1.0 });
            //LinearGradientBrush linearGradientBrush = new LinearGradientBrush()
            //{

            //    EndPoint = new Point(0, 1),
            //    GradientStops = gradientStops
            //};
            //page.BarBackground = linearGradientBrush;
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
        public async void showAsyncNewPage(Page page)
        {
            
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
        public async void showPushModalAsync(Page page)
        {

            await Application.Current.MainPage.Navigation.PushModalAsync(page);
        }
    }
}
