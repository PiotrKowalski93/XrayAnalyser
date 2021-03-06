﻿/*
  In App.xaml:
  <Application.Resources>
      <vm:MvvmViewModelLocator xmlns:vm="clr-namespace:XrayPhotoAnalyser.ViewModels"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using XrayPhotoAnalyser.Converters;
using XrayPhotoAnalyser.Services;

namespace XrayPhotoAnalyser.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MvvmViewModelLocator
    {
        static MvvmViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IBitmapConverter, BitmapConverter>();
                SimpleIoc.Default.Register<IImageModificatorService, ImageModificatorService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<ChartsViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public ChartsViewModel Charts
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ChartsViewModel>();
            }
        }

    }
}