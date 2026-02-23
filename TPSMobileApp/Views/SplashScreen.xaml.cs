namespace TPSMobileApp.Views;

public partial class SplashScreen : ContentPage
{
    public SplashScreen()
    {
        InitializeComponent();
        App.g_db = Database.Instance();
        Task.Run(async() =>
        {
            await LoadSettings();
            await LoadDataFromServer();
            await LoadCustomerFromServer();
        }).ContinueWith((t) =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                App.g_IsSplashShown = true;
                App.g_Shell.GoToHome();
            });
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }



    private async Task LoadSettings()
    {
        App.g_FlyerFilename = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "MonthlyFlyer.pdf");

        if (!App.g_IsLoggedIn)
        {
            App.g_UserName = "";

            if (App.g_db.GetSetting("LoggedIn") == "1")
            {
                App.g_IsLoggedIn = true;
                App.g_UserName = App.g_db.GetSetting("UserName");
            }

            if (App.g_UserName == "app_test")
            {
                App.g_ServerURL = "https://ramtest.qwikpoint.net";
            }
            else
            {
                App.g_ServerURL = "https://ramdistributors.qwikpoint.net";
            }

            App.UpdateServerLinks();

            if (App.g_db.GetSetting("Credits") == "1")
            {
                App.g_IsCredits = true;
            }
            else
            {
                App.g_IsCredits = false;
            }

            App.g_QOHDisplay = App.g_db.GetSetting("QOHDisplay");
            App.g_IsScannerDisabled = App.g_db.GetSetting("ScannerDisabled");
            if (App.g_db.GetSetting("MonthlyFlyer") == "1")
            {
                App.g_IsMonthlyFlyer = true;
            }
            else
            {
                App.g_IsMonthlyFlyer = false;
            }
            string sFlyerStartDate = App.g_db.GetSetting("FlyerStartDate");
            if (sFlyerStartDate == "")
            {
                App.g_FlyerStartDate = 0;
            }
            else
            {
                int FlyerStartDate = 0;
                int.TryParse(sFlyerStartDate, out FlyerStartDate);
                App.g_FlyerStartDate = FlyerStartDate;
            }
            string sFlyerEndDate = App.g_db.GetSetting("FlyerEndDate");
            if (sFlyerEndDate == "")
            {
                App.g_FlyerEndDate = 0;
            }
            else
            {
                int FlyerEndDate = 0;
                int.TryParse(sFlyerEndDate, out FlyerEndDate);
                App.g_FlyerEndDate = FlyerEndDate;
            }
            if (App.g_db.GetSetting("AutoAdd1") == "1")
            {
                App.g_IsAutoAdd1 = true;
            }
            else
            {
                App.g_IsAutoAdd1 = false;
            }
            if (App.g_db.GetSetting("RefNoLookup") == "1")
            {
                App.g_IsRefNoLookup = true;
            }
            else
            {
                App.g_IsRefNoLookup = false;
            }

            App.g_Company = "";
            App.g_SearchText = "";
            App.g_SearchFromPage = "";
            App.g_ScanBarcode = "";
            App.g_SectionName = "";
            App.g_CurrentPage = "";
            App.g_IsTopSellers = false;
            App.g_OrderNo = "";
            App.g_HeaderTitle = "";

            if (App.g_db.GetSetting("IsSalesUser") == "1")
            {
                App.g_IsSalesUser = true;
            }
            else
            {
                App.g_IsSalesUser = false;
            }
            if (App.g_db.GetSetting("IsChainManager") == "1")
            {
                App.g_IsChainManager = true;
            }
            else
            {
                App.g_IsChainManager = false;
            }
            if (App.g_db.GetSetting("HoldForReview") == "1")
            {
                App.g_HoldForReview = true;
            }
            else
            {
                App.g_HoldForReview = false;
            }
            if (App.g_db.GetSetting("BlockItemsNoQOH") == "1")
            {
                App.g_BlockItemsNoQOH = true;
            }
            else
            {
                App.g_BlockItemsNoQOH = false;
            }
            App.g_IsScandit = true;
            App.g_ShoppingCartSort = App.g_db.GetSetting("ShoppingCartSort");

            App.g_IsScannerInit = false;
            App.g_ScanditViewModel = null;

            App.g_Category = new Category();
            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory = new Subcategory();
            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_Subsubcategory = new Subsubcategory();
            App.g_Subsubcategory.Code = "";
            App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";
            Constants.Load();

            Location location = new Location();
            location.Refresh();

            App.g_Customer = new Customer();
            App.g_ShoppingCartItems = App.g_db.GetCartPieces();


            try
            {
                App.g_Customer = new Customer();
                App.g_Customer = App.g_db.GetCustomer();
                if (App.g_Customer == null)
                {
                    App.g_Customer = new Customer();
                }
            }
            catch
            {
                App.g_Customer = new Customer();
            }

            //App.g_CategoryList = App.g_db.GetCategories();
            App.g_HomePageCategoryList = App.g_db.GetHomePageCategories();
            App.g_ItemList = App.g_db.GetItems();
            App.g_ReorderItemList = App.g_db.GetReorderItems();
        }

    }
    private async Task LoadDataFromServer()
    {
        if (App.g_IsLoggedIn)
        {
            await App.CommManager.ValidateUserActive(App.g_UserName);
        }
        await App.CommManager.GetSettings();
        await App.RefreshAll();

        await App.RefreshOrderHistory();
    }

    private async Task LoadCustomerFromServer()
    {
        if (App.g_IsSalesUser || App.g_IsChainManager)
        {
            await App.CommManager.GetSalespersonCustomers(App.g_UserName);
        }
        await App.RefreshQOH();
    }
}