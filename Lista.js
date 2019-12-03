window.Import(window._ROOTUTILS + "Utils/ArrayUtils.js");

class Lista {
    Lista() {
        this._parents = new Map(); //IdLista,Lista
        this._id = 0; //obtener idUnico
        this._idCategorias = new Map(); //idCategoria,Categoria
        this._idHerenciaTareasOcultas = new Map(); //idTarea,Tarea
        this._dicTareasHechas = new Map(); //idTarea,fecha
    }
    get Id() {

        return this._id;
    }
    set Id(id) {
        Lista.Remove(this);
        this._id = id;
        Lista.Add(this);
    }
    Tareas() {
        //hago una lista con todas las tareas
        return Tarea.Get(this).then((tareasLista) => {
            var tareas = tareasLista;
            this._parents.forEach((parent) => {
                parent.Tareas().then((tareasParent) => ArrayUtils.AddRange(tareas, tareasParent));
            });
            return ArrayUtils.Filter(tareas, (t) => !this._idHerenciaTareasOcultas.has(t.Id));

        });


    }
    TareasPorHacer() {
        return this.Tareas().then((tareas) => {
            return ArrayUtils.Filter(tareas, (t) => !this._dicTareasHechas.has(t.Id));
        });
    }
    EstaHecha(tarea) {
        return this._dicTareasHechas.get(tarea.Id);
    }
    HacerTarea(tarea) {
        this._dicTareasHechas.put(tarea.Id, Date.now());
    }
    RemoveTarea(tarea) {
        //si es heredada la oculta
        if (tarea.IdLista == this.Id) {
            //la quito
            Tarea.Remove(tarea);
        } else {
            //la oculto
            if (!this._idHerenciaTareasOcultas.has(tarea.Id)) {
                this._idHerenciaTareasOcultas.set(tarea.Id, tarea);
            }
        }
    }
    HasCategoria(categoria) {
        var has = this._idCategorias.has(categoria.Id);
        var iterParents;
        if (!has) {
            iterParents = this._parents.entries;
            for (var i = 0; i < iterParents.length && !has; i++)
                has = iterParents[i][1].HasCategoria(categoria);

        }
        return has;
    }
    HasParent(list, throwException = false) {
        var has;
        var iterParents;

        if (list.Id == this.Id && throwException) {
            throw "It's the Same list!!";
        } else {
            has = this._parents.has(list.Id);

            if (!has) { //así evito la recursividad infinita
                iterParents = this._parents.entries;
                for (var i = 0; i < iterParents.length && !has; i++)
                    has = iterParents[i][1].HasParent(list);

            }
        }
        return has;
    }
    AddParent(list, throwException = false) {
        try {
            if (!this.HasParent(list, true)) {
                this.parent.put(list.Id, list);
            } else {
                if (throwException)
                    throw "Ya se está heredando!!";
            }
        } catch (ex) {
            if (throwException)
                throw ex;
        }
    }

    Load(json) {

        this.Id = json.Id;
        this._parents = new Map(json.Parents);
        this._idCategorias = new Map(json.IdCategorias);
        this._idHerenciaTareasOcultas = new Map(json.IdHerenciaTareasOcultas);
        this._dicTareasHechas = new Map(json.DicTareasHechas);
    }
    Export() {
        var obj = {
            Parents = this._parents.entries(), //IdLista,Lista
            Id = 0, //obtener idUnico
            IdCategorias = this._idCategorias.entries(), //idCategoria,Categoria
            IdHerenciaTareasOcultas = this.IdHerenciaTareasOcultas.entries(), //idTarea,Tarea
            DicTareasHechas = this._dicTareasHechas.entries(), //idTarea,fecha
        };
        return JSON.stringify(obj);
    }
    static GetById(id) {
        var lista;
        if (Lista._dic) {
            lista = Lista._dic.get(id);
        }
        return lista;
    }
    static Remove(lista) {
        if (Lista._dic && Lista._dic.has(lista.Id))
            Lista._dic.delete(lista.Id);
    }
    static Add(lista) {
        if (!Lista._dic)
            Lista._dic = new Map();
        if (!Lista._dic.has(lista.Id))
            Lista._dic.put(lista.Id, lista);
    }

    static Get(categoria) {
        return Promise.resolve().then(() => {
            var listasCategoria = [];
            if (Lista._dic) {
                Lista._dic.forEach((l) => { if (l.HasCategoria(categoria)) ArrayUtils.Add(listasCategoria, l); });
            }
            return listasCategoria;
        });
    }


    static GetAll() {
        return Promise.resolve().then(() => {
            var todas;
            if (Lista._dic) {
                todas = Lista._dic.values();
            } else {
                todas = [];
            }
            return todas;
        });
    }
    static ExportAll() {
        return GetAll().then((listas) => listas.map((l) => l.Export())).then((listas) => {
            return JSON.stringify(listas);
        });
    }
    static Import(strJSONListas) {
        return Promise.resolve().then(() => {
            var listasJSON = JSON.parse(strJSONListas);
            for (var i = 0; i < listasJSON.length; i++) {
                new Lista().Load(listasJSON[i]);

            }
        });
    }


}