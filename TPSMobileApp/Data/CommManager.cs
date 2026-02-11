namespace TPSMobileApp.Data
{
    public class CommManager
    {
        ISoapService soapService;

        public CommManager(ISoapService service)
        {
            soapService = service;

        }


        public async void GetBanners()
        {
            String banner = await soapService.GetBannersAsync();
            XMLResponseParser.commService_GetBannersCompleted(banner);
        }

        public async void GetCategoriesAndSubcategories()
        {
            String response = await soapService.GetCategoriesAndSubcategoriesAsync();
            XMLResponseParser.commService_GetCategoriesAndSubcategoriesCompleted(response);
        }

        public async void GetCategoriesAndSubcategoriesCust(string sCust)
        {
            String response = await soapService.GetCategoriesAndSubcategoriesCustAsync(sCust);
            XMLResponseParser.commService_GetCategoriesAndSubcategoriesCustCompleted(response);
        }

        public async void GetItems(String sCustomer, String sDate)
        {
            String response = await soapService.GetItemsAsync(sCustomer, sDate);
            XMLResponseParser.commService_GetItemsCompletedAsync(response);
        }
        public async void GetItemQOH(String sCustomer)
        {
            String response = await soapService.GetItemQOHAsync(sCustomer);
            XMLResponseParser.commService_GetItemQOHCompletedAsync(response);
        }

        public async void GetItemQOH2(String sUser, String sCustomer)
        {
            String response = await soapService.GetItemQOH2Async(sUser, sCustomer);
            XMLResponseParser.commService_GetItemQOH2CompletedAsync(response);
        }

        public async void ValidateLogin(String sUser, String sPassword, String sDeviceId)
        {
            String response = await soapService.ValidateLoginAsync(sUser, sPassword, sDeviceId);
            XMLResponseParser.commService_ValidateLoginCompletedAsync(response);
        }

        public async void ValidateUserActive(String sUser)
        {
            String response = await soapService.ValidateUserActiveAsync(sUser);
            XMLResponseParser.commService_ValidateUserActiveCompletedAsync(response);
        }
        public async void GetSettings()
        {
            String response = await soapService.GetSettingsAsync();
            XMLResponseParser.commService_GetSettingsCompletedAsync(response);
        }
        public async void SubmitOrder(string sCustNo, string sPO, string sPaymentMethod, string sCCInfo, string sOrderInfo, string sDeliveryPickup, string sUser, string sNotes, int iHoldForReview, string sOrderType)
        {
            String response = await soapService.SubmitOrderAsync(sCustNo, sPO, sPaymentMethod, sCCInfo, sOrderInfo, sDeliveryPickup, sUser, sNotes, iHoldForReview, sOrderType);
            XMLResponseParser.commService_SubmitOrderCompletedAsync(response);
        }
        public async void SubmitReturn(string sCustNo, string sOrderInfo, string sUser, string sNotes)
        {
            String response = await soapService.SubmitReturnAsync(sCustNo, sOrderInfo, sUser, sNotes);
            XMLResponseParser.commService_SubmitReturnCompletedAsync(response);
        }

        public async void GetOrderHistory(string sCustNo)
        {
            String response = await soapService.GetOrderHistoryAsync(sCustNo);
            XMLResponseParser.commService_GetOrderHistoryCompletedAsync(response);
        }

        public async void GetSalespersonCustomers(string sUser)
        {
            String response = await soapService.GetSalespersonCustomersAsync(sUser);
            XMLResponseParser.commService_GetSalespersonCustomersCompletedAsync(response);
        }

        public async void GetFlyerItemsPDF()
        {
            String response = await soapService.GetFlyerItemsPDFAsync();
            XMLResponseParser.commService_GetFlyerItemsPDFCompleted(response);
        }
    }
}
