namespace TPSMobileApp.Data
{
    class SaveLayout
    {
    }

    /*
    
                        <StackLayout Orientation="Vertical" Margin="0,0,0,0" Spacing="0" VerticalOptions="Start" >
                            <Label Text="{Binding Description}" Margin="40,7,0,0" FontSize="16" FontAttributes="Bold" HorizontalOptions="Start"  BackgroundColor="White" TextColor="Red"/>
                            <StackLayout Orientation="Horizontal" Margin="0,5,0,0" HorizontalOptions="Start" VerticalOptions="Center">
                                <!--<ffimageloading:CachedImage Aspect="AspectFit" HorizontalOptions="Fill" VerticalOptions="Fill" HeightRequest="90" WidthRequest="90" Margin="15,15,15,15" Source="{Binding ImageURL}" />-->
                                <ffimageloading:CachedImage Source="{Binding ImageURL}" Aspect="AspectFit" HorizontalOptions="Fill" VerticalOptions="Fill" HeightRequest="90" WidthRequest="90" Margin="15,15,15,15"  RetryCount="200"  />
                                <StackLayout Orientation="Vertical" Margin="0,0,0,0" HorizontalOptions="Start" VerticalOptions="Center">
                                    <Label Text="{Binding ItemNoDisplay}" Margin="5,5,0,0" FontSize="16" FontAttributes="Bold" HorizontalOptions="Start"  BackgroundColor="White" TextColor="Black"/>
                                    <Label Text="{Binding VendorName}" Margin="5,-8,0,0" FontSize="16" FontAttributes="Bold" HorizontalOptions="Start"  BackgroundColor="White" TextColor="Black"/>
                                    <StackLayout Orientation="Horizontal" Margin="0,0,0,0" HorizontalOptions="Start" VerticalOptions="Start">
                                        <StackLayout Orientation="Vertical" Margin="0,0,0,0" Spacing="0" VerticalOptions="Start">
                                            <Label Text="{Binding SizeDisplay}" Margin="5,0,0,0" FontSize="14" WidthRequest="120" HorizontalOptions="Start"  VerticalOptions="Start" BackgroundColor="White" TextColor="Black"/>
                                            <Label Text="{Binding UnitPriceDisplay}" Margin="5,0,0,0" FontSize="14" WidthRequest="120" HorizontalOptions="Start"  VerticalOptions="Start" BackgroundColor="White" TextColor="Black"/>
                                            <Label Text="{Binding PriceDisplay}" Margin="5,0,0,0" FontSize="22" WidthRequest="120" FontAttributes="Bold" HorizontalOptions="Start"  VerticalOptions="Start" BackgroundColor="White" TextColor="Black"/>
                                        </StackLayout>
                                        <controls:CustomStepper ItemNo="{Binding ItemNo}">
                                        </controls:CustomStepper>
                                    </StackLayout>
                                </StackLayout>
                            </StackLayout>
                            <Label Text="Quantity Breaks" Margin="0,5,0,0" FontSize="12" FontAttributes="Bold" HorizontalOptions="Center" BackgroundColor="White" TextColor="Black" IsVisible="{Binding IsQtyBreaks, Mode=OneWay}" />
                            <StackLayout Orientation="Horizontal" Margin="0,0,0,0" HorizontalOptions="Center" VerticalOptions="Start" IsVisible="{Binding IsQtyBreaks, Mode=OneWay}">
                                <controls:QtyBreakLabel Margin="10,5,0,10" Text="{Binding QtyBreak1Label}" ItemNo="{Binding ItemNo}" Qty="{Binding QtyBreak1}" Price="{Binding PriceBreak1}" HeightRequest="25" FontSize="12" FontAttributes="Bold" WidthRequest="100" XAlign="Center" YAlign="Center" HorizontalOptions="Center" VerticalOptions="Center" BackgroundColor="LightGray" TextColor="Black" IsVisible="{Binding IsQtyBreak1, Mode=OneWay}" />
                                <Label Margin="10,5,0,10" Text="{Binding QtyBreak2Label}" HeightRequest="25" FontSize="12" FontAttributes="Bold" WidthRequest="100" XAlign="Center" YAlign="Center" HorizontalOptions="Center" VerticalOptions="Center" BackgroundColor="LightGray" TextColor="Black" IsVisible="{Binding IsQtyBreak2, Mode=OneWay}" />
                                <Label Margin="10,5,0,10" Text="{Binding QtyBreak3Label}" HeightRequest="25" FontSize="12" FontAttributes="Bold" WidthRequest="100" XAlign="Center" YAlign="Center" HorizontalOptions="Center" VerticalOptions="Center" BackgroundColor="LightGray" TextColor="Black" IsVisible="{Binding IsQtyBreak3, Mode=OneWay}" />
                            </StackLayout>
                            <BoxView BackgroundColor="gray" Color="lightgray" HeightRequest="3" HorizontalOptions="Fill" VerticalOptions="Fill" />
                        </StackLayout>
    
    */
}
