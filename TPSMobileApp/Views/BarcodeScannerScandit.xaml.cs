using TPSMobileApp;
using TPSMobileApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPSMobileApp;

namespace TPSMobileApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class BarcodeScannerScandit : ContentPage
    {
        ScanditViewModelBase _viewModel;

        public BarcodeScannerScandit()
        {
            App.g_CurrentPage = "BarcodeScannerPageScandit";
            try
            {
                InitializeComponent();
                //BindingContext = _viewModel = new ScanditViewModelBase(this);
            }
            catch
            {
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override async void OnAppearing()
        {
            try
            {
                base.OnAppearing();

                //_viewModel.StartCameraScanner();
                //_viewModel.StartCameraScannerReadingAsync();
            }
            catch
            {
            }
        }

        protected override void OnDisappearing()
        {
            try
            {
                base.OnDisappearing();

                //_viewModel.StopCameraScanner();
                //_viewModel.StopCameraScannerReadingAsync();
            }
            catch
            {
            }
        }
    }
}