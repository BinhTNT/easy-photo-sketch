using Android.Graphics;

namespace EasyPhotoSketch
{

    public class ConvolutionMatrix
    {
        public static int SIZE = 3;
        public double[,] Matrix;
        public double Factor = 1;
        public double Offset = 1;

        //Constructor with argument of size    
        public ConvolutionMatrix(int size)
        {
            Matrix = new double[size, size];
        }

        public void setAll(double value)
        {
            for (int x = 0; x < SIZE; ++x)
            {
                for (int y = 0; y < SIZE; ++y)
                {
                    Matrix[x, y] = value;
                }
            }
        }

        public void applyConfig(double[, ] config)
        {
            for (int x = 0; x < SIZE; ++x)
            {
                for (int y = 0; y < SIZE; ++y)
                {
                    Matrix[x, y] = config[x, y];
                }
            }
        }

        public static Bitmap ComputeConvolution3x3(Bitmap src, ConvolutionMatrix matrix)
        {
            int width = src.Width;
            int height = src.Height;
            Bitmap result = Bitmap.CreateBitmap(width, height, src.GetConfig());

            int A, R, G, B;
            int sumR, sumG, sumB;
            int[,] pixels = new int[SIZE, SIZE];

            for (int y = 0; y < height - 2; ++y)
            {
                for (int x = 0; x < width - 2; ++x)
                {

                    // get pixel matrix                
                    for (int i = 0; i < SIZE; ++i) {
                        for (int j = 0; j < SIZE; ++j)
                        {
                            pixels[i, j] = src.GetPixel(x + i, y + j);
                        }
                    }

                    // get alpha of center pixel                
                    A = Color.GetAlphaComponent(pixels[1, 1]);

                    // init color sum                
                    sumR = sumG = sumB = 0;

                    // get sum of RGB on matrix                
                    for (int i = 0; i < SIZE; ++i) {
                        for (int j = 0; j < SIZE; ++j)
                        {
                            sumR += (int)(Color.GetRedComponent(pixels[i, j]) * matrix.Matrix[i, j]);
                            sumG += (int)(Color.GetGreenComponent(pixels[i, j]) * matrix.Matrix[i, j]);
                            sumB += (int)(Color.GetBlueComponent(pixels[i, j]) * matrix.Matrix[i, j]);
                        }
                    }

                    // get final Red                
                    R = (int)(sumR / matrix.Factor + matrix.Offset);
                    if (R < 0) { R = 0; }
                    else if (R > 255) { R = 255; }

                    // get final Green                
                    G = (int)(sumG / matrix.Factor + matrix.Offset);
                    if (G < 0) { G = 0; }
                    else if (G > 255) { G = 255; }

                    // get final Blue                
                    B = (int)(sumB / matrix.Factor + matrix.Offset);
                    if (B < 0) { B = 0; }
                    else if (B > 255) { B = 255; }

                    // apply new pixel                
                    result.SetPixel(x + 1, y + 1, Color.Argb(A, R, G, B));
                }
            }
            // final image       
            return result;
        }
    }
}
