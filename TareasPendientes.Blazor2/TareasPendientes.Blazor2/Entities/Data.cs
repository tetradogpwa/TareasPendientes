using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TareasPendientes.Blazor2.Extension;
using System.Text.Json;
using Gabriel.Cat.S.Extension;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TareasPendientes.Blazor2.Entities
{
    public class Data
    {
        Lista[] listasJson;
        Categoria[] categoriasJson;
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



        [System.Text.Json.Serialization.JsonIgnore]
        public SortedList<long, Lista> Listas { get; set; }
        [JsonPropertyName("Listas")]
        public Lista[] IListas
        {


            get => Listas.GetValues();
            set => listasJson = value;

        }
        [System.Text.Json.Serialization.JsonIgnore]
        public SortedList<long, Categoria> Categorias { get; set; }
        [JsonPropertyName("Categorias")]
        public Categoria[] ICategorias
        {


            get => Categorias.GetValues();
            set => categoriasJson = value;

        }
        public string ToJson()
        {
            return System.Text.Json.JsonSerializer.Serialize<Data>(this);
        }
        public void LoadJson(string json)
        {
            Data aux;
            SortedList<long, Tarea> tareas;
            
            
            
            if (!string.IsNullOrEmpty(json))
            {
                Listas.Clear();
                Categorias.Clear();
                aux = System.Text.Json.JsonSerializer.Deserialize<Data>(json);
                tareas = new SortedList<long, Tarea>();

               

                for (int i = 0; i < aux.listasJson.Length; i++)
                {
                    Listas.Add(aux.listasJson[i]);
                }
                for (int i = 0; i < aux.listasJson.Length; i++)
                {
                    tareas.AddRange(aux.listasJson[i].Tareas.Values);
                }
                for (int i = 0; i < aux.listasJson.Length; i++)
                {
                    foreach (var item in aux.listasJson[i].TareasOcultas)
                        aux.listasJson[i].TareasOcultas[item.Key] = tareas[item.Key];
                    foreach (var item in aux.listasJson[i].ListasHerencia)
                        aux.listasJson[i].ListasHerencia[item.Key] = Listas[item.Key];
                }
                for (int i = 0; i < aux.categoriasJson.Length; i++)
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
