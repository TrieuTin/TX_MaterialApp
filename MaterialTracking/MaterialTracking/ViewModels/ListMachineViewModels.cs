using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using MaterialTracking.Models;

namespace MaterialTracking.ViewModels
{
    public class ListMachineViewModels:BaseViewModels
    {
        private ObservableCollection<MachineNoModels> machineCollection;

        private MachineNoModels machine;

        public ObservableCollection<MachineNoModels>  MachineCollection
            {
                get => machineCollection;
                set
                {
                    machineCollection = value;
                    OnPropertyChanged();
                }
            }
        public MachineNoModels Machine { get => machine; set => machine = value; }
        public void GetListMachine()
        {
            ObservableCollection<MachineNoModels> listmachine = new ObservableCollection<MachineNoModels>();

            string sql = "select * from cutting_machine";
            var ta = MaterialTracking.DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            for (int i = 0; i < ta.Rows.Count; i++)
           {             

                listmachine.Add(new MachineNoModels { MachineName = ta.Rows[i]["MachineName"].ToString(), MachineNo = ta.Rows[i]["MachineID"].ToString() });
                
            }

            MachineCollection = listmachine;

        }
    }
}
