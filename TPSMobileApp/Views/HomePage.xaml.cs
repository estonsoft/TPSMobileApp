using System.Diagnostics;

namespace TPSMobileApp.Views
{
    public partial class HomePage : ContentPage
    {
        List<Category> category;
        public HomePage()
        {
            InitializeComponent();

            BindingContext = this;            
            InitializeBannersAsync();
        }

        public async Task InitializeBannersAsync()
        {
            try
            {
                // Move DB call off UI thread
                _banners = await Task.Run(() => App.g_db.GetBanners());
            }
            catch
            {
                _banners = new List<Banner>();
            }

            // Fallback
            if (_banners == null || _banners.Count == 0)
            {
                BannerImage.Source = ImageSource.FromUri(new Uri(Constants.LogoUrl));
                return;
            }

            // Start rotation every 3 seconds
            Dispatcher.StartTimer(TimeSpan.FromSeconds(3), () =>
            {
                UpdateBanner();
                return true; // keep rotating
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

        private int _currentIndex = 0;
        private List<Banner> _banners = new();

        private async void UpdateBanner()
        {
            try
            {
                if (_banners == null || _banners.Count == 0)
                {
                    BannerImage.Source = ImageSource.FromUri(new Uri(Constants.LogoUrl));
                    return;
                }

                var banner = _banners[_currentIndex];

                BannerImage.Source = ImageSource.FromUri(new Uri(banner.BannerURL));

                // Move to next index (circular)
                _currentIndex = (_currentIndex + 1) % _banners.Count;
            }
            catch
            {
                BannerImage.Source = ImageSource.FromResource("logo.png");
            }
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            App.g_HomePage = this;
            LoadApp(); 
        }

        private async void LoadApp()
        {
            SearchBox.Query = App.g_SearchText;
            App.g_CurrentPage = "HomePage";

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
            LoadCategories();
            TopCategoriesCollectionView.SelectedItem = null;
            LoadingIndicator.IsVisible = false;
        }

        public void SetLoginControls()
        {
            lblWelcome.Text = "Welcome - " + App.g_UserName;
            lblUserName.Text = App.g_Customer.CompanyName;
        }

        public void LoadCategories()
        {
            
            if (category == null || category?.Count == 0)
            {
                Task.Delay(1000).ContinueWith(t =>
                {
                    category = App.g_db.GetHomePageCategories();
                    App.g_HomePageCategoryList = category;
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TopCategoriesCollectionView.ItemsSource = category;
                    });
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
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
