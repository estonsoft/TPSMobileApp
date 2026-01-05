using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using TPSMobileApp.Views;
using TPSMobileApp.ViewModels;
using TPSMobileApp;
using TPSMobileApp.Controls;
using System.Globalization;
using Zebra.Sdk.Printer.Discovery;
using Zebra.Sdk.Printer;
using System.Collections.ObjectModel;

namespace TPSMobileApp.Views
{
    public partial class LabelPrintPage : ContentPage
    {
        public delegate void PrinterSelectedHandler(DiscoveredPrinter printer);
        public static event PrinterSelectedHandler OnPrinterSelected;
        ObservableCollection<DiscoveredPrinter> printers = new ObservableCollection<DiscoveredPrinter>();
        protected DiscoveredPrinter ChoosenPrinter;

        public LabelPrintPage()
        {
            InitializeComponent();

            //BindingContext = _viewModel = new ShoppingCartViewModel();
            BindingContext = this;

            //App.g_LabelCartPage = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //Database db = new Database();
            List<Item> items = App.g_db.GetReturnCartItems();

            if (items.Count > 0)
            {
                App.g_CurrentPage = "LabelCartPage";

                RefreshList();
            }
        }

        public async void RefreshList()
        {
            ItemsListCart.ItemsSource = null;

            ItemsListCart.ItemsSource = App.g_db.GetLabelCartItems();

            foreach (Item i in (List<Item>)ItemsListCart.ItemsSource)
            {
                Item.SetListItem(i, "L");
            }
        }

        private async void btnCheckout_Clicked(object sender, EventArgs e)
        {
            App.g_Shell.GoToCheckout();
        }

        private async void btnClearCart_Clicked(object sender, EventArgs e)
        {
            bool bClear = await DisplayAlert("Profit Order", "Are you sure you wish to remove all the items from your label print cart?", "Yes", "No");

            if (bClear)
            {
                App.g_db.ClearLabelCartItems();
                App.g_Shell.GoToHome();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
