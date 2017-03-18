using Dicom.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XrayPhotoAnalyser.Converters;
using Controls = System.Windows.Controls;
using Drawing = System.Drawing;

namespace XrayPhotoAnalyser.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private BitmapImage changedXrayBitmapImage;
        public BitmapImage ChangedXrayBitmapImage {
            get
            {
                return changedXrayBitmapImage;
            }
            set
            {
                changedXrayBitmapImage = value;
                RaisePropertyChanged("ChangedXrayBitmapImage");
            }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        private string lodedImage;
        public string LoadedImage {
            get
            {
                return lodedImage;
            }
            set
            {
                lodedImage = value;
                RaisePropertyChanged("LoadedImage");
            }
        }

        public ICommand LoadImageCommand { get; set; }
        public ICommand InvertColorsCommand { get; set; }

        private Bitmap xrayBitmap;
        private Bitmap changedXrayBitmap;

        private IBitmapConverter _bitmapConverter;

        public MainViewModel(IBitmapConverter bitmapConverter)
        {
            _bitmapConverter = bitmapConverter;

            LoadImageCommand = new RelayCommand(LoadImageAndSameAsJpg);
            InvertColorsCommand = new RelayCommand(InvertColours);
        }

        public void LoadImageAndSameAsJpg()
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();

            string dcmImagePath = fd.FileName;
            
            var image = new DicomImage(dcmImagePath);

            string filePath = @"C:\Users\Kowal\Source\Repos\XrayAnalyser\XrayPhotoAnalyser\XrayPhotoAnalyser\Images\test.jpg";

            image.RenderImage().Save(filePath);

            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(filePath, UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            
            xrayBitmap = _bitmapConverter.BitmapImage2Bitmap(src);

            LoadedImage = filePath;
        }

        public void InvertColours()
        {
            Bitmap temp = xrayBitmap;
            Bitmap bmap = (Bitmap)temp.Clone();

            Drawing.Color c;

            for (int i = 0; i < bmap.Width; i++)
            {
                for (int j = 0; j < bmap.Height; j++)
                {
                    c = bmap.GetPixel(i, j);
                    bmap.SetPixel(i, j,
                    Drawing.Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                }
            }

            changedXrayBitmap = (Bitmap)bmap.Clone();

            ChangedXrayBitmapImage = _bitmapConverter.BitmapToBitmapImage(changedXrayBitmap);
        }

       

    }
}