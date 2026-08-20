using System.Windows.Input;

namespace TPSMobileApp.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private List<Category> _categories = new();
        public List<Category> categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        public HomeViewModel()
        {
            Title = ""; // "Home";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamain-quickstart"));

            OpenLogin = new Command(async () => await Shell.Current.GoToAsync("LoginPage"));
            OpenRegister = new Command(async () => await Shell.Current.GoToAsync("RegisterVerifyPage"));
            categories = new List<Category>();
        }

        public ICommand OpenLogin { get; }

        public ICommand OpenRegister { get; }

        public ICommand OpenWebCommand { get; }

        public ICommand RefreshDataCommand { get; }

        public async Task LoadCategories()
        {
            try
            {
                // 1. Fetch data on a background thread pool worker
                var topcategories = await App.g_db.GetHomePageCategories();
                categories = topcategories;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading categories on iOS: " + ex.Message);
            }
        }
    }
}