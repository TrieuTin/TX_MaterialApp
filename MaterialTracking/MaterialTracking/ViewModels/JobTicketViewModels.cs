using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using MaterialTracking.Views;
namespace MaterialTracking.ViewModels
{
    public class JobTicketViewModels
    {
        private readonly ServicesMessage.IMessageServices messageServices;
        public Command MachineNoCommand { get; set; }
        public JobTicketViewModels()
        {
            this.messageServices = DependencyService.Get<ServicesMessage.IMessageServices>();
            this.MachineNoCommand = new Command(this.MachineNoClickedAsync);
        }
        private void MachineNoClickedAsync()
        {
            Alarm.Sound.Click();
            this.messageServices.showAsyncNewPage(new MachineNoViews());
        }
    }
}
