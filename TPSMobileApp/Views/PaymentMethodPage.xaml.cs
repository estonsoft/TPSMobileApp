using TPSMobileApp.ViewModels;

namespace TPSMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentMethodPage : ContentPage
    {
        public PaymentMethodPage()
        {
            InitializeComponent();
            BindingContext = new PaymentMethodViewModel();

            App.g_CurrentPage = "PaymentMethodPage";
        }

        async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DeliveryOptionsPage
            {
                BindingContext = new PaymentMethodViewModel()
            });
        }

        void OnTapGestureRecognizerTapped_1(object sender, EventArgs args)
        {
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}