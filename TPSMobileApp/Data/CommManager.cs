namespace TPSMobileApp.Data
{
    public class CommManager
    {
        ISoapService soapService;

        public CommManager(ISoapService service)
        {
            soapService = service;
        }


        public async Task GetBanners()
        {
            await App.UpdateProgress(5, "Downloading banner data");
            String banner = await soapService.GetBannersAsync();
            await XMLResponseParser.commService_GetBannersCompleted(banner);
        }

        public async Task GetCategoriesAndSubcategories()
        {
            await App.UpdateProgress(10, "Downloading Categories data");
            String response = await soapService.GetCategoriesAndSubcategoriesAsync();
            await XMLResponseParser.commService_GetCategoriesAndSubcategoriesCompleted(response);
        }

        public async Task GetCategoriesAndSubcategoriesCust(string sCust)
        {
            await App.UpdateProgress(10, "Downloading Categories data");
            String response = await soapService.GetCategoriesAndSubcategoriesCustAsync(sCust);
            await XMLResponseParser.commService_GetCategoriesAndSubcategoriesCustCompleted(response);
        }

        public async Task GetItems(String sCustomer, String sDate)
        {
            await App.UpdateProgress(20, "Downloading Items data");
            String response = await soapService.GetItemsAsync(sCustomer, sDate);
            await XMLResponseParser.commService_GetItemsCompletedAsync(response);
        }
        public async Task GetItemQOH(String sCustomer)
        {
            await App.UpdateProgress(60, "Downloading Item quantity");
            String response = await soapService.GetItemQOHAsync(sCustomer);
            await XMLResponseParser.commService_GetItemQOHCompletedAsync(response);
        }

        public async Task GetItemQOH2(String sUser, String sCustomer)
        {
            await App.UpdateProgress(87, "Downloading Item quantity");
            String response = await soapService.GetItemQOH2Async(sUser, sCustomer);
            await XMLResponseParser.commService_GetItemQOH2CompletedAsync(response);
        }

        public async Task ValidateLogin(String sUser, String sPassword, String sDeviceId)
        {
            String response = await soapService.ValidateLoginAsync(sUser, sPassword, sDeviceId);
            await XMLResponseParser.commService_ValidateLoginCompletedAsync(response);
        }

        public async Task ValidateUserActive(String sUser)
        {
            String response = await soapService.ValidateUserActiveAsync(sUser);
            await XMLResponseParser.commService_ValidateUserActiveCompletedAsync(response);
        }
        public async Task GetSettings()
        {
            await App.UpdateProgress(0, "Downloading Settings");
            String response = await soapService.GetSettingsAsync();
            await XMLResponseParser.commService_GetSettingsCompletedAsync(response);
        }
        public async Task SubmitOrder(string sCustNo, string sPO, string sPaymentMethod, string sCCInfo, string sOrderInfo, string sDeliveryPickup, string sUser, string sNotes, int iHoldForReview, string sOrderType)
        {
            String response = await soapService.SubmitOrderAsync(sCustNo, sPO, sPaymentMethod, sCCInfo, sOrderInfo, sDeliveryPickup, sUser, sNotes, iHoldForReview, sOrderType);
            await XMLResponseParser.commService_SubmitOrderCompletedAsync(response);
        }
        public async Task SubmitReturn(string sCustNo, string sOrderInfo, string sUser, string sNotes)
        {
            String response = await soapService.SubmitReturnAsync(sCustNo, sOrderInfo, sUser, sNotes);
            await XMLResponseParser.commService_SubmitReturnCompletedAsync(response);
        }

        public async Task GetOrderHistory(string sCustNo)
        {
            await App.UpdateProgress(70, "Downloading Order History");
            String response = await soapService.GetOrderHistoryAsync(sCustNo);
            await XMLResponseParser.commService_GetOrderHistoryCompletedAsync(response);
        }

        public async Task<string> GetInvoicePDF(string sOrder)
        {
            String response = await soapService.GetInvoicePDFAsync(sOrder);
            return response;
            //await XMLResponseParser.commService_GetInvoicePDFCompletedAsync(response);
        }

        public async Task GetSalespersonCustomers(string sUser)
        {
            String response = await soapService.GetSalespersonCustomersAsync(sUser);
            await XMLResponseParser.commService_GetSalespersonCustomersCompletedAsync(response);
        }

        public async Task GetFlyerItemsPDF()
        {
            String response = await soapService.GetFlyerItemsPDFAsync();
            await XMLResponseParser.commService_GetFlyerItemsPDFCompleted(response);
        }
    }
}
