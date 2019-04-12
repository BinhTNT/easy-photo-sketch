App infos:
- App Name: Easy Photo Sketch
- App Version: 1.0
- App Descriptions: Process the images saved on device and result the pencil sketching styles images
- Technology: Xamarin Forms
- Supported platform: Android
- Author: BTNT (binh.tangocthai@gmail.com)

Used Plugins:
- Xam.Plugin.Media (https://github.com/jamesmontemagno/MediaPlugin)
- Plugin.Permissions (https://github.com/jamesmontemagno/Xamarin.Plugins/tree/master/Permissions)
- Plugin.CurrentActivity (https://github.com/jamesmontemagno/CurrentActivityPlugin)

Logical structures:
- MainPage.xaml.cs: 		main xaml page
- MainViewModel.cs:			main ViewModel for whole application that is bound to MainPage.xaml content
- ImageSketchingHelper.cs: 	helper class for processing images
- DiaLogManager.cs: 		helper class for showing AlertDialog
- MediaScannerHelper.cs: 	helper class for scanning saved images
