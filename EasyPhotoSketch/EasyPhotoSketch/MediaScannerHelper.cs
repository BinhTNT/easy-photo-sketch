namespace EasyPhotoSketch
{

    public class MediaScannerHelper
    {
        public delegate void ScanMediaFileCallback(string filePath);
        private ScanMediaFileCallback m_scanMediaFileCallback = null;
        private static MediaScannerHelper instance = null;
        public static MediaScannerHelper Instance()
        {
            if (instance == null)
            {
                instance = new MediaScannerHelper();
            }
            return instance;
        }

        public void SetScanMediaFileCallback(ScanMediaFileCallback scanMediaFileCallback)
        {
            m_scanMediaFileCallback = scanMediaFileCallback;
        }

        public void ScanMediaFile(string filePath)
        {
            m_scanMediaFileCallback?.Invoke(filePath);          
        }
    }
}