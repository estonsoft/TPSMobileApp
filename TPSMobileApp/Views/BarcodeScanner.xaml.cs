using Scandit.DataCapture.Barcode.Capture;
using TPSMobileApp.Models;

namespace TPSMobileApp.Views
{
    public partial class BarcodeScanner : ContentPage
    {
        private readonly DataCaptureManager _captureManager;

        public BarcodeScanner()
        {
            InitializeComponent();

            App.g_CurrentPage = "BarcodeScannerPage";

            _captureManager = DataCaptureManager.Instance;
            _captureManager.InitializeCamera();
            _captureManager.InitializeBarcodeCapture();

            _captureManager.BarcodeCapture.BarcodeScanned += OnBarcodeScanned;

            //var view = DataCaptureView.Create(_captureManager.DataCaptureContext);
            //Content = view;
        }

        private async void OnBarcodeScanned(object sender, BarcodeCaptureEventArgs e)
        {
            //if (e.Session.NewlyRecognizedBarcodes.Count == 0)
            //    return;

            //var barcode = e.Session.NewlyRecognizedBarcodes[0];

            //MainThread.BeginInvokeOnMainThread(() =>
            //{
            try
            {
                ResetCategories();

                try
                {
                    Vibration.Vibrate(TimeSpan.FromMilliseconds(100));
                }
                catch { }

                //App.g_ScanBarcode = barcode.Data;
                await App.g_Shell.GoToItemSearch();
            }
            catch
            {
                App.g_ScanBarcode = "9999999999999999";
                await App.g_Shell.GoToItemSearch();
            }
            //});
        }

        private void ResetCategories()
        {
            //App.g_Category ??= new Category();
            //App.g_Subcategory ??= new Category();
            //App.g_Subsubcategory ??= new Category();

            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_Subsubcategory.Code = "";
            App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _captureManager.BarcodeCapture.BarcodeScanned -= OnBarcodeScanned;
            Content = null;
        }
    }
}
