using SQLite;

namespace TPSMobileApp
{
    public class CartItem
    {
        [PrimaryKey]
        public int ItemNo { get; set; }
        public int QtyOrder { get; set; }
        public int QtyCredit { get; set; }
        public int QtyLabel { get; set; }
        public int LineNo { get; set; }
    }
}
