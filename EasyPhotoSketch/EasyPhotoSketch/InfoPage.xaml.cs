using Xamarin.Forms;
using EasyPhotoSketch.ViewModel;

namespace EasyPhotoSketch
{
	public partial class InfoPage : ContentPage
	{
		public InfoPage ()
		{
			InitializeComponent();
            BindingContext = new InfoPageViewModel();
        }

        private async void CloseButton_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}