window.networkStatusInterop = {
    dotNetReference: null,

    initialize: function (dotNetRef) {
        this.dotNetReference = dotNetRef;

        // Set initial state
        this.dotNetReference.invokeMethodAsync('UpdateNetworkStatus', navigator.onLine);

        // Add event listeners for network status changes
        window.addEventListener('online', () => {
            this.dotNetReference.invokeMethodAsync('UpdateNetworkStatus', true);
        });

        window.addEventListener('offline', () => {
            this.dotNetReference.invokeMethodAsync('UpdateNetworkStatus', false);
        });
    }
};