using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;

namespace Services
{
  public  class DisplayToast
    {
        private DisplayToast()
        {

        }
        private static DisplayToast _show;

        internal static DisplayToast Show { get => _show ==null? _show = new DisplayToast(): _show;}

        public async void Toast(string Content, Color BgColor, int secondShow = 3, int FontSize = 18)
        {
            var opt = new ToastOptions
            {
                BackgroundColor = BgColor,
                Duration = TimeSpan.FromSeconds(secondShow),
                IsRtl = false,
                
                CornerRadius = new Thickness(15,15,0,0),
                
                MessageOptions = new MessageOptions
                {
                    Foreground = Color.White,
                    Message =Content,
                    Font = Font.SystemFontOfSize(FontSize),

                }
            };
            await Application.Current.MainPage.DisplayToastAsync(opt);
        }
       
        public async void ToastIcon(string iconFont, Color BgColor, int secondShow = 3, int FontSize = 18)
        {
            
            var opt = new ToastOptions
            {
                BackgroundColor = BgColor,
                Duration = TimeSpan.FromSeconds(secondShow),
                IsRtl = false,

                CornerRadius = new Thickness(15, 15, 0, 0),
                MessageOptions = new MessageOptions
                {
                    Foreground = Color.White,
                    Message = iconFont,
                    Font = Font.SystemFontOfSize(FontSize,FontAttributes.Bold),

                }
            };
            await Application.Current.MainPage.DisplayToastAsync(opt);
        }


        public async void ToastPromt(string Content,ContentPage namePage ,Color BgColor  , int secondShow = 3 , int FontSize=18)
        {

           

            var act = new SnackBarActionOptions
            {
                Action = () => Task.Run(() => GotoPage(namePage)),
                Text = string.Format("Go to page {0}", namePage.Title)
            };
            var opt = new SnackBarOptions
            {
                MessageOptions = new MessageOptions
                {
                    Foreground = Color.White,
                    Message = Content ,
                    Font = Font.SystemFontOfSize(FontSize, FontAttributes.Italic)//NamedSize.Title

                },
                BackgroundColor = BgColor,
                Duration = TimeSpan.FromSeconds(secondShow),
                Actions = new[] { act }
            };
           
            await Application.Current.MainPage.DisplaySnackBarAsync(opt);
        }
        //internal INavigation Navigation { get; set; }
        private async void GotoPage(ContentPage name)
        {
           
                await Application.Current.MainPage.Navigation.PushAsync(name);
        }

     
    }
}
