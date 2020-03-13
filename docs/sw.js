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
    "images/icons/icon-144x144.png",
    "images/icons/icon-512x512.png",
    "sw.js",
    "manifest.json",


"_framework/blazor.boot.json",
"_framework/blazor.webassembly.js",
"_framework/wasm/dotnet.js",
"_framework/wasm/dotnet.wasm",

"css/site.css",
"css/bootstrap/bootstrap.min.css",
"css/bootstrap/bootstrap.min.css.map",
"css/open-iconic/README.md",
"css/open-iconic/FONT-LICENCE",
"css/open-iconic/ICON-LICENCE",
"css/open-iconic/font/css/open-iconic-bootstrap.min.css",
"css/open-iconic/font/fonts/open-iconic.eot",
"css/open-iconic/font/fonts/open-iconic.otf",
"css/open-iconic/font/fonts/open-iconic.svg",
"css/open-iconic/font/fonts/open-iconic.ttf",
"css/open-iconic/font/fonts/open-iconic.woff"
];

const CONTENT=[
"_content/Tewr.Blazor.FileReader/FileReaderComponent.js",
"_content/Tewr.Blazor.FileReader/FileReaderComponent.js.map"

];
const DLL=[
"_framework/Blazor.FileReader.dll",
"_framework/Gabriel.Cat.S.Binaris.dll",
"_framework/Gabriel.Cat.S.Seguretat.dll",
"_framework/Gabriel.Cat.S.Utilitats.dll",
"_framework/Microsoft.AspNetCore.Blazor.dll",
"_framework/Microsoft.AspNetCore.Components.dll",
"_framework/Microsoft.AspNetCore.Components.Web.dll",
"_framework/Microsoft.Bcl.AsyncInterfaces.dll",
"_framework/Microsoft.CSharp.dll",
"_framework/Microsoft.Extensions.Configuration.Abstractions.dll",
"_framework/Microsoft.Extensions.Configuration.dll",
"_framework/Microsoft.Extensions.DependencyInjection.Abstractions.dll",
"_framework/Microsoft.Extensions.DependencyInjection.dll",
"_framework/Microsoft.Extensions.Logging.Abstractions.dll",
"_framework/Microsoft.Extensions.Primitives.dll",
"_framework/Microsoft.JSInterop.dll",
"_framework/Mono.WebAssembly.Interop.dll",
"_framework/mscorlib.dll",
"_framework/System.Core.dll",
"_framework/System.dll",
"_framework/System.Drawing.Common.dll",
"_framework/System.Net.Http.dll",
"_framework/System.Runtime.CompilerServices.Unsafe.dll",
"_framework/System.Text.Encodings.Web.dll",
"_framework/System.Text.Json.dll",
"_framework/TareasPendientes.Blazor2.dll",
"_framework/WebAssembly.Bindings.dll",
"_framework/WebAssembly.Net.Http.dll"


];
self.addEventListener('install', e => {

    var inmutables = self.FetchCache(CACHE_INMUTABLE + CACHE_VERSION, INMUTABLES);
    var shell = self.FetchCache(CACHE_SHELL + CACHE_VERSION, SHELL);
var content= self.FetchCache(CACHE_SHELL + CACHE_VERSION, CONTENT);
var dll= self.FetchCache(CACHE_SHELL + CACHE_VERSION, DLL);
    console.log("installing version " + CACHE_VERSION);
    e.waitUntil(Promise.all([inmutables, shell,content,dll]));

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