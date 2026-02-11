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

        public async void RefreshList()
        {
            OrderHistoryList.ItemsSource = null;

            List<OrderHeader> orderHeaders = App.g_db.GetOrderHeaders();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OrderHistoryList.ItemsSource = orderHeaders;
            });
        }

        async void OnTappedDetails(object sender, EventArgs args)
        {
            var lbl = sender as OrderLabel;
            App.g_OrderNo = lbl.OrderNo;

            await App.g_Shell.GoToOrderDetail();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
