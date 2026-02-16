using TPSMobileApp.ViewModels;

namespace TPSMobileApp.Views
{
    public partial class CategoryPage : ContentPage
    {
        CategoryViewModel _viewModel;

        public CategoryPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new CategoryViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();

            App.g_CurrentPage = "CategoryPage";

            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_Subsubcategory.Code = "";
            App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";

            App.g_SearchText = "";

            CategoriesListSearch.ItemsSource = App.g_db.GetCategories();
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //Category item = (Category) e.SelectedItem;
            //App.g_Category = item.Code;

            //App.g_Shell.GoToItemSearch();
        }

        private async void CategoriesListSearch_ItemTapped(object sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
        {
            App.g_Category = (Category)e.DataItem;
            App.g_ScanBarcode = "";

            int iSubcategories = App.g_db.GetSubcategoryCount(App.g_Category.Code);

            if (iSubcategories > 0)
            {
                await App.g_Shell.GoToSubcategories();
            }
            else
            {
                App.g_SearchFromPage = "CategoryPage";
                await App.g_Shell.GoToItemSearch();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}