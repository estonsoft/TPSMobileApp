using System.Diagnostics;
using BarcodeScanning;
using FluentFTP.Helpers;
using Scandit.DataCapture.Barcode.Data;
using TPSMobileApp.ViewModels;

namespace TPSMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuickEntryPage : ContentPage
    {
        List<Item> lstItems;

        public QuickEntryPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Methods.AskForRequiredPermissionAsync();

            App.g_CurrentPage = "QuickEntryPage";

            lstItems = new List<Item>();
            ClearItemInfo();

            await Task.Delay(100);

            ScanItem.Text = "";

            RequestCameraPermission();
        }

        async void RequestCameraPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status == PermissionStatus.Granted)
            {
                // Explicitly switch the hardware feed on after permission is secure
                ScannerControl.CameraEnabled = true; //
            }
            else
            {
                await DisplayAlertAsync("Permission Denied", "Camera access is required to scan.", "OK");
            }
        }

        protected override void OnDisappearing()
        {
            ScannerControl.CameraEnabled = false;
            try
            {
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

            TapToScan.IsVisible = true;

            Item item = FindItem();

            if (item == null)
            {
                ClearItemInfo();
                SetMessage("Item Not Found " + ScanItem.Text);
                ScanItem.Text = "";
                return;
            }

            if (App.g_db.GetItemQty(item.ItemNo) > 0)
            {
                SetMessage("Item Already In Shopping Cart");
            }

            ShowItemInfo(item);
            ScanItem.Unfocus();
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

        private void OnBarcodeDetected(object sender, OnDetectionFinishedEventArg e)
        {
            // Check if anything was read in the current frame
            if (e.BarcodeResults.Count == 0) return;

            // The engine processes frames on a background threat thread; route to UI thread
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                // Pause camera processing to handle the logic flow
                ScannerControl.CameraEnabled = false;

                var primaryItem = e.BarcodeResults.First();
                ScanComplete(primaryItem.DisplayValue.Trim());
                //await DisplayAlertAsync("Native Scan Match",
                //    $"Value: {primaryItem.DisplayValue}\nType: {primaryItem.BarcodeFormat}",
                //    "OK");
            });
        }

        async void OnScannerEnable(object sender, EventArgs e)
        {
            ClearItemInfo();
            // Resume scanning pipeline
            ScannerControl.CameraEnabled = true;
            TapToScan.IsVisible = false;
            ScanItem.Text = "";
            Message.Text = "";
        }
    }
}
