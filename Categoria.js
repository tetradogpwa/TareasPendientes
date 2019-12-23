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
        if (!(id instanceof Promise))
            id = Promise.resolve(id);
        return id.then((i) => {
            var categoria;
            if (Categoria._dic) {
                categoria = Categoria._dic.get(i);
            }
            return categoria;
        });
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
        return CacheUtils.GetAllJson(Categoria.CacheName).then((categoriasJson) => {
            var categorias = [];
            var loads=[];
            for (var i = 0; i < categoriasJson.length; i++) {
                ArrayUtils.Add(categorias, new Categoria());
                ArrayUtils.Add(loads,  categorias[i].Load(categoriasJson[i]));
            }
            return Promise.all(loads).then(()=> categorias);
        });
    }
    static Import(strJSONCategorias) {
        if (!(strJSONCategorias instanceof Promise))
            strJSONCategorias = Promse.resolve(strJSONCategorias);

        return strJSONCategorias.then((json) => {
            var categoriasJson = JSON.parse(json);
            var categorias=[];
            var loads=[];
            var categoria;
            for (var i = 0; i < categoriasJson.length; i++) {
                categoria=new Categoria();
                ArrayUtils.Add(categorias,categoria);
                ArrayUtils.Add(loads, categoria.Load(categoriasJson[i]));

            }
            return Promise.all(loads).then(()=> categorias);
        });
    }

}
