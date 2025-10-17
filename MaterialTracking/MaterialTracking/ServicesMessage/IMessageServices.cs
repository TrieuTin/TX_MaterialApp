using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MaterialTracking.ServicesMessage
{
    public interface IMessageServices
    {
        Task ShowAsync(string mess);
        Task ShowAsync(string mess, string title);
        Task ShowAsync(string mess, string title, string accept, string cancel);
        void showAsyncNewPage(NavigationPage page);
        void showAsyncNewPage(NavigationPage page, bool hasNavitionBar);
        void showAsyncNewPage(Page page);
        void showPushModalAsync(Page page);
    }
}
