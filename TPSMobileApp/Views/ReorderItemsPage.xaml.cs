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

            await Task.Run(() =>
            {
                try
                {
                    // Fix 5: reuse already loaded list
                    var lstItem = App.g_ItemList as List<Item>;
                    if (lstItem == null || lstItem.Count == 0) return;

                    // Fix 3: safe cast
                    if (App.g_ReorderItemList is not List<Item> reorderList
                        || reorderList.Count == 0) return;

                    // Fix 4: O(1) lookup with Dictionary
                    var itemLookup = lstItem.ToDictionary(i => i.ItemNo);

                    foreach (Item ri in reorderList)
                    {
                        ri.IsLoggedIn = App.g_IsLoggedIn;

                        if (itemLookup.TryGetValue(ri.ItemNo, out Item? matched) && matched != null)
                        {
                            ri.QtyOrder = matched.QtyOrder;
                            ri.IsPriceVisible = matched.IsPriceVisible;
                        }

                        Item.SetListItem(ri, "O");
                    }

                    // Fix 2: correct list as ItemsSource
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ReorderItemsList.ItemsSource = null;           // force refresh
                        ReorderItemsList.ItemsSource = reorderList;    // ✅ reorder list
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ReorderList error: {ex.Message}");
                }
            });
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

