using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace XrayPhotoAnalyser.Services
{
    public interface IImageModificatorService
    {
        Task<BitmapImage> InvertColoursAsync(Bitmap xrayBitmap);
        Task<BitmapImage> OtsuMethodAsync(Bitmap xrayBitmap);
        Task<BitmapImage> HistogramBasedSegmentationAsync(Bitmap xrayBitmap);
    }
}
