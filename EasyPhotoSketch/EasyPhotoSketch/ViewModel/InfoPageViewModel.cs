using Android.Content;

namespace EasyPhotoSketch.ViewModel
{
    class InfoPageViewModel : BaseViewModel
    {
        private string m_appName = "Easy Photo Sketch";
        private string m_appVersion = "v1.0";
        private string m_appDescription = "Take awesome pencil sketching effect for your photos";
        private string m_appAuthor = "BTNT";

        public string AppName
        {
            get { return m_appName; }
            set
            {
                SetProperty(ref m_appName, value);
            }
        }

        public string AppVersion
        {
            get { return m_appVersion; }
            set
            {
                SetProperty(ref m_appVersion, value);
            }
        }

        public string AppDescription
        {
            get { return m_appDescription; }
            set
            {
                SetProperty(ref m_appDescription, value);
            }
        }

        public string AppAuthor
        {
            get { return m_appAuthor; }
            set
            {
                SetProperty(ref m_appAuthor, value);
            }
        }
    }
}
