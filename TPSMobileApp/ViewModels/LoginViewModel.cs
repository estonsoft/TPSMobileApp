namespace TPSMobileApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        public String User { get; set; }
        public String Password { get; set; }
        public bool RememberMe { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);

            try
            {
                RememberMe = App.g_Customer.RememberMe;
            }
            catch
            {
                RememberMe = false;
            }
        }

        private void OnLoginClicked(object obj)
        {
            if (User.ToLower() == "app_test")
            {
                App.g_ServerURL = "https://ramtest.qwikpoint.net";
                App.UpdateServerLinks();
            }

            App.g_IsLoggedIn = true;
            App.g_UserName = User;

            App.g_Customer.User = User;
            App.g_Customer.RememberMe = RememberMe;

            //Database db = new Database();
            App.g_db.SaveCustomer(App.g_Customer);

            App.CommManager.ValidateLogin(User, Password, App.g_Customer.UniqueId);
        }
    }
}
