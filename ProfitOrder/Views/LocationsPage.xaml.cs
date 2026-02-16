using TPSMobileApp.ViewModels;

namespace TPSMobileApp.Views
{
    public partial class LocationsPage : ContentPage
    {
        LocationsViewModel _viewModel;

        public LocationsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new LocationsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "LocationsPage";

            _viewModel.OnAppearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}