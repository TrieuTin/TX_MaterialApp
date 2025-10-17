using Android.Media;
using Xamarin.Forms;

using PlayAudio.Droid;

[assembly:Dependency(typeof(AudioRender))]
namespace PlayAudio.Droid
{
    class AudioRender :Alarm.IAudioService
    {
        public void Play(string fileName)
        {
            //AssetFileDescriptor descriptor = global::Android.App.Application.Context.Assets.OpenFd(fileName);
            var player = new MediaPlayer();
            var file = global::Android.App.Application.Context.Assets.OpenFd(fileName);

            player.SetDataSource(file.FileDescriptor, file.StartOffset, file.Length);

            player.Prepared += (s, e) => { player.Start(); };
            player.Prepare();
        }
    }
}