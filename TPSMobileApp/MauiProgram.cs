using BarcodeScanning;
using FFImageLoading.Maui;
using Maui.PDFView;
using Microsoft.Extensions.Logging;
using Scandit.DataCapture.Barcode;
using Scandit.DataCapture.Core;
using Scandit.DataCapture.Core.UI.Maui;
using SQLitePCL;
using Syncfusion.Maui.Core.Hosting;
using TPSMobileApp.Data;
#if MAUI_DEVFLOW
using Microsoft.Maui.DevFlow.Agent;
#endif
namespace TPSMobileApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiPdfView()
                .UseBarcodeScanning()
                .ConfigureSyncfusionCore()
                .UseFFImageLoading()
                .UseScanditCore()
                .UseScanditBarcode(configure =>
                {
                    configure.AddSparkScanView();
                    configure.AddBarcodePickView();
                    configure.AddBarcodeFindView();
                    configure.AddBarcodeCountView();
                    configure.AddBarcodeArView();

                })
                .ConfigureMauiHandlers(handlers =>
                {
                    // Explicitly register the Scandit DataCaptureView handler
                    handlers.AddHandler(typeof(DataCaptureView), typeof(DataCaptureViewHandler));
                })

                .ConfigureFonts(fonts =>
                {

                    //fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("Font Awesome 5 Brands-Regular-400.otf", "FontAwesomeBrandsReg");
                    fonts.AddFont("Font Awesome 5 Free-Regular-400.otf", "FontAwesomeFreeReg");
                    fonts.AddFont("Font Awesome 5 Free-Solid-900.otf", "FontAwesomeFreeSolid");
                    fonts.AddFont("Font Awesome 6 Pro-Light-300.otf", "FontAwesomePro6Light");
                    fonts.AddFont("Font Awesome 6 Pro-Regular-400.otf", "FontAwesomePro6Regular");
                    fonts.AddFont("Font Awesome 6 Pro-Solid-900.otf", "FontAwesomePro6Solid");
                    fonts.AddFont("Font Awesome 6 Pro-Thin-100.otf", "FontAwesomePro6Thin");
                });

#if MAUI_DEVFLOW
            builder.AddMauiDevFlowAgent();
#endif

            try
            {
                Batteries_V2.Init();
                System.Diagnostics.Debug.WriteLine("SQLite Batteries.Init OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SQLite Init FAILED: " + ex);
            }

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // ✅ REQUIRED
            builder.Services.AddTransient<ISoapService>(sp =>
            {
                var httpClient = new HttpClient();
                return new SoapService(httpClient);
            });

            // Optional manager
            builder.Services.AddSingleton<CommManager>();

            return builder.Build();
        }
    }
}
