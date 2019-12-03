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
        this.Text = json.Text;
        this.IdLista = json.IdLista;
        this.Id = json.Id;
    }
    Export() {
        return JSON.stringify(this);
    }
    static GetById(id) {
        var tarea;
        if (Tarea._dic) {
            tarea = Tarea._dic.get(id);
        }
        return tarea;
    }
    static Remove(tarea) {
        return Promise.resolve().then(() => {
            if (Tarea._dic && Tarea._dic.has(tarea.Id))
                Tarea._dic.delete(tarea.Id);
        });
    }
    static Add(tarea) {
        return Promise.resolve().then(() => {
            if (!Tarea._dic)
                Tarea._dic = new Map();
            if (!Tarea._dic.has(tarea.Id))
                Tarea._dic.put(tarea.Id, tarea);
        });
    }
    static Get(lista) {
        return Promise.resolve().then(() => {
            var tareasLista;

            if (Tarea._dic) {
                tareasLista = ArrayUtils.Filter(Tarea._dic.values(), (tarea) => tarea.IdLista == lista.Id);

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
        return Promise.resolve().then(() => {
            var tareasJSON = JSON.parse(strJSONTareas);
            for (var i = 0; i < tareasJSON.length; i++) {
                new Tarea().Load(tareasJSON[i]);

            }
        });
    }
}