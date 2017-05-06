using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using XrayPhotoAnalyser.Models;

namespace XrayPhotoAnalyser.Services
{
    public interface IImageModificatorService
    {
        Task<BitmapImage> InvertColoursAsync(Bitmap xrayBitmap);
        Task<Result> ManualMethodAsync(Bitmap xrayBitmap, int T);
        Task<Result> OtsuMethodAsync(Bitmap xrayBitmap);
        Task<Result> IterativeMethodAsync(Bitmap xrayBitmap);
        Task<Result> BernsenMethodAsync(Bitmap xrayBitmap, int epsilon, int windowRange, int GlobalT);
        Task<Result> NiblackMethodAsync(Bitmap xrayBitmap, double k, int windowRange);
        Task<Result> SouvolaPietikainenMethodAsync(Bitmap xrayBitmap, double k, int windowRange);
    }
}
