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
	public partial class UCInput_LYV : Grid
	{
		public UCInput_LYV ()
		{
			InitializeComponent ();
		}
	} 
	//------------------------------------------------------------------------------------------------------------------------
	public class InputViewModel_LYV : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


	}
}