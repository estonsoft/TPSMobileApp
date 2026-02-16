using CommunityToolkit.Mvvm.Messaging;

namespace TPSMobileApp.Views
{
    public partial class ShoppingCartPage : ContentPage
    {
        int iCartItems = 0;
        int iCartPieces = 0;
        decimal dCartTotal = 0;

        string sCartItems;
        string sCartPieces;
        string sCartTotal;

        List<Item> lstItems = new List<Item>();

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

        public ShoppingCartPage()
        {
            InitializeComponent();

            //BindingContext = _viewModel = new ShoppingCartViewModel();
            BindingContext = this;

            App.g_ShoppingCartPage = this;

            WeakReferenceMessenger.Default.Register<ShoppingCartPage>(
            this,
            (r, m) =>
            {
                RefreshList();
            });
            //MessagingCenter.Subscribe<ShoppingCartPage>(this, "RefreshShoppingCart", (sender) =>
            //{

            //});
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            lstItems = App.g_db.GetOrderCartItems();

            if (lstItems.Count > 0)
            {
                App.g_CurrentPage = "ShoppingCartPage";

                RefreshList();
            }
            else
            {
                Dispatcher.Dispatch(async () =>
                {
                    await Shell.Current.Navigation.PopToRootAsync();
                    await App.g_Shell.GoToHome();
                    await Shell.Current.DisplayAlertAsync("Profit Order", "Your shopping cart is empty", "Ok");
                });
            }

            if (App.g_IsLoggedIn)
            {
                btnCheckout.IsVisible = true;
                btnSignIn.IsVisible = false;
            }
            else
            {
                btnCheckout.IsVisible = false;
                btnSignIn.IsVisible = true;
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
                    if (item.QtyOrder > 0)
                    {
                        item.PriceOrder = item.Price;

                        iCartItems += 1;
                        dCartTotal += (item.PriceOrder * item.QtyOrder);
                        iCartPieces += item.QtyOrder;
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

            lstItems = App.g_db.GetOrderCartItems();

            foreach (Item i in lstItems)
            {
                Item.SetListItem(i, "O");
            }

            ItemsListCart.ItemsSource = lstItems;

            UpdateTotals();
        }

        private async void btnCheckout_Clicked(object sender, EventArgs e)
        {
            await App.g_Shell.GoToCheckout();
        }

        private async void btnSignIn_Clicked(object sender, EventArgs e)
        {
            await App.g_Shell.GoToLogin();
        }

        private async void btnClearCart_Clicked(object sender, EventArgs e)
        {
            bool bClear = await DisplayAlertAsync("Profit Order", "Are you sure you wish to remove all the items from your shopping cart?", "Yes", "No");

            if (bClear)
            {
                App.g_db.ClearOrderCartItems();
                await App.g_Shell.GoToHome();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void ItemsListCart_ItemAppearing(object sender, Syncfusion.Maui.ListView.ItemAppearingEventArgs e)
        {
            Item item = (Item)e.DataItem;

            if (item.QtyOrder > 0)
            {
                item.IsStepperVisible = true;
                item.IsAddToOrderVisible = false;
            }
            else
            {
                item.IsStepperVisible = false;
                item.IsAddToOrderVisible = true;
            }
        }

        private void btnCheckout_Clicked_1(object sender, EventArgs e)
        {

        }
    }
}