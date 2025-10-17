using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.UsersControl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UC_SetVolum : Grid, INotifyPropertyChanged
    {
        #region property change
        public event EventHandler CheckedEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        public UC_SetVolum()
        {
            InitializeComponent();
            BindingContext = this;
            //Article = art;
            //imgShoe.Source = imgSource(art);
            
        }
        bool _checked = true;
      
        string _article = "";
        string _modelname = "";
        string _pairs = "0";
        string _old_pairs = "0";
        string _comp;
        Color _colorselected;
        ImageSource _img;
        string _ry;
        public bool CheckedSelect { get => _checked; set { _checked = value; OnPropertyChanged(nameof(CheckedSelect)); } }
        public string Ry { get => _ry; set { _ry = value; OnPropertyChanged(nameof(Ry)); } }
        public string Article { get => _article; 
            set 
            { 
                _article = value;

                Img = imgSource(value);
                OnPropertyChanged(nameof(Article));
            } 
        }
        public string ModelName { get => _modelname; set { _modelname = value; OnPropertyChanged(nameof(ModelName)); } }

        public Color ColorSelected { get => _colorselected; set { _colorselected = value; OnPropertyChanged(nameof(ColorSelected)); } }

        public ImageSource Img { get => _img; set { _img = value; OnPropertyChanged(nameof(Img)); } }

        public string Pairs { get => _pairs; set { _pairs = value;  OnPropertyChanged(nameof(Pairs)); } }

        public string Old_pairs { get => _old_pairs; set { _old_pairs = value; OnPropertyChanged(nameof(Old_pairs)); } }

        public string Component { get => _comp; set { _comp = value; OnPropertyChanged(nameof(Component)); } }

        ImageSource imgSource(string art)
        {

            string article = art + ".bmp";
            
            string url = "";

            switch (DB.StoreLocal.Instant.Myfac)  
            {
                case DB.MyFactory.LVL:
                    {
                        url = @"http://192.168.60.15:5000/shoes-photos/" + article;
                        break;
                    }
                case DB.MyFactory.LHG:
                    {
                        url = @"http://192.168.30.19:5000/shoes-photos/" + article;
                        break;
                    }
                case DB.MyFactory.LYV:
                    {
                        url = @"http://192.168.0.96:9999/shoes-photos/" + article;
                        break;
                    }
                case DB.MyFactory.LYM:
                    {
                        url = @"http://192.168.55.229:5000/shoes-photos/" + article;
                        break;
                    }               
                default: //LYF
                    url = @"http://192.168.60.15:5000/shoes-photos/" + article;
                    break;
            }

          

            byte[] imageData = null;
            try
            {
                if (IsUrlExists(url))
                    using (var wc = new System.Net.WebClient())
                        imageData = wc.DownloadData(url);
            }
            catch (Exception)
            {

                return null;
            }


            return ImageSource.FromStream(() => new System.IO.MemoryStream(imageData));

        }
        bool IsUrlExists(string url)
        {
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                request.Method = "HEAD"; // Chỉ yêu cầu tiêu đề để kiểm tra trạng thái, không cần tải nội dung
                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }
            }
            catch (System.Net.WebException ex)
            {
                // Nếu xảy ra lỗi, ví dụ như 404, 500, URL không hợp lệ
                if (ex.Response is System.Net.HttpWebResponse errorResponse)
                {
                    return errorResponse.StatusCode == System.Net.HttpStatusCode.NotFound ? false : true;
                }
                return false;
            }
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var check = sender as CheckBox;

            ColorSelected = check.IsChecked ? Color.DarkGreen : Color.Gray;

            CheckedEvent?.Invoke(this, EventArgs.Empty);
        }

       

        //private async void OnEntryTapped(object sender, EventArgs e)
        //{
        //    var entry = sender as Entry;
        //    if (entry != null)
        //    {
        //        entry.Focus(); 
        //        await Task.Delay(100);
        //        entry.CursorPosition = 0;
        //        entry.SelectionLength = entry.Text?.Length ?? 0;
        //    }
        //}

       

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            var entry = sender as Entry;
            
            if (Old_pairs =="0" || entry.Text == "0"  )
            {
                entry.BackgroundColor = Color.White;

                entry.TextColor = Color.Black;

                return;
            }

            if(!double.TryParse(entry.Text, out double p))
            {
                entry.BackgroundColor = Class.Style.Red;

                entry.TextColor = Color.White;

                return;
            }

            if (p > double.Parse(Old_pairs))
            {
                entry.BackgroundColor = Class.Style.Red;
                
                entry.TextColor = Color.White;
            }
            else { entry.BackgroundColor = Color.White; entry.TextColor = Color.Black; }
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var entry = sender as Entry;
            if (double.TryParse(entry.Text, out double p))
            {
                if (p > double.Parse(Old_pairs))
                {
                    entry.Text = Old_pairs;

                }
                else entry.Text = p.ToString();
            }
            else entry.Text = "0";

          

        }
    }
}