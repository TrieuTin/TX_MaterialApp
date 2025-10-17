using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Services
{
    public class DisplayPage
    {
        private static DisplayPage _instant;

        public static DisplayPage Instant { get=> _instant ==null? _instant = new DisplayPage():_instant; }

        public async void PushAsync(Page pageName)
        {
            await Application.Current.MainPage.Navigation.PushAsync(pageName);
        }
    }
}
