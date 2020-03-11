using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TareasPendientes.Blazor2.Extension;
using System.Text.Json;
using Gabriel.Cat.S.Extension;
using Newtonsoft.Json;

namespace TareasPendientes.Blazor2.Entities
{
    public class Data
    {
        IList<Lista> listasJson;
        IList<Categoria> categoriasJson;
        public Data()
        {
            Listas = new SortedList<long, Lista>();
            Categorias = new SortedList<long, Categoria>();
            Listas.Add(new Lista("Mi primera lista", 0));
            Categorias.Add(new Categoria("Todas", 0) { Listas = Listas });
        }
        public Data(string json) : this()
        {
            LoadJson(json);

        }



        [JsonIgnore]
        public SortedList<long, Lista> Listas { get; set; }
        [JsonProperty("Listas")]
        public IList<Lista> IListas
        {
            get => Listas.GetValues();
            set => listasJson = value;
        }
        [JsonIgnore]
        public SortedList<long, Categoria> Categorias { get; set; }
        [JsonProperty("Categorias")]
        public IList<Categoria> ICategorias
        {
            get => Categorias.GetValues();
            set => categoriasJson = value;
        }
        public string ToJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        public void LoadJson(string json)
        {
            Data aux;
            SortedList<long, Tarea> tareas;
            
            if (!string.IsNullOrEmpty(json))
            {
                Listas.Clear();
                Categorias.Clear();
                Console.WriteLine(json);
                aux = Newtonsoft.Json.JsonConvert.DeserializeObject<Data>(json);
                Console.WriteLine("Deserializado el json");
                tareas = new SortedList<long, Tarea>();

               

                for (int i = 0; i < aux.listasJson.Count; i++)
                {
                    Listas.Add(aux.listasJson[i]);
                }
                for (int i = 0; i < aux.listasJson.Count; i++)
                {
                    tareas.AddRange(aux.listasJson[i].Tareas.Values);
                }
                for (int i = 0; i < aux.listasJson.Count; i++)
                {
                    foreach (var item in aux.listasJson[i].TareasOcultas)
                        aux.listasJson[i].TareasOcultas[item.Key] = tareas[item.Key];
                    foreach (var item in aux.listasJson[i].ListasHerencia)
                        aux.listasJson[i].ListasHerencia[item.Key] = Listas[item.Key];
                }
                for (int i = 0; i < aux.categoriasJson.Count; i++)
                {
                    Categorias.Add(aux.categoriasJson[i]);
                    foreach (var lst in aux.categoriasJson[i].Listas)
                        aux.categoriasJson[i].Listas[lst.Key] = Listas[lst.Key];
                }

                Console.WriteLine("Se ha acabado de cargar correctamente :)");
            }
            else Console.WriteLine("No hay datos a cargar");
        }
    }
}
