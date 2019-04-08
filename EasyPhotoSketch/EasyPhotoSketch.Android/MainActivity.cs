using System;

using Android;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Plugin.CurrentActivity;
using Plugin.Media;
using Java.IO;

namespace EasyPhotoSketch.Droid
{
    [Activity(Label = "@string/app_name", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            DialogManager.Instance().SetShowDiaLogCallback(ShowAlertDiaLog2);

            MediaScannerHelper.Instance().SetScanMediaFileCallback(ScanMedia);

            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            int PERMISSION_ALL = 1;
            string[] PERMISSIONS = {
                  Manifest.Permission.Camera,
                  Manifest.Permission.ReadExternalStorage,
                  Manifest.Permission.WriteExternalStorage
            };

            if (!HasPermissions(this, PERMISSIONS))
            {
                ActivityCompat.RequestPermissions(this, PERMISSIONS, PERMISSION_ALL);
            }

            await CrossMedia.Current.Initialize();
            Plugin.InputKit.Platforms.Droid.Config.Init(this, savedInstanceState);
        
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            //Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public static bool HasPermissions(Context context, string[] permissions)
        {
            if (context != null && permissions != null)
            {
                foreach (string permission in permissions)
                {
                    if (ContextCompat.CheckSelfPermission(context, permission) != (int)Permission.Granted)
                        return false;
                }

            }
            return true;
        }

        public void ShowAlertDiaLog2(string title, string message)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(CrossCurrentActivity.Current.Activity);
            AlertDialog dialog = builder.Create();
            dialog.SetTitle(title);
            dialog.SetMessage(message);
            dialog.SetButton("OK", (c, ev) =>
            {
                DialogManager.Instance().DoAction(DialogManager.DIALOG_ACTION.DA_OK);
            });
            dialog.SetButton2("CANCEL", (c, ev) =>
            {
                DialogManager.Instance().DoAction(DialogManager.DIALOG_ACTION.DA_CANCEL);
            });

            dialog.Show();
        }

        public void ScanMedia(string filePath)
        {
            //Broadcast the Media Scanner Intent to trigger it
            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
            File file = new File(filePath);
            Android.Net.Uri contentUri = Android.Net.Uri.FromFile(file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);
        }
    }
}