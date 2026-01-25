using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Source;

namespace TPSMobileApp.Models
{
    public class DataCaptureManager
    {
        private static readonly Lazy<DataCaptureManager> instance =
            new Lazy<DataCaptureManager>(() => new DataCaptureManager(), LazyThreadSafetyMode.PublicationOnly);

        public static DataCaptureManager Instance => instance.Value;

        public bool IsLicenseValid { get; private set; }

        [Obsolete]
        private DataCaptureManager()
        {
            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                this.DataCaptureContext = DataCaptureContext.ForLicenseKey("AajWhqWBMbiEC9TwbzOrIiVC9bwaGN2XjFgSPG5JfcmYZQSPBmjR8tQHV9HxUSlS8UbFAw4969tNVPetAXr574kHbdI6X40gmhcVPQhwN4VOa8H7yFLfeS1AfQK1bAPEniAHWmIykj66GFqTLxFPaTImTU9OOz3w6s7mx6uU5w9Mtuo/+qw/MIQeYi2PLQipaJiF3rjuSTUSRI9oPDqS4c9TLVtVOyFxdRWuz6AGlqvZ6kXfhiHrZOrghTxdvNxMGh9Shn9xmdfCAd8H8skKVMOiespzTztSOZJ/c6ZOZvcN4vNzziIjzSwRk6ZM/0N2Oi/gABfR9N8S5BdsBSIG7uu8OcRDFFV7bD0iSmEAmEirPMvIQPcvNtx+2GwDYvQccx1tT7W7bWwy8Ljo/Mg5xx8Cn3EYeYGB/9JQJZOHGsNDLFEMF/7wKkdXDvb3MAOaDbpPwZs8xYbpe87jFLGJWuHG6b82Zuo+Q42GQQPaXhF1esOG0e1bhAdXd3K7+jozi6FisKdZzS758vJAWEwMI4QlwALB8kMzD+9ldsEsngVaMoA8eiUec7+yj6DJFDM1wqxCS4ci/vlXPUojBznNi9rfKc4Q1qYnV7t558PLX6nLlrakz8MbYsKeEm5XHncnWQVndrK20NRqLGc3tHDn4nZ0W94gcvmf205omHDgdk9RWIzYV3wEItXD41l4ht4lYDCixScq2r75CUQvJEUP3dCw8CaP8qks9by/i/geYysFxd1cE9m3DIzxawTE/2klyqJIdXRM1A4tr0TdGC9ggrfN9JnDxbBdShXu0E+q5bNI06PuCtAKF6wow8EOGvVEjg==");
            }
            else if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                this.DataCaptureContext = DataCaptureContext.ForLicenseKey("AajWhqWBMbiEC9TwbzOrIiVC9bwaGN2XjFgSPG5JfcmYZQSPBmjR8tQHV9HxUSlS8UbFAw4969tNVPetAXr574kHbdI6X40gmhcVPQhwN4VOa8H7yFLfeS1AfQK1bAPEniAHWmIykj66GFqTLxFPaTImTU9OOz3w6s7mx6uU5w9Mtuo/+qw/MIQeYi2PLQipaJiF3rjuSTUSRI9oPDqS4c9TLVtVOyFxdRWuz6AGlqvZ6kXfhiHrZOrghTxdvNxMGh9Shn9xmdfCAd8H8skKVMOiespzTztSOZJ/c6ZOZvcN4vNzziIjzSwRk6ZM/0N2Oi/gABfR9N8S5BdsBSIG7uu8OcRDFFV7bD0iSmEAmEirPMvIQPcvNtx+2GwDYvQccx1tT7W7bWwy8Ljo/Mg5xx8Cn3EYeYGB/9JQJZOHGsNDLFEMF/7wKkdXDvb3MAOaDbpPwZs8xYbpe87jFLGJWuHG6b82Zuo+Q42GQQPaXhF1esOG0e1bhAdXd3K7+jozi6FisKdZzS758vJAWEwMI4QlwALB8kMzD+9ldsEsngVaMoA8eiUec7+yj6DJFDM1wqxCS4ci/vlXPUojBznNi9rfKc4Q1qYnV7t558PLX6nLlrakz8MbYsKeEm5XHncnWQVndrK20NRqLGc3tHDn4nZ0W94gcvmf205omHDgdk9RWIzYV3wEItXD41l4ht4lYDCixScq2r75CUQvJEUP3dCw8CaP8qks9by/i/geYysFxd1cE9m3DIzxawTE/2klyqJIdXRM1A4tr0TdGC9ggrfN9JnDxbBdShXu0E+q5bNI06PuCtAKF6wow8EOGvVEjg==");
            }

            DataCaptureContext.StatusChanged += OnStatusChanged;
        }

        #region Initialization

        public void InitializeCamera()
        {
            DataCaptureContext.SetFrameSourceAsync(CurrentCamera);
            CurrentCamera?.ApplySettingsAsync(CameraSettings);
        }

        public void InitializeBarcodeCapture()
        {
            DataCaptureContext.RemoveAllModes();

            BarcodeCaptureSettings = BarcodeCaptureSettings.Create();

            var symbologies = new HashSet<Symbology>
            {
                Symbology.Ean13Upca,
                Symbology.Ean8,
                Symbology.Upce,
                Symbology.Code39,
                Symbology.Code128,
                Symbology.InterleavedTwoOfFive
            };

            BarcodeCaptureSettings.EnableSymbologies(symbologies);
            BarcodeCapture = BarcodeCapture.Create(DataCaptureContext, BarcodeCaptureSettings);
        }

        #endregion

        #region DataCaptureContext

        public DataCaptureContext DataCaptureContext { get; }

        #endregion

        #region Camera

        public Camera CurrentCamera { get; } = Camera.GetDefaultCamera();
        public CameraSettings CameraSettings { get; } = BarcodeCapture.RecommendedCameraSettings;

        #endregion

        #region BarcodeCapture

        public BarcodeCapture BarcodeCapture { get; private set; }
        public BarcodeCaptureSettings BarcodeCaptureSettings { get; private set; }

        #endregion

        private void OnStatusChanged(object sender, StatusChangedEventArgs e)
        {
            IsLicenseValid = e.Status.Valid;
        }
    }
}
