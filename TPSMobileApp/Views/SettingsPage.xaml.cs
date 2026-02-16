namespace TPSMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = this;

            App.g_CurrentPage = "Settings";
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
            }
            catch { }

            try
            {
                if (App.g_IsScannerDisabled == "1")
                {
                    DisableScanner.IsChecked = true;
                }
                else
                {
                    DisableScanner.IsChecked = false;
                }
            }
            catch
            {
                DisableScanner.IsChecked = false;
            }
        }

        private async void Save_Clicked(object sender, EventArgs e)
        {
            if (DisableScanner.IsChecked)
            {
                App.g_IsScannerDisabled = "1";
                App.g_db.SaveSetting("ScannerDisabled", "1");
            }
            else
            {
                App.g_IsScannerDisabled = "0";
                App.g_db.SaveSetting("ScannerDisabled", "0");
            }

            await App.g_Shell.GoToLogin();
        }

        protected override bool OnBackButtonPressed()
        {
            App.g_Shell.GoToLogin();
            return true;
        }
    }
}