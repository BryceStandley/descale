window.barcodeScannerInterop = {
    dotNetReference: null,

    initialize: function (dotNetRef) {
        this.dotNetReference = dotNetRef;
        console.log("Barcode scanner initialized");
    },

    startScanning: async function () {
        try {
            // Check if the browser supports the BarcodeDetector API
            if ('BarcodeDetector' in window) {
                const barcodeDetector = new BarcodeDetector({
                    formats: ['qr_code', 'code_128', 'code_39', 'ean_13', 'upc_a', 'upc_e']
                });

                const stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: "environment" } });
                const videoElement = document.getElementById('barcode-scanner-view');

                if (!videoElement) {
                    const video = document.createElement('video');
                    video.id = 'barcode-scanner-view';
                    video.style.position = 'fixed';
                    video.style.top = '0';
                    video.style.left = '0';
                    video.style.width = '100%';
                    video.style.height = '100%';
                    video.style.objectFit = 'cover';
                    video.style.zIndex = '1000';
                    document.body.appendChild(video);

                    video.srcObject = stream;
                    video.play();

                    this.scanningInterval = setInterval(async () => {
                        const barcodes = await barcodeDetector.detect(video);
                        if (barcodes.length > 0) {
                            for (const barcode of barcodes) {
                                this.dotNetReference.invokeMethodAsync('OnBarcodeScanned', barcode.rawValue);
                                this.stopScanning();
                                break;
                            }
                        }
                    }, 100);
                }
            } else {
                console.log("BarcodeDetector is not supported by this browser");
                // Fallback to manual entry or other libraries like QuaggaJS
                // For now, simulate a scan with a prompt
                const barcode = prompt("Enter barcode manually:");
                if (barcode) {
                    this.dotNetReference.invokeMethodAsync('OnBarcodeScanned', barcode);
                }
            }
        } catch (error) {
            console.error("Error starting barcode scanner:", error);
        }
    },

    stopScanning: function () {
        clearInterval(this.scanningInterval);
        const videoElement = document.getElementById('barcode-scanner-view');
        if (videoElement) {
            const stream = videoElement.srcObject;
            if (stream) {
                const tracks = stream.getTracks();
                tracks.forEach(track => track.stop());
            }
            videoElement.remove();
        }
    }
};