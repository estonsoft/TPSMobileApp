using System.Linq;
using Microsoft.Maui.Controls;
using TPSMobileApp.Controls;

namespace TPSMobileApp.Views
{
    public partial class PurchaseHistoryPage : ContentPage
    {
        public PurchaseHistoryPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "PurchaseHistoryPage";

            RefreshList();
        }

        public void RefreshList()
        {
            OrderHistoryList.ItemsSource = null;
            Task.Run(async () =>
            {
                List<OrderHeader> orderHeaders = await App.g_db.GetOrderHeaders();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OrderHistoryList.ItemsSource = orderHeaders;
                });
            });
        }

        async void OnTappedDetails(object sender, TappedEventArgs e)
        {
            var lbl = sender as OrderLabel;
            App.g_OrderNo = lbl.OrderNo;

            await App.g_Shell.GoToOrderDetail();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                loading.IsRunning = true;
                loading.IsVisible = true;
            } );
            var lbl = sender as OrderImage;
            App.g_OrderNo = lbl.OrderNo;

            try
            {
                string response = await App.CommManager.GetInvoicePDF(App.g_OrderNo);
                byte[] data = Convert.FromBase64String(response);

                // 1. Define a writable path using FileSystem helpers
                string fileName = "OrderID" + App.g_OrderNo + ".pdf"; // Added .pdf extension for the viewer
                string filePath = Path.Combine(FileSystem.Current.CacheDirectory, fileName);

                // 2. Delete if exists (optional, WriteAllBytes overwrites by default)
                if (File.Exists(filePath))
                    File.Delete(filePath);

                // 3. Write to the safe location
                File.WriteAllBytes(filePath, data);
                await App.g_Shell.GoToInvoiceViewer();
            }
            catch (Exception error)
            {                 
                await DisplayAlertAsync("Error", error.Message, "Ok");
            }
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                loading.IsRunning = false;
                loading.IsVisible = false;
            });
        }
    }
}
