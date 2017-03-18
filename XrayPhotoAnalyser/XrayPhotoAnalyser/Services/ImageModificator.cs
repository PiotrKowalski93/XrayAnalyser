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
                // Dobor progu jasnosci TODO
                float t = 0.8F;

                Bitmap temp = xrayBitmap;
                Bitmap bmap = (Bitmap)temp.Clone();

                Color c;

                for (int i = 0; i < bmap.Width; i++)
                {
                    for (int j = 0; j < bmap.Height; j++)
                    {
                        float brightness = bmap.GetPixel(i, j).GetBrightness();

                        if (brightness >= t)
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
