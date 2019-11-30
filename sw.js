const CACHE_VERSION_ANTERIOR = 0; //subo aqui para no tener problemas :D
const CACHE_VERSION = CACHE_VERSION_ANTERIOR + 1;
const APP = "TareasPendientes";

const CACHE_INMUTABLE = "CACHE_INMUTABLE_" + APP;
const CACHE_SHELL = "CACHE_SHELL_" + APP;
const CACHE_DINAMICO = "CACHE_DINAMICO_" + APP;
const INMUTABLES = [

];
const SHELL = [

    "index.html",
    "style.css",
    "images/icons/icon-144x144.png",
    "images/icons/icon-512x512.png",
    "sw.js",
    "manifest.json",
    "main.js",
    "Lista.js",
    "Tarea.js",
    "Categoria.js",

];


self.addEventListener('install', e => {

    var inmutables = self.FetchCache(CACHE_INMUTABLE + CACHE_VERSION, INMUTABLES);
    var shell = self.FetchCache(CACHE_SHELL + CACHE_VERSION, SHELL);
    console.log("installing version " + CACHE_VERSION);
    e.waitUntil(Promise.all([inmutables, shell]));

});


self.addEventListener('activate', e => {
    console.log("uninstalling version " + CACHE_VERSION_ANTERIOR);
    e.waitUntil(Promise.all([caches.delete(CACHE_INMUTABLE + CACHE_VERSION_ANTERIOR),
        caches.delete(CACHE_SHELL + CACHE_VERSION_ANTERIOR),
        caches.delete(CACHE_DINAMICO + CACHE_VERSION)
    ]));

});

self.addEventListener('fetch', e => {

    e.respondWith(caches.match(e.request).then(resp => {
        var respuesta;
        if (resp)
            respuesta = resp;
        else {
            respuesta = fetch(e.request)
                .then(data => {
                    return caches.open(CACHE_DINAMICO + CACHE_VERSION_ANTERIOR)
                        .then(cache => {
                            cache.put(e.request, data.clone());
                            return data;
                        });
                });
        }
        return respuesta;

    }));

});



function FetchCache(cache_name, urls) {
    return caches.open(cache_name)
        .then(cache => {

            cache.addAll(urls);

        });
}