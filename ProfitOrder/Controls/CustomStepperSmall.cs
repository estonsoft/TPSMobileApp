namespace TPSMobileApp.Controls;

public class CustomStepperSmall : StackLayout
{
    public static readonly BindableProperty ItemNoProperty =
        BindableProperty.Create(nameof(ItemNo), typeof(int), typeof(CustomStepperSmall), 0);

    public static readonly BindableProperty QtyOrderProperty =
        BindableProperty.Create(nameof(QtyOrder), typeof(int), typeof(CustomStepperSmall), 0);

    public static readonly BindableProperty UOMProperty =
        BindableProperty.Create(nameof(UOM), typeof(string), typeof(CustomStepperSmall), string.Empty);

    public static readonly BindableProperty SellUnitProperty =
        BindableProperty.Create(nameof(SellUnit), typeof(int), typeof(CustomStepperSmall), 0);

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(int), typeof(CustomStepperSmall), 0, BindingMode.TwoWay);

    public static readonly BindableProperty IsStepperVisibleProperty =
        BindableProperty.Create(nameof(IsStepperVisible), typeof(bool), typeof(CustomStepperSmall), false, BindingMode.TwoWay);

    public static readonly BindableProperty IsAddToOrderVisibleProperty =
        BindableProperty.Create(nameof(IsAddToOrderVisible), typeof(bool), typeof(CustomStepperSmall), false, BindingMode.TwoWay);

    public int ItemNo
    {
        get => (int)GetValue(ItemNoProperty);
        set => SetValue(ItemNoProperty, value);
    }

    public int QtyOrder
    {
        get => (int)GetValue(QtyOrderProperty);
        set => SetValue(QtyOrderProperty, value);
    }

    public string UOM
    {
        get => (string)GetValue(UOMProperty);
        set => SetValue(UOMProperty, value);
    }

    public int SellUnit
    {
        get => (int)GetValue(SellUnitProperty);
        set => SetValue(SellUnitProperty, value);
    }

    public int Text
    {
        get => (int)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsStepperVisible
    {
        get => (bool)GetValue(IsStepperVisibleProperty);
        set => SetValue(IsStepperVisibleProperty, value);
    }

    public bool IsAddToOrderVisible
    {
        get => (bool)GetValue(IsAddToOrderVisibleProperty);
        set => SetValue(IsAddToOrderVisibleProperty, value);
    }

    ImageButton PlusBtn;
    ImageButton MinusBtn;
    StackLayout QtyStack;
    Label QtyLabel;
    Label InCartLabel;
    StackLayout AddToOrderStack;
    Button AddToOrderBtn;

    public CustomStepperSmall()
    {
        Orientation = StackOrientation.Horizontal;
        HeightRequest = 30;

        PlusBtn = new ImageButton
        {
            WidthRequest = 33,
            HeightRequest = 40,
            Source = "blue_plus.png",
            Aspect = Aspect.AspectFit,
            BackgroundColor = Colors.Transparent
        };
        PlusBtn.Clicked += PlusBtn_Clicked;
        PlusBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        MinusBtn = new ImageButton
        {
            WidthRequest = 33,
            HeightRequest = 40,
            Margin = new Thickness(5, 0, 0, 0),
            Source = "blue_minus.png",
            Aspect = Aspect.AspectFit,
            BackgroundColor = Colors.Transparent
        };
        MinusBtn.Clicked += MinusBtn_Clicked;
        MinusBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        AddToOrderStack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            Margin = new Thickness(0, 5, 0, 0)
        };

        AddToOrderBtn = new Button
        {
            Text = "Add To Order",
            HeightRequest = 28,
            WidthRequest = 103,
            CornerRadius = 15,
            Margin = new Thickness(5, -4, 0, 5),
            Padding = Thickness.Zero,
            TextTransform = TextTransform.None,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Colors.LightGray,
            TextColor = Colors.Blue
        };
        AddToOrderBtn.Clicked += PlusBtn_Clicked;
        AddToOrderBtn.SetBinding(IsVisibleProperty, new Binding(nameof(IsAddToOrderVisible), source: this));

        AddToOrderStack.Children.Add(AddToOrderBtn);

        QtyStack = new StackLayout { Orientation = StackOrientation.Vertical };

        QtyLabel = new Label
        {
            WidthRequest = 40,
            Margin = new Thickness(0, 2, 0, 0),
            TextColor = Colors.Black,
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center
        };
        QtyLabel.SetBinding(Label.TextProperty, new Binding(nameof(Text), source: this));
        QtyLabel.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        InCartLabel = new Label
        {
            Text = "In Cart",
            WidthRequest = 35,
            Margin = new Thickness(0, -10, 0, 0),
            TextColor = Colors.Gray,
            FontSize = 8,
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            MaxLines = 1
        };
        InCartLabel.SetBinding(IsVisibleProperty, new Binding(nameof(IsStepperVisible), source: this));

        QtyStack.Children.Add(QtyLabel);
        QtyStack.Children.Add(InCartLabel);

        Children.Add(MinusBtn);
        Children.Add(QtyStack);
        Children.Add(PlusBtn);
        Children.Add(AddToOrderStack);
    }

    void MinusBtn_Clicked(object sender, EventArgs e)
    {
        if (Text <= 0)
            return;

        int iQty = App.g_db.GetItemQty(ItemNo);
        if (iQty > 0)
            App.g_db.UpdateItemQty(ItemNo, -1);

        Text--;
        QtyOrder--;

        App.g_ShoppingCartItems = App.g_db.GetCartPieces();

        try { App.g_ShoppingCartPage.UpdateTotals(); } catch { }
        try { App.g_CheckoutPage.UpdateTotals(); } catch { }

        if (Text == 0)
        {
            IsStepperVisible = false;
            IsAddToOrderVisible = true;
        }
    }

    void PlusBtn_Clicked(object sender, EventArgs e)
    {
        if (Text == 999)
            return;

        App.g_db.UpdateItemQty(ItemNo, 1);

        Text++;
        QtyOrder++;

        App.g_ShoppingCartItems = App.g_db.GetCartPieces();

        try { App.g_ShoppingCartPage.UpdateTotals(); } catch { }
        try { App.g_CheckoutPage.UpdateTotals(); } catch { }

        IsStepperVisible = true;
        IsAddToOrderVisible = false;
    }
}
