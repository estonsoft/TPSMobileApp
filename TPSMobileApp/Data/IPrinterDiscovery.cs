namespace TPSMobileApp.Data
{
    public interface IPrinterDiscovery
    {
        //void FindBluetoothPrinters(IDiscoveryHandler handler);
        //void FindUSBPrinters(IDiscoveryHandler handler);
        //void RequestUSBPermission(IDiscoveredPrinterUsb printer);
        void CancelDiscovery();
    }
}
