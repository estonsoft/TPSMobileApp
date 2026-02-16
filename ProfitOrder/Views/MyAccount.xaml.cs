namespace TPSMobileApp.Views
{
    public partial class MyAccountPage : ContentPage
    {
        public MyAccountPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "MyAccountPage";

            CompanyName.Text = App.g_Customer.CompanyName;
            Address.Text = App.g_Customer.Address1;
            if (App.g_Customer.Address2 != "")
            {
                Address.Text += "\n" + App.g_Customer.Address2;
            }
            CityStateZip.Text = App.g_Customer.CityStateZip;
            Phone.Text = App.g_Customer.Phone;
            Email.Text = App.g_Customer.Email;
            if (App.g_Customer.CreditLimit == 0)
            {
                CreditLimit.Text = "N/A";
            }
            else
            {
                CreditLimit.Text = string.Format("{0:C}", App.g_Customer.CreditLimit);
            }
            ARBalance.Text = string.Format("{0:C}", App.g_Customer.ARBalance);
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}