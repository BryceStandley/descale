self.addEventListener('install', function (e) {
    e.waitUntil(
        caches.open('wms-cache-v1').then(function (cache) {
            return cache.addAll([
                './',
                './index.html',
                './manifest.json',
                './css/app.css',
                './js/app.js',
                './icon-192.png',
                './icon-512.png'
            ]);
        })
    );
});

self.addEventListener('fetch', function (e) {
    e.respondWith(
        caches.match(e.request).then(function (response) {
            return response || fetch(e.request).then(function(response) {
                // Cache new requests for offline use
                if (e.request.method === 'GET' && !e.request.url.includes('/api/')) {
                    let responseClone = response.clone();
                    caches.open('wms-cache-v1').then(function (cache) {
                        cache.put(e.request, responseClone);
                    });
                }
                return response;
            }).catch(function() {
                // If both network and cache fail, show offline page
                if (e.request.url.includes('/api/')) {
                    return new Response(JSON.stringify({
                        error: 'Network error. You are offline.'
                    }), {
                        headers: new Headers({ 'Content-Type': 'application/json' })
                    });
                }
            });
        })
    );
});