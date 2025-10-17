using System;
using System.Collections.Generic;
using System.Text;

namespace MTM.Model
{
public    class Model_Base:PI.Mvvm.BindableBase
    {
        private string _title;
        private bool _isbusy;

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public bool IsBusy
        {
            get => _isbusy;
            set
            {
                if (SetProperty(ref _isbusy, value))
                {
                    RaisePropertyChanged(nameof(IsNotBusy));
                }
            }
        }
        public bool IsNotBusy => !IsBusy;
    }
}
