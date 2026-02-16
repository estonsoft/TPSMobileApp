namespace TPSMobileApp.Controls;

public class CustomHeader : StackLayout
{
    StackLayout StackContainer;
    StackLayout StackBack;
    Image BackIcon;
    Label TitleText;
    TapGestureRecognizer TapBack;

    public CustomHeader()
    {
        Orientation = StackOrientation.Horizontal;
        HeightRequest = 60;
        BackgroundColor = Colors.White;

        StackContainer = new StackLayout
        {
            Orientation = StackOrientation.Horizontal,
            BackgroundColor = Colors.White,
            HeightRequest = 60,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Center
        };

        Children.Add(StackContainer);

        StackBack = new StackLayout
        {
            Orientation = StackOrientation.Vertical,
            BackgroundColor = Colors.White,
            WidthRequest = 60,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        StackContainer.Children.Add(StackBack);

        TapBack = new TapGestureRecognizer();
        TapBack.Tapped += OnBackTapped;

        BackIcon = new Image
        {
            BackgroundColor = Colors.White,
            Margin = new Thickness(0)
        };

        BackIcon.Source = new FontImageSource
        {
            Glyph = "\uF060",
            FontFamily = "FontAwesomeFreeSolid",
            Size = 20,
            Color = Colors.Blue
        };

        BackIcon.GestureRecognizers.Add(TapBack);
        StackBack.Children.Add(BackIcon);

        TitleText = new Label
        {
            Margin = new Thickness(12, 0, 0, 0),
            TextColor = Colors.Blue,
            FontSize = 21,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Start,
            HorizontalTextAlignment = TextAlignment.Start,
            VerticalTextAlignment = TextAlignment.Center,
            Text = App.g_HeaderTitle
        };

        StackContainer.Children.Add(TitleText);
    }

    async void OnBackTapped(object sender, EventArgs e)
    {
        if (TitleText.Text == "Checkout")
        {
            await App.g_Shell.GoToShoppingCart();
        }
        else if (TitleText.Text == "Submit Order")
        {
            await App.g_Shell.GoToHome();
        }
        else if (TitleText.Text == "Order Detail")
        {
            await App.g_Shell.GoToMyPurchases();
        }
        else if (TitleText.Text == "Settings")
        {
            await App.g_Shell.GoToLogin();
        }
        else if (TitleText.Text == "Product Categories")
        {
            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_Subsubcategory.Code = "";
            App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";

            await App.g_Shell.GoToHome();
        }
        else if (TitleText.Text == "Product Subcategories")
        {
            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_Subsubcategory.Code = "";
            App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";

            await App.g_Shell.GoToCategories();
        }
        else if (TitleText.Text == "Product Sub-subcategories")
        {
            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_Subsubcategory.Code = "";
            App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";

            await App.g_Shell.GoToSubcategories();
        }
        else if (TitleText.Text == "Search Products")
        {
            if (App.g_SearchFromPage == "PurchaseHistoryPage")
            {
                await App.g_Shell.GoToMyPurchases();
            }
            else if (App.g_SearchFromPage == "ReorderItemsPage")
            {
                await App.g_Shell.GoToReorderItems();
            }
            else if (App.g_SearchFromPage == "HomePage")
            {
                App.g_Category.Code = "";
                App.g_Category.Description = "ALL CATEGORIES";

                App.g_Subcategory.Code = "";
                App.g_Subcategory.Description = "ALL SUBCATEGORIES";

                App.g_Subsubcategory.Code = "";
                App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";

                await App.g_Shell.GoToHome();
            }
            else
            {
                if (App.g_Subsubcategory.Code != "")
                {
                    App.g_Subsubcategory.Code = "";
                    App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";
                    await App.g_Shell.GoToSubsubcategories();
                }
                else if (App.g_Subcategory.Code != "")
                {
                    App.g_Subcategory.Code = "";
                    App.g_Subcategory.Description = "ALL SUBCATEGORIES";
                    await App.g_Shell.GoToSubcategories();
                }
                else
                {
                    App.g_Category.Code = "";
                    App.g_Category.Description = "ALL CATEGORIES";
                    await App.g_Shell.GoToHome();
                }
            }
        }
        else
        {
            App.g_Category.Code = "";
            App.g_Category.Description = "ALL CATEGORIES";

            App.g_Subcategory.Code = "";
            App.g_Subcategory.Description = "ALL SUBCATEGORIES";

            App.g_Subsubcategory.Code = "";
            App.g_Subsubcategory.Description = "ALL SUB-SUBCATEGORIES";

            await App.g_Shell.GoToHome();
        }
    }
}
