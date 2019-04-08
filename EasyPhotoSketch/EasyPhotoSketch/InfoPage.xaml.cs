using Xamarin.Forms;
namespace EasyPhotoSketch
{
	public partial class InfoPage : ContentPage
	{
		public InfoPage ()
		{
			InitializeComponent ();
		}

        private async void CloseButton_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new MainPage());
        }
    }
}