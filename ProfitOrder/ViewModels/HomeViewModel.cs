using System.Windows.Input;

namespace TPSMobileApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public HomeViewModel()
        {
            Title = ""; // "Home";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamain-quickstart"));

            OpenLogin = new Command(async () => await Shell.Current.GoToAsync("LoginPage"));
            OpenRegister = new Command(async () => await Shell.Current.GoToAsync("RegisterVerifyPage"));
        }

        public ICommand OpenLogin { get; }

        public ICommand OpenRegister { get; }

        public ICommand OpenWebCommand { get; }
    }
}