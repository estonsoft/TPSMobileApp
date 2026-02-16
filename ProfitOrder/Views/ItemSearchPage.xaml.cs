namespace TPSMobileApp.Views
{
    public partial class ItemSearchPage : ContentPage
    {
        string _category;
        string _subcategory;
        string _subsubcategory;
        bool _topSellers;
        bool _inStockOnly;
        string _search_text;
        List<Item> lstItems;

        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                OnPropertyChanged();
            }
        }

        public string Subcategory
        {
            get { return _subcategory; }
            set
            {
                _subcategory = value;
                OnPropertyChanged();
            }
        }

        public string Subsubcategory
        {
            get { return _subsubcategory; }
            set
            {
                _subsubcategory = value;
                OnPropertyChanged();
            }
        }

        public bool TopSellersValue
        {
            get { return _topSellers; }
            set
            {
                _topSellers = value;
                OnPropertyChanged();
            }
        }

        public bool InStockOnlyValue
        {
            get { return _inStockOnly; }
            set
            {
                _inStockOnly = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get { return _search_text; }
            set
            {
                _search_text = value;
                OnPropertyChanged();
            }
        }

        public ItemSearchPage()
        {
            InitializeComponent();
            BindingContext = this;

            App.g_SearchPage = this;

            try
            {
                TopSellersValue = App.g_IsTopSellers;
                InStockOnlyValue = App.g_InStockOnly;
                App.g_IsTopSellers = false;
            }
            catch
            {
                TopSellersValue = false;
                InStockOnlyValue = false;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "ItemSearchPage";

            Search.Text = App.g_SearchText;

            if ((App.g_QOHDisplay != "Q") && (App.g_QOHDisplay != "I"))
            {
                InStockOnly.IsChecked = false;
                InStockOnly.IsVisible = false;
                InStockLabel.IsVisible = false;
            }
            Dispatcher.Dispatch(async () =>
            {
                RefreshList();
            });
        }

        public async void RefreshList()
        {

            ItemsListSearch.ItemsSource = null;

            Category = App.g_Category.Description;
            Subcategory = App.g_Subcategory.Description;
            Subsubcategory = App.g_Subsubcategory.Description;

            if (App.g_db.GetSubcategoryCount(App.g_Category.Code) == 0)
            {
                SubcategoryLabel.IsVisible = false;
                SubsubcategoryLabel.IsVisible = false;
            }

            if (App.g_db.GetSubsubcategoryCount(App.g_Category.Code, App.g_Subcategory.Code) == 0)
            {
                SubsubcategoryLabel.IsVisible = false;
            }

            if (App.g_ScanBarcode == "")
            {
                if (App.g_IsMonthlyAdPDFClick)
                {
                    lstItems = App.g_db.SearchItemsMonthlyAdClick(App.g_MonthlyAdPage, App.g_MonthlyAdX, App.g_MonthlyAdY);
                    App.g_IsMonthlyAdPDFClick = false;
                }
                else
                {
                    lstItems = App.g_db.SearchItems(App.g_SearchText, App.g_Category, App.g_ScanBarcode, App.g_Subcategory, App.g_Subsubcategory);
                }
            }
            else
            {
                lstItems = App.g_db.SearchItemsQuickEntry(App.g_ScanBarcode);
            }

            int iItems = 0;

            foreach (Item i in lstItems)
            {
                iItems++;
                Item.SetListItem(i, "O");
            }

            ItemsListSearch.ItemsSource = lstItems;

            if (iItems == 0)
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "No items found matching search criteria", "Ok");
            }
        }

        private void OnTappedClearCategory(object sender, EventArgs e)
        {
            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            RefreshList();
        }

        private async void OnTappedCategory(object sender, EventArgs e)
        {
            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_Subsubcategory.Code = "";
            App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";

            await App.g_Shell.GoToCategories();
        }

        private void OnTappedClearSubcategory(object sender, EventArgs e)
        {
            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            RefreshList();
        }

        private async void OnTappedSubcategory(object sender, EventArgs e)
        {
            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_Subsubcategory.Code = "";
            App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";

            await App.g_Shell.GoToSubcategories();
        }

        private async void OnTappedSubsubcategory(object sender, EventArgs e)
        {
            App.g_Subsubcategory.Code = "";
            App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";

            await App.g_Shell.GoToSubsubcategories();
        }

        async void OnTopSellersClick(object sender, EventArgs e)
        {
            //App.g_Subcategory.Code = "TOPSELLERS";
            //App.g_Subcategory.Description = "TOP SELLERS";

            //RefreshList();
        }

        private void TopSellers_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            App.g_IsTopSellers = TopSellers.IsChecked;
            RefreshList();
        }

        private void InStockOnly_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            App.g_InStockOnly = InStockOnly.IsChecked;
            RefreshList();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        void OnTappedSearch(object sender, EventArgs e)
        {
            App.g_SearchText = Search.Text;
            RefreshList();
        }

        private void ItemsListSearch_ItemAppearing(object sender, Syncfusion.Maui.ListView.ItemAppearingEventArgs e)
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

