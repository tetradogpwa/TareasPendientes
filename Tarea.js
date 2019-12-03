window.Import(window._ROOTUTILS + "Utils/ArrayUtils.js");


class Tarea {
    Tarea(lista = null) {
        this._text = "";
        this._idLista = lista != null ? lista.Id : -1;
        this._id = 0; //poner uno unico
    }
    get Id() {
        return this._id;
    }
    set Id(id) {

        Tarea.Remove(this);
        this._id = id;
        Tarea.Add(this);
    }
    get Text() {
        return this._text;
    }
    set Text(text) {
        this._text = text;
    }
    get IdLista() {
        return this._idLista;
    }
    set IdLista(idLista) {
        this._idLista = idLista;
    }
    Load(json) {
        if (!(json instanceof Promise))
            json = Promise.resolve(json);
        return json.then((j) => {
            this.Text = j.Text;
            this.IdLista = j.IdLista;
            this.Id = j.Id;
        });
    }
    Export() {
        return Promise.resolve(JSON.stringify(this));
    }
    static GetById(id) {
        if (!(id instanceof Promise))
            id = Promise.resolve(id);
        return id.then((i) => {
            var tarea;

            if (Tarea._dic) {
                tarea = Tarea._dic.get(i);
            }
            return tarea;
        });
    }
    static Remove(tarea) {
        if (!(tarea instanceof Promise))
            tarea = Promise.resolve(tarea);
        return tarea.then((t) => {
            if (Tarea._dic && Tarea._dic.has(t.Id))
                Tarea._dic.delete(t.Id);
        });
    }
    static Add(tarea) {
        if (!(tarea instanceof Promise))
            tarea = Promise.resolve(tarea);
        return tarea.then((t) => {
            if (!Tarea._dic)
                Tarea._dic = new Map();
            if (!Tarea._dic.has(t.Id))
                Tarea._dic.put(t.Id, t);
        });
    }
    static Get(lista) {
        if (!(lista instanceof Promise))
            lista = Promise.resolve(lista);
        return lista.then((l) => {
            var tareasLista;

            if (Tarea._dic) {
                tareasLista = ArrayUtils.Filter(Tarea._dic.values(), (tarea) => tarea.IdLista == l.Id);

            } else tareasLista = [];

            return tareasLista;
        });
    }


    static GetAll() {
        return Promise.resolve().then(() => {
            var todas;
            if (Tarea._dic) {
                todas = Tarea._dic.values();
            } else {
                todas = [];
            }
            return todas;
        });
    }
    static ExportAll() {
        return GetAll().then((tareas) => {
            return JSON.stringify(tareas);
        });
    }
    static Import(strJSONTareas) {
        if (!(strJSONTareas instanceof Promise))
            strJSONTareas = Promise.resolve(strJSONTareas);
        return strJSONTareas.then((json) => {
            var tareasJSON = JSON.parse(json);
            var promesas = [];
            for (var i = 0; i < tareasJSON.length; i++) {
                ArrayUtils.Add(promesas, new Tarea().Load(tareasJSON[i]));

            }
            return Promise.all(promesas);
        });
    }
}