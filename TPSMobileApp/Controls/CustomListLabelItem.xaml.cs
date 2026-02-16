namespace TPSMobileApp.Controls
{
    public partial class CustomListLabelItem : ContentView
    {
        public CustomListLabelItem()
        {
            InitializeComponent();
        }

        private void MinusButton_Clicked(object sender, EventArgs e)
        {
            PlusMinusButton button = (PlusMinusButton)sender;

            int.TryParse(QtyEntry.Text, out int iQty);

            if (iQty > 0)
            {
                iQty--;

                App.g_db.UpdateItemLabelQtySet(button.ItemNo, iQty);

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

            int.TryParse(QtyEntry.Text, out int iQty);

            if (iQty == 999)
                return;

            iQty++;

            App.g_db.UpdateItemLabelQtySet(button.ItemNo, iQty);

            QtyEntry.Text = iQty.ToString();

            StepperStack.IsVisible = true;
            AddToOrderButton.IsVisible = false;
        }

        private void AddToOrderButton_Clicked(object sender, EventArgs e)
        {
            AddToOrderButton button = (AddToOrderButton)sender;

            App.g_db.UpdateItemLabelQty(button.ItemNo, 1);

            QtyEntry.Text = "1";

            StepperStack.IsVisible = true;
            AddToOrderButton.IsVisible = false;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var current = args.NewTextValue?.TrimStart('0') ?? "0";

            if (current.Length == 0)
                current = "0";

            if (string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                ((Entry)sender).Text = "0";
                return;
            }

            if (!int.TryParse(args.NewTextValue, out int iValue))
            {
                ((Entry)sender).Text = args.OldTextValue;
            }
            else
            {
                ((Entry)sender).Text = iValue.ToString();
            }
        }

        private void OnQtyEntry_Completed(object sender, EventArgs e)
        {
            QtyEntry qtyEntry = (QtyEntry)sender;

            int.TryParse(qtyEntry.Text, out int iTextQty);

            if (iTextQty > 999)
            {
                qtyEntry.Text = "999";
                iTextQty = 999;
            }

            App.g_db.UpdateItemLabelQtySet(qtyEntry.ItemNo, iTextQty);

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
        }
    }
}
