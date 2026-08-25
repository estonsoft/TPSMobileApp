using TPSMobileApp.ViewModels;



namespace TPSMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = new LoginViewModel();
            App.g_LoginPage = this;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            User.IsEnabled = false;
            User.IsEnabled = true;
            Password.IsEnabled = false;
            Password.IsEnabled = true;
            RememberMe.IsEnabled = false;
            RememberMe.IsEnabled = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "LoginPage";

            AppVersion.Text = Constants.Version;

            try
            {
                if (Constants.LogoUrl != "")
                {
                    Logo.Source = ImageSource.FromUri(new Uri(Constants.LogoUrl));
                }
            }
            catch (Exception)
            {
            }
            if (App.g_Customer.RememberMe)
            {
                User.Text = App.g_Customer.User;
            }
            HideAnimation();
        }

        public void ShowAnimation()
        {
            User.IsEnabled = false;
            Password.IsEnabled = false;
            buttonLogin.IsEnabled = false;
            LoadingAlert.IsVisible = true;
            LoadingAlert.IsEnabled = true;
        }

        public void HideAnimation()
        {

            User.IsEnabled = true;
            Password.IsEnabled = true;
            buttonLogin.IsEnabled = true;
            LoadingAlert.IsVisible = false;
            LoadingAlert.IsEnabled = false;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void Settings_Clicked(object sender, EventArgs e)
        {
            try
            {
                App.g_SettingsUser = User.Text.ToUpper();
            }
            catch
            {
                App.g_SettingsUser = "";
            }
            App.g_HeaderTitle = "Settings";
            await App.g_Shell.GoToSettings();
        }

        private async void OnImageLoaded(object sender, EventArgs e)
        {
            if (sender is Image img && img.Source != null)
            {
                try
                {
                    // Ensure Handler and MauiContext are not null before using them
                    var handler = img.Handler;
                    var mauiContext = handler?.MauiContext;
                    if (mauiContext != null)
                    {
                        var result = await img.Source.GetPlatformImageAsync(mauiContext);

                        if (result == null)
                        {
                            img.Source = "missing_item.jpg"; // Set to yingour local resource name
                        }
                    }
                    else
                    {
                        img.Source = "missing_item.jpg";
                    }
                }
                catch (Exception)
                {
                    // If the URI is malformed or download fails immediately
                    img.Source = "missing_item.jpg";
                }
            }
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