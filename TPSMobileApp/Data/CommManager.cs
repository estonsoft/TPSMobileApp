namespace TPSMobileApp.Data
{
    public class CommManager
    {
        ISoapService soapService;

        public CommManager(ISoapService service)
        {
            soapService = service;

        }


        public void GetBanners()
        {
            Task.Run(async () =>
            {
                String banner = await soapService.GetBannersAsync();
                XMLResponseParser.commService_GetBannersCompleted(banner);
            });
        }

        public void GetCategoriesAndSubcategories()
        {
            Task.Run(async () =>
            {
                String response = await soapService.GetCategoriesAndSubcategoriesAsync();
                XMLResponseParser.commService_GetCategoriesAndSubcategoriesCompleted(response);
            });
        }

        public void GetCategoriesAndSubcategoriesCust(string sCust)
        {
            Task.Run(async () =>
            {
                String response = await soapService.GetCategoriesAndSubcategoriesCustAsync(sCust);
                XMLResponseParser.commService_GetCategoriesAndSubcategoriesCustCompleted(response);
            });
        }

        public void GetItems(String sCustomer, String sDate)
        {
            Task.Run(async () =>
            {
                String response = await soapService.GetItemsAsync(sCustomer, sDate);
                XMLResponseParser.commService_GetItemsCompletedAsync(response);
            });

        }

        public void GetItemQOH(String sCustomer)
        {
            Task.Run(async () =>
            {
                String response = await soapService.GetItemQOHAsync(sCustomer);
                XMLResponseParser.commService_GetItemQOHCompletedAsync(response);
            });

        }

        public void GetItemQOH2(String sUser, String sCustomer)
        {
            Task.Run(async () =>
            {
                String response = await soapService.GetItemQOH2Async(sUser, sCustomer);
                XMLResponseParser.commService_GetItemQOH2CompletedAsync(response);
            });

        }

        public void ValidateLogin(String sUser, String sPassword, String sDeviceId)
        {
            Task.Run(async () =>
            {
                String response = await soapService.ValidateLoginAsync(sUser, sPassword, sDeviceId);
                XMLResponseParser.commService_ValidateLoginCompletedAsync(response);
            });

        }

        public void ValidateUserActive(String sUser)
        {
            Task.Run(async () =>
            {
                String response = await soapService.ValidateUserActiveAsync(sUser);
                XMLResponseParser.commService_ValidateUserActiveCompletedAsync(response);
            });

        }

        public void GetSettings()
        {
            Task.Run(async () =>
            {
                String response = await soapService.GetSettingsAsync();
                XMLResponseParser.commService_GetSettingsCompletedAsync(response);
            });

        }

        public void SubmitOrder(string sCustNo, string sPO, string sPaymentMethod, string sCCInfo, string sOrderInfo, string sDeliveryPickup, string sUser, string sNotes, int iHoldForReview, string sOrderType)
        {
            Task.Run(async () =>
            {
                String response = await soapService.SubmitOrderAsync(sCustNo, sPO, sPaymentMethod, sCCInfo, sOrderInfo, sDeliveryPickup, sUser, sNotes, iHoldForReview, sOrderType);
                XMLResponseParser.commService_SubmitOrderCompletedAsync(response);
            });

        }

        public void SubmitReturn(string sCustNo, string sOrderInfo, string sUser, string sNotes)
        {
            Task.Run(async () =>
            {
                String response = await soapService.SubmitReturnAsync(sCustNo, sOrderInfo, sUser, sNotes);
                XMLResponseParser.commService_SubmitReturnCompletedAsync(response);
            });

        }

        public void GetOrderHistory(string sCustNo)
        {
            Task.Run(async () =>
            {
                String response = await soapService.GetOrderHistoryAsync(sCustNo);
                XMLResponseParser.commService_GetOrderHistoryCompletedAsync(response);
            });

        }

        public void GetSalespersonCustomers(string sUser)
        {
            Task.Run(async () =>
            {
                String response = await soapService.GetSalespersonCustomersAsync(sUser);
                XMLResponseParser.commService_GetSalespersonCustomersCompletedAsync(response);
            });

        }

        public void GetFlyerItemsPDF()
        {
            Task.Run(async () =>
            {
                String response = await soapService.GetFlyerItemsPDFAsync();
                XMLResponseParser.commService_GetFlyerItemsPDFCompleted(response);
            });
        }
    }
}
