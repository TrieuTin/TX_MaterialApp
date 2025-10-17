using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
namespace MaterialTracking.Class
{
    public class Style
    {
        public static Color Error { get => Color.FromHex("#EB5757"); }
        public static Color Warning { get => Color.FromHex("#FFB000"); }
        public static Color Success { get => Color.FromHex("#2E7D32"); }
        public static Color Notification { get => Color.FromHex("#5a7dfd"); }
        public static Color Question { get => Color.FromHex("#2c8a9d"); }
        public static Color Organ { get => Color.FromHex("#d87700"); }
        public static Color Red { get => Color.FromHex("#C62828"); }
        public static Color Primary { get => Color.FromHex("#4D96FF"); }
        public static Color Normal { get => Color.FromHex("#12486B"); }
        public static Color EnableOff { get => Color.FromHex("#7D7C7C"); }
        public static Color LightSky { get => Color.FromHex("#7D7C7C"); }
        public static Color Yellow { get => Color.FromHex("#FF9800"); }
        public static Color Blue { get => Color.FromHex("#00235B"); }

        public static (Color BlueSea, Color Cream, Color BlueSky, Color Green) ColdKidsSky { get => coldKidsSky(); }
        public static (Color LGreen, Color MGreen, Color DGreen, Color MPink) SoftColor { get => _softColor(); }
        public static (Color DPurple, Color MPurple, Color LGreen, Color Pink) Purple { get => _purple(); }

        private static (Color DPurple, Color MPurple, Color LGreen, Color Pink) _purple()
        {
            (Color, Color, Color, Color) a;

            a.Item1 = Color.FromHex("#392467");

            a.Item2 = Color.FromHex("#5D3587");

            a.Item3 = Color.FromHex("#A367B1");

            a.Item4 = Color.FromHex("#FFD1E3");

            return a;
        }

        private static (Color LGreen, Color MGreen, Color DGreen, Color MPink) _softColor()
        {
            (Color, Color, Color, Color) a;

            a.Item1 = Color.FromHex("#F2FFE9");
            
            a.Item2 = Color.FromHex("#A6CF98");
            
            a.Item3 = Color.FromHex("#557C55");
            
            a.Item4 = Color.FromHex("#FA7070");

            return a;
        }

        private static (Color BlueSea, Color Cream, Color BlueSky, Color Green) coldKidsSky()
        {
            
            (Color ,Color, Color,Color) a;

            a.Item1 = Color.FromHex("#0b80ec");
          
            a.Item2 = Color.FromHex("#FFF6E9");
           
            a.Item3 = Color.FromHex("#BBE2EC");
           
            a.Item4 = Color.FromHex("#0D9276");
           
            return a;
        }




    }
}
