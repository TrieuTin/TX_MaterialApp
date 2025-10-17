using System;
using System.Collections.Generic;
using System.Text;

namespace MaterialTracking.Models
{
    public class MachineNoModels
    {
        private string _MachineNo;
        private string _MachineName;


        public string MachineNo { get => _MachineNo; set => _MachineNo = value; }
        public string MachineName { get => _MachineName; set => _MachineName = value; }
    }
}
