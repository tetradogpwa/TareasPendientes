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
            var tareasLista = [];
            if (Tarea._dic) {
                Tarea._dic.foreach((tarea) => {
                    if (tarea.IdLista == lista.Id) {
                        ArrayUtils.Add(tareasLista, tarea);
                    }
                });

            }
            return tareasLista;
        });
    }
}