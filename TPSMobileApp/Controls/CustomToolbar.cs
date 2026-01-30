namespace TPSMobileApp.Controls;

public class CustomToolbar : StackLayout
{
    public static readonly BindableProperty CartItemsProperty =
        BindableProperty.Create(nameof(CartItems), typeof(int), typeof(CustomToolbar), 0);

    public int CartItems
    {
        get => (int)GetValue(CartItemsProperty);
        set => SetValue(CartItemsProperty, value);
    }

    //static int ColumnWidth = 70;

    Grid gridContainer;

    VerticalStackLayout StackHome;
    VerticalStackLayout StackShoppingCart;
    VerticalStackLayout StackPurchaseHistory;
    VerticalStackLayout StackShopNow;
    VerticalStackLayout StackScanBarcode;

    Image LabelHomeIcon;
    Label LabelHomeText;
    Image LabelShoppingCartIcon;
    Label LabelShoppingCartText;
    Label LabelShoppingCartItems;
    Image LabelPurchaseHistoryIcon;
    Label LabelPurchaseHistoryText;
    Image LabelShopNowIcon;
    Label LabelShopNowText;
    Image LabelScanBarcodeIcon;
    Label LabelScanBarcodeText;

    TapGestureRecognizer TapHome;
    TapGestureRecognizer TapShoppingCart;
    TapGestureRecognizer TapPurchaseHistory;
    TapGestureRecognizer TapShopNow;
    TapGestureRecognizer TapScanBarcode;

    public CustomToolbar()
    {
        HeightRequest = 80;
        BackgroundColor = Colors.Blue;

        gridContainer = new Grid
        {
            ColumnSpacing = 10,
            Padding = 10,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Start,
            BackgroundColor = Colors.Blue,
            HeightRequest = 80,
        };
        gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        gridContainer.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

        Children.Add(gridContainer);

        StackHome = CreateStack(out LabelHomeIcon, out LabelHomeText, "\uF015", "Home");
        TapHome = new TapGestureRecognizer();
        TapHome.Tapped += OnHomeTapped;
        AddTap(StackHome, TapHome);

        StackShoppingCart = CreateStack(out LabelShoppingCartIcon, out LabelShoppingCartText, "\uF07A", "Shopping\nCart");
        TapShoppingCart = new TapGestureRecognizer();
        TapShoppingCart.Tapped += OnShoppingCartTapped;
        AddTap(StackShoppingCart, TapShoppingCart);

        LabelShoppingCartItems = new Label
        {
            TextColor = Colors.Black,
            FontSize = 10,
            HorizontalTextAlignment = TextAlignment.Center
        };
        LabelShoppingCartItems.SetBinding(Label.TextProperty, new Binding(nameof(CartItems), source: this));
        StackShoppingCart.Children.Add(LabelShoppingCartItems);

        StackPurchaseHistory = CreateStack(out LabelPurchaseHistoryIcon, out LabelPurchaseHistoryText, "\uF571", "Order\nHistory");
        TapPurchaseHistory = new TapGestureRecognizer();
        TapPurchaseHistory.Tapped += OnPurchaseHistoryTapped;
        AddTap(StackPurchaseHistory, TapPurchaseHistory);

        StackShopNow = CreateStack(out LabelShopNowIcon, out LabelShopNowText, "\uF0CA", "Shop Now");
        TapShopNow = new TapGestureRecognizer();
        TapShopNow.Tapped += OnShopNowTapped;
        AddTap(StackShopNow, TapShopNow);

        StackScanBarcode = CreateStack(out LabelScanBarcodeIcon, out LabelScanBarcodeText, "\uF464", "Scan\nBarcode", "FontAwesomePro6Regular");
        TapScanBarcode = new TapGestureRecognizer();
        TapScanBarcode.Tapped += OnScanBarcodeTapped;
        AddTap(StackScanBarcode, TapScanBarcode);

        CartItems = App.g_ShoppingCartItems;
    }

    VerticalStackLayout CreateStack(out Image icon, out Label text, string glyph, string label, string font = "FontAwesomeFreeSolid")
    {
        var stack = new VerticalStackLayout
        {
            BackgroundColor = Colors.Blue,
            //WidthRequest = ColumnWidth,            
            HeightRequest = 80,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        if (DeviceInfo.Platform == DevicePlatform.iOS)
            stack.Margin = new Thickness(0, 10, 0, 0);

        glyph = glyph.Replace("/u", "0x");
        icon = new Image
        {
            Margin = new Thickness(0, 8, 0, 0),
            Source = new FontImageSource
            {
                Glyph = glyph,
                FontFamily = font,
                Size = 24,
                Color = Colors.White
            }
        };

        text = new Label
        {
            Text = label,
            Margin = new Thickness(5, 5, 5, 5),
            TextColor = Colors.White,
            FontSize = 10,
            HorizontalTextAlignment = TextAlignment.Center
        };

        stack.Children.Add(icon);
        stack.Children.Add(text);
        Grid.SetColumn(stack, gridContainer.Children.Count);
        gridContainer.Children.Add(stack);

        return stack;
    }

    void AddTap(VerticalStackLayout stack, TapGestureRecognizer tap)
    {
        stack.GestureRecognizers.Add(tap);
    }

    async void OnHomeTapped(object sender, EventArgs e)
    {
        App.g_Shell.bStopNavigating = false;
        await App.g_Shell.GoToHome();
        App.g_Shell.bStopNavigating = true;
    }

    async void OnShoppingCartTapped(object sender, EventArgs e)
    {
        List<Item> items = App.g_db.GetOrderCartItems();

        if (items.Count == 0)
            await Shell.Current.DisplayAlertAsync("Profit Order", "Your shopping cart is empty", "Ok");
        else
            await App.g_Shell.GoToShoppingCart();
    }

    async void OnShopNowTapped(object sender, EventArgs e)
    {
        await App.g_Shell.GoToCategories();
    }

    async void OnScanBarcodeTapped(object sender, EventArgs e)
    {
        await App.g_Shell.GoToScanBarcode();
    }

    async void OnPurchaseHistoryTapped(object sender, EventArgs e)
    {
        await App.g_Shell.GoToMyPurchases();
    }
}
