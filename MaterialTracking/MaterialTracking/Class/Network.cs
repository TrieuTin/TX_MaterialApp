using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MaterialTracking.Class
{
    public class Network
    {
        static Network _net;

        private Network()
        {

        }

        public static Network Net { get => _net == null ? _net = new Network() : _net; }

        public bool HasNet
        {
            
            get
            {
                var connectivity = Connectivity.NetworkAccess.ToString();
                switch (connectivity)
                {
                    case "Unknown":
                    case "None":
                        {                                            
                           // Services.Alert.Msg("Khẩn Cấp", "Không có mạng", "OK");
                            return false;
                        }
                    default:
                        return true;
                }
            }
        }
    }
}
