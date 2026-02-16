namespace TPSMobileApp.Controls
{
    public class ItemLayout : StackLayout
    {
        /*

        public static readonly BindableProperty ItemNoProperty = BindableProperty.Create("ItemNo", typeof(int), typeof(NumericEntryBehavior), 0);

        public int ItemNo
        {
            get => (int) GetValue(ItemNoProperty);
            set => SetValue(ItemNoProperty, value);
        }

        ImageButton PlusBtn;
        ImageButton MinusBtn;
        Entry QtyEntry;

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                propertyName: "Text",
                returnType: typeof(int),
                declaringType: typeof(ItemLayout),
                defaultValue: 0,
                defaultBindingMode: BindingMode.TwoWay);

        public int Text
        {
            get { return (int) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public ItemLayout()
        {
            Orientation = StackOrientation.Vertical;
            Margin = new Thickness(0, 0, 0, 0);
            Spacing = 0;
            VerticalOptions = LayoutOptions.Start;

            Label DescLabel = new Label();
            DescLabel.Text = 




            PlusBtn = new ImageButton { WidthRequest = 30, HeightRequest = 30, Source = "red_plus.png", Aspect = Aspect.AspectFit };
            PlusBtn.BackgroundColor = Color.Transparent;
            PlusBtn.Clicked += PlusBtn_Clicked;

            MinusBtn = new ImageButton { WidthRequest = 30, HeightRequest = 30, Source = "red_minus.png", Aspect = Aspect.AspectFit };
            MinusBtn.BackgroundColor = Color.Transparent;
            MinusBtn.Clicked += MinusBtn_Clicked;

            QtyEntry = new Entry { PlaceholderColor = Color.Gray, Keyboard = Keyboard.Numeric, HeightRequest = 35, WidthRequest = 40, BackgroundColor = Color.Transparent, FontSize = 20 };
            QtyEntry.Keyboard = Keyboard.Numeric;
            QtyEntry.Behaviors.Add(new NumericEntryBehavior());
            QtyEntry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
            QtyEntry.HorizontalTextAlignment = TextAlignment.Center;
            QtyEntry.TextChanged += Entry_TextChanged;

            Children.Add(MinusBtn);
            Children.Add(QtyEntry);
            Children.Add(PlusBtn);
        }

        public void ItemLayoutX()
        {
            Orientation = StackOrientation.Horizontal;
            HeightRequest = 30;

            PlusBtn = new ImageButton { WidthRequest = 30, HeightRequest = 30, Source = "red_plus.png", Aspect = Aspect.AspectFit };
            PlusBtn.BackgroundColor = Color.Transparent;
            PlusBtn.Clicked += PlusBtn_Clicked;

            MinusBtn = new ImageButton { WidthRequest = 30, HeightRequest = 30, Source = "red_minus.png", Aspect = Aspect.AspectFit };
            MinusBtn.BackgroundColor = Color.Transparent;
            MinusBtn.Clicked += MinusBtn_Clicked;

            Entry = new Entry { PlaceholderColor = Color.Gray, Keyboard = Keyboard.Numeric, HeightRequest = 35, WidthRequest = 40, BackgroundColor = Color.Transparent, FontSize = 20 };
            Entry.Keyboard = Keyboard.Numeric;
            Entry.Behaviors.Add(new NumericEntryBehavior());
            Entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
            Entry.HorizontalTextAlignment = TextAlignment.Center;
            Entry.TextChanged += Entry_TextChanged;

            Children.Add(MinusBtn);
            Children.Add(Entry);
            Children.Add(PlusBtn);
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            int item = ItemNo;

            if (!string.IsNullOrEmpty(e.NewTextValue))
            {
                try
                {
                    this.Text = int.Parse(e.NewTextValue);
                }
                catch
                {
                }
            }
        }

        private void MinusBtn_Clicked(object sender, EventArgs e)
        {
            if (Text > 0)
                Text--;
        }

        private void PlusBtn_Clicked(object sender, EventArgs e)
        {
            Text++;
        }
        */
    }
}
