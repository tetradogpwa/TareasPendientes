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

window.Import(window._ROOTUTILS + "Utils/Utils.js");


window.onload = () => {
    var promesas;

    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register('/TareasPendientes/sw.js');

        //init

        //cargo
        promesas = [Categoria.LoadAll(),
            Tarea.LoadAll(),
            Lista.LoadAll()
        ];
        Promise.all(promesas).then(() => {
            //inicializo la interface
            SetCategoria(0);
        });

    } else alert("Imposible work in this Browser!!");

};

function SetCategoria(pos) {
    var categoria = Categoria.GetById(document.getElementById("lstCategorias").children[pos + 1].getAttribute("IdCategoria"));
    var lblCategoriaActual = document.getElementById("lblCategoriaActual");
    var lstListas = document.getElementById("lstListas");
    var lst;
    lblCategoriaActual.setAttribute("IdCategoria", categoria.Id);
    lblCategoriaActual.innerText = categoria.Name;

    lstListas.children.Remove();

    Lista.Get(categoria).then((lsts) => {
        for (var i = 0; i < lsts.length; i++) {
            lst = document.createElement("li");
            lst.setAttribute("IdLista", lsts[i].Id);
            lst.innerText = lsts[i].Name;
            lst.onclick = () => PonLista(lst);
            lstListas.appendChild(lst);
        }
    });

}

function PonLista(lst) {
    var lstTareas = document.getElementById("lstTareas");
    var tarea;
    var tareaInp;
    var tareaDate;
    var chkTarea;
    var date;

    lstTareas.children.Remove();
    document.getElementById("inpNameList").innerText = lst.Name;

    lst.Tareas().then((tareas) => {
        for (var i = 0; i < tareas.length; i++) {
            tarea = document.createElement("div");
            tarea.classList.add("tarea");
            tarea.setAttribute("IdTarea", tareas[i].Id);
            tareaInp = document.createElement("input");
            tareaInp.setAttribute("type", "text");
            tareaInp.innerText = tareas[i].Text;
            tarea.appendChild(tareaInp);
            tareaDate = document.createElement("input");
            tareaDate.setAttribute("type", "text");
            tareaDate.readOnly = true;
            tarea.appendChild(tareaDate);
            chkTarea = document.createElement("input");
            chkTarea.setAttribute("type", "checkbox");

            tarea.appendChild(chkTarea);
            date = lst.EstaHecha(tareas[i]);
            if (date != undefined) {
                chkTarea.checked = true;
                tareaDate.innerText = new Date(date).toDateString();
            }
            chkTarea.onclick = () => lst.HacerTarea(tarea);
            lstTareas.appendChild(tarea);

        }

    });
}