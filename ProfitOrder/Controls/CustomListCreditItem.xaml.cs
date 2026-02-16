namespace TPSMobileApp.Controls
{
    public partial class CustomListCreditItem : ContentView
    {
        public CustomListCreditItem()
        {
            InitializeComponent();
        }

        private void MinusButton_Clicked(object sender, EventArgs e)
        {
            var button = (PlusMinusButton)sender;

            int iQty = 0;
            int.TryParse(QtyEntry.Text, out iQty);

            if (iQty > 0)
            {
                iQty--;

                App.g_db.UpdateItemCreditQtySet(button.ItemNo, iQty);
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
            var button = (PlusMinusButton)sender;

            int iQty = 0;
            int.TryParse(QtyEntry.Text, out iQty);

            if (iQty == 999)
                return;

            iQty++;

            App.g_db.UpdateItemCreditQtySet(button.ItemNo, iQty);
            QtyEntry.Text = iQty.ToString();

            StepperStack.IsVisible = true;
            AddToOrderButton.IsVisible = false;
        }

        private void AddToOrderButton_Clicked(object sender, EventArgs e)
        {
            var button = (AddToOrderButton)sender;

            App.g_db.UpdateItemCreditQty(button.ItemNo, 1);
            QtyEntry.Text = "1";

            StepperStack.IsVisible = true;
            AddToOrderButton.IsVisible = false;
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            var current = args.NewTextValue?.TrimStart('0') ?? "0";

            if (string.IsNullOrWhiteSpace(args.NewTextValue))
            {
                ((Entry)sender).Text = "0";
                return;
            }

            if (!int.TryParse(args.NewTextValue, out var iValue))
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
            var qtyEntry = (QtyEntry)sender;

            int iTextQty = 0;
            int.TryParse(qtyEntry.Text, out iTextQty);

            if (iTextQty > 999)
                iTextQty = 999;

            qtyEntry.Text = iTextQty.ToString();
            App.g_db.UpdateItemCreditQtySet(qtyEntry.ItemNo, iTextQty);

            StepperStack.IsVisible = iTextQty > 0;
            AddToOrderButton.IsVisible = iTextQty <= 0;
        }
    }
}
