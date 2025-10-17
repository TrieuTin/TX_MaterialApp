using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Alarm
{
   public class Sound
    {
        public static void Completed()
        {
            DependencyService.Get<IAudioService>().Play("Completed.wav");
        } 
        public static void Error()
        {
            DependencyService.Get<IAudioService>().Play("False.wav");
        }
        public static void Click()
        {
            DependencyService.Get<IAudioService>().Play("Click.wav");
        }
        
    }
}
