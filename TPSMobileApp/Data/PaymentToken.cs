namespace TPSMobileApp
{
    public class PaymentToken
    {
        public int CustId { get; set; }
        public string Token { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string CVV { get; set; }
        public string Last4 { get; set; }
        public string BillingZip { get; set; }
    }
}

