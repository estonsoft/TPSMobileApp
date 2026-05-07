using TPSMobileApp.Data;
using TPSMobileApp.ViewModels;
using TPSMobileApp.Views;

namespace TPSMobileApp
{
    public partial class App : Application
    {
        public static App g_App;
        public static AppShell g_Shell;

        public static Database g_db;

        public static ItemSearchPage g_SearchPage;
        public static HomePage g_HomePage;
        public static LoginPage g_LoginPage;
        public static ShoppingCartPage g_ShoppingCartPage;
        public static ReturnCartPage g_ReturnCartPage;
        public static LabelCartPage g_LabelCartPage;
        public static CheckoutPage g_CheckoutPage;
        public static Customer g_Customer;
        public static Category g_Category;
        public static Subcategory g_Subcategory;
        public static Subsubcategory g_Subsubcategory;

        //public static List<Category> g_CategoryList;
        public static List<Category> g_HomePageCategoryList;
        public static List<Item> g_ItemList;
        public static List<Item> g_ReorderItemList;

        public static CommManager CommManager { get; set; }
        public static String g_SearchText { get; set; }
        public static String g_SectionName { get; set; }
        public static String g_ScanBarcode { get; set; }
        public static String g_UserName { get; set; }
        public static String g_ServerURL { get; set; }
        public static String g_Company { get; set; }
        public static String g_CurrentPage { get; set; }
        public static String g_SearchFromPage { get; set; }
        public static Boolean g_IsLoggedIn { get; set; }
        public static Boolean g_IsTopSellers { get; set; }
        public static Boolean g_InStockOnly { get; set; }
        public static Boolean g_IsCredits { get; set; }
        public static Boolean g_HoldForReview { get; set; }
        public static Boolean g_ForceSubmit { get; set; }
        public static Boolean g_BlockItemsNoQOH { get; set; }

        public static Boolean g_IsScandit { get; set; }
        public static Boolean g_IsSalesUser { get; set; }
        public static Boolean g_IsChainManager { get; set; }
        public static Boolean g_IsAutoAdd1 { get; set; }
        public static String g_QOHDisplay { get; set; }
        public static String g_OrderNo { get; set; }
        public static String g_HeaderTitle { get; set; }
        public static int g_ShoppingCartItems { get; set; }
        public static String g_SettingsUser { get; set; }
        public static bool g_IsScannerInit { get; set; }
        public static ScanditViewModelBase g_ScanditViewModel { get; set; }
        public static String g_IsScannerDisabled { get; set; }
        public static String g_FlyerFilename { get; set; }
        public static Boolean g_IsMonthlyAdPDFClick { get; set; }
        public static int g_MonthlyAdPage { get; set; }
        public static int g_MonthlyAdX { get; set; }
        public static int g_MonthlyAdY { get; set; }
        public static Boolean g_IsMonthlyFlyer { get; set; }
        public static Boolean g_IsRefNoLookup { get; set; }
        public static int g_FlyerStartDate { get; set; }
        public static int g_FlyerEndDate { get; set; }
        public static string g_Notes { get; set; }
        public static string g_ShoppingCartSort { get; set; }

        public class MessageKeys
        {
            public const string OnStart = nameof(OnStart);
            public const string OnSleep = nameof(OnSleep);
            public const string OnResume = nameof(OnResume);
        }

        public App(CommManager _commManager)
        {
            InitializeComponent();
            Application.Current.UserAppTheme = AppTheme.Light;
            g_App = this;
            CommManager = _commManager;

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JHaF5cWWdCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdlWXpednVURGVdVk1+XkJWZ0g=");
            App.g_db = Database.Instance();
            
            LoadSettings();
            LoadDataFromServer();
            LoadCustomerFromServer();            
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
                    App.g_ServerURL = "https://store.qwikpoint.net";
                }
                else
                {
#if DEBUG
                    App.g_ServerURL = "https://ramtest.qwikpoint.net";
#else
                App.g_ServerURL = "https://ramdistributors.qwikpoint.net";
#endif
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
            await App.CommManager.GetSettings();
            await App.RefreshAll();
            await App.RefreshOrderHistory();
        }

        protected override async void OnResume()
        {
            if (App.g_IsLoggedIn)
            {
                await App.CommManager.ValidateUserActive(App.g_UserName);
            }

            try
            {
                App.CommManager.GetSettings();
            }
            catch { }
        }
        private async Task LoadCustomerFromServer()
        {
            if (App.g_IsSalesUser || App.g_IsChainManager)
            {
                await App.CommManager.GetSalespersonCustomers(App.g_UserName);
            }
            await App.RefreshQOH();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
        public static void UpdateServerLinks()
        {
            Constants.BaseURL = App.g_ServerURL;
            Constants.SoapUrl = App.g_ServerURL + "/RemotePhoneApp.asmx";
            Constants.LogoUrl = App.g_ServerURL + "/images/logo/logo.png";
            Constants.BannerUrl = App.g_ServerURL + "/images/banner phone/";
            Constants.CategoryImageUrl = App.g_ServerURL + "/images/category/";
            Constants.ItemImageUrl = App.g_ServerURL + "/images/items/";
        }

        public static async Task RefreshAll()
        {
            if (App.g_ServerURL != "")
            {
                // start with banners  services will call next when one is done
                await App.CommManager.GetBanners();
            }
        }

        public static async Task RefreshQOH()
        {
            try
            {
                if ((App.g_Customer.CustNo != null) && (App.g_Customer.CustNo != "") && (App.g_Customer.CustNo != "0"))
                {
                    await App.CommManager.GetItemQOH2(App.g_UserName, App.g_Customer.CustNo);
                }
            }
            catch { }
        }

        public static async Task RefreshOrderHistory()
        {
            try
            {
                if ((App.g_Customer.CustNo != null) && (App.g_Customer.CustNo != "") && (App.g_Customer.CustNo != "0"))
                {
                    await App.CommManager.GetOrderHistory(App.g_Customer.CustNo);
                }
            }
            catch { }
        }
    }
}
