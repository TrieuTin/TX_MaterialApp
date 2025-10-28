
using MaterialTracking.PageViews;
using MaterialTracking.Popups;
using MaterialTracking.UsersControl;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace Services
{
    public class Alert
    {


        public static async void Msg(string title, string content, string button1 = "OK", string button2 = "")
        {
           

            await Application.Current.MainPage.DisplayAlert(title, content, button1);
        }
        public static async Task<string> MsgAction(string Title, string[] Btn_list, string button1 = "CANCEL")
        {
          
            var ac = await Application.Current.MainPage.DisplayActionSheet(Title, button1,null, Btn_list);
            return ac;
        }
        public static async Task<bool> MsgSelect(string Title,string Content)
        {
            var repon=   await Application.Current.MainPage.DisplayAlert(Title, Content, "Yes", "No");
            return repon;
            
        }  public static async Task<string> MsgPromt(string Title,string Content)
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync(Title, Content, "OK","CANCEL");
            
            return result;
        }
        public static void ToastMsg(string message, Android.Widget.ToastLength lenght = Android.Widget.ToastLength.Long)
        {
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, lenght).Show();
        }


        public static async Task<List<UC_SetVolum>> PopupMenu_SetVolum(List<UC_SetVolum> UcRecentl, bool Dismiss = true)
        {

            var b = new SetvolumViewModel
            {
                UcRecentl = UcRecentl,
                DismissEnable = Dismiss
            };

            var a = await Application.Current.MainPage.Navigation.ShowPopupAsync(new PopupSetVolum(b)) as List<UC_SetVolum>;

            return a;
        }
        public static void PopupNotification(string Title,string Content,string ButtonName, Xamarin.Forms.Color PanelColor)
        {
            BindingViewModel b = new BindingViewModel
            {
                LabelNotification = Title,
                LabelContent = Content,
                ButtonName = ButtonName,
                ColorNotify = PanelColor
            };
             Application.Current.MainPage.Navigation.ShowPopup(new MaterialTracking.PageViews.PopupNotifiaction(b));
        }

        public  static async Task<object>PopupYesNo(string Title,string Content, string Button_name1, string Button_name2)
        {

            var b = new BindingViewModel_PopupYN
            {
                ButtonName1 = Button_name1,
                ButtonName2 = Button_name2,
                LabelContent = Content,
                LabelNotification = Title,
                Theme = MaterialTracking.Class.Style.Question

            };
           
            return await Application.Current.MainPage.Navigation.ShowPopupAsync(new MaterialTracking.Popups.PopupYN(b));
            
        }
        public static async Task<object> PopuSearch( string title="")
        {
            var a = await Application.Current.MainPage.Navigation.ShowPopupAsync(new MaterialTracking.Popups.PopupSearch(title));

            return a;
        }

        public static async Task<string> PopupMenu(string Title,  List<Button>arr_Button ,Xamarin.Forms.Color PanelColor,bool Dismiss=true)
        {

            var b = new BindingViewModel_Choise
            {                
                ColorNotify = PanelColor,
                
                LabelNotification = Title,
                
                DismissEnable=Dismiss
            };

            var a = await Application.Current.MainPage.Navigation.ShowPopupAsync(new MaterialTracking.Popups.PopupChoise(b,arr_Button)) as string;
            return a;
        }

        public static async Task<string> PopupRequest(string Warehouse, bool Dismiss = true)
        {

            var b = new ModelPopupRequest()
            {
                Dismiss = Dismiss,
                Warehouse = Warehouse
            };
           // var aa = b.Data;
            var a = await Application.Current.MainPage.Navigation.ShowPopupAsync(new MaterialTracking.Popups.PopupRequest(b)) as string;
            return a;
        }

        public static void PopupShowRequest(bool ShowAgain=false)
        {
         
            Application.Current.MainPage.Navigation.ShowPopupAsync(new MaterialTracking.Popups.PopupShowRequest(ShowAgain));
            
        }
        public static void PopupListCombine(List<Model_PopupCombine> list)
        {

            Application.Current.MainPage.Navigation.ShowPopupAsync(new MaterialTracking.Popups.PopupComponent_Combine(list));

        }
    }

   

}
