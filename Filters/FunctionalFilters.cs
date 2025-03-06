using System.Windows;
using System.Windows.Media.Imaging;

namespace CG_Test.Filters
{
    public static class FunctionalFilters
    {
        public static BitmapSource Invert(BitmapSource source)
        {

            WriteableBitmap bitmap = new WriteableBitmap(source);

            try
            {
                bitmap.Lock();
                unsafe
                {
                    byte* ptr = (byte*)bitmap.BackBuffer;

                    for (int y = 0; y < bitmap.PixelHeight; y++)
                    {
                        for (int x = 0; x < bitmap.PixelWidth; x++)
                        {
                            int index = y * bitmap.BackBufferStride + x * 4; // 4 bytes per pixel (B, G, R, A)

                            //B, G, R channels (leave Alpha untouched)
                            ptr[index] = (byte)Math.Clamp(255 - ptr[index], 0, 255);
                            ptr[index + 1] = (byte)Math.Clamp(255 - ptr[index + 1], 0, 255);
                            ptr[index + 2] = (byte)Math.Clamp(255 - ptr[index + 2], 0, 255);
                        }
                    }
                }
                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            }
            finally
            {
                bitmap.Unlock();
            }

            return bitmap;
        }
        public static BitmapSource BrightnessCorrection(BitmapSource source, int brightness = -100)
        {

            WriteableBitmap bitmap = new WriteableBitmap(source);

            try
            {
                bitmap.Lock();
                unsafe
                {
                    byte* ptr = (byte*)bitmap.BackBuffer;

                    for (int y = 0; y < bitmap.PixelHeight; y++)
                    {
                        for (int x = 0; x < bitmap.PixelWidth; x++)
                        {
                            int index = y * bitmap.BackBufferStride + x * 4; // 4 bytes per pixel (B, G, R, A)

                            //B, G, R channels (leave Alpha untouched)
                            ptr[index] = (byte)Math.Clamp(ptr[index] + brightness, 0, 255);
                            ptr[index + 1] = (byte)Math.Clamp(ptr[index + 1] + brightness, 0, 255);
                            ptr[index + 2] = (byte)Math.Clamp(ptr[index + 2] + brightness, 0, 255);
                        }
                    }
                }
                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            }
            finally
            {
                bitmap.Unlock();
            }

            return bitmap;
        }

        public static BitmapSource ContrastEnchancement(BitmapSource source, double contrast = 1.2)
        {

            WriteableBitmap bitmap = new WriteableBitmap(source);

            try
            {
                bitmap.Lock();
                unsafe
                {
                    byte* ptr = (byte*)bitmap.BackBuffer;

                    for (int y = 0; y < bitmap.PixelHeight; y++)
                    {
                        for (int x = 0; x < bitmap.PixelWidth; x++)
                        {
                            int index = y * bitmap.BackBufferStride + x * 4; // 4 bytes per pixel (B, G, R, A)

                            //B, G, R channels (leave Alpha untouched)
                            ptr[index] = (byte)Math.Clamp((ptr[index] - 128) * contrast + 128, 0, 255);
                            ptr[index + 1] = (byte)Math.Clamp((ptr[index + 1] - 128) * contrast + 128, 0, 255);
                            ptr[index + 2] = (byte)Math.Clamp((ptr[index + 1] - 128) * contrast + 128, 0, 255);
                        }
                    }
                }
                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            }
            finally
            {
                bitmap.Unlock();
            }

            return bitmap;
        }

        public static BitmapSource GammaCorrection(BitmapSource source, double gamma = 1.2)
        {

            WriteableBitmap bitmap = new WriteableBitmap(source);

            try
            {
                bitmap.Lock();
                unsafe
                {
                    byte* ptr = (byte*)bitmap.BackBuffer;

                    for (int y = 0; y < bitmap.PixelHeight; y++)
                    {
                        for (int x = 0; x < bitmap.PixelWidth; x++)
                        {
                            int index = y * bitmap.BackBufferStride + x * 4; // 4 bytes per pixel (B, G, R, A)

                            //B, G, R channels (leave Alpha untouched)
                            ptr[index] = (byte)Math.Clamp(255 * Math.Pow(ptr[index] / 255.0, gamma), 0, 255);
                            ptr[index + 1] = (byte)Math.Clamp(255 * Math.Pow(ptr[index + 1] / 255.0, gamma), 0, 255);
                            ptr[index + 2] = (byte)Math.Clamp(255 * Math.Pow(ptr[index + 2] / 255.0, gamma), 0, 255);
                        }
                    }
                }
                bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
            }
            finally
            {
                bitmap.Unlock();
            }

            return bitmap;
        }
    }
}
