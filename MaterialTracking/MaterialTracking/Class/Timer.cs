using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;



namespace Tools
{
    public class Timer
    {
        private readonly TimeSpan _timeSpan;
        private readonly Action _callback;
        private bool _isRunning = false;

        private static CancellationTokenSource _cancellationTokenSource;

        public bool IsRunning { get => _isRunning; set => _isRunning = value; }

        public Timer(TimeSpan timeSpan, Action callback)
        {
            _timeSpan = timeSpan;
            _callback = callback;
            _cancellationTokenSource = new CancellationTokenSource();
        }
        public void Start()
        {
            var cts = _cancellationTokenSource; // safe copy
            Device.StartTimer(_timeSpan, () =>
            {
                if (cts.IsCancellationRequested)
                {

                    return IsRunning= false;
                }
                _callback.Invoke();
                return IsRunning= true; //true to continuous, false to single use
            });
        }

        public void Stop()
        {
            //if(!_isRunning)
            Interlocked.Exchange(ref _cancellationTokenSource, new CancellationTokenSource()).Cancel();
        }
    }
}
