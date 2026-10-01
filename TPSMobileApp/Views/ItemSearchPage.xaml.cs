using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TPSMobileApp.Views
{
    public partial class ItemSearchPage : ContentPage, INotifyPropertyChanged
    {
        string _category;
        string _subcategory;
        string _subsubcategory;
        bool _topSellers;
        bool _inStockOnly;
        string _search_text;
        private const int PageSize = 30;
        private readonly ObservableCollection<Item> lstItems = new();
        private int _nextItemOffset;
        private int _searchGeneration;
        private bool _isLoadingItems;
        private bool _hasMoreItems;
        private bool _isBarcodeSearch;
        private bool _isMonthlyAdSearch;


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
            int generation = ++_searchGeneration;
            _nextItemOffset = 0;
            _hasMoreItems = true;
            _isBarcodeSearch = !string.IsNullOrEmpty(App.g_ScanBarcode);
            _isMonthlyAdSearch = !_isBarcodeSearch && App.g_IsMonthlyAdPDFClick;
            lstItems.Clear();
            ItemsListSearch.ItemsSource = lstItems;

            Category = App.g_Category.Description;
            Subcategory = App.g_Subcategory.Description;
            Subsubcategory = App.g_Subsubcategory.Description;

            if (await App.g_db.GetSubcategoryCount(App.g_Category.Code) == 0)
            {
                SubcategoryLabel.IsVisible = false;
                SubsubcategoryLabel.IsVisible = false;
            }

            if (await App.g_db.GetSubsubcategoryCount(App.g_Category.Code, App.g_Subcategory.Code) == 0)
            {
                SubsubcategoryLabel.IsVisible = false;
            }

            int itemCount = await LoadNextItemsPageAsync(generation);
            if (generation != _searchGeneration)
            {
                return;
            }

            if (itemCount == 0 && App.g_Category.Description != "ALL CATEGORIES")
            {
                bool answer = await Shell.Current.DisplayAlertAsync(
                    "Profit Order",
                    "No items found in selected category. Do you want to search in all categories?",
                    "Yes",
                    "No");

                if (answer)
                {
                    App.g_SearchText = Search.Text;
                    App.g_Category.Code = "";
                    App.g_Category.Description = "ALL CATEGORIES";

                    App.g_Subcategory.Code = "";
                    App.g_Subcategory.Description = "ALL SUBCATEGORIES";

                    await App.g_Shell.GoToItemSearch();
                }
                else
                {
                    // User tapped 'No' - Handle cancellation or do nothing
                }
            }
            else if (itemCount == 0)
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "No items found in selected category. Please modify your search.", "Cancel");
            }
        }

        private async Task<int> LoadNextItemsPageAsync(int generation)
        {
            if (_isLoadingItems || !_hasMoreItems || generation != _searchGeneration)
            {
                return 0;
            }

            _isLoadingItems = true;
            try
            {
                List<Item> page;
                if (_isBarcodeSearch)
                {
                    page = await App.g_db.SearchItemsQuickEntry(App.g_ScanBarcode, _nextItemOffset, PageSize);
                }
                else if (_isMonthlyAdSearch)
                {
                    page = await App.g_db.SearchItemsMonthlyAdClick(
                        App.g_MonthlyAdPage,
                        App.g_MonthlyAdX,
                        App.g_MonthlyAdY,
                        _nextItemOffset,
                        PageSize);
                }
                else
                {
                    page = await App.g_db.SearchItems(
                        App.g_SearchText,
                        App.g_Category,
                        App.g_ScanBarcode,
                        App.g_Subcategory,
                        App.g_Subsubcategory,
                        _nextItemOffset,
                        PageSize);

                    if (generation != _searchGeneration)
                    {
                        return 0;
                    }
                }

                if (generation != _searchGeneration)
                {
                    return 0;
                }

                _hasMoreItems = page.Count == PageSize;
                if (_isMonthlyAdSearch && App.g_IsMonthlyAdPDFClick)
                {
                    App.g_IsMonthlyAdPDFClick = false;
                }

                foreach (var item in page)
                {
                    Item.SetListItem(item, "O");
                    lstItems.Add(item);
                }

                _nextItemOffset += page.Count;
                return page.Count;
            }
            finally
            {
                _isLoadingItems = false;
            }
        }

        private async void ItemsListSearch_RemainingItemsThresholdReached(object sender, EventArgs e)
        {
            await LoadNextItemsPageAsync(_searchGeneration);
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

        async void OnTappedSearch(object sender, EventArgs e)
        {
            App.g_SearchText = Search.Text;
            // User tapped 'Yes' - Call your search method here
            App.g_SearchText = Search.Text;
            //App.g_SearchFromPage = "HomePage";
            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            await App.g_Shell.GoToItemSearch();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            ImageOverlay.IsVisible = false;
            if (ItemsListSearch.SelectedItem != null)
            {
                ItemsListSearch.SelectedItem = null;
                FullImage.Source = null;
            }
        }

        private void ItemsListSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = e.CurrentSelection?.FirstOrDefault() as Item;
            if (selectedItem == null)
                return;
            ImageOverlay.IsVisible = true;
            FullImage.Source = selectedItem.ImageURL;
        }
    }
}

