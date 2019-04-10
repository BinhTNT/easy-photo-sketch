using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.IO;
using Android.Graphics;
using Android.Content;

namespace EasyPhotoSketch.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private static Page sMainPage;
        private static String m_storagePath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath + "/";
        private ImageSketchingHelper m_imageSketchingHelper = null;
        Bitmap m_sketchedBitmap = null;
        ImageSource m_sketchedImageSource = null;
        float m_boldSliderValue = ImageSketchingHelper.RS_BLUR_RADIUS_MAX;
        bool m_isBusy = false;

        PermissionStatus cameraStatus;
        PermissionStatus storageStatus;

        public Command TakePhotoCommand { get; private set; }
        public Command PickPhotoCommand { get; private set; }
        public Command SavePhotoCommand { get; private set; }

        public static void SetMainPage(Page page)
        {
            sMainPage = page;
        }

        public ImageSource SketchedImageSource
        {
            get { return m_sketchedImageSource; }
            set
            {
                SetProperty(ref m_sketchedImageSource, value);
            }
        }

        public float BoldSliderValue
        {
            get { return m_boldSliderValue; }
            set
            {
                SetProperty(ref m_boldSliderValue, value);
            }
        }

        public bool IsBusy
        {
            get { return m_isBusy; }
            set
            {
                SetProperty(ref m_isBusy, value);
                TakePhotoCommand.ChangeCanExecute();
                PickPhotoCommand.ChangeCanExecute();
                SavePhotoCommand.ChangeCanExecute();
            }
        }

        public MainViewModel()
        {

            m_imageSketchingHelper = new ImageSketchingHelper();

            DialogManager.Instance().SetDialogActionCallback_OK(ExcuteDiaLogAction_OK);

            TakePhotoCommand = new Command(async () => await TakePhoto(), () => !IsBusy);
            PickPhotoCommand = new Command(async () => await PickPhoto(), () => !IsBusy);
            SavePhotoCommand = new Command(SavePhoto, () => !IsBusy);
        }

        private async Task TakePhoto()
        {

            try
            {
                var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

                if (CrossMedia.Current.IsTakePhotoSupported && cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
                {
                    try
                    {
                        IsBusy = true;

                        var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                        {
                            Directory = "com.EasyPhotoSketch",
                            Name = DateTime.Now.Ticks.ToString() + "_" + "easy_photo_sketch_base.jpg",
                            SaveToAlbum = true
                        });

                        if (file == null)
                        {
                            IsBusy = false;
                            return;
                        }

                        using (var memory_stream = new MemoryStream())
                        {
                            file.GetStream().CopyTo(memory_stream);
                            file.Dispose();
                            Bitmap bitmap = CreateBitmapFromByteArrray(memory_stream);
                            m_imageSketchingHelper.SetBaseBitmap(bitmap);
                            m_imageSketchingHelper.SetRSBlurRadius(m_boldSliderValue);
                        }

                        /*
                        Bitmap grayScale = ConvetGrayscale(m_bitmapBase);
                        Bitmap invertColor = InvertColor(grayScale);

                        var filePath = System.IO.Path.Combine(m_storagePath, "easy_sketch_grayscale.jpg");
                        var stream = new FileStream(filePath, FileMode.Create);
                        grayScale.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                        stream.Close();

                        filePath = System.IO.Path.Combine(m_storagePath, "easy_sketch_inverted.jpg");
                        stream = new FileStream(filePath, FileMode.Create);
                        invertColor.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                        stream.Close();
                        */

                        m_sketchedBitmap = m_imageSketchingHelper.Process();


                        SketchedImageSource = ImageSource.FromStream(() =>
                        {
                            return GetImageStreamFromBitmap(m_sketchedBitmap);
                        });

                        IsBusy = false;

                    }
                    catch (Exception ex)
                    {
                        IsBusy = false;
                        await sMainPage.DisplayAlert("Opps", "Something went wrong!", "OK");
                    }
                }
                else
                {
                    IsBusy = false;
                    var statusResults = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                    cameraStatus = statusResults[Permission.Camera];
                    storageStatus = statusResults[Permission.Storage];
                    return;
                }
            }
            catch(Exception ex)
            {
                IsBusy = false;
                await sMainPage.DisplayAlert("Opps", "Something went wrong!", "OK");
            }
        }


        private async Task PickPhoto()
        {
            try
            {
                storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
               
                if (CrossMedia.Current.IsPickPhotoSupported && storageStatus == PermissionStatus.Granted)
                {
                    try
                    {

                        IsBusy = true;

                        Stream stream = null;
                        var file = await CrossMedia.Current.PickPhotoAsync().ConfigureAwait(true);


                        if (file == null)
                        {
                            IsBusy = false;
                            return;
                        }

                        stream = file.GetStream();
                        file.Dispose();
                        Bitmap bitmap = CreateBitmapFromStream(stream);
                        m_imageSketchingHelper.SetBaseBitmap(bitmap);
                        m_imageSketchingHelper.SetRSBlurRadius(m_boldSliderValue);
                        m_sketchedBitmap = m_imageSketchingHelper.Process();
                        stream.Close();


                        SketchedImageSource = ImageSource.FromStream(() =>
                        {   
                            return GetImageStreamFromBitmap(m_sketchedBitmap);
                        });

                        IsBusy = false;

                    }
                    catch (Exception ex)
                    {
                        IsBusy = false;
                        await sMainPage.DisplayAlert("Opps", "Something went wrong!", "OK");
                    }
                }
                else
                {
                    IsBusy = false;
                    var statusResult = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });                     
                    storageStatus = statusResult[Permission.Storage];
                    return;
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await sMainPage.DisplayAlert("Opps", "Something went wrong!", "OK");
            }
        }

        private void SavePhoto()
        {
            DialogManager.Instance().ShowDialog("", "Save Image?");
        }

        private void ExcuteDiaLogAction_OK()
        {
            SaveSketchImage();

        }

        private void SaveSketchImage()
        {
            if (m_sketchedBitmap != null)
            {
                string state = Android.OS.Environment.ExternalStorageState;
                if (Android.OS.Environment.MediaMounted.Equals(state))
                {
                    var filePath = System.IO.Path.Combine(m_storagePath, DateTime.Now.Ticks.ToString() + "_" + "easy_photo_sketch.jpg");
                    var stream = new FileStream(filePath, FileMode.Create);
                    m_sketchedBitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                    stream.Close();
                    //Make the image available in Gallery by invoke media scanner event
                    MediaScannerHelper.Instance().ScanMediaFile(filePath);
                }
            }
        }


        public Bitmap CreateBitmapFromByteArrray(MemoryStream stream)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            options.InMutable = true;
            Bitmap bitmap = BitmapFactory.DecodeByteArray(stream.GetBuffer(), 0, (int)stream.Length);
            return bitmap;
        }

        public Bitmap CreateBitmapFromStream(Stream stream)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            options.InMutable = true;
            Bitmap bitmap = BitmapFactory.DecodeStream(stream);
            return bitmap;
        }

        public MemoryStream GetImageStreamFromBitmap(Bitmap bitmap)
        {
            var memory_stream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, memory_stream);
            var imageStream = new MemoryStream(memory_stream.ToArray());
            return imageStream;
        }
    }
}
