namespace TPSMobileApp.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            BindingContext = this;

            App.g_HomePage = this;

            LoadCategories();

            BannerImage.Source = ImageSource.FromUri(new Uri(Constants.LogoUrl));
            RequestCameraPermission();
            InitializeTimer();
        }

        async void RequestCameraPermission()
        {
            // Check current status
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status == PermissionStatus.Granted)
            {
                // Permission already granted, proceed with camera action
                // e.g., StartCamera();
            }
            else if (status == PermissionStatus.Denied && OperatingSystem.IsAndroid())
            {
                // Android specific: If denied, shouldShowRationale might tell you if you can ask again
                if (Permissions.ShouldShowRationale<Permissions.Camera>())
                {
                    // Show an alert to explain why you need it, then request again
                    if (await DisplayAlertAsync("Permission Needed", "We need camera access to take photos. Allow access?", "OK", "Cancel"))
                    {
                        await Permissions.RequestAsync<Permissions.Camera>();
                    }
                }
            }
            else if (status != PermissionStatus.Granted) // For iOS/Others if not granted or just denied
            {
                // Request permission
                status = await Permissions.RequestAsync<Permissions.Camera>();
                if (status == PermissionStatus.Granted)
                {
                    // Permission granted after request
                    // e.g., StartCamera();
                }
                else
                {
                    // Permission denied permanently (iOS) or still denied (Android)
                    // Inform the user they need to enable it in settings.
                }
            }
        }

        private void InitializeTimer()
        {
            Dispatcher.StartTimer(TimeSpan.FromSeconds(10), () =>
            {
                MainThread.BeginInvokeOnMainThread(UpdateBanner);
                return true;
            });
        }

        async void OnShopNow(object sender, EventArgs e)
        {
            await App.g_Shell.GoToCategories();
        }

        async void OnNewItemsAll(object sender, EventArgs e)
        {
            await App.g_Shell.GoToCategories();
        }

        private async void UpdateBanner()
        {
            //Database db = new Database();
            var banners = App.g_db.GetBanners();

            try
            {
                if (banners.Count == 0)
                {
                    BannerImage.Source = ImageSource.FromUri(new Uri(Constants.LogoUrl));
                    return;
                }
            }
            catch (Exception ex)
            {
                BannerImage.Source = ImageSource.FromUri(new Uri(Constants.LogoUrl));
                return;
            }

            int iNextIndex = 0;
            String CurrentBanner = BannerImage.Source.ToString();

            foreach (var b in banners)
            {
                iNextIndex++;

                if (CurrentBanner.Contains(b.BannerName))
                {
                    break;
                }
            }

            if (iNextIndex >= banners.Count)
            {
                iNextIndex = 0;
            }

            Banner banner = banners[iNextIndex];

            BannerImage.Source = ImageSource.FromUri(new Uri(banner.BannerURL));
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (!App.g_IsLoggedIn)
            {
                await App.g_Shell.GoToLogin();
                return;
            }

            App.g_Shell.SetMenu();

            if ((App.g_ServerURL.ToLower() == "http://muswicksales.ddns.net:8040") && (App.g_UserName != "MANDANI"))
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "Muswick Wholesale Grocers customers must download and use the Muswick app", "Ok");
                await App.g_Shell.GoToLogin();
                return;
            }

            //SearchBox.Query = App.g_SearchText;

            App.g_CurrentPage = "HomePage";

            if (App.g_Customer.Status == "3")
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "Registration request has been completed.  Please check your email for instructions.", "Ok");
                return;
            }

            SetLoginControls();

            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_SearchText = "";
            App.g_ScanBarcode = "";
            SearchText.Text = "";

            RefreshCategoryList();
        }

        public void SetLoginControls()
        {
            lblWelcome.Text = "Welcome - " + App.g_UserName;
            lblUserName.Text = App.g_Customer.CompanyName;
        }

        public void LoadCategories()
        {
            List<Category> categories = App.g_HomePageCategoryList;
            TopCategoriesCollectionView.ItemsSource = categories;
        }

        async void CategoryTapped(String Code, String Description)
        {
            Category cat = new Category();
            cat.Code = Code;
            cat.Description = Description;

            App.g_Category = cat;
            App.g_ScanBarcode = "";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            await App.g_Shell.GoToItemSearch();
            //App.g_Shell.GoToSubcategories();

            try
            {
                App.g_SearchPage.RefreshList();
            }
            catch
            {
            }
        }


        private void TopCategoriesCollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = e.CurrentSelection.FirstOrDefault() as Category;
            if (selectedCategory != null)
            {
                CategoryTapped(selectedCategory.Code, selectedCategory.Description);
            }
        }
        async void OnCategoryTapped(object sender, EventArgs e)
        {
            TappedEventArgs te = (TappedEventArgs)e;

            string CategoryCode = (string)te.Parameter;

            //Database db = new Database();
            Category cat = App.g_db.GetCategory(CategoryCode);

            App.g_Category.Code = cat.Code;
            App.g_Category.Description = cat.Description;
            App.g_Category.ImageURL = cat.ImageURL;

            App.g_ScanBarcode = "";
            App.g_SearchText = "";

            await App.g_Shell.GoToItemSearch();
        }

        async void OnSignInClick(object sender, EventArgs e)
        {
            if (!App.g_IsLoggedIn)
            {
                await App.g_Shell.GoToLogin();
            }
            else
            {
                ConfirmLogout();
            }
        }

        public async void ConfirmLogout()
        {
            bool bLogout = await DisplayAlertAsync("Profit Order", "Are you sure you wish to logout?", "Yes", "No");

            if (bLogout)
            {
                try
                {
                    App.g_db.SuspendCartItems(App.g_Customer.CustNo);
                }
                catch { }
                App.g_db.SaveSetting("LoggedIn", "0");
                App.g_IsLoggedIn = false;
                SetLoginControls();
                await App.g_Shell.GoToLogin();
            }
        }

        public async void RefreshCategoryList()
        {
            //CategoryList.ItemsSource = null;
            //CategoryList.ItemsSource = App.g_HomePageCategoryList;
        }

        async void OnPastPurchases(object sender, EventArgs e)
        {
            //Database db = new Database();
            int iReorderItems = App.g_db.GetReorderItemsCount();

            if (iReorderItems == 0)
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "Past purchases not found", "Ok");
            }
            else
            {
                await App.g_Shell.GoToReorderItems();
            }
        }

        async void OnRegisterClick(object sender, EventArgs e)
        {
            try
            {
                if (App.g_Customer.Status == "3")
                {
                    await Shell.Current.DisplayAlertAsync("Profit Order", "Registration request has been completed.  Please check your email for instructions.", "Ok");
                    return;
                }
                else if (App.g_Customer.Status == "4")
                {
                    await Shell.Current.DisplayAlertAsync("Profit Order", "Registration request needs further review.  Please check your email for instructions.", "Ok");
                    return;
                }
                else if (App.g_Customer.Status == "8")
                {
                    await Shell.Current.DisplayAlertAsync("Profit Order", "Registration request denied.  Please contact customer service for assistance.", "Ok");
                    return;
                }
                else if (App.g_Customer.Status == "9")
                {
                    await Shell.Current.DisplayAlertAsync("Profit Order", "Registration active.", "Ok");
                    return;
                }
            }
            catch (Exception ex)
            {
            }

            await App.g_Shell.GoToRegister();
        }

        protected override bool OnBackButtonPressed()
        {
            // ignore button
            return true;

            // if want to allow back button
            //base.OnBackButtonPressed();
            //return false;
        }
        void OnMenuTapped(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = true;
        }

        async void OnSearchTapped(object sender, EventArgs e)
        {
            App.g_SearchText = SearchText.Text;
            App.g_SearchFromPage = "HomePage";

            if (SearchText.Text.Length < 3)
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "Please enter at least 3 characters for search criteria", "Ok");
                return;
            }

            await App.g_Shell.GoToItemSearch();
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length >= 3)
            {
                App.g_SearchText = SearchText.Text;
                App.g_SearchFromPage = "HomePage";

                //await App.g_Shell.GoToHome();
            }
        }
    }
}
