using System; 
using System.ComponentModel; 

namespace WPF_MVVM_Template.ViewModels
{
	public class BaseModel : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged(string propertyname)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
		}

		public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;
		public static void RaiseStaticPropertyChanged(string propName)
		{
			EventHandler<PropertyChangedEventArgs> handler = StaticPropertyChanged;
			if (handler != null)
				handler(null, new PropertyChangedEventArgs(propName));

		}
	}
}
