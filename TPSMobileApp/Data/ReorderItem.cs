using SQLite;

namespace TPSMobileApp
{
    public class ReorderItem
    {
        [PrimaryKey]
        public int ItemNo { get; set; }
        public string ItemNoDisplay { get; set; }
        public DateTime LastPurchDate { get; set; }
        public string LastPurchDateDisplay { get; set; }
        public int QtyLastOrder { get; set; }
        public string QtyOrderDisplay { get; set; }
        public int QtyLast90 { get; set; }
        public string QtyLast90Display { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PriceDisplay { get; set; }
        public string UOM { get; set; }
        public string Size { get; set; }
        public string Form { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryDesc { get; set; }
        public string SubcategoryCode { get; set; }
        public string SubcategoryDesc { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string SellUnitsInPurch { get; set; }
        public string SizeDisplay { get; set; }
        public string UPC { get; set; }
        public string ItemNoDisplayUPC { get; set; }
        public string ImageURL { get; set; }
        public string ImageBase64 { get; set; }
        public bool IsLoggedIn { get; set; }
        public int RowHeight { get; set; }
        public Boolean IsStepperVisible { get; set; }
        public Boolean IsAddToOrderVisible { get; set; }
        public int QtyOrder { get; set; }
        public string SizeUOM { get; set; }
        public string UnitPriceDisplay { get; set; }
        public string UnitPriceStandardSell { get; set; }
        public string Status { get; set; }
        public int QOH { get; set; }
        public Boolean IsQOHVisible { get; set; }
        public Boolean IsInStockVisible { get; set; }
        public Boolean IsOutOfStockVisible { get; set; }
        public Boolean IsStockRowVisible { get; set; }
        public Boolean IsQOHRedVisible { get; set; }
        public Boolean IsQOHBlackVisible { get; set; }
        public bool IsLastPurchStack { get; set; }
        public int IsPriceVisible { get; set; }
        public bool IsSalesUser { get; set; }
        public bool IsChainManager { get; set; }

        public static void SetListItem(ReorderItem i)
        {
            i.IsLoggedIn = App.g_IsLoggedIn;

            i.IsStepperVisible = false;
            i.IsAddToOrderVisible = true;

            try
            {
                if (i.QtyOrder > 0)
                {
                    i.IsStepperVisible = true;
                    i.IsAddToOrderVisible = false;
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

            if (App.g_BlockItemsNoQOH)
            {
                if (i.QOH <= 0)
                {
                    i.IsStepperVisible = false;
                    i.IsAddToOrderVisible = false;
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
