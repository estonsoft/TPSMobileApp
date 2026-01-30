namespace TPSMobileApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuickEntryPageBasic : ContentPage
    {
        Item item { get; set; }

        public QuickEntryPageBasic()
        {
            try
            {
                this.InitializeComponent();
            }
            catch
            {
            }
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            App.g_CurrentPage = "QuickEntryPage";

            await Task.Delay(100);
            EntryFocus();
        }

        protected override void OnDisappearing()
        {
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

        private async void EntryFocus()
        {
            ScanItem.Unfocus();
            ScanItem.Text = "";
            await Task.Delay(100);
            ScanItem.Focus();
        }

        private async void QtyFocus()
        {
            Qty.Unfocus();
            await Task.Delay(100);
            Qty.Focus();
        }

        private void ClearItemInfo()
        {
            ScanItem.Text = "";

            Message.IsVisible = false;
            ItemDesc.IsVisible = false;

            EntryFocus();
        }

        private void SetMessage(string sMessage)
        {
            ClearItemInfo();

            ItemDesc.Text = "";
            Message.Text = sMessage;
            Message.IsVisible = true;
            ItemDesc.IsVisible = false;
        }

        public void ScanComplete()
        {
            item = FindItem();

            if (item == null)
            {
                SetMessage("Item Not Found " + ScanItem.Text);
                ScanItem.Text = "";
                Qty.Text = "";
                EntryFocus();
                return;
            }
            else
            {
                Message.IsVisible = false;
                ItemDesc.IsVisible = true;
                ItemDesc.Text = item.Description;
            }

            int iQty = App.g_db.GetItemQty(item.ItemNo);

            if (iQty > 0)
            {
                SetMessage("Item Already In Shopping Cart");
                ItemDesc.Text = item.Description;
                ItemDesc.IsVisible = true;
                Qty.Text = iQty.ToString();
                QtyFocus();
            }
            else
            {
                ItemDesc.Text = item.Description;
                ItemDesc.IsVisible = true;
                Message.IsVisible = false;

                Qty.Text = "1";
                QtyFocus();
            }
        }

        private void ScanItem_Completed(object sender, EventArgs e)
        {
            ScanComplete();
        }

        private void Qty_Completed(object sender, EventArgs e)
        {
            QtyComplete();
        }

        private void EnterButton_Clicked(object sender, EventArgs e)
        {
            Message.Text = "";
            ScanComplete();
        }

        private void QtyEnterButton_Clicked(object sender, EventArgs e)
        {
            QtyComplete();
        }

        private async void QtyComplete()
        {
            int iQty = 0;
            int.TryParse(Qty.Text, out iQty);

            if (iQty == 0)
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "Invalid Qty", "Ok");
                return;
            }

            App.g_db.UpdateItemQtySet(item.ItemNo, iQty);

            Qty.Text = "";
            ItemDesc.Text = "";
            ItemDesc.IsVisible = false;
            Message.IsVisible = true;
            Message.Text = "Item updated in shopping cart";
            EntryFocus();
        }

        private Item FindItem()
        {
            Item item = null;
            List<Item> items = new List<Item>();
            int ItemNo = 0;

            string ScanText = ScanItem.Text.Trim();
            int.TryParse(ScanItem.Text, out ItemNo);

            if (ItemNo > 0)
            {
                item = App.g_db.FindItem(ItemNo, ItemNo.ToString());
            }
            else
            {
                item = App.g_db.FindItem(0, ScanText);
            }

            return item;
        }

        private void Qty_Focused(object sender, FocusEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Qty.CursorPosition = 0;
                Qty.SelectionLength = Qty.Text != null ? Qty.Text.Length : 0;
            });
        }
    }
}
