using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace MaterialTracking.ViewModels
{
    public class ModelLocal : ViewModels_Base
    {
        public ObservableCollection<DB.Table_Barcode> DataModel { get; set; }
     
       
       public ModelLocal()
        {           
            GetData();
        }
        // public Command<object> SelectionCommand { get; set; }
        //public ViewModel()
        //{
        //    SelectionCommand = new Command<object>(OnSelection);
        //}

        //private void OnSelection(object item)
        //{
        //    var selectedActor = (item as Actor);
        //}
      
        private void GetData()
        {
            var lst = DB.DataLocal.Table.Data;
            if (lst == null) return;
            DataModel = new ObservableCollection<DB.Table_Barcode>();
            foreach (DB.Table_Barcode item in lst)
            {
                DataModel.Add(new DB.Table_Barcode
                {
                    Barcode = item.Barcode,
                    BWBH = item.BWBH,
                    CLBH = item.CLBH,
                    Id = item.Id,
                    Qty = item.Qty,
                    Target = item.Target,
                    UserDate = item.UserDate,
                    UserID = item.UserID,
                    XXCC = item.XXCC,
                    ZLBH = item.ZLBH,
                    YN = item.YN,
                    isEdited = item.isEdited
                });
            }
        }
    }
  
}
