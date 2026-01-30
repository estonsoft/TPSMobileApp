namespace TPSMobileApp.Data
{

    public interface ISoapService
    {
        Task<string> GetBannersAsync();
        Task<string> GetCategoriesAndSubcategoriesAsync();
        Task<string> GetCategoriesAndSubcategoriesCustAsync(string sCust);
        Task<string> GetItemsAsync(string sCustomer, string sDate);
        Task<string> GetItemQOHAsync(string sCustomer);
        Task<string> GetItemQOH2Async(string sUser, string sCustomer);
        Task<string> ValidateLoginAsync(String sUser, String sPassword, String sDeviceId);
        Task<string> ValidateUserActiveAsync(String sUser);
        Task<string> GetSettingsAsync();
        Task<string> SubmitOrderAsync(string sCustNo, string sPO, string sPaymentMethod, string sCCInfo, string sOrderInfo, string sDeliveryPickup, string sUser, string sNotes, int iHoldForReview, string sOrderType);
        Task<string> SubmitReturnAsync(string sCustNo, string sOrderInfo, string sUser, string sNotes);
        Task<string> GetOrderHistoryAsync(string sCustNo);
        Task<string> GetSalespersonCustomersAsync(string sUser);
        Task<string> GetFlyerItemsPDFAsync();
    }
}
