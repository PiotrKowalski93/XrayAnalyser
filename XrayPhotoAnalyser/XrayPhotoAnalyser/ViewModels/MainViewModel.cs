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
using XrayPhotoAnalyser.Models;
using XrayPhotoAnalyser.Services;
using XrayPhotoAnalyser.Views;
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
        #region Properties
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
                return !isBusy;
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
        #endregion

        public ICommand LoadImageCommand { get; set; }
        public ICommand InvertColorsCommand { get; set; }
        public ICommand StartOtsuMethodCommaand { get; set; }
        public ICommand BasicThresholdingCommand { get; set; }
        public ICommand ShowChartsCommand { get; set; }
        public ICommand StartBernsenMethodCommaand { get; set; }
        public ICommand StartNiblackMethodCommand { get; set; }
        public ICommand StartSouvolaPietikainenMethodCommand { get; set; }

        private Bitmap xrayBitmap;
        private IBitmapConverter _bitmapConverter;
        private IImageModificatorService _imageModyficator;

        public MainViewModel(IImageModificatorService imageModificator, IBitmapConverter bitmapConverter)
        {
            _bitmapConverter = bitmapConverter;
            _imageModyficator = imageModificator;

            LoadImageCommand = new RelayCommand(LoadImageAndSaveAsJpg);
            InvertColorsCommand = new RelayCommand(InvertColours);
            StartOtsuMethodCommaand = new RelayCommand(StartOtsuMethod);
            BasicThresholdingCommand = new RelayCommand(BasicThresholding);
            ShowChartsCommand = new RelayCommand(ShowCharts);
            StartBernsenMethodCommaand = new RelayCommand(BernsenMethod);
            StartNiblackMethodCommand = new RelayCommand(NiblackMethod);
            StartSouvolaPietikainenMethodCommand = new RelayCommand(SouvolaPietikainenMethod);

        }
        
        public void LoadImageAndSaveAsJpg()
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.ShowDialog();

            string dcmImagePath = fd.FileName;

            var image = new DicomImage(dcmImagePath);

            string filePath = @"C:\Users\Kowal\Source\Repos\XrayAnalyser\XrayPhotoAnalyser\XrayPhotoAnalyser\Images\" + Guid.NewGuid() + ".jpg";
            image.RenderImage().Save(filePath);

            BitmapImage src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri(filePath, UriKind.Absolute);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();

            xrayBitmap = _bitmapConverter.BitmapImage2Bitmap(src);

            GlobalData.XrayBitmap = src;

            GlobalData.SavedJPGImagePath = filePath;
            LoadedImage = filePath;
        }
                 
        private void ShowCharts()
        {
            ChartsView chv = new ChartsView();
            chv.Show();
        }

        private async void InvertColours()
        {
            IsBusy = true;

            ChangedXrayBitmapImage = await _imageModyficator.InvertColoursAsync(xrayBitmap);

            IsBusy = false;
        }

        private async void StartOtsuMethod()
        {
            IsBusy = true;

            ChangedXrayBitmapImage = await _imageModyficator.OtsuMethodAsync(xrayBitmap);

            IsBusy = false;
        }

        private async void BasicThresholding()
        {
            IsBusy = true;

            ChangedXrayBitmapImage = await _imageModyficator.IterativeMethodAsync(xrayBitmap);

            IsBusy = false;
        }

        private async void BernsenMethod()
        {
            IsBusy = true;

            ChangedXrayBitmapImage = await _imageModyficator.BernsenMethodAsync(xrayBitmap);

            IsBusy = false;
        }

        private async void NiblackMethod()
        {
            IsBusy = true;

            ChangedXrayBitmapImage = await _imageModyficator.NiblackMethodAsync(xrayBitmap);

            IsBusy = false;
        }

        private async void SouvolaPietikainenMethod()
        {
            IsBusy = true;

            ChangedXrayBitmapImage = await _imageModyficator.SouvolaPietikainenMethodAsync(xrayBitmap);

            IsBusy = false;
        }

    }
}