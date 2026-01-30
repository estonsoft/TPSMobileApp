namespace TPSMobileApp.Views
{
    public partial class SubcategoryPage : ContentPage
    {
        public SubcategoryPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            CategoryLabel.Text = App.g_Category.Description;

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_Subsubcategory.Code = "";
            App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";

            App.g_SearchText = "";

            App.g_CurrentPage = "SubcategoryPage";

            List<Subcategory> lst = App.g_db.GetSubcategory(App.g_Category.Code);

            //Subcategory subcat = new Subcategory();
            //subcat.Code = "TOPSELLERS";
            //subcat.Description = "TOP SELLERS";
            //lst.Insert(0, subcat);

            SubcategoriesListSearch.ItemsSource = lst;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //Category item = (Category) e.SelectedItem;
            //App.g_Category = item.Code;

            //Shell.Current.GoToAsync("//HomePage/ItemSearchPage");
        }

        private async void OnSubcategoryTapped(object sender, ItemTappedEventArgs e)
        {
            App.g_Subcategory = (Subcategory)e.Item;
            App.g_ScanBarcode = "";

            int iSubsubcategories = App.g_db.GetSubsubcategory(App.g_Category.Code, App.g_Subcategory.Code).Count;

            if (iSubsubcategories < 1)
            {
                await App.g_Shell.GoToSubsubcategories();
            }
            else
            {
                App.g_SearchFromPage = "SubcategoryPage";
                await App.g_Shell.GoToItemSearch();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}