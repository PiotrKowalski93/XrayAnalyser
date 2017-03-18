using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace XrayPhotoAnalyser.Converters
{
    public interface IBitmapConverter
    {
        Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage);
        BitmapImage BitmapToBitmapImage(Bitmap bitmap);
    }
}
