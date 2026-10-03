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
            //ReorderItemsList.ItemsSource = App.g_ReorderItemList;

            try
            {
                // Cached lists are stale snapshots; reload so QtyOrder reflects the cart.
                var reorderList = await App.g_db.GetReorderItems();
                App.g_ReorderItemList = reorderList;

                foreach (Item ri in reorderList)
                {
                    Item.SetListItem(ri, "O");
                }

                ReorderItemsList.ItemsSource = reorderList;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ReorderList error: {ex.Message}");
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

