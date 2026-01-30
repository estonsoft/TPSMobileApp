using System.Text;
using System.Xml.Linq;

namespace TPSMobileApp.Data
{
    public class SoapService : ISoapService
    {
        private readonly HttpClient _httpClient;
        private const string SoapUrl = "https://ramtest.qwikpoint.net/RemotePhoneApp.asmx";

        public SoapService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<string> SendSoapRequestAsync(string soapAction, string soapBody)
        {
            var content = new StringContent(soapBody, Encoding.UTF8, "text/xml");

            content.Headers.Clear();
            content.Headers.Add("Content-Type", "text/xml; charset=utf-8");
            content.Headers.Add("SOAPAction", soapAction);

            var response = await _httpClient.PostAsync(SoapUrl, content);
            response.EnsureSuccessStatusCode();
            string responseValue = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Login Response" + responseValue);
            responseValue = ExtractSoapResult(responseValue);
            return responseValue;
        }

        private string ExtractSoapResult(string soapXml)
        {
            var doc = XDocument.Parse(soapXml);

            return doc
                .Descendants()
                .FirstOrDefault(x => x.Name.LocalName.EndsWith("Result"))
                ?.Value;
        }

        public Task<string> GetBannersAsync()
        {
            return SendSoapRequestAsync(
                "http://turningpointremotephoneapp.com/GetBanners",
                @"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                  <soap:Body>
                    <GetBanners xmlns=""http://turningpointremotephoneapp.com/"" />
                  </soap:Body>
                </soap:Envelope>");
        }

        public Task<string> GetCategoriesAndSubcategoriesAsync()
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/GetCategoriesAndSubcategories",
                SoapEnvelope("GetCategoriesAndSubcategories"));

        public Task<string> GetCategoriesAndSubcategoriesCustAsync(string sCust)
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/GetCategoriesAndSubcategoriesCust",
                SoapEnvelope("GetCategoriesAndSubcategoriesCust", $"<sCust>{sCust}</sCust>"));

        public Task<string> GetItemsAsync(string sCust, string sDate)
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/GetItems",
                SoapEnvelope("GetItems",
                    $"<sCust>{sCust}</sCust><sDate>{sDate}</sDate>"));

        public Task<string> GetItemQOHAsync(string sCust)
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/GetItemQOH",
                SoapEnvelope("GetItemQOH", $"<sCust>{sCust}</sCust>"));

        public Task<string> GetItemQOH2Async(string sUser, string sCust)
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/GetItemQOH2",
                SoapEnvelope("GetItemQOH2",
                    $"<sUser>{sUser}</sUser><sCust>{sCust}</sCust>"));

        public Task<string> ValidateLoginAsync(string sUser, string sPassword, string sDeviceId)
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/ValidateLogin",
                SoapEnvelope("ValidateLogin",
                    $"<sUser>{sUser}</sUser><sPassword>{sPassword}</sPassword><sDeviceId>{sDeviceId}</sDeviceId>"));

        public Task<string> ValidateUserActiveAsync(string sUser)
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/ValidateUserActive",
                SoapEnvelope("ValidateUserActive", $"<sUser>{sUser}</sUser>"));

        public Task<string> GetSettingsAsync()
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/GetSettings",
                SoapEnvelope("GetSettings"));

        public Task<string> SubmitOrderAsync(string sCustNo, string sPO, string sPaymentMethod,
            string sCCInfo, string sOrderInfo, string sDeliveryPickup,
            string sUser, string sNotes, int iHoldForReview, string sOrderType)
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/SubmitOrder",
                SoapEnvelope("SubmitOrder",
                    $"<sCustNo>{sCustNo}</sCustNo><sPO>{sPO}</sPO><sPaymentMethod>{sPaymentMethod}</sPaymentMethod>" +
                    $"<sCCInfo>{sCCInfo}</sCCInfo><sOrderInfo>{sOrderInfo}</sOrderInfo>" +
                    $"<sDeliveryPickup>{sDeliveryPickup}</sDeliveryPickup><sUser>{sUser}</sUser>" +
                    $"<sNotes>{sNotes}</sNotes><iHoldForReview>{iHoldForReview}</iHoldForReview>" +
                    $"<sOrderType>{sOrderType}</sOrderType>"));

        public Task<string> SubmitReturnAsync(string sCust, string sOrderInfo, string sUser, string sNotes)
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/SubmitReturn",
                SoapEnvelope("SubmitReturn",
                    $"<sCust>{sCust}</sCust><sOrderInfo>{sOrderInfo}</sOrderInfo><sUser>{sUser}</sUser><sNotes>{sNotes}</sNotes>"));

        public Task<string> GetOrderHistoryAsync(string sCust)
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/GetOrderHistory",
                SoapEnvelope("GetOrderHistory", $"<sCust>{sCust}</sCust>"));

        public Task<string> GetSalespersonCustomersAsync(string sUser)
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/GetSalespersonCustomers",
                SoapEnvelope("GetSalespersonCustomers", $"<sUser>{sUser}</sUser>"));

        public Task<string> GetFlyerItemsPDFAsync()
            => SendSoapRequestAsync("http://turningpointremotephoneapp.com/GetFlyerItemsPDF",
                SoapEnvelope("GetFlyerItemsPDF"));

        private static string SoapEnvelope(string method, string parameters = "")
        {
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
            <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                           xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
                           xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
              <soap:Body>
                <{method} xmlns=""http://turningpointremotephoneapp.com/"">
                  {parameters}
                </{method}>
              </soap:Body>
            </soap:Envelope>";
        }
    }
}
