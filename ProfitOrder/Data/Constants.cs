namespace TPSMobileApp
{
    public static class Constants
    {
        public static string Version = "1.016";

        // URL of ASMX service
        public static string SoapUrl = "https://www.turningpointsystems.com/RemotePhoneApp.asmx";
        public static string BaseURL = "";
        public static string LogoUrl = "";
        public static string BannerUrl = "";
        public static string CategoryImageUrl = "";
        public static string ItemImageUrl = "";

        public static string DBName = "profitorder.db3";

        public static string UserName = "";
        public static string LastUserName = "";
        public static string CustomerNo = "";

        public static string LastCategoryUpdate = "";
        public static string LastItemUpdate = "";

        public static string FieldDelimiter = "~";
        public static string RecordDelimiter = "#";

        public static int TimerHour = 3600;
        public static int TimerMinute = 60;
        public static int Timer5Minute = 300;

        public async static void Load()
        {
            try
            {
                //Database db = new Database();

                UserName = App.g_db.GetSetting("UserName");
                LastUserName = App.g_db.GetSetting("LastUserName");
                CustomerNo = App.g_db.GetSetting("CustomerNo");
                LastCategoryUpdate = App.g_db.GetSetting("LastCategoryUpdate");
                LastItemUpdate = App.g_db.GetSetting("LastItemUpdate");
            }
            catch { }
        }
    }
}
