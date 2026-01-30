using SQLite;

namespace TPSMobileApp
{
    public class FlyerItem
    {
        [PrimaryKey]
        public int ItemNo { get; set; }
        public String Section { get; set; }
        public int Page { get; set; }
        public int Box { get; set; }
        public int StartDate { get; set; }
        public int EndDate { get; set; }
        public decimal TopLeftX { get; set; }
        public decimal TopLeftY { get; set; }
        public decimal BottomRightX { get; set; }
        public decimal BottomRightY { get; set; }
    }
}
