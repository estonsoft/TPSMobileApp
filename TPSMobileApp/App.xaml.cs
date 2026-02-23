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
        public static Boolean g_IsSplashShown { get; set; }
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
            g_IsSplashShown = false;
            CommManager = _commManager;

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjQ0OTcyOEAzMTM5MmUzNDJlMzBoTVFSazNhbDdpOTVGMVE3VXExSzNPZENwUFJ5WmhnT2ZxaDQrK2dBQ0hJPQ==");
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
