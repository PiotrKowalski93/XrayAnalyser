using Dicom.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XrayPhotoAnalyser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadImageBtn_Click(object sender, RoutedEventArgs e)
        {
            var image = new DicomImage(@"C:\Users\Kowal\Desktop\xr_chest.dcm");

            //string filePath = @"C:\Users\Kowal\Source\Repos\XrayAnalyser\XrayPhotoAnalyser\XrayPhotoAnalyser\Images\test" + Guid.NewGuid() +".jpg";
            string filePath = @"C:\Users\Kowal\Source\Repos\XrayAnalyser\XrayPhotoAnalyser\XrayPhotoAnalyser\Images\test.jpg";

            //image.RenderImage().Save(filePath);

            Image i = new Image();
            BitmapImage src = new BitmapImage();

            src.BeginInit();
            src.UriSource = new Uri(filePath, UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();

            i.Source = src;
            i.Stretch = Stretch.Uniform;
            
            LoadedXrayImage.Children.Add(i);
        }
    }
}
