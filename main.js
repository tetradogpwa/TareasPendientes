window._USER = "tetradogpwa"; //tiene que ser el usuario/organización donde está el fork

window._ROOTUTILS = "https://" + window._USER + ".github.io/Utils/";

window.Import = (url) => {

    var scriptNode = document.createElement("script");
    scriptNode.setAttribute("language", "JavaScript");
    scriptNode.setAttribute("type", "text/JavaScript");
    scriptNode.setAttribute("src", url);
    if (!window._MapImportScript)
        window._MapImportScript = new Map();
    //source:http://www.forosdelweb.com/f13/importar-archivo-js-dentro-javascript-387358/
    if (!window._MapImportScript.has(url)) {
        document.write(scriptNode.outerHTML);
        window._MapImportScript.set(url, url);
    }
};


window.onload = () => {
    var promesas;
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register('/TareasPendientes/sw.js');
        //cargo
        promesas = [Categoria.Load(),
            Tarea.Load(),
            Lista.Load()
        ];
        Promise.all(promesas).then(() => {
            //inicializo la interface
        });

    } else alert("Imposible work in this Browser!!");

};