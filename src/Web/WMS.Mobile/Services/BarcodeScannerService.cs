using Microsoft.JSInterop;

namespace descale.Web.WMS.Mobile.Services
{
    public class BarcodeScannerService
    {
        private readonly IJSRuntime _jsRuntime;
        private DotNetObjectReference<BarcodeScannerService> _objRef;

        public BarcodeScannerService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public event Action<string> BarcodeScanned;

        public async Task InitializeAsync()
        {
            _objRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync("barcodeScannerInterop.initialize", _objRef);
        }

        public async Task StartScanningAsync()
        {
            await _jsRuntime.InvokeVoidAsync("barcodeScannerInterop.startScanning");
        }

        public async Task StopScanningAsync()
        {
            await _jsRuntime.InvokeVoidAsync("barcodeScannerInterop.stopScanning");
        }

        [JSInvokable]
        public void OnBarcodeScanned(string barcode)
        {
            BarcodeScanned?.Invoke(barcode);
        }

        public void Dispose()
        {
            _objRef?.Dispose();
        }
    }
}