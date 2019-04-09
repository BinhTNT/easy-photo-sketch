using Xamarin.Forms;
using EasyPhotoSketch.ViewModel;

namespace EasyPhotoSketch
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
            MainViewModel.SetMainPage(this);
        }

        private async void ViewInfoButton_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new InfoPage());
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await DisplayAlert("", "Do you really want to close this application?", "Yes", "No");
                if (result)
                {
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                    }
                }
            });
            return true;
        }
    }
}
