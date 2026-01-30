using TPSMobileApp.Views;

namespace TPSMobileApp
{
    public partial class AppShell : Shell
    {

        MenuItem custMenu;
        MenuItem returnMenu;
        //MenuItem labelMenu;
        MenuItem myAccountMenu;
        MenuItem flyerMenu;

        Boolean bIsCustMenuVisible = false;
        Boolean bIsMyAccountMenuVisible = false;
        Boolean bIsMonthlyFlyerMenuVisible = false;

        public AppShell()
        {
            InitializeComponent();
            App.g_Shell = this;

            //LogoURL = Constants.LogoUrl;

            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(DeliveryOptionsPage), typeof(DeliveryOptionsPage));
            Routing.RegisterRoute(nameof(LocationsPage), typeof(LocationsPage));
            Routing.RegisterRoute(nameof(ItemSearchPage), typeof(ItemSearchPage));
            Routing.RegisterRoute(nameof(CategoryPage), typeof(CategoryPage));
            Routing.RegisterRoute(nameof(SubcategoryPage), typeof(SubcategoryPage));
            Routing.RegisterRoute(nameof(SubsubcategoryPage), typeof(SubsubcategoryPage));
            Routing.RegisterRoute(nameof(MyAccountPage), typeof(MyAccountPage));
            Routing.RegisterRoute(nameof(ShoppingCartPage), typeof(ShoppingCartPage));
            Routing.RegisterRoute(nameof(ReturnCartPage), typeof(ReturnCartPage));
            Routing.RegisterRoute(nameof(LabelCartPage), typeof(LabelCartPage));
            Routing.RegisterRoute(nameof(CheckoutPage), typeof(CheckoutPage));
            Routing.RegisterRoute(nameof(SubmitOrderPage), typeof(SubmitOrderPage));
            Routing.RegisterRoute(nameof(SubmitReturnPage), typeof(SubmitReturnPage));
            Routing.RegisterRoute(nameof(PaymentMethodPage), typeof(PaymentMethodPage));
            Routing.RegisterRoute(nameof(PurchaseHistoryPage), typeof(PurchaseHistoryPage));
            Routing.RegisterRoute(nameof(PurchaseHistoryDetailPage), typeof(PurchaseHistoryDetailPage));
            Routing.RegisterRoute(nameof(ReorderItemsPage), typeof(ReorderItemsPage));
            Routing.RegisterRoute(nameof(BarcodeScanner), typeof(BarcodeScanner));
            Routing.RegisterRoute(nameof(BarcodeScannerScandit), typeof(BarcodeScannerScandit));
            Routing.RegisterRoute(nameof(QuickEntryPage), typeof(QuickEntryPage));
            Routing.RegisterRoute(nameof(QuickEntryPageBasic), typeof(QuickEntryPageBasic));
            Routing.RegisterRoute(nameof(QuickEntryPageNoCamera), typeof(QuickEntryPageNoCamera));
            Routing.RegisterRoute(nameof(CustomerListPage), typeof(CustomerListPage));
            Routing.RegisterRoute(nameof(FlyerPDFPage), typeof(FlyerPDFPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));

            custMenu = MenuCustomers;
            returnMenu = MenuCreditCart;
            //labelMenu = MenuLabels;
            myAccountMenu = MenuMyAccount;
            flyerMenu = MenuFlyerPDF;

            Shell.SetTabBarIsVisible(this, false);
            Shell.SetNavBarIsVisible(this, false);

            SetMenu();
        }

        public void ShowLoggedInMenu()
        {
            foreach (ShellItem item in Items)
            {
                if (item.Title == "Login")
                {
                    //Items.Remove(item);
                    break;
                }
            }
        }

        public async void Logout()
        {
            App.g_db.SaveSetting("LoggedIn", "0");
            App.g_IsLoggedIn = false;
            await App.g_Shell.GoToLogin();
        }

        public async Task<int> GoToHome()
        {
            App.g_HeaderTitle = "Profit Order";
            try
            {
                await Current.GoToAsync("//HomePage");
            }
            catch
            {
                await Navigation.PopToRootAsync();
            }
            return 0;
        }
        public async Task<int> GoToShoppingCart()
        {
            if (App.g_db.GetOrderCartItems().Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "Your shopping cart is empty", "Ok");
                return 0;
            }

            App.g_HeaderTitle = "Shopping Cart";
            await Current.GoToAsync("//HomePage/ShoppingCartPage");
            return 0;
        }
        public async Task<int> GoToReturnCart()
        {
            if (App.g_db.GetReturnCartItems().Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "Your return cart is empty", "Ok");
                return 0;
            }

            App.g_HeaderTitle = "Return Cart";
            await Current.GoToAsync("//HomePage/ReturnCartPage");
            return 0;
        }
        public async Task<int> GoToLabelCart()
        {
            if (App.g_db.GetLabelCartItems().Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "Your label print cart is empty", "Ok");
                return 0;
            }

            App.g_HeaderTitle = "Print Labels";
            await Current.GoToAsync("//HomePage/LabelCartPage");
            return 0;
        }
        public async Task<int> GoToSettings()
        {
            App.g_HeaderTitle = "Settings";
            await Current.GoToAsync("//HomePage/LoginPage/SettingsPage");
            return 0;
        }
        public async Task<int> GoToScanBarcode()
        {
            App.g_HeaderTitle = "Scan Barcode";
            if (App.g_IsScannerDisabled == "1")
            {
                await Current.GoToAsync("//HomePage/QuickEntryPageNoCamera");
            }
            else
            {
                await Current.GoToAsync("//HomePage/QuickEntryPage");
            }
            return 0;
        }
        public async Task<int> GoToQuickEntry()
        {
            App.g_HeaderTitle = "Quick Entry";
            await Current.GoToAsync("//HomePage/QuickEntryPageBasic");
            return 0;
        }
        public async Task<int> GoToMyPurchases()
        {
            App.g_HeaderTitle = "Order History";
            await Current.GoToAsync("//HomePage/PurchaseHistoryPage");
            return 0;
        }
        public async Task<int> GoToCategories()
        {
            App.g_HeaderTitle = "Product Categories";
            await Current.GoToAsync("//HomePage/CategoryPage");
            return 0;
        }
        public async Task<int> GoToSubcategories()
        {
            App.g_HeaderTitle = "Product Subcategories";
            await Current.GoToAsync("//HomePage/SubcategoryPage");
            return 0;
        }
        public async Task<int> GoToSubsubcategories()
        {
            App.g_HeaderTitle = "Product Sub-subcategories";
            await Current.GoToAsync("//HomePage/SubsubcategoryPage");
            return 0;
        }
        public async Task<int> GoToLocations()
        {
            await Current.GoToAsync("//HomePage/LocationsPage");
            return 0;
        }
        public async Task<int> GoToLogin()
        {
            await Current.GoToAsync("//HomePage/LoginPage");
            return 0;
        }
        public async Task<int> GoToMyAccount()
        {
            App.g_HeaderTitle = "My Account";
            await Current.GoToAsync("//HomePage/MyAccountPage");
            return 0;
        }
        public async Task<int> GoToCustomerList()
        {
            App.g_HeaderTitle = "Customers";
            await Current.GoToAsync("//HomePage/CustomerListPage");
            return 0;
        }
        public async Task<int> GoToCheckout()
        {
            App.g_HeaderTitle = "Checkout";
            await Current.GoToAsync("//HomePage/ShoppingCartPage/CheckoutPage");
            return 0;
        }
        public async Task<int> GoToSubmitOrderPage()
        {
            App.g_HeaderTitle = "Submit Order";
            await Current.GoToAsync("//HomePage/ShoppingCartPage/CheckoutPage/SubmitOrderPage");
            return 0;
        }
        public async Task<int> GoToSubmitReturnPage()
        {
            App.g_HeaderTitle = "Submit Return";
            await Current.GoToAsync("//HomePage/ReturnCartPage/SubmitReturnPage");
            return 0;
        }
        public async Task<int> GoToFlyerPDF()
        {
            if (!App.g_IsMonthlyFlyer)
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "No active monthly ads at this time", "Ok");
                return 0;
            }

            int iNow = Convert.ToInt32(DateTime.Now.ToString("1yyMMdd"));
            if ((iNow < App.g_FlyerStartDate) || (iNow > App.g_FlyerEndDate))
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "No active monthly ads at this time", "Ok");
                return 0;
            }

            if (!File.Exists(App.g_FlyerFilename))
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "No monthly ad PDF at this time", "Ok");
                return 0;
            }

            App.g_HeaderTitle = "Monthly Ad PDF";

            await Current.GoToAsync("//HomePage/FlyerPDFPage");
            return 0;
        }
        public async Task<int> PopModal()
        {
            try
            {
                await Navigation.PopModalAsync();
            }
            catch
            {
            }
            return 0;
        }
        public async Task<int> GoToOrderDetail()
        {
            App.g_HeaderTitle = "Order Detail";
            await Current.GoToAsync("//HomePage/PurchaseHistoryPage/PurchaseHistoryDetailPage");
            return 0;
        }
        public async Task<int> GoToReorderItems()
        {
            await Current.GoToAsync("//HomePage/ReorderItemsPage");
            return 0;
        }
        public async Task<int> GoToRegister()
        {
            await Current.GoToAsync("//HomePage/LoginPage");
            return 0;
        }
        public async Task<int> GoToRegisterVerify()
        {
            await Current.GoToAsync("//HomePage/RegisterVerifyPage");
            return 0;
        }
        public async Task<int> GoToItemSearch()
        {
            if (App.g_CurrentPage != "ItemSearchPage")
            {
                App.g_HeaderTitle = "Search Products";
                await Current.GoToAsync("//HomePage/CategoryPage/ItemSearchPage");
            }
            else
            {
                try
                {
                    App.g_SearchPage.RefreshList();
                }
                catch
                {
                }
            }

            return 0;
        }

        public void ShowNavBar()
        {
            SetNavBarIsVisible(this, true);
        }

        public bool bStopNavigating = true;
        public bool bStopHome = false;
        public string sNavTo = "";
        public string sLastNavTo = "";

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            // implement your logic
            base.OnNavigating(args);
        }

        private async void MenuShoppingCart_Clicked(object sender, EventArgs e)
        {
            await GoToShoppingCart();
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuCreditCart_Clicked(object sender, EventArgs e)
        {
            await GoToReturnCart();
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuLabels_Clicked(object sender, EventArgs e)
        {
            await GoToLabelCart();
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuScanBarcode_Clicked(object sender, EventArgs e)
        {
            await GoToScanBarcode();
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuCustomers_Clicked(object sender, EventArgs e)
        {
            await GoToCustomerList();
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuQuickEntry_Clicked(object sender, EventArgs e)
        {
            await GoToQuickEntry();
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuMyPurchases_Clicked(object sender, EventArgs e)
        {
            await GoToMyPurchases();
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuCategories_Clicked(object sender, EventArgs e)
        {
            await GoToCategories();
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuLocations_Clicked(object sender, EventArgs e)
        {
            await GoToLocations();
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuLogout_Clicked(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = false;
            if (!App.g_IsLoggedIn)
            {
                await GoToLogin();
            }
            else
            {
                App.g_HomePage.ConfirmLogout();
            }
        }
        private async void MenuMyAccount_Clicked(object sender, EventArgs e)
        {
            await GoToMyAccount();
            Shell.Current.FlyoutIsPresented = false;
        }
        private async void MenuFlyerPDF_Clicked(object sender, EventArgs e)
        {
            await GoToFlyerPDF();
            Shell.Current.FlyoutIsPresented = false;
        }
        public void SetMenu()
        {
            HideCustomerMenu();
            HideMyAccountMenu();
            HideMonthlyFlyerMenu();

            if (App.g_IsMonthlyFlyer)
            {
                ShowMonthlyFlyerMenu();
            }
            if (App.g_IsLoggedIn)
            {
                if (App.g_IsSalesUser || App.g_IsChainManager)
                {
                    ShowCustomerMenu();
                }
                else
                {
                    ShowMyAccountMenu();
                }
            }
        }

        public void HideCustomerMenu()
        {
            try
            {
                foreach (ShellItem item in Items)
                {
                    if (item.Title == "Customers")
                    {
                        Items.Remove(item);
                        bIsCustMenuVisible = false;
                    }
                }
            }
            catch
            {
            }
            try
            {
                foreach (ShellItem item in Items)
                {
                    if (item.Title == "Return Cart")
                    {
                        Items.Remove(item);
                        bIsCustMenuVisible = false;
                    }
                }
            }
            catch
            {
            }
            try
            {
                foreach (ShellItem item in Items)
                {
                    if (item.Title == "Print Labels")
                    {
                        Items.Remove(item);
                        bIsCustMenuVisible = false;
                    }
                }
            }
            catch
            {
            }
        }

        public void HideMonthlyFlyerMenu()
        {
            try
            {
                foreach (ShellItem item in Items)
                {
                    if (item.Title == "Monthly Ad Flyer")
                    {
                        Items.Remove(item);
                        bIsMonthlyFlyerMenuVisible = false;
                    }
                }
            }
            catch
            {
            }
        }

        public void HideMyAccountMenu()
        {
            try
            {
                foreach (ShellItem item in Items)
                {
                    if (item.Title == "My Account")
                    {
                        Items.Remove(item);
                        bIsMyAccountMenuVisible = false;
                    }
                }
            }
            catch
            {
            }
        }

        public void ShowCustomerMenu()
        {
            HideMyAccountMenu();

            if (!bIsCustMenuVisible)
            {
                bIsCustMenuVisible = true;
                Items.Add(custMenu);
                Items.Add(returnMenu);
                //Items.Add(labelMenu);
            }
        }

        public void ShowMyAccountMenu()
        {
            HideCustomerMenu();

            if (!bIsMyAccountMenuVisible)
            {
                bIsMyAccountMenuVisible = true;
                Items.Add(myAccountMenu);
            }
        }

        public void ShowMonthlyFlyerMenu()
        {
            if (!bIsMonthlyFlyerMenuVisible)
            {
                bIsMonthlyFlyerMenuVisible = true;
                Items.Add(flyerMenu);
            }
        }
    }
}