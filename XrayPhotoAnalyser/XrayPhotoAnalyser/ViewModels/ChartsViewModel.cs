using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using XrayPhotoAnalyser.Converters;
using XrayPhotoAnalyser.Models;

namespace XrayPhotoAnalyser.ViewModels
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ChartsViewModel : ViewModelBase
    {
        private BitmapImage xrayBitmapImage;
        private Bitmap xrayBitmap;

        private bool isBusy;
        public bool IsBusy
        {
            get
            {
                return !isBusy;
            }
            set
            {
                isBusy = value;
                RaisePropertyChanged("IsBusy");
            }
        }

        private BitmapImage imageBitmap;
        public BitmapImage ImageBitmap
        {
            get
            {
                return imageBitmap;
            }
            set
            {
                imageBitmap = value;
                RaisePropertyChanged("ImageBitmap");
            }
        }

        private int horizontalSliderValue;
        public int HorizontalSliderValue
        {
            get
            {
                return horizontalSliderValue;
            }
            set
            {
                horizontalSliderValue = value;
                RaisePropertyChanged("HorizontalSliderValue");
            }
        }

        private int verticalSliderValue;
        public int VerticalSliderValue
        {
            get
            {
                return verticalSliderValue;
            }
            set
            {
                verticalSliderValue = value;
                RaisePropertyChanged("VerticalSliderValue");
            }
        }

        private ObservableCollection<KeyValuePair<double, double>> verticalBrightness;
        public ObservableCollection<KeyValuePair<double, double>> VerticalBrightness
        {
            get
            {
                return verticalBrightness;
            }
            set
            {
                verticalBrightness = value;
                RaisePropertyChanged("VerticalBrightness");
            }
        }

        private ObservableCollection<KeyValuePair<double, double>> horizontalBrightness;
        public ObservableCollection<KeyValuePair<double, double>> HorizontalBrightness
        {
            get
            {
                return horizontalBrightness;
            }
            set
            {
                horizontalBrightness = value;
                RaisePropertyChanged("HorizontalBrightness");
            }
        }

        public ICommand GenerateChartsCommand { get; set; }

        private IBitmapConverter _bitmapoConverter;
        
        public ChartsViewModel(IBitmapConverter bitmapConverter)
        {
            GenerateChartsCommand = new RelayCommand(GenerateCharts);
            horizontalBrightness = new ObservableCollection<KeyValuePair<double, double>>();
            verticalBrightness = new ObservableCollection<KeyValuePair<double, double>>();

            _bitmapoConverter = bitmapConverter;

            ImageBitmap = GlobalData.XrayBitmap;
            xrayBitmap = _bitmapoConverter.BitmapImage2Bitmap(ImageBitmap);            
        }

        private void GenerateCharts()
        {
            IsBusy = true;

            horizontalBrightness.Clear();
            verticalBrightness.Clear();

            int rowNumber = (xrayBitmap.Width * verticalSliderValue) / 100;
            int columnNumber = (xrayBitmap.Height * horizontalSliderValue) / 100;

            for (int i = 0; i < xrayBitmap.Width; i++)
            {
                HorizontalBrightness.Add(new KeyValuePair<double, double>(i, xrayBitmap.GetPixel(i, rowNumber).GetBrightness()));
            }

            for (int i = 0; i < xrayBitmap.Height; i++)
            {
                VerticalBrightness.Add(new KeyValuePair<double, double>(i, xrayBitmap.GetPixel(columnNumber, i).GetBrightness()));
            }
            
            IsBusy = false;
        }
    }
}