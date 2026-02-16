using System.Diagnostics;
using FluentFTP.Helpers;
using Scandit.DataCapture.Barcode.Data;
using TPSMobileApp.ViewModels;

namespace TPSMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuickEntryPage : ContentPage
    {
        public ScanditViewModelBase viewModel = null;
        List<Item> lstItems;

        public QuickEntryPage()
        {
            InitializeComponent();
            if (App.g_ScanditViewModel == null)
            {
                App.g_ScanditViewModel = new ScanditViewModelBase(this);
            }

            this.viewModel = App.g_ScanditViewModel;
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "QuickEntryPage";

            lstItems = new List<Item>();
            ClearItemInfo();

            await Task.Delay(100);
            EntryFocus();

            await viewModel.OnResumeAsync();
        }

        private async void EntryFocus()
        {
            ScanItem.Unfocus();
            ScanItem.Text = "";
            await Task.Delay(500);
            ScanItem.Focus();
        }

        protected override void OnDisappearing()
        {
            try
            {
                _ = this.viewModel.OnSleep();
                base.OnDisappearing();
                Content = null;
            }
            catch
            {
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void ShowItemInfo(Item item)
        {
            Item.SetListItem(item, "O");
            
            item.IsBoxViewVisible = false;

            lstItems.Clear();
            lstItems.Add(item);
            ItemsListSearch.ItemsSource = null;
            ItemsListSearch.ItemsSource = lstItems;

            ScanItem.Text = "";

            Message.IsVisible = false;
        }

        private void ClearItemInfo()
        {
            ScanItem.Text = "";

            Message.IsVisible = false;

            lstItems.Clear();
            ItemsListSearch.ItemsSource = null;
            ItemsListSearch.ItemsSource = lstItems;

            EntryFocus();
        }

        private void SetMessage(string sMessage)
        {
            ClearItemInfo();
            Message.Text = sMessage;
            Message.IsVisible = true;
        }

        public void ScanComplete(String barcode)
        {
            ClearItemInfo();
            ScanItem.Text = barcode;
            viewModel.OnSleep();

            TapToScan.IsVisible = true;

            Item item = FindItem();

            if (item == null)
            {
                ClearItemInfo();
                SetMessage("Item Not Found " + ScanItem.Text);
                ScanItem.Text = "";
                EntryFocus();
                return;
            }

            if (App.g_db.GetItemQty(item.ItemNo) > 0)
            {
                SetMessage("Item Already In Shopping Cart");
            }

            ShowItemInfo(item);
        }

        private void ScanItem_Completed(object sender, EventArgs e)
        {
            ScanComplete(ScanItem.Text.Trim());
        }

        public void SetScanItem(string barcode)
        {
            ScanItem.Text = barcode;
        }

        private void EnterButton_Clicked(object sender, EventArgs e)
        {
            Message.Text = "";
            ScanComplete(ScanItem.Text.Trim());
        }

        private Item FindItem()
        {
            //Database db = new Database();

            Item item = null;
            List<Item> items = new List<Item>();
            int ItemNo = 0;

            string ScanText = ScanItem.Text.Trim();
            if (ScanText.IsBlank() || ScanText.Length == 0)
                return item;
            else
            {
                int.TryParse(ScanItem.Text, out ItemNo);

                if (ItemNo > 0)
                {
                    item = App.g_db.FindItem(ItemNo, ItemNo.ToString());
                }

                if (item == null)
                {
                    items = App.g_db.SearchItemsQuickEntry(ScanText);

                    if (items.Count >= 1)
                    {
                        item = items[0];
                    }
                }

                if (item != null)
                {
                    if (App.g_IsAutoAdd1)
                    {
                        item.QtyOrder += 1;
                        App.g_db.UpdateItemQtySet(item.ItemNo, item.QtyOrder);
                    }
                }
            }

            return item;
        }

        async void OnScannerEnable(object sender, EventArgs e)
        {
            ClearItemInfo();
            TapToScan.IsVisible = false;
            _ = this.viewModel.OnResumeAsync();
            ScanItem.Text = "";
            Message.Text = "";
        }
    }
}
