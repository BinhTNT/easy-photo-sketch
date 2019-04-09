App Name: Easy Photo Sketch
App Descriptions: Process the images saved on device and result the pencil sketching styles images
App Type: Xamarin Forms
Supported Stores: Google Play
Author: BTNT (binh.tangocthai@gmail.com)

Used Plugins:
Xam.Plugin.Media
Plugin.Permissions
Plugin.CurrentActivity

Logical structures:
- MainPage.xaml.cs: 		main xaml page
- MainViewModel.cs:			main ViewModel for whole application that is bound to MainPage.xaml content
- ImageSketchingHelper.cs: 	helper class for processing images
- DiaLogManager.cs: 		helper class for showing AlertDialog
- MediaScannerHelper.cs: 	helper class for scanning saved images
