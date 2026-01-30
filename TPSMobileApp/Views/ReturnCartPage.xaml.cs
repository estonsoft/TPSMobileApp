namespace TPSMobileApp.Views
{
    public partial class ReturnCartPage : ContentPage
    {
        int iCartItems = 0;
        int iCartPieces = 0;
        decimal dCartTotal = 0;

        string sCartItems;
        string sCartPieces;
        string sCartTotal;

        public string CartItems
        {
            get { return iCartItems.ToString(); }
            set
            {
                sCartItems = value;
                OnPropertyChanged();
            }
        }

        public string CartPieces
        {
            get { return iCartPieces.ToString(); }
            set
            {
                sCartPieces = value;
                OnPropertyChanged();
            }
        }

        public string CartTotal
        {
            get { return string.Format("{0:C}", dCartTotal); }
            set
            {
                sCartTotal = value;
                OnPropertyChanged();
            }
        }

        public ReturnCartPage()
        {
            InitializeComponent();

            //BindingContext = _viewModel = new ShoppingCartViewModel();
            BindingContext = this;

            App.g_ReturnCartPage = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Database db = new Database();
            List<Item> items = App.g_db.GetReturnCartItems();

            if (items.Count > 0)
            {
                App.g_CurrentPage = "ReturnCartPage";

                RefreshList();
            }
        }

        public void UpdateTotals()
        {
            iCartItems = 0;
            iCartPieces = 0;
            dCartTotal = 0;

            foreach (Item item in (List<Item>)ItemsListCart.ItemsSource)
            {
                try
                {
                    if (item.QtyCredit > 0)
                    {
                        item.PriceOrder = item.Price;

                        iCartItems += 1;
                        dCartTotal += (item.PriceOrder * item.QtyCredit);
                        iCartPieces += item.QtyCredit;
                    }
                }
                catch { }
            }

            CartItems = iCartItems.ToString();
            CartPieces = iCartPieces.ToString();
            CartTotal = dCartTotal.ToString("{0:C2}");
        }

        public async void RefreshList()
        {
            ItemsListCart.ItemsSource = null;

            ItemsListCart.ItemsSource = App.g_db.GetReturnCartItems();

            foreach (Item i in (List<Item>)ItemsListCart.ItemsSource)
            {
                Item.SetListItem(i, "C");

            }

            UpdateTotals();
        }

        private async void btnCheckout_Clicked(object sender, EventArgs e)
        {
            await App.g_Shell.GoToSubmitReturnPage();
        }

        private async void btnClearCart_Clicked(object sender, EventArgs e)
        {
            bool bClear = await DisplayAlertAsync("Profit Order", "Are you sure you wish to remove all the items from your return cart?", "Yes", "No");

            if (bClear)
            {
                App.g_db.ClearReturnCartItems();
                await App.g_Shell.GoToHome();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}