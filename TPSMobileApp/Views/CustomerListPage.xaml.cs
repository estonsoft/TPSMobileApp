using TPSMobileApp.Controls;

namespace TPSMobileApp.Views
{
    public partial class CustomerListPage : ContentPage
    {
        private List<SalesCustomer> customers = new List<SalesCustomer>();

        public CustomerListPage()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                Console.WriteLine("InitializeComponent Error " + Environment.NewLine + ex.ToString() + Environment.NewLine + ex.StackTrace);
            }

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

            App.g_db.UpdateCustomerCartItems();

            if (PendingOrdersCheckbox.IsChecked)
            {
                customers = App.g_db.GetSalesCustomersWithPendingOrders(CustomerSearch.Text);
            }
            else
            {
                customers = App.g_db.GetSalesCustomers(CustomerSearch.Text);
            }

            foreach (SalesCustomer customer in customers)
            {
                if (customer.ShoppingCartItems > 0)
                {
                    customer.IsShoppingCart = true;
                    customer.ShoppingCartItemsDisplay = customer.ShoppingCartItems.ToString();
                }
            }

            CustomerList.ItemsSource = customers;

            App.g_CurrentPage = "CustomerListPage";
        }

        async void OnTappedSearch(object sender, EventArgs args)
        {
            RefreshList();
        }

        async void OnTappedCustomer(object sender, EventArgs args)
        {
            string OldCustNo = App.g_Customer.CustNo;

            var c = sender as CustomerStackLayout;

            SalesCustomer cust = App.g_db.FindSalesCustomer(c.CustNo);

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

            App.g_db.SaveCustomer(App.g_Customer);

            App.g_HomePage.SetLoginControls();

            App.g_db.SuspendCartItems(OldCustNo);
            App.g_db.ClearCartItems();
            //App.g_db.ClearFavorites();
            App.g_db.DeleteOrderHistory();

            App.g_db.RestoreCartItems(App.g_Customer.CustNo);

            try
            {
                if (!string.IsNullOrEmpty(App.g_Customer.CustNo) && App.g_Customer.CustNo != "0")
                {
                    App.CommManager.GetItems(App.g_Customer.CustNo, "0");
                }
            }
            catch
            {
            }

            //App.g_Shell.GoToUpdatingPage();

            await App.g_Shell.GoToHome();
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
    }
}
