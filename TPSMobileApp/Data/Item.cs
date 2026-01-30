using SQLite;

namespace TPSMobileApp
{
    public class Item
    {
        [PrimaryKey]
        public int ItemNo { get; set; }
        public string ItemNoDisplay { get; set; }
        public string ItemNoDisplayUPC { get; set; }
        public int Qty { get; set; }
        public string QtyDisplay { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public string ImageBase64 { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryDesc { get; set; }
        public string SubcategoryCode { get; set; }
        public string SubcategoryDesc { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string UOM { get; set; }
        public int Size { get; set; }
        public string SizeDisplay { get; set; }
        public string Form { get; set; }
        public string RetailUOM { get; set; }
        public string RetailSize { get; set; }
        public string PackSize { get; set; }
        public int SellUnitsInPurchaseUnit { get; set; }
        public decimal Price { get; set; }
        public string PriceDisplay { get; set; }
        public decimal Tax { get; set; }
        public string TaxDisplay { get; set; }
        public decimal RetailPrice { get; set; }
        public string RetailPriceDisplay { get; set; }
        public String SizeUOM { get; set; }  // 12/14oz&#10;Unit: $2.29
        public int RowHeight { get; set; }
        public String UPC_1 { get; set; }
        public String UPC_2 { get; set; }
        public String UPC_3 { get; set; }
        public String UPC_4 { get; set; }
        public String Status { get; set; }
        public int QtyOrder { get; set; }
        public decimal PriceOrder { get; set; }
        public decimal ExtPriceOrder { get; set; }
        public String PriceOrderDisplay { get; set; }
        public int QtyCredit { get; set; }
        public int QtyLabel { get; set; }
        public Boolean IsCart { get; set; }
        public Boolean IsCheckout { get; set; }
        public Boolean IsLoggedIn { get; set; }
        public int CategoryRank { get; set; }
        public Boolean IsStepperVisible { get; set; }
        public Boolean IsAddToOrderVisible { get; set; }
        public string AddToOrderDisplay { get; set; }
        public int QOH { get; set; }
        public Boolean IsQOHVisible { get; set; }
        public Boolean IsInStockVisible { get; set; }
        public Boolean IsOutOfStockVisible { get; set; }
        public Boolean IsStockRowVisible { get; set; }
        public Boolean IsQOHRedVisible { get; set; }
        public Boolean IsQOHBlackVisible { get; set; }
        public int AllocationQty { get; set; }
        public Boolean IsNew { get; set; }
        public string AddedDateDisplay { get; set; }
        public DateTime LastPurchDate { get; set; }
        public string LastPurchDateDisplay { get; set; }
        public int QtyLastOrder { get; set; }
        public string QtyLastOrderDisplay { get; set; }
        public int MaxOrderQty { get; set; }
        public Boolean IsMaxOrderQtyVisible { get; set; }
        public string MaxOrderQtyDisplay { get; set; }
        public string QtyOrderDisplay { get; set; }
        public int QtyLast90 { get; set; }
        public string QtyLast90Display { get; set; }
        public bool IsLastPurchStack { get; set; }
        public int IsPriceVisible { get; set; }
        public bool IsSalesUser { get; set; }
        public bool IsChainManager { get; set; }
        public int FlyerPageNo { get; set; }
        public int FlyerBoxNo { get; set; }
        public string FlyerSection { get; set; }
        public int FlyerStartDate { get; set; }
        public int FlyerEndDate { get; set; }
        public decimal FlyerTopLeftX { get; set; }
        public decimal FlyerTopLeftY { get; set; }
        public decimal FlyerBottomRightX { get; set; }
        public decimal FlyerBottomRightY { get; set; }
        public Boolean IsBoxViewVisible { get; set; }
        public string Keyword1 { get; set; }
        public string Keyword2 { get; set; }
        public string Keyword3 { get; set; }
        public string SubsubcategoryCode { get; set; }
        public string SubsubcategoryDesc { get; set; }
        public string ItemRefNo { get; set; }
        public int LineNo { get; set; }

        public static void SetListItem(Item i, string Type)
        {
            i.IsLoggedIn = App.g_IsLoggedIn;

            i.IsStepperVisible = false;
            i.IsAddToOrderVisible = true;
            i.IsBoxViewVisible = true;

            try
            {
                if (Type == "O")
                {
                    if (i.QtyOrder > 0)
                    {
                        i.IsStepperVisible = true;
                        i.IsAddToOrderVisible = false;
                    }
                }
                else if (Type == "C")
                {
                    if (i.QtyCredit > 0)
                    {
                        i.IsStepperVisible = true;
                        i.IsAddToOrderVisible = false;
                    }
                }
                else if (Type == "L")
                {
                    if (i.QtyLabel > 0)
                    {
                        i.IsStepperVisible = true;
                        i.IsAddToOrderVisible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                i.IsStepperVisible = false;
                i.IsAddToOrderVisible = true;
            }

            i.IsQOHBlackVisible = false;
            i.IsQOHRedVisible = false;
            if (App.g_QOHDisplay == "Q")
            {
                i.IsQOHVisible = true;
                i.IsInStockVisible = false;
                i.IsOutOfStockVisible = false;
                if (i.QOH > 0)
                {
                    i.IsQOHBlackVisible = true;
                }
                else
                {
                    i.IsQOHRedVisible = true;
                }
            }
            else if (App.g_QOHDisplay == "I")
            {
                i.IsQOHVisible = false;
                if (i.QOH > 0)
                {
                    i.IsInStockVisible = true;
                    i.IsOutOfStockVisible = false;
                }
                else
                {
                    i.IsInStockVisible = false;
                    i.IsOutOfStockVisible = true;
                }
            }
            else
            {
                i.IsQOHVisible = false;
                i.IsInStockVisible = false;
                i.IsOutOfStockVisible = false;
            }
            if (i.IsQOHVisible || i.IsInStockVisible || i.IsOutOfStockVisible)
            {
                i.IsStockRowVisible = true;
            }
            else
            {
                i.IsStockRowVisible = false;
            }

            if (Type == "O")
            {
                if (App.g_BlockItemsNoQOH)
                {
                    if (i.QOH <= 0)
                    {
                        i.IsStepperVisible = false;
                        i.IsAddToOrderVisible = false;
                    }
                }
            }

            if (i.QtyLastOrder == 0)
            {
                i.IsLastPurchStack = false;
            }
            else
            {
                i.IsLastPurchStack = true;
            }

            if (App.g_IsSalesUser)
            {
                i.IsSalesUser = true;
            }
            else
            {
                i.IsSalesUser = false;
            }

            if (App.g_IsChainManager)
            {
                i.IsChainManager = true;
            }
            else
            {
                i.IsChainManager = false;
            }
        }
    }
}
