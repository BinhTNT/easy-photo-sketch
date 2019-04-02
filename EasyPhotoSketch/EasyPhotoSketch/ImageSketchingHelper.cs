using System;
using Java.Nio;
using Android.Graphics;
using static Android.Graphics.Bitmap;
using Android.Renderscripts;

namespace EasyPhotoSketch
{
    public class ImageSketchingHelper
    {
        public const int SCALED_BMP_MAX_SIZE = 640;
        public const int RS_BLUR_RADIUS_MIN = 5;
        public const int RS_BLUR_RADIUS_MAX = 25;

        private Bitmap m_baseBitmap = null;
        private Bitmap m_scaledBitmap = null;
        private float m_rsBlurRadius = RS_BLUR_RADIUS_MIN;

        public void SetBaseBitmap(Bitmap bitmap)
        {
            m_baseBitmap = bitmap.Copy(bitmap.GetConfig(), true);
        }

        public void SetRSBlurRadius(float radiusValue)
        {
            m_rsBlurRadius = radiusValue;
        }

        public Bitmap Process()
        {
            m_scaledBitmap = GetResizedBitmap(m_baseBitmap, SCALED_BMP_MAX_SIZE);
            Bitmap sketchedBitmap = null;
            sketchedBitmap = PencilSketch(m_scaledBitmap);
            return sketchedBitmap;
        }

        /* Bitmap processing utilities */
        public Bitmap GetResizedBitmap(Bitmap image, int maxSize)
        {
            int width = image.Width;
            int height = image.Height;

            float bitmapRatio = (float)width / (float)height;
            if (bitmapRatio > 0)
            {
                width = maxSize;
                height = (int)(width / bitmapRatio);
            }
            else
            {
                height = maxSize;
                width = (int)(height * bitmapRatio);
            }
            return Bitmap.CreateScaledBitmap(image, width, height, true);
        }

        public Bitmap ConvetGrayscale(Bitmap bmpOriginal)
        {
            int width, height;
            height = bmpOriginal.Height;
            width = bmpOriginal.Width;

            Bitmap bmpGrayscale = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas c = new Canvas();
            c.SetBitmap(bmpGrayscale);
            Paint paint = new Paint();
            ColorMatrix cm = new ColorMatrix();
            cm.SetSaturation(0);
            ColorMatrixColorFilter f = new ColorMatrixColorFilter(cm);
            paint.SetColorFilter(f);
            c.DrawBitmap(bmpOriginal, 0, 0, paint);
            return bmpGrayscale;
        }

        public Bitmap InvertColor(Bitmap bmpOriginal)
        {
            ColorMatrix colorMatrix_Inverted = new ColorMatrix(new float[] {
                -1.0f, 0, 0, 0, 255, //red   
                0, -1.0f, 0, 0, 255, //green
                0, 0, -1.0f, 0, 255, //blue
                0, 0, 0, 1.0f, 0 //alpha
            });

            ColorFilter ColorFilter_Sepia = new ColorMatrixColorFilter(colorMatrix_Inverted);

            Bitmap bitmapInverted = Bitmap.CreateBitmap(bmpOriginal.Width, bmpOriginal.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas();
            canvas.SetBitmap(bitmapInverted);

            Paint paint = new Paint();

            paint.SetColorFilter(ColorFilter_Sepia);
            canvas.DrawBitmap(bmpOriginal, 0, 0, paint);

            return bitmapInverted;
        }

        public Bitmap ApplyGaussianBlur(Bitmap bmpOriginal)
        {

            double[,] GaussianBlurConfig = new double[,]{
                {1, 2, 1},
                {2, 4, 2},
                {1, 2, 1}
            };

            ConvolutionMatrix convMatrix = new ConvolutionMatrix(3);

            convMatrix.applyConfig(GaussianBlurConfig);
            convMatrix.Factor = 16;
            convMatrix.Offset = 0;

            return ConvolutionMatrix.ComputeConvolution3x3(bmpOriginal, convMatrix);
        }

        public Bitmap BlurUseRenderScript(Bitmap originalBitmap)
        {
            // Create the Renderscript instance that will do the work.
            RenderScript rs = RenderScript.Create(Android.App.Application.Context);

            // Allocate memory for Renderscript to work with
            Allocation input = Allocation.CreateFromBitmap(rs, originalBitmap);
            Allocation output = Allocation.CreateTyped(rs, input.Type);

            // Load up an instance of the specific script that we want to use.
            ScriptIntrinsicBlur script = ScriptIntrinsicBlur.Create(rs, Android.Renderscripts.Element.U8_4(rs));
            script.SetInput(input);

            // Set the blur radius
            script.SetRadius(m_rsBlurRadius);

            // Start the ScriptIntrinisicBlur
            script.ForEach(output);

            // Copy the output to the blurred bitmap
            output.CopyTo(originalBitmap);

            return originalBitmap;
        }

        public Bitmap PencilSketch(Bitmap bmpOriginal)
        {
            Bitmap bmpGrayScale = ConvetGrayscale(bmpOriginal);
            Bitmap bmpInvert = InvertColor(bmpGrayScale);
            Bitmap bmpBlur = BlurUseRenderScript(bmpInvert);//ApplyGaussianBlur(bmpInvert);     

            Bitmap result = ColorDodgeBlend(bmpBlur, bmpGrayScale);
            Canvas canvas = new Canvas();
            canvas.SetBitmap(result);
            Paint paint = new Paint();
            canvas.DrawBitmap(result, 0, 0, paint);
            return result;
        }

        private int ColorDodge(int in1, int in2)
        {
            float image = in2;
            float mask = in1;
            return ((int)((image == 255) ? image : Math.Min(255, (((long)mask << 8) / (255 - image)))));
        }

        /**
         * Blends 2 bitmaps to one and adds the color dodge blend mode to it.
        */
        private Bitmap ColorDodgeBlend(Bitmap source, Bitmap layer)
        {
            Bitmap baseBitmap = source.Copy(Config.Argb8888, true);
            Bitmap blend = layer.Copy(Config.Argb8888, false);

            IntBuffer buffBase = IntBuffer.Allocate(baseBitmap.Width * baseBitmap.Height);
            baseBitmap.CopyPixelsToBuffer(buffBase);
            buffBase.Rewind();

            IntBuffer buffBlend = IntBuffer.Allocate(blend.Width * blend.Height);
            blend.CopyPixelsToBuffer(buffBlend);
            buffBlend.Rewind();

            IntBuffer buffOut = IntBuffer.Allocate(baseBitmap.Width * baseBitmap.Height);
            buffOut.Rewind();

            while (buffOut.Position() < buffOut.Limit())
            {
                int filterInt = buffBlend.Get();
                int srcInt = buffBase.Get();

                int redValueFilter = Android.Graphics.Color.GetRedComponent(filterInt);
                int greenValueFilter = Android.Graphics.Color.GetGreenComponent(filterInt);
                int blueValueFilter = Android.Graphics.Color.GetBlueComponent(filterInt);

                int redValueSrc = Android.Graphics.Color.GetRedComponent(srcInt);
                int greenValueSrc = Android.Graphics.Color.GetGreenComponent(srcInt);
                int blueValueSrc = Android.Graphics.Color.GetBlueComponent(srcInt);

                int redValueFinal = ColorDodge(redValueFilter, redValueSrc);
                int greenValueFinal = ColorDodge(greenValueFilter, greenValueSrc);
                int blueValueFinal = ColorDodge(blueValueFilter, blueValueSrc);

                int pixel = Android.Graphics.Color.Argb(255, redValueFinal, greenValueFinal, blueValueFinal);

                buffOut.Put(pixel);
            }

            buffOut.Rewind();

            baseBitmap.CopyPixelsFromBuffer(buffOut);
            blend.Recycle();

            return baseBitmap;
        }
    }
}