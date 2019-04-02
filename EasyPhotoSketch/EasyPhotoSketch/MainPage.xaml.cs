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
    }
}
