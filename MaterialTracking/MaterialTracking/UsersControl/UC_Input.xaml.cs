using MaterialTracking.DB;
using MaterialTracking.Models;
using MTM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.UsersControl
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UC_Input : Grid
    {
        bool isLYV = DB.StoreLocal.Instant.Myfac == MyFactory.LYV;
        public UC_Input(InputViewModel inputModel)
        {
            InitializeComponent();


            this.BindingContext =inputModel;



            //  this.Wheelpicker.SelectedIndex = IndexPicker(inputModel.Temp);
            Wheelpicker.SelectedIndexChanged += (sender, args) =>
            {
                inputModel.HandleSelectedIndexChanged();

              
            };

            inputModel.SelectedIndex =inputModel.Temp;
        }
        public UC_Input(InputViewModel2 inputModel2) 
        {
            InitializeComponent();


            this.BindingContext = inputModel2;



            //  this.Wheelpicker.SelectedIndex = IndexPicker(inputModel.Temp);
            Wheelpicker.SelectedIndexChanged += (sender, args) =>
            {
                inputModel2.HandleSelectedIndexChanged();


            };

          inputModel2.SelectedIndex =inputModel2.TempLocal<= inputModel2.Qtysql? inputModel2.Temp: inputModel2.TotalQty -inputModel2.TempLocal;
        }
        //private void txtTemp_Focused(object sender, FocusEventArgs e)
        //{
            
        //    Device.BeginInvokeOnMainThread(() =>
        //    {
        //        Entry myEntry = sender as Entry;
        //        myEntry.CursorPosition = 0;
        //        myEntry.SelectionLength = myEntry.Text != null ? myEntry.Text.Length : 0;
        //    });
        //}
        
        public event EventHandler ButtonPlusClicked;

        private void btn_Plus_Clicked(object sender, EventArgs e)
        {
            ButtonPlusClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btn_Minus_Clicked(object sender, EventArgs e)
        {
            ButtonPlusClicked?.Invoke(this, EventArgs.Empty);
        }

        private void Wheelpicker_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
    }
    //------------------------------------------------------------------------------------------------------------------------
    public class InputViewModel2: BaseViewModel
    {
        private int _temp;
        private int _temp_local;
        private int _qtyed;
        private int _target;
        private int _qtysql;
        private string _prono;
        private bool _isFul;
        private bool _setOnOFF;


        public string barcode;
        public string bwbh;
        public string clbh;
        public string gsize;
        public string tua;
        public string zlbh;

        private Color _bColor;
      

        private ObservableCollection<int> _items;
        private int _selectedIndex;



        ICommand _plusCommand;
        ICommand _minusCommand;
        //ICommand _cmdReturn;
        ICommand _modifyQtyedCommand;
        ICommand _longPress;
        public InputViewModel2()
        {
            MinusCommand = new Command(Decrease);

            PlusCommand = new Command(Increase);

            //CmdReturn = new Command(ReturnNum);

            LongPress = new Command(LongPressCmd);

            ModifyQtyedCommand = new Command(modify);

          

        }
        public int Temp { get => _temp; 
            set 
            { 
                _temp = value;
                //Qtyed = TempLocal == 0 ? Temp : Math.Abs(TempLocal + Temp) >= TotalQty ? TotalQty : Math.Abs(TempLocal + Temp);

                Qtyed = Qtysql + Temp >= TotalQty ? TotalQty : Qtysql + Temp;
                OnPropertyChanged("Temp");
            } 
        }

        public int TotalQty { get => _target; set { _target = value;  OnPropertyChanged("TotalQty"); } }

        public int Qtyed { get => _qtyed; set { _qtyed = value; OnPropertyChanged("Qtyed"); } }

        public int TempLocal { get => _temp_local; set { _temp_local = value; OnPropertyChanged("TempLocal"); } }

        public ICommand PlusCommand { get => _plusCommand; set { _plusCommand = value; OnPropertyChanged("PlusCommand"); } }
        public ICommand ModifyQtyedCommand
        {
            get => _modifyQtyedCommand;
            set { _modifyQtyedCommand = value; OnPropertyChanged("ModifyQtyedCommand"); }
        }
        public ICommand MinusCommand { get => _minusCommand; set { _minusCommand = value; OnPropertyChanged("MinusCommand"); } }
        //public ICommand CmdReturn { get => _cmdReturn; set { _cmdReturn = value; OnPropertyChanged("CmdReturn"); } }

        public ICommand LongPress { get => _longPress; set { _longPress = value; OnPropertyChanged("LongPress"); } }

        public string Prono { get => _prono; 
            set 
            {
                _prono = value;

                var a = _prono.Split('_');

                barcode = a[0];

                bwbh = a[1];

                clbh = a[2];

                gsize = a[3];

                tua = a[4];

                zlbh = a[5];

                OnPropertyChanged("Prono"); 
            } 
        }

        public Color BColor
        {
            get => IsFul ?_bColor = Class.Style.ColdKidsSky.Green : Qtyed != TotalQty || TotalQty > 0 ?_bColor = Class.Style.Organ : Class.Style.EnableOff;
            set { _bColor = value; OnPropertyChanged("BColor"); }
        }

        public bool IsFul
        {
            get { if (Temp == 0 && Qtysql == TotalQty && TotalQty > 0) return _isFul = true; else return _isFul = false; }

            set {_isFul = value; SetOnOFF = TotalQty == 0 ? false : !value; OnPropertyChanged("IsFul"); }
        }

        public int Qtysql { get => _qtysql; set => _qtysql = value; }

        public bool SetOnOFF { get => _setOnOFF; set { _setOnOFF =  value; OnPropertyChanged("SetOnOFF"); } }

        public ObservableCollection<int> Items
        {
            get
            {
                _items = new ObservableCollection<int>();

                var val = Qtysql == 0 ? TotalQty : TotalQty - Qtysql;

                for (int i = 0; i <= val; i++)
                {
                    _items.Add(i);
                }
                return _items;
            }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        public int SelectedIndex { get => _selectedIndex; 
            set
            { 
                Temp=_selectedIndex = value; 
                OnPropertyChanged("SelectedIndex"); 
            } 
        }

        public void HandleSelectedIndexChanged()
        {
            try
            {
                if(SelectedIndex >-1)
                    Temp = Items[SelectedIndex];
                else
                    SelectedIndex = Temp = Items.Count - 1;
                // Qtyed = TempLocal == 0 ? Temp : Math.Abs(TempLocal + Temp) >= TotalQty ? TotalQty : Math.Abs(TempLocal + Temp);
            }
            catch (Exception)
            {

                SelectedIndex = Temp = Items.Count - 1;

            }


        }
        public void modify()
        {
            //var arr = _prono.Split('_');

            //var a = DB.DataLocal.Table.Data_Barcode(arr[0]);

            //var rs = a.Where(x => x.Prono == _prono).ToList();

            //if (rs.Any() )
            //{
            //    if (rs[0].Isfull) return;

            //    rs[0].Qty = Qtysql;

            //    rs[0].QtySql = Qtysql;

            //    //rs[0].TempQty = Qtysql == 0 ? TotalQty : Qtysql;

            //    DB.DataLocal.Table.Update_Barcode(rs[0]);

            //    rs[0].TempQty = SelectedIndex = Temp = Qtysql == 0 ? Qtyed : Math.Abs(TotalQty - Qtysql);

            //    Qtyed = Qtysql == 0 ? 0 : rs[0].QtySql;
            //}
        }

       public bool SaveLocal()
        {
            bool isDone = false;

            if (Qtyed > 0)
            {
                var exist = DB.DataLocal.Table.ExistRow_Local_LYV(barcode, tua, bwbh, clbh, gsize);

                if (exist.Any())
                {
                    var s = new Barcode_Edit_LVY();
                    s.id = exist[0].id;
                    s.Barcode = barcode;
                    s.BWBH = bwbh;
                    s.CLBH = clbh;
                    s.GXXCC = gsize;
                    s.UserDate = DateTime.Now.Ticks;
                    s.Target = _target;
                    s.LocalQty = Qtyed;
                    s.Prono = Prono;
                    s.UserID = DB.StoreLocal.Instant.UserName;
                    s.Tua = tua;
                    s.ZLBHs = zlbh;

                    isDone = DB.DataLocal.Table.Update_Local_LYV(s) > 0;

                }
                else
                {
                    var s = new Barcode_Edit_LVY();

                    s.Barcode = barcode;
                    s.BWBH = bwbh;
                    s.CLBH = clbh;
                    s.GXXCC = gsize;
                    s.UserDate = DateTime.Now.Ticks;
                    s.Target = _target;
                    s.LocalQty = Qtyed;
                    s.Prono = Prono;
                    s.UserID = DB.StoreLocal.Instant.UserName;
                    s.Tua = tua;
                    s.ZLBHs = zlbh;

                    isDone = DB.DataLocal.Table.Insert_Local_LYV(s) > 0;                    
                }

            }
          

            return isDone;
        }
        private void Increase(object obj)
        {
            if (TotalQty > 0)
            {

                if (Qtyed < TotalQty)
                {
                    int remain = TotalQty - Qtysql;

                    if (Temp + 1 > remain) Temp = remain; else Temp += 1;

                    SelectedIndex = Temp;

                    Alarm.Sound.Click();
                }
            }
            else
            {
                var btn = (Button)obj;
                btn.IsEnabled = false;
                Alarm.Sound.Error();
            }

        }

        private void Decrease(object obj)
        {
            var btn = obj as Button;

            if (TotalQty > 0)
            {

                if (Qtyed > Qtysql)
                {
          
                    Temp -= 1;

                    SelectedIndex = Temp;

                    Alarm.Sound.Click();
                }
                else
                {
                    Temp -= 1;

                    SelectedIndex = Temp;

                    Alarm.Sound.Click();

                }
            }
          
            


        }

        private void LongPressCmd(object obj)
        {
            var btn = obj as Button;
            if (btn.Text == "+")
            {
                int remain = TotalQty - Qtysql;

                if (Temp + 10 > remain) Temp = remain; else Temp += 10;

                SelectedIndex = Temp;
            }
            else
            {
                //int remain = TotalQty - Qtysql;

                //if (Temp - 10 < 0) Temp = 0; else Temp -= 10;

                Temp = 0;

                SelectedIndex = Temp;
            }

           // if (IsFul) BColor = Class.Style.Primary; else BColor = Class.Style.Organ;

            Alarm.Sound.Click();

        }
    }

    //------------------------------------------------------------------------------------------------------------------------
    public class InputViewModel : BaseViewModel
    {
        private int _temp;        
        private int _qtyed;        
        private int _target;
        private int _qtysql;

        
        
        private string _prono;

        private bool _isFul ;
        private bool _setOnOFF;


        private Color _bColor;
       
        private ObservableCollection<int> _items;
        private int _selectedIndex;
       
   

        ICommand _plusCommand;
        ICommand _minusCommand;
        //ICommand _cmdReturn;
        ICommand _modifyQtyedCommand;
        ICommand _longPress;
        

        public InputViewModel()
        {
            MinusCommand = new Command(Decrease);

            PlusCommand= new Command(Increase);

            //CmdReturn = new Command(ReturnNum);

            LongPress = new Command(LongPressCmd);

            ModifyQtyedCommand = new Command(modify);
        }

        

        public int Temp { get => _temp; set { _temp = value; OnPropertyChanged("Temp"); } }

        public int TotalQty { get => _target; set { _target = value; OnPropertyChanged("TotalQty"); } }
        
        public int Qtyed { get => _qtyed; set { _qtyed = value; OnPropertyChanged("Qtyed"); } }
        
        public ICommand PlusCommand { get => _plusCommand; set { _plusCommand = value; OnPropertyChanged("PlusCommand"); } }
        public ICommand ModifyQtyedCommand
        {
            get => _modifyQtyedCommand;
            set { _modifyQtyedCommand = value; OnPropertyChanged("ModifyQtyedCommand"); }
        }
        public ICommand MinusCommand { get => _minusCommand; set { _minusCommand = value; OnPropertyChanged("MinusCommand"); } }
        //public ICommand CmdReturn { get => _cmdReturn; set { _cmdReturn = value; OnPropertyChanged("CmdReturn"); } }

        public ICommand LongPress { get => _longPress; set { _longPress = value; OnPropertyChanged("LongPress"); } }

        public string Prono { get => _prono; set { _prono = value; OnPropertyChanged("Prono"); } }
      
      

       
      
        public Color BColor { get =>IsFul ?
                _bColor = Class.Style.Primary: Qtyed != TotalQty || TotalQty >0? 
                _bColor = Class.Style.Organ:Class.Style.EnableOff;
            set { _bColor =value; OnPropertyChanged("BColor"); } }

        public bool IsFul 
        {
            get { if ( Temp == 0 && Qtysql == TotalQty && TotalQty >0) return _isFul = true; else return _isFul = false; }
              
           set { _isFul = value; OnPropertyChanged("IsFul"); } }

        public int Qtysql { get => _qtysql; set => _qtysql = value; }

        public bool SetOnOFF { get => _setOnOFF; set { _setOnOFF = value; OnPropertyChanged("SetOnOFF"); } }

        public ObservableCollection<int> Items { get
        {
                _items = new ObservableCollection<int>();

                var val = Qtysql == 0 ? TotalQty  :TotalQty - Qtysql;
                
                for (int i = 0; i <= val; i++)
                {
                    _items.Add(i);
                }
                return _items;
        }
            set { _items = value; OnPropertyChanged("Items"); }
        }

        public int SelectedIndex { get => _selectedIndex; set { _selectedIndex = value; OnPropertyChanged("SelectedIndex"); } }
        
        public void HandleSelectedIndexChanged()
        {
            try
            {
                Temp = Items[SelectedIndex];

            }
            catch (Exception)
            {

                SelectedIndex = Temp = Items.Count - 1;

            }
            
            
        }
        public void modify()
        {
            //var arr = _prono.Split('_');

            //var a = DB.DataLocal.Table.Data_Barcode(arr[0]);
            
            //var rs = a.Where(x => x.Prono == _prono).ToList();

            //if (rs.Any() )
            //{
            //    if (rs[0].Isfull) return;

            //    rs[0].Qty = Qtysql;
                
            //    rs[0].QtySql = Qtysql;

            //    //rs[0].TempQty = Qtysql == 0 ? TotalQty : Qtysql;
                
            //    DB.DataLocal.Table.Update_Barcode(rs[0]);

            //    rs[0].TempQty = SelectedIndex = Temp = Qtysql == 0 ? Qtyed : Math.Abs(TotalQty - Qtysql);
                
            //    Qtyed = Qtysql == 0 ? 0 : rs[0].QtySql;
            //}
        }
      
        public void InserLocal()
        {

            List<Table_Barcode> list = new List<Table_Barcode>();

            var listprono = DB.DataLocal.Table.Exist_Prono_TableBarcode(_prono, out list);


            //(0) Barcode; (1) RY; (2) CLBH; (3) Size; (4) Bwbh

            if (listprono)
            {

                list[0].TempQty = TotalQty- Temp;

                list[0].isEdited = true;

                list[0].Qty =Qtysql ==0? Qtysql + Temp: Qtyed;

                list[0].UserDate = DateTime.Now.Ticks;

                list[0].UserID = DB.StoreLocal.Instant.UserName;

                if (DB.DataLocal.Table.Update_Barcode(list[0]) > 0) Alarm.Sound.Completed();



            }
            else
            {
                var arr = _prono.Split('_');

                var barcodeTable = new Table_Barcode();

                barcodeTable.Barcode = arr[0];

                barcodeTable.ZLBH = arr[1];

                barcodeTable.CLBH = arr[2];

                barcodeTable.XXCC = arr[3];

                barcodeTable.BWBH = arr[4];

                barcodeTable.Target = TotalQty;

                barcodeTable.TempQty= Temp;

                barcodeTable.Qty = Qtysql+  Temp;

                barcodeTable.UserDate = DateTime.Now.Ticks;

                barcodeTable.UserID = DB.StoreLocal.Instant.UserName;

                barcodeTable.Prono = Prono;

                barcodeTable.isEdited = true;

                if (DB.DataLocal.Table.Insert_Barcode(barcodeTable) > 0) Alarm.Sound.Completed();
            }
        }
        private void Increase(object obj)
        {
            var button = obj as Button;

            int Remain = TotalQty - Qtysql;

            if (Temp <TotalQty && Temp  < Remain  )
            {
                if (Qtyed + Temp < TotalQty)

                    Temp++;
                SelectedIndex = Temp;

               // Qtyed = TotalQty - Temp;
            }

            if (IsFul)
            {
                BColor = Class.Style.Primary;
                
            }
            else BColor = Class.Style.Organ;


            Alarm.Sound.Click();

            
        }

        private void Decrease(object obj)
        {
            var button = obj as Button;

            
            if (Temp > 0)
            {
                var a = Qtyed;

                Math.Abs(Temp--);

                SelectedIndex = Temp;
               // Math.Abs(Qtyed--);
            }

            if (IsFul) BColor = Class.Style.Primary; else BColor = Class.Style.Organ;

            Alarm.Sound.Click();
        }
     
        private void LongPressCmd(object obj)
        {
            var btn = obj as Button;
            if(btn.Text == "+")
            {
                int remain = TotalQty - Qtysql;

                if (Temp + 10 > remain) Temp = remain; else Temp += 10;

                SelectedIndex = Temp;
            }
            else
            {
                int remain = TotalQty - Qtysql;

                if (Temp - 10 < 0) Temp = 0; else Temp -= 10;

                SelectedIndex = Temp;
            }

            if (IsFul) BColor = Class.Style.Primary; else BColor = Class.Style.Organ;

            Alarm.Sound.Click();

        }
      

    }

}