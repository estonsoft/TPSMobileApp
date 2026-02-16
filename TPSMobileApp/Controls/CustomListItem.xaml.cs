namespace TPSMobileApp.Controls
{
    public partial class CustomListItem : ContentView
    {
        public CustomListItem()
        {
            InitializeComponent();

            try
            {
                QtyEntry.Focus();
            }
            catch { }
        }

        private void MinusButton_Clicked(object sender, EventArgs e)
        {
            PlusMinusButton button = (PlusMinusButton)sender;

            int iQty = 0;
            int.TryParse(QtyEntry.Text, out iQty);

            if (iQty > 0)
            {
                iQty--;

                App.g_db.UpdateItemQtySet(button.ItemNo, iQty);

                QtyEntry.Text = iQty.ToString();

                if (iQty == 0)
                {
                    StepperStack.IsVisible = false;
                    AddToOrderButton.IsVisible = true;
                }
            }
        }

        private void PlusButton_Clicked(object sender, EventArgs e)
        {
            PlusMinusButton button = (PlusMinusButton)sender;

            int iQty = 0;
            int.TryParse(QtyEntry.Text, out iQty);

            if (iQty == 999)
            {
                return;
            }

            iQty++;

            if ((button.AllocationQty > 0) && (iQty > button.AllocationQty))
            {
                return;
            }

            App.g_db.UpdateItemQtySet(button.ItemNo, iQty);

            QtyEntry.Text = iQty.ToString();

            StepperStack.IsVisible = true;
            AddToOrderButton.IsVisible = false;
        }

        private void AddToOrderButton_Clicked(object sender, EventArgs e)
        {

            AddToOrderButton button = (AddToOrderButton)sender;

            App.g_db.UpdateItemQty(button.ItemNo, 1);

            QtyEntry.Text = "1";

            StepperStack.IsVisible = true;
            AddToOrderButton.IsVisible = false;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var current = args.NewTextValue;
            if (current != null)
            {
                current = current.TrimStart('0');

                if (current.Length == 0)
                {
                    current = "0";
                }

                if (string.IsNullOrWhiteSpace(args.NewTextValue))
                {
                    ((Entry)sender).Text = 0.ToString();
                    //OnQtyEntry_Completed(sender, args);
                    return;
                }

                int iValue = 0;
                if (!int.TryParse(args.NewTextValue, out iValue))
                {
                    ((Entry)sender).Text = args.OldTextValue;
                }
                else
                {
                    ((Entry)sender).Text = iValue.ToString();
                }
            }


            //OnQtyEntry_Completed(sender, args);
        }

        private async void OnQtyEntry_Completed(object sender, EventArgs e)
        {
            Boolean bOverAllocation = false;
            QtyEntry qtyEntry = (QtyEntry)sender;
            int iTextQty = 0;

            int.TryParse(qtyEntry.Text, out iTextQty);

            if ((qtyEntry.AllocationQty > 0) && (iTextQty > qtyEntry.AllocationQty))
            {
                qtyEntry.Text = qtyEntry.AllocationQty.ToString();
                iTextQty = qtyEntry.AllocationQty;
                bOverAllocation = true;
            }

            if (iTextQty > 999)
            {
                qtyEntry.Text = "999";
            }

            App.g_db.UpdateItemQtySet(qtyEntry.ItemNo, iTextQty);

            if (iTextQty <= 0)
            {
                StepperStack.IsVisible = false;
                AddToOrderButton.IsVisible = true;
            }
            else
            {
                StepperStack.IsVisible = true;
                AddToOrderButton.IsVisible = false;
            }

            if (bOverAllocation)
            {
                await Shell.Current.DisplayAlertAsync("Profit Order", "Qty is greater than max allocation of " + qtyEntry.AllocationQty.ToString() + ", qty has been adjusted", "Ok");
            }
        }

        private async void CreditButton_Clicked(object sender, EventArgs e)
        {
            CreditButton button = (CreditButton)sender;

            bool bResult = await Shell.Current.DisplayAlertAsync("Profit Order", "Add this item for return?", "Yes", "No");

            if (bResult)
            {
                App.g_db.UpdateItemCreditQty(button.ItemNo, 1);
            }
        }

        private async void LabelButton_Clicked(object sender, EventArgs e)
        {
            LabelButton button = (LabelButton)sender;

            bool bResult = await Shell.Current.DisplayAlertAsync("Profit Order", "Add this item for label print?", "Yes", "No");

            if (bResult)
            {
                App.g_db.UpdateItemLabelQty(button.ItemNo, 1);
            }
        }

        private void QtyEntry_Focused(object sender, FocusEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                QtyEntry.CursorPosition = 0;
                QtyEntry.SelectionLength = QtyEntry.Text?.Length ?? 0;
            });
        }
    }
}