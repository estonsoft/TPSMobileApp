namespace TPSMobileApp.Views
{
    public partial class ReorderItemsPage : ContentPage
    {
        public ReorderItemsPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "ReorderItemsPage";

            RefreshList();
        }

        public async void RefreshList()
        {
            ReorderItemsList.ItemsSource = null;
            ReorderItemsList.ItemsSource = App.g_ReorderItemList;

            List<Item> lstItem = App.g_db.GetItems();

            foreach (Item ri in (List<Item>)ReorderItemsList.ItemsSource)
            {
                ri.IsLoggedIn = App.g_IsLoggedIn;

                foreach (Item i in lstItem)
                {
                    if (ri.ItemNo == i.ItemNo)
                    {
                        ri.QtyOrder = i.QtyOrder;
                        ri.IsPriceVisible = i.IsPriceVisible;
                        break;
                    }
                }

                Item.SetListItem(ri, "O");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void ReorderItemsList_ItemAppearing(object sender, Syncfusion.Maui.ListView.ItemAppearingEventArgs e)
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
    }
}

