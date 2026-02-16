namespace TPSMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubmitOrderPage : ContentPage
    {
        public SubmitOrderPage()
        {
            InitializeComponent();
            BindingContext = this;

            App.g_CurrentPage = "SubmitOrderPage";
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Database db = new Database();
            List<Item> lstCartItems = App.g_db.GetOrderCartItems();
            String sOrderInfo = "";

            foreach (Item item in lstCartItems)
            {
                try
                {
                    sOrderInfo += item.ItemNo.ToString() + "|";
                    sOrderInfo += item.QtyOrder.ToString() + "|";
                    sOrderInfo += "0" + "~";
                }
                catch { }
            }

            String sDeliveryPickup;
            if (App.g_CheckoutPage._IsDeliveryHighlighted)
            {
                sDeliveryPickup = "D";
            }
            else
            {
                sDeliveryPickup = "P";
            }

            int iHoldForReview = 0;
            if (App.g_CheckoutPage.HoldForReview)
            {
                iHoldForReview = 1;
            }

            App.CommManager.SubmitOrder(App.g_Customer.CustNo, "", "", "", sOrderInfo, sDeliveryPickup, App.g_UserName, App.g_Notes, iHoldForReview, "O");
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}