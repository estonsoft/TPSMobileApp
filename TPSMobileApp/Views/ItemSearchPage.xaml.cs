using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
        List<Item> lstItems = new ();

        // The items actually shown in the CollectionView
        public ObservableCollection<Item> DisplayedItems { get; } = new();

        private int _pageSize = 10;

        // Remove [ObservableProperty] from itemtoload field
        // and implement as a property with OnPropertyChanged

        
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


            if (iItems == 0 && App.g_Category.Description != "ALL CATEGORIES")
            {
                //await Shell.Current.DisplayAlertAsync("Profit Order", "No items found matching search criteria", "Ok");
                bool answer = await Shell.Current.DisplayAlertAsync(
                "Profit Order",
                "No items found in selected category. Do you want to search in all categories?",
                "Yes",
                "No");

                if (answer)
                {
                    DisplayedItems.Clear();
                    // User tapped 'Yes' - Call your search method here
                    App.g_SearchText = Search.Text;
                    //App.g_SearchFromPage = "HomePage";
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
            else if (iItems == 0 ){
                await Shell.Current.DisplayAlertAsync(
               "Profit Order",
               "No items found in selected category.Please modify your search.",
               "Cancel");
            }
            loadMoreCommand?.Execute(null);
        }

        [RelayCommand]
        private void LoadMore()
        {
            // 1. Calculate how many items are already shown
            int currentCount = DisplayedItems.Count;

            // 2. Check if there's more to load
            if (currentCount < lstItems.Count)
            {
                // 3. Take the next batch from your master list
                var nextBatch = lstItems
                    .Skip(currentCount)
                    .Take(_pageSize);

                // 4. Add them to the observable collection
                foreach (var item in nextBatch)
                {
                    DisplayedItems.Add(item);
                }
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
            lstItems.Clear();
            DisplayedItems.Clear();
            RefreshList();
        }

        private void InStockOnly_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            App.g_InStockOnly = InStockOnly.IsChecked;
            lstItems.Clear();
            DisplayedItems.Clear();
            RefreshList();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        async void OnTappedSearch(object sender, EventArgs e)
        {
            App.g_SearchText = Search.Text;
            DisplayedItems.Clear();
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
            if (ItemsListSearch.SelectedItem!= null)
            {   
                ItemsListSearch.SelectedItem = null;
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

