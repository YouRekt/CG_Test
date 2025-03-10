using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace CG_Test.Filters
{
    public static class MorphologicalFilters
    {
        public static BitmapSource Erode(BitmapSource source)
        {
            WriteableBitmap bitmap = new WriteableBitmap(source);
            bitmap.Lock();

            byte[] originalPixels = new byte[bitmap.PixelHeight * bitmap.BackBufferStride];
            Marshal.Copy(bitmap.BackBuffer, originalPixels, 0, originalPixels.Length);

            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int stride = bitmap.BackBufferStride;

            unsafe
            {
                byte* ptr = (byte*)bitmap.BackBuffer;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte[] min = new byte[3] { 255, 255, 255 };

                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                int px = Math.Clamp(x + dx, 0, width - 1);
                                int py = Math.Clamp(y + dy, 0, height - 1);
                                int index = py * stride + px * 4;

                                for (int c = 0; c < 3; c++)
                                    min[c] = Math.Min(min[c], originalPixels[index + c]);
                            }
                        }

                        int outIndex = y * stride + x * 4;
                        for (int c = 0; c < 3; c++)
                            ptr[outIndex + c] = min[c];

                        ptr[outIndex + 3] = originalPixels[outIndex + 3];
                    }
                }
            }

            bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            bitmap.Unlock();

            return bitmap;
        }

        public static BitmapSource Dilate(BitmapSource source)
        {
            WriteableBitmap bitmap = new WriteableBitmap(source);
            bitmap.Lock();

            byte[] originalPixels = new byte[bitmap.PixelHeight * bitmap.BackBufferStride];
            Marshal.Copy(bitmap.BackBuffer, originalPixels, 0, originalPixels.Length);

            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int stride = bitmap.BackBufferStride;

            unsafe
            {
                byte* ptr = (byte*)bitmap.BackBuffer;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte[] max = new byte[3] { 0, 0, 0 };

                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                int px = Math.Clamp(x + dx, 0, width - 1);
                                int py = Math.Clamp(y + dy, 0, height - 1);
                                int index = py * stride + px * 4;

                                for (int c = 0; c < 3; c++)
                                    max[c] = Math.Max(max[c], originalPixels[index + c]);
                            }
                        }

                        int outIndex = y * stride + x * 4;
                        for (int c = 0; c < 3; c++)
                            ptr[outIndex + c] = max[c];

                        ptr[outIndex + 3] = originalPixels[outIndex + 3];
                    }
                }
            }

            bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            bitmap.Unlock();

            return bitmap;
        }
    }
}