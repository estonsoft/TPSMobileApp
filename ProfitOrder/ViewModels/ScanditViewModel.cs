/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using CommunityToolkit.Mvvm.Messaging;
using Scandit.DataCapture.Barcode.Capture;
using Scandit.DataCapture.Barcode.Data;
using Scandit.DataCapture.Core.Capture;
using Scandit.DataCapture.Core.Data;
using Scandit.DataCapture.Core.Source;
using TPSMobileApp.Models;
using TPSMobileApp.Views;

namespace TPSMobileApp.ViewModels
{
    public class ScanditViewModelBase : BaseViewModel, IBarcodeCaptureListener
    {
        public QuickEntryPage ScannerPage;

        private readonly DataCaptureManager dataCaptureManager = DataCaptureManager.Instance;

        public DataCaptureContext DataCaptureContext => this.dataCaptureManager.DataCaptureContext;
        public BarcodeCapture BarcodeCapture => this.dataCaptureManager.BarcodeCapture;

        public ScanditViewModelBase(QuickEntryPage quickEntryPage)
        {
            this.InitializeScanner();
            this.ScannerPage = quickEntryPage;
            this.SubscribeToAppMessages();
        }

        public Task OnSleep()
        {
            BarcodeCapture.Enabled = false;
            return this.dataCaptureManager.CurrentCamera?.SwitchToDesiredStateAsync(FrameSourceState.Off);
        }

        public async Task OnResumeAsync()
        {
            var permissionStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();

            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<Permissions.Camera>();
                if (permissionStatus == PermissionStatus.Granted)
                {
                    BarcodeCapture.Enabled = true;
                    await this.ResumeFrameSource();
                }
            }
            else
            {
                BarcodeCapture.Enabled = true;
                await this.ResumeFrameSource();
            }
        }

        private void SubscribeToAppMessages()
        {
            //WeakReferenceMessenger.Default.Register<ScanditViewModelBase>(this, async (recipient, message) =>
            //{
            //    await OnResumeAsync();
            //    await OnSleep();
            //});
        }

        private void InitializeScanner()
        {
            this.dataCaptureManager.InitializeCamera();
            this.dataCaptureManager.InitializeBarcodeCapture();

            // Register self as a listener to get informed whenever a new barcode got recognized.
            this.dataCaptureManager.BarcodeCapture.AddListener(this);
        }

        private Task ResumeFrameSource()
        {
            // Switch camera on to start streaming frames.
            // The camera is started asynchronously and will take some time to completely turn on.
            return this.dataCaptureManager.CurrentCamera?.SwitchToDesiredStateAsync(FrameSourceState.On);
        }

        #region IBarcodeCaptureListener
        public void OnObservationStarted(BarcodeCapture barcodeCapture)
        { }

        public void OnObservationStopped(BarcodeCapture barcodeCapture)
        { }

        public void OnBarcodeScanned(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData frameData)
        {
            //if (!session.NewlyLocalizedBarcodes.Any())
            //{
            //    session.Reset();
            //    return;
            //}

            Barcode barcode = null;

            try
            {
                barcode = session.NewlyRecognizedBarcode;
            }
            catch
            {
                return;
            }

            // Stop recognizing barcodes for as long as we are displaying the result. There won't be any new results until
            // the capture mode is enabled again. Note that disabling the capture mode does not stop the camera, the camera
            // continues to stream frames until it is turned off.
            OnSleep();

            // If you are not disabling barcode capture here and want to continue scanning, consider
            // setting the codeDuplicateFilter when creating the barcode capture settings to around 500
            // or even -1 if you do not want codes to be scanned more than once.

            // Get the human readable name of the symbology and assemble the result to be shown.

            //SymbologyDescription description = new SymbologyDescription(barcode.Symbology);
            //string result = string.Format(AppResources.ScanResultFormat, description.ReadableName, barcode.Data, barcode.SymbolCount);

            //DependencyService.Get<IMessageService>().ShowAsync(barcode.Data, () => barcodeCapture.Enabled = true);

            try
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        Vibration.Vibrate();
                    }
                    catch (Exception e)
                    {
                    }

                    try
                    {
                        //ScannerPage.SetScanItem(barcode.Data);
                        ScannerPage.ScanComplete(barcode.Data);
                        //ScannerPage.FindItem(barcode.Data);
                    }
                    catch
                    {
                    }

                    //App.g_ScanBarcode = barcode.Data;
                    //session.Reset();
                    //App.g_Shell.GoToItemSearch();
                });
            }
            catch (Exception e)
            {
                //barcodeCapture.Enabled = true;
                //OnResumeAsync();
            }
        }

        public void OnSessionUpdated(BarcodeCapture barcodeCapture, BarcodeCaptureSession session, IFrameData frameData)
        { }
        #endregion
    }
}
