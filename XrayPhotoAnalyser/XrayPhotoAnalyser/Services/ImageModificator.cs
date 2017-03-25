using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using XrayPhotoAnalyser.Converters;
using Drawing = System.Drawing;

namespace XrayPhotoAnalyser.Services
{
    public class ImageModificatorService : IImageModificatorService
    {
        private IBitmapConverter _bitmapConverter;

        public ImageModificatorService(IBitmapConverter bitmapConverter)
        {
            _bitmapConverter = bitmapConverter;
        }

        public async Task<BitmapImage> InvertColoursAsync(Bitmap xrayBitmap)
        {
            return await Task.Run(() =>
            {
                Bitmap temp = xrayBitmap;
                Bitmap bmap = (Bitmap)temp.Clone();

                Color c;

                for (int i = 0; i < bmap.Width; i++)
                {
                    for (int j = 0; j < bmap.Height; j++)
                    {
                        c = bmap.GetPixel(i, j);
                        bmap.SetPixel(i, j,
                        Drawing.Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                    }
                }

                var changedXrayBitmap = (Bitmap)bmap.Clone();
                return _bitmapConverter.BitmapToBitmapImage(changedXrayBitmap);
            });                      
        }

        public async Task<BitmapImage> OtsuMethodAsync(Bitmap xrayBitmap)
        {
            return await Task.Run(() =>
            {      
                // Clonowanie bitmapy
                Bitmap temp = xrayBitmap;
                Bitmap bmap = (Bitmap)temp.Clone();

                // Hoitogram z poziomami jasnosci od 0 do 255
                int[] histogram = new int[256];

                // Przygotowanie histogramu szarości
                for (int i = 0; i < xrayBitmap.Width; i++)
                {
                    for (int j = 0; j < xrayBitmap.Height; j++)
                    {
                        int brightness = (int)Math.Round(xrayBitmap.GetPixel(i,j).GetBrightness() * 255.0);
                        histogram[brightness]++;
                    }
                }

                // Zliczenie pikseli
                int totalPixels = xrayBitmap.Width * xrayBitmap.Height;

                float sum = 0;
                for (int t = 0; t < 256; t++) sum += t * histogram[t];

                float sumB = 0;
                int wB = 0;
                int wF = 0;

                float varMax = 0;
                int threshold = 0;
                
                for (int i = 0; i < 256; i++)
                {
                    wB += histogram[i];               // Weight Background
                    if (wB == 0) continue;

                    wF = totalPixels - wB;                 // Weight Foreground
                    if (wF == 0) break;

                    sumB += (float)(i * histogram[i]);

                    float mB = sumB / wB;            // Mean Background
                    float mF = (sum - sumB) / wF;    // Mean Foreground

                    // Calculate Between Class Variance
                    float varBetween = (float)wB * (float)wF * (mB - mF) * (mB - mF);

                    // Check if new maximum found
                    if (varBetween > varMax)
                    {
                        varMax = varBetween;
                        threshold = i;
                    }
                }

                Color c;

                for (int i = 0; i < bmap.Width; i++)
                {
                    for (int j = 0; j < bmap.Height; j++)
                    {
                        int brightness = (int)Math.Round(bmap.GetPixel(i, j).GetBrightness() * 255.0);

                        if (brightness >= threshold)
                        {
                            bmap.SetPixel(i, j, Color.White);
                        }
                        else
                        {
                            bmap.SetPixel(i, j, Color.Black);
                        }
                    }
                }

                var changedXrayBitmap = (Bitmap)bmap.Clone();
                return _bitmapConverter.BitmapToBitmapImage(changedXrayBitmap);
            });
                       
        }

        public async Task<BitmapImage> HistogramBasedSegmentationAsync(Bitmap xrayBitmap)
        {
            return await Task.Run(() =>
            {
                Bitmap temp = xrayBitmap;
                Bitmap bmap = (Bitmap)temp.Clone();

                // Średnia jasność tła, średnia jasność czterech narożnych pikseli
                float Ub = (bmap.GetPixel(0, 0).GetBrightness() + bmap.GetPixel(bmap.Width - 1, bmap.Height - 1).GetBrightness() +
                            bmap.GetPixel(bmap.Width - 1, 0).GetBrightness() + bmap.GetPixel(0, bmap.Height - 1).GetBrightness())/4;

                // Średnia jasność obiektu
                float Uob = (bmap.GetPixel(bmap.Width / 2, bmap.Height / 2 - 1).GetBrightness() + bmap.GetPixel(bmap.Width / 2, bmap.Height / 2 + 1).GetBrightness() +
                            bmap.GetPixel(bmap.Width / 2 - 1, bmap.Height / 2).GetBrightness() + bmap.GetPixel(bmap.Width / 2 + 1, bmap.Height / 2).GetBrightness())/4;

                // Początkowa wartość progowa
                float OldT = (Ub + Uob) / 2;
                float NewT = OldT;

                float backgroudPixelsBrightnes;
                float objectPixelsBrightness;

                int backgroudPixelsCount;
                int objectPixelsCount;

                do
                {
                    OldT = NewT;

                    backgroudPixelsBrightnes = 0;
                    objectPixelsBrightness = 0;

                    backgroudPixelsCount = 0;
                    objectPixelsCount = 0;

                    for (int i = 0; i < bmap.Width; i++)
                    {
                        for (int j = 0; j < bmap.Height; j++)
                        {
                            float brightness = bmap.GetPixel(i, j).GetBrightness();

                            if (brightness >= OldT)
                            {
                                backgroudPixelsCount++;
                                backgroudPixelsBrightnes += brightness;
                            }
                            else
                            {
                                objectPixelsCount++;
                                objectPixelsBrightness += brightness;
                            }
                        }
                    }

                    Ub = backgroudPixelsBrightnes / backgroudPixelsCount;
                    Uob = objectPixelsBrightness / objectPixelsCount;

                    NewT = (Ub + Uob) / 2;

                } while (NewT != OldT);


                // Wyznaczone T
                float T = NewT;

                // Segmentacja przez progowanie
                for (int i = 0; i < bmap.Width; i++)
                {
                    for (int j = 0; j < bmap.Height; j++)
                    {
                        float brightness = bmap.GetPixel(i, j).GetBrightness();

                        if (brightness >= T)
                        {
                            bmap.SetPixel(i, j, Color.White);
                        }
                        else
                        {
                            bmap.SetPixel(i, j, Color.Black);
                        }

                    }
                }

                var changedXrayBitmap = (Bitmap)bmap.Clone();
                return _bitmapConverter.BitmapToBitmapImage(changedXrayBitmap);
            });

        }
    }
}
