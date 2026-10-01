using System.Diagnostics;
using TPSMobileApp.Controls;

namespace TPSMobileApp.Views
{
    public partial class CustomerListPage : ContentPage
    {
        private List<SalesCustomer> customers = new List<SalesCustomer>();

        public CustomerListPage()
        {
            InitializeComponent();
            App.g_CustomerPage = this;
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "CustomerListPage";
            RefreshList();
        }

        public async void RefreshList()
        {
            CustomerList.ItemsSource = null;

            await App.g_db.UpdateCustomerCartItems();

            await Task.Run(async () =>
            {
                if (PendingOrdersCheckbox.IsChecked)
                {
                    customers = await App.g_db.GetSalesCustomersWithPendingOrders(CustomerSearch.Text);
                }
                else
                {
                    customers = await App.g_db.GetSalesCustomers(CustomerSearch.Text);
                }

                foreach (SalesCustomer customer in customers)
                {
                    if (customer.ShoppingCartItems > 0)
                    {
                        customer.IsShoppingCart = true;
                        customer.ShoppingCartItemsDisplay = customer.ShoppingCartItems.ToString();
                    }
                }
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CustomerList.ItemsSource = customers;
                });
            });
        }

        async void OnTappedSearch(object sender, EventArgs args)
        {
            RefreshList();
        }

        async void OnTappedCustomer(object sender, EventArgs args)
        {
            var c = sender as CustomerStackLayout;
            if (c == null) return;
            string selectedCustNo = c.CustNo;
            App.g_Customer = await App.g_db.GetCustomer() ?? new Customer();
            string OldCustNo = App.g_Customer.CustNo;

            LoadingAlert.IsVisible = true;
            LoadingAlert.IsEnabled = true;
            await App.ResetProgressAsync();

            CustomerList.IsVisible = false;
            await Task.Run(async () =>
            {
                SalesCustomer cust = await App.g_db.FindSalesCustomer(selectedCustNo);
                App.g_Customer.CustNo = cust.CustNo;
                App.g_Customer.CompanyName = cust.CompanyName;
                App.g_Customer.Address1 = cust.Address1;
                App.g_Customer.Address2 = cust.Address2;
                App.g_Customer.City = cust.City;
                App.g_Customer.State = cust.State;
                App.g_Customer.Zip = cust.Zip;
                App.g_Customer.CityStateZip = cust.CityStateZip;
                App.g_Customer.Phone = cust.Phone;
                App.g_Customer.Contact = cust.Contact;
                App.g_Customer.Email = cust.Email;
                App.g_Customer.Delivery = cust.Delivery;
                App.g_Customer.Warehouse = cust.Warehouse;
                App.g_Customer.TermsDesc = cust.TermsDesc;
                App.g_Customer.ARBalance = cust.ARBalance;
                App.g_Customer.CreditLimit = cust.CreditLimit;
                App.g_Customer.LastPaymentDate = cust.LastPaymentDate;
                App.g_Customer.LastOrderDate = cust.LastOrderDate;
                App.g_Customer.MinOrderAmount = cust.MinOrderAmount;
                App.g_Customer.MinOrderQty = cust.MinOrderQty;
                App.g_Customer.ShippingFee = cust.ShippingFee;

                await App.g_db.SaveCustomer(App.g_Customer);

                await App.g_db.SuspendCartItems(OldCustNo);
                await App.g_db.ClearCartItems();
                //await App.g_db.ClearFavorites();
                await App.g_db.DeleteOrderHistory();
                await App.g_db.RestoreCartItems(App.g_Customer.CustNo);
                await App.g_App.LoadAppData();
            }).ContinueWith((t) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await App.g_Shell.GoToHome();
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
            LoadingAlert.IsVisible = false;
            LoadingAlert.IsEnabled = false;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void CustomerSearch_Completed(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void PendingOrdersCheckbox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RefreshList();
        }

        private void SubmitAll_Clicked(object sender, EventArgs e)
        {

        }

        public void UpdateSyncProgress(
            double current,
            string status)
        {
            int total = 100;

            var progress = (double)current / total;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                LoadingAlert.ProgressValue = progress;
                LoadingAlert.ProgressPercentage = (int)(progress * 100);
                LoadingAlert.SyncStatus = status;
            });
        }
    }
}
