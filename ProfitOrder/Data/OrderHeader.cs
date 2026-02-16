using SQLite;

namespace TPSMobileApp
{
    public class OrderHeader
    {
        [PrimaryKey]
        public string OrderNo { get; set; }
        public int CustId { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderDateDisplay { get; set; }
        public int Items { get; set; }
        public int Pieces { get; set; }
        public decimal Total { get; set; }
        public string TotalDisplay { get; set; }
        public string Status { get; set; }
    }
}
