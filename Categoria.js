class Categoria {
    Categoria() {
        this._name = "";
        this._id = 0; //hacer idUnico

    }
    get Name() {
        return this._name;
    }
    set Name(name) {
        this._name = name;
    }
    get Id() {
        return this._id;
    }
    set Id(id) {
        if (!Categoria._dic)
            Categoria._dic = new Map();

        if (Categoria._dic.has(id))
            throw "Id most be unique";

        if (Categoria._dic.has(this._id))
            Categoria._dic.delete(this._id);

        this._id = id;

        Categoria._dic.put(id, this);
    }
    Load(json) {
        this.Name = json.Name;
        this.Id = json.Id;
    }
    Export() {
        return JSON.stringify(this);
    }
    static GetById(id) {
        var categoria;
        if (Categoria._dic) {
            categoria = Categoria._dic.get(id);
        }
        return categoria;
    }

    static GetAll() {
        return Promise.resolve().then(() => {
            var todas;
            if (Categoria._dic) {
                todas = Categoria._dic.values();
            } else {
                todas = [];
            }
            return todas;
        });
    }
    static ExportAll() {
        return GetAll().then((categorias) => {
            return JSON.stringify(categorias);
        });
    }
    static Import(strJSONCategorias) {
        return Promise.resolve().then(() => {
            var categoriasJson = JSON.parse(strJSONCategorias);
            for (var i = 0; i < categoriasJson.length; i++) {
                new Categoria().Load(categoriasJson[i]);

            }
        });
    }

}