using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginView : ContentPage
    {
        public LoginView()
        {
            InitializeComponent();

            var vm = new ViewModels.LoginViewModel();

            vm.Navigation = Navigation;

            var orientation = DeviceDisplay.MainDisplayInfo.Orientation;

            //if (orientation == DisplayOrientation.Landscape) vm.PathClip = "ms-appx:///Landscape.mp4";

            //else vm.PathClip = "ms-appx:///Portrait_1.mp4";

            vm.PathClip = orientation == DisplayOrientation.Landscape ? "ms-appx:///Landscape.mp4" : "ms-appx:///Portrait_1.mp4";

            Xamarin.Essentials.DeviceDisplay.MainDisplayInfoChanged += (s, e) =>
            {
                orientation = e.DisplayInfo.Orientation;

                switch (orientation)
                {
                    case DisplayOrientation.Unknown:
                        break;
                    case DisplayOrientation.Portrait:
                        vm.PathClip = "ms-appx:///Portrait_1.mp4";
                        break;
                    case DisplayOrientation.Landscape:
                        vm.PathClip = "ms-appx:///Landscape.mp4";
                        break;
                    default:
                        break;
                }
            };
            
            vm.Aspected = Aspect.AspectFill;           

            var acc = DB.DataLocal.Table.User;

            if (acc.Count != 0)
            {
                vm.Id= acc[0].UID;
                vm.Pwd= acc[0].Pwd;

                switch (acc[0].Company)
                {
                    case "LVL":
                        {
                            vm.FacLVL = true;
                            break;
                        }
                    case "LHG":
                        {
                            vm.FacHG = true;
                            break;
                        }
                    case "LYV":
                        {
                            vm.FacLYV = true;
                            break;
                        }
                    case "Debug"://LYM
                        {
                            vm.FacLYM = true;
                            break;
                        }
                    default:
                        vm.FacHG = false;
                        vm.FacLVL = false;
                        vm.FacLYV = false;
                        vm.FacLYM = false;
                        break;
                }

            }



            BindingContext = vm;
           
        }

       
    }
}