namespace TPSMobileApp.Views
{
    public partial class CheckoutPage : ContentPage
    {
        int iCartItems = 0;
        int iCartPieces = 0;
        decimal dCartTotal = 0;

        string sCartItems;
        string sCartPieces;
        string sCartTotal;

        public bool _IsDeliveryHighlighted;
        public bool _IsPickupHighlighted;

        public Location _Location = new Location();

        public string Notes
        {
            get { return App.g_Notes; }
            set
            {
                App.g_Notes = value;
                OnPropertyChanged();
            }
        }

        public bool HoldForReview
        {
            get { return App.g_HoldForReview; }
            set
            {
                App.g_HoldForReview = value;
                if (App.g_HoldForReview)
                {
                    App.g_db.SaveSetting("HoldForReview", "1");
                }
                else
                {
                    App.g_db.SaveSetting("HoldForReview", "0");
                }
                OnPropertyChanged();
            }
        }

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

        public Boolean IsDeliveryHighlighted
        {
            get { return _IsDeliveryHighlighted; }
            set
            {
                _IsDeliveryHighlighted = value;
                OnPropertyChanged();
            }
        }

        public Boolean IsPickupHighlighted
        {
            get { return _IsPickupHighlighted; }
            set
            {
                _IsPickupHighlighted = value;
                OnPropertyChanged();
            }
        }

        public String CompanyName
        {
            get { return App.g_Customer.CompanyName; }
            set
            {
                App.g_Customer.CompanyName = value;
                OnPropertyChanged();
            }
        }

        public String CompanyAddress
        {
            get { return App.g_Customer.Address1; }
            set
            {
                App.g_Customer.Address1 = value;
                OnPropertyChanged();
            }
        }

        public String CompanyCityStateZip
        {
            get { return App.g_Customer.CityStateZip; }
            set
            {
                App.g_Customer.CityStateZip = value;
                OnPropertyChanged();
            }
        }

        public String LocationName
        {
            get { return _Location.Name; }
            set
            {
                _Location.Name = value;
                OnPropertyChanged();
            }
        }

        public String LocationAddress
        {
            get { return _Location.Address; }
            set
            {
                _Location.Address = value;
                OnPropertyChanged();
            }
        }

        public String LocationCityStateZip
        {
            get { return _Location.CityStateZip; }
            set
            {
                _Location.CityStateZip = value;
                OnPropertyChanged();
            }
        }

        public CheckoutPage()
        {
            InitializeComponent();

            //BindingContext = _viewModel = new CheckoutViewModel();
            BindingContext = this;

            App.g_CheckoutPage = this;

            HoldForReview = App.g_HoldForReview;

            if (App.g_Customer.Delivery == 1)
            {
                IsDeliveryHighlighted = true;
                IsPickupHighlighted = false;

                CompanyName = App.g_Customer.CompanyName;
                CompanyAddress = App.g_Customer.Address1;
                CompanyCityStateZip = App.g_Customer.CityStateZip;
            }
            else
            {
                IsDeliveryHighlighted = false;
                IsPickupHighlighted = true;

                CompanyName = App.g_Customer.CompanyName;
                CompanyAddress = "Delivery Not Available";
                DeliveryAddress.TextColor = Colors.Red;
                CompanyCityStateZip = "";
            }

            //Database db = new Database();
            _Location = App.g_db.GetLocation(App.g_Customer.Warehouse);

            try
            {
                LocationName = _Location.Name;
                LocationAddress = _Location.Address;
                LocationCityStateZip = _Location.CityStateZip;
            }
            catch { }

            if (App.g_IsSalesUser || App.g_IsChainManager)
            {
                DeliveryBlock.IsVisible = false;
                Delivery.IsVisible = false;
                Pickup.IsVisible = false;
                IsDeliveryHighlighted = true;
                IsPickupHighlighted = false;
            }
        }

        private async void OnDeliveryOptionsClicked(object obj)
        {
            await Shell.Current.GoToAsync("DeliveryOptionsPage");
        }

        private async void OnPaymentMethodClicked(object obj)
        {
            await Shell.Current.GoToAsync("PaymentMethodPage");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //_viewModel.OnAppearing();

            if (App.g_ForceSubmit)
            {
                HoldForReviewCheckbox.IsVisible = false;
                HoldForReviewLabel.IsVisible = false;
            }

            RefreshList();

            App.g_CurrentPage = "CheckoutPage";
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
                        item.PriceOrderDisplay = string.Format("{0:C}", item.PriceOrder);

                        iCartItems += 1;
                        dCartTotal += item.PriceOrder * item.QtyOrder;
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

            //Database db = new Database();
            ItemsListCart.ItemsSource = App.g_db.GetOrderCartItems();

            foreach (Item i in (List<Item>)ItemsListCart.ItemsSource)
            {
                i.IsLoggedIn = App.g_IsLoggedIn;
                i.IsStepperVisible = false;
                i.IsAddToOrderVisible = false;
            }

            UpdateTotals();
        }

        async void OnPlaceOrderClicked(object sender, EventArgs e)
        {
            if (App.g_IsSalesUser)
            {
                if (dCartTotal >= App.g_Customer.MinOrderAmount)
                {
                    if (iCartPieces >= App.g_Customer.MinOrderQty)
                    {
                        await App.g_Shell.GoToSubmitOrderPage();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlertAsync("Profit Order", "Order minimum of quantity min " + App.g_Customer.MinOrderQty + " not met, please add additional items/quantities to order", "Ok");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Profit Order", "Order minimum of order min $" + App.g_Customer.MinOrderAmount + " not met, please add additional items/quantities to order", "Ok");
                }
            }
            else
            {
                if (dCartTotal >= App.g_Customer.MinOrderAmount)
                {
                    if (iCartPieces >= App.g_Customer.MinOrderQty)
                    {
                        await App.g_Shell.GoToSubmitOrderPage();
                    }
                    else
                    {
                        await Shell.Current.DisplayAlertAsync("Profit Order", "Order minimum of quantity min "+App.g_Customer.MinOrderQty+" not met, please add additional items/quantities to order", "Ok");
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Profit Order", "Order minimum of order min $" + App.g_Customer.MinOrderAmount + " not met, please add additional items/quantities to order", "Ok");
                }
            }
        }

        async void OnDelivery(object sender, EventArgs e)
        {
            if (App.g_Customer.Delivery == 1)
            {
                IsDeliveryHighlighted = true;
                IsPickupHighlighted = false;
            }
        }

        async void OnPickup(object sender, EventArgs e)
        {
            IsDeliveryHighlighted = false;
            IsPickupHighlighted = true;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        async void OnNotes(object sender, EventArgs e)
        {
            NotesLabel.IsVisible = true;
            NotesEntry.IsVisible = true;
            ItemStack.IsVisible = false;
        }

        async void OnCloseNotes(object sender, EventArgs e)
        {
            NotesLabel.IsVisible = false;
            NotesEntry.IsVisible = false;
            ItemStack.IsVisible = true;
        }
    }
}