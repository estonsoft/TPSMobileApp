using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



using TPSMobileApp.Views;
using TPSMobileApp.ViewModels;
using System.Windows.Input;
using TPSMobileApp;
using TPSMobileApp.Controls;

namespace TPSMobileApp.Views
{
    public partial class PurchaseHistoryPage : ContentPage
    {
        public PurchaseHistoryPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "PurchaseHistoryPage";

            RefreshList();
        }

        public async void RefreshList()
        {
            OrderHistoryList.ItemsSource = null;

            //Database db = new Database();
            OrderHistoryList.ItemsSource = App.g_db.GetOrderHeaders();
        }

        void OnTappedDetails(object sender, EventArgs args)
        {
            var lbl = sender as OrderLabel;
            App.g_OrderNo = lbl.OrderNo;

            App.g_Shell.GoToOrderDetail();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }
    }
}
