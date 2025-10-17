using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using System.Net.Http;

using System;
using System.Collections.Generic;

using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Xamarin.Forms;

namespace MaterialTracking.Class
{
   public class Helper
    {
        private const string APK_URL = @"http://192.168.60.15/apk/Material.apk";
        private const string VERSION = @"http://192.168.60.15/apk/version.txt";
        


        private static Helper helper;

        public static Helper Instant { get => helper == null ? helper = new Helper() : helper; }
       
        public async void CheckForUpdateAsync(string currentVersion)
        {
            var convert = new ConvertBytes(VERSION);

            var Info = convert.ReadText.Split(',');

            var updateInfo = new UpdateInfo
            {
                LatestVersionCode = Info[0].Split(':')[1],
                DownloadUrl = Info[1].Split('"')[1]
            };


            if (VersionCompare.IsNewVersion(currentVersion, updateInfo.LatestVersionCode))
            {
                var answer = await Services.Alert.MsgSelect("", $"Có phiên bản mới bạn có muốn download không ({updateInfo.LatestVersionCode.Trim()})?");
                if (answer)
                {
                   var filename = "Material.apk"; 

                    convert = new ConvertBytes(APK_URL);
                    convert.DownloadBytes(filename);
                }
            }
            else
            {
                //Services.DisplayToast
                Console.WriteLine("Ứng dụng đang ở phiên bản mới nhất.");
            }
        }
      
       
    }
   
    public class UpdateInfo
    {
        public string LatestVersionCode { get; set; }
        public string DownloadUrl { get; set; }      
    }
    public class VersionCompare
    {
        public static bool IsNewVersion(string currentVersion, string latestVersion)
        {
            var currentParts = currentVersion.Split('.').Select(int.Parse).ToArray();
            var latestParts = latestVersion.Split('.').Select(int.Parse).ToArray();

            for (int i = 0; i < Math.Max(currentParts.Length, latestParts.Length); i++)
            {
                int currentPart = i < currentParts.Length ? currentParts[i] : 0;
                int latestPart = i < latestParts.Length ? latestParts[i] : 0;

                if (latestPart > currentPart)
                    return true;
                if (latestPart < currentPart)
                    return false;
            }

            return false; // Nếu hai phiên bản bằng nhau
        }
    }
    public class ConvertBytes
    {
        byte[] _bytes = null;
        string url = "";
        public ConvertBytes(string URL)
        {
            url = URL;
           
        }
        public ImageSource imgSource()
        {           

            return ImageSource.FromStream(() => new System.IO.MemoryStream(Bytes));

        }
        public byte[] Bytes 
        {
            get
            {
                try
                {

                    using (var wc = new System.Net.WebClient())
                        _bytes = wc.DownloadData(url);
                         //wc.UploadData(url);
                }
                catch (Exception)
                {


                }
                return _bytes;
            }
        }
        public string ReadText
        {
            get
            {
                var txt = Bytes;

                if (txt != null && txt.Length > 0)
                {
                    return Encoding.UTF8.GetString(txt);

                }
                else return "";
            }
        }
        public void DownloadBytes(string filename, string PathSave="")
        {
            var file = Bytes;
            var path = "";

            if (PathSave == "")
            {
                path = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, filename);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            else path = PathSave;

            if (file == null || file.Length == 0)
            {

                return;
            }

            try
            {

               File.WriteAllBytes(path, file);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lưu file: {ex.Message}");
            }
        }
    }
    
}
