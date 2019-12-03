window.Import(window._ROOTUTILS + "Utils/CacheUtils.js");

class Categoria {
    static get CacheName() {
        if (!Categoria._cacheName)
            Categoria._cacheName = "TareasPendientes_Categorias";
        return Categoria._cacheName;
    }
    static set CacheName(cacheName) {
        Categoria._cacheName = cacheName;
    }
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
        return Promise.resolve().then(() => {
            this.Name = json.Name;
            this.Id = json.Id;
        });
    }
    Export() {
        return Priomise.resolve(JSON.stringify(this));
    }
    Save() {
        return CacheUtils.SetJson(Categoria.CacheName, this.Id, this.Export());
    }
    static GetById(id) {

        var categoria;
        if (Categoria._dic) {
            categoria = Categoria._dic.get(id);
        }
        return Promise.resolve(categoria);
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
        return Categoria.GetAll().then((categorias) => {
            return JSON.stringify(categorias);
        });
    }
    static SaveAll() {
        return Categoria.ExportAll().then((categoriasJson) => {
            var promesas = [];
            for (var i = 0; i < categoriasJson.length; i++)
                ArrayUtils.Add(promesas, CacheUtils.SetJson(Categoria.CacheName, categoriasJson[i].Id, categoriasJson[i]));
            return Promise.all(promesas);
        });
    }
    static LoadAll() {
        return CacheUtils.GetKeys(Categoria.CacheName).then((keys) => {
                var categoria;
                var promesas = [];
                for (var i = 0; i < keys.length; i++) {
                    categoria = new Categoria();
                    ArrayUtils.Add(promesas, CacheUtils.GetJson(Categoria.CacheName, keys[i]).then(categoria.Load);
                    }
                    return Promise.all(promesas);
                });
        }
        static Import(strJSONCategorias) {
            if (!(strJSONCategorias instanceof Promise))
                strJSONCategorias = Promse.resolve(strJSONCategorias);

            return strJSONCategorias.then((json) => {
                var categoriasJson = JSON.parse(json);
                for (var i = 0; i < categoriasJson.length; i++) {
                    new Categoria().Load(categoriasJson[i]);

                }
            });
        }

    }