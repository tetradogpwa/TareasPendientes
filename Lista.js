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
        if (!(tarea instanceof Promise))
            tarea = Promise.resolve(tarea);
        return tarea.then((t) => this._dicTareasHechas.get(t.Id));
    }
    HacerTarea(tarea) {
        if (!(tarea instanceof Promise))
            tarea = Promise.resolve(tarea);
        return tarea.then((t) => {
            this._dicTareasHechas.put(t.Id, Date.now());
        });
    }
    RemoveTarea(tarea) {
        if (!(tarea instanceof Promise))
            tarea = Promise.resolve(tarea);
        return tarea.then((t) => {
            //si es heredada la oculta
            if (t.IdLista == this.Id) {
                //la quito
                Tarea.Remove(t);
            } else {
                //la oculto
                if (!this._idHerenciaTareasOcultas.has(t.Id)) {
                    this._idHerenciaTareasOcultas.set(t.Id, t);
                }
            }
        });
    }
    HasCategoria(categoria) {
        if (!(categoria instanceof Promise))
            categoria = Promise.resolve(categoria);
        return categoria.then((c) => {
            var has = this._idCategorias.has(c.Id);
            var iterParents;
            if (!has) {
                iterParents = this._parents.values();
                for (var i = 0; i < iterParents.length && !has; i++)
                    has = iterParents[i].HasCategoria(c).resolve();

            }
            return has;
        });

    }
    HasParent(list, throwException = false) {
        if (!(list instanceof Promise))
            list = Promise.resolve(list);
        return list.then((l) => {
            var has;
            var iterParents;

            if (l.Id == this.Id && throwException) {
                throw "It's the Same list!!";
            } else {
                has = this._parents.has(l.Id);

                if (!has) { //así evito la recursividad infinita
                    iterParents = this._parents.values();
                    for (var i = 0; i < iterParents.length && !has; i++)
                        has = iterParents[i].HasParent(l).revolve();

                }
            }
            return has;
        });

    }
    AddParent(list, throwException = false) {
        if (!(list instanceof Promise))
            list = Promise.resolve(list);
        return list.then((l) => {
            return this.HasParent(l, true).then((has) => {
                if (!has) {
                    this.parent.put(l.Id, l);
                } else {
                    if (throwException)
                        throw "Ya se está heredando!!";
                }
            }).catch((ex) => {
                if (throwException)
                    throw ex;
            });
        });
    }

    Load(json) {
        if (!(json instanceof Promise))
            json = Promise.resolve(json);
        return json.then((j) => {
            this.Id = j.Id;
            this._parents = new Map(j.Parents);
            this._idCategorias = new Map(j.IdCategorias);
            this._idHerenciaTareasOcultas = new Map(j.IdHerenciaTareasOcultas);
            this._dicTareasHechas = new Map(j.DicTareasHechas);
        });
    }
    Export() {
        return Promise.revolve().then(() => {
            var obj = {
                Parents = this._parents.entries(), //IdLista,Lista
                Id = 0, //obtener idUnico
                IdCategorias = this._idCategorias.entries(), //idCategoria,Categoria
                IdHerenciaTareasOcultas = this.IdHerenciaTareasOcultas.entries(), //idTarea,Tarea
                DicTareasHechas = this._dicTareasHechas.entries(), //idTarea,fecha
            };
            return JSON.stringify(obj);
        });
    }
    static GetById(id) {
        var lista;
        if (Lista._dic) {
            lista = Lista._dic.get(id);
        }
        return Promsise.revolve(lista);
    }
    static Remove(lista) {
        if (!(lista instanceof Promise))
            lista = Promise.resolve(lista);
        return lista.then((l) => {
            if (Lista._dic && Lista._dic.has(l.Id))
                Lista._dic.delete(l.Id);
        });

    }
    static Add(lista) {
        if (!(lista instanceof Promise))
            lista = Promise.resolve(lista);
        return lista.then((l) => {
            if (!Lista._dic)
                Lista._dic = new Map();
            if (!Lista._dic.has(l.Id))
                Lista._dic.put(l.Id, lista);
        });
    }

    static Get(categoria) {
        if (!(categoria instanceof Promise))
            categoria = Promise.resolve(categoria);
        return categoria.then((c) => {
            var listasCategoria = [];
            if (Lista._dic) {
                Lista._dic.forEach((l) => { if (l.HasCategoria(c)) ArrayUtils.Add(listasCategoria, l); });
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
        if (!(strJSONListas instanceof Promise))
            strJSONListas = Promise.resolve(strJSONListas);
        return strJSONListas.then((json) => {
            var listasJSON = JSON.parse(json);
            var promseas = [];
            for (var i = 0; i < listasJSON.length; i++) {
                ArrayUtils.Add(promesas, new Lista().Load(listasJSON[i]));

            }
            return Promise.all(promseas);
        });
    }


}