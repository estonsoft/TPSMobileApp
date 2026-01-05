using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using TPSMobileApp.ViewModels;
using TPSMobileApp;
using System.IO;

namespace TPSMobileApp.Views
{
    public partial class SubsubcategoryPage : ContentPage
    {
        public SubsubcategoryPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            CategoryLabel.Text = App.g_Category.Description;
            SubcategoryLabel.Text = App.g_Subcategory.Description;

            App.g_Subsubcategory.Code = "";
            App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";

            App.g_SearchText = ""; 

            App.g_CurrentPage = "SubsubcategoryPage";

            List<Subsubcategory> lst = App.g_db.GetSubsubcategory(App.g_Category.Code, App.g_Subcategory.Code);

            SubsubcategoriesListSearch.ItemsSource = lst;
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            //Category item = (Category) e.SelectedItem;
            //App.g_Category = item.Code;

            //Shell.Current.GoToAsync("//HomePage/ItemSearchPage");
        }

        private void OnSubsubcategoryTapped(object sender, ItemTappedEventArgs e)
        {
            App.g_Subsubcategory = (Subsubcategory) e.Item;
            App.g_ScanBarcode = "";

            App.g_SearchFromPage = "SubsubcategoryPage";
            App.g_Shell.GoToItemSearch();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}