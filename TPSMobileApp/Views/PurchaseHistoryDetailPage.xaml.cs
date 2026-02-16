namespace TPSMobileApp.Views
{
    public partial class PurchaseHistoryDetailPage : ContentPage
    {
        OrderHeader _OrderHdr = new OrderHeader();

        public string OrderNo
        {
            get { return _OrderHdr.OrderNo; }
            set
            {
                _OrderHdr.OrderNo = value;
                OnPropertyChanged();
            }
        }
        public string OrderDateDisplay
        {
            get { return _OrderHdr.OrderDateDisplay; }
            set
            {
                _OrderHdr.OrderDateDisplay = value;
                OnPropertyChanged();
            }
        }
        public int Items
        {
            get { return _OrderHdr.Items; }
            set
            {
                _OrderHdr.Items = value;
                OnPropertyChanged();
            }
        }
        public int Pieces
        {
            get { return _OrderHdr.Pieces; }
            set
            {
                _OrderHdr.Pieces = value;
                OnPropertyChanged();
            }
        }
        public string TotalDisplay
        {
            get { return _OrderHdr.TotalDisplay; }
            set
            {
                _OrderHdr.TotalDisplay = value;
                OnPropertyChanged();
            }
        }

        public PurchaseHistoryDetailPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "PurchaseHistoryDetailPage";

            RefreshList();
        }

        public async void RefreshList()
        {
            OrderItemsList.ItemsSource = null;

            //Database db = new Database();
            _OrderHdr = App.g_db.GetOrderHeader(App.g_OrderNo);

            OrderNo = _OrderHdr.OrderNo;
            OrderDateDisplay = _OrderHdr.OrderDateDisplay;
            Items = _OrderHdr.Items;
            Pieces = _OrderHdr.Pieces;
            TotalDisplay = _OrderHdr.TotalDisplay;

            List<Item> lstItem = App.g_db.GetItems();

            OrderItemsList.ItemsSource = App.g_db.GetOrderDetail(App.g_OrderNo);

            foreach (OrderDetail d in (List<OrderDetail>)OrderItemsList.ItemsSource)
            {
                d.IsLoggedIn = App.g_IsLoggedIn;

                foreach (Item i in lstItem)
                {
                    if (d.ItemNo == i.ItemNo)
                    {
                        d.QtyOrder = i.QtyOrder;
                        d.IsPriceVisible = i.IsPriceVisible;
                        break;
                    }
                }

                OrderDetail.SetListItem(d);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void OrderItemsList_ItemAppearing(object sender, Syncfusion.Maui.ListView.ItemAppearingEventArgs e)
        {
            OrderDetail item = (OrderDetail)e.DataItem;

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
    }
}

