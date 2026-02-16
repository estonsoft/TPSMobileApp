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

        //private async void OnRegisterClicked(object obj)
        //{
        //    await App.g_Shell.GoToRegisterVerify();
        //}

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

            if (Constants.LogoUrl != "")
            {
                Logo.Source = ImageSource.FromUri(new Uri(Constants.LogoUrl));
            }

            if (App.g_Customer.RememberMe)
            {
                User.Text = App.g_Customer.User;
                //Password.Focus();
            }
            else
            {
                //User.Focus();
            }
        }

        public void HideAnimation()
        {
            WaitImage.IsVisible = false;
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
    }
}