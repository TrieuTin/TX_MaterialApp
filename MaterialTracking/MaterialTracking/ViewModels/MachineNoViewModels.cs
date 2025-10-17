using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MaterialTracking.ViewModels
{
    public class MachineNoViewModels:ListMachineViewModels
    {
        private readonly ServicesMessage.IMessageServices messageServices;       
        public MachineNoViewModels()
        {
            this.messageServices = DependencyService.Get<ServicesMessage.IMessageServices>();
            GetListMachine();
        }
        private void MachineClickedAsync()
        {

        }
    }
}
