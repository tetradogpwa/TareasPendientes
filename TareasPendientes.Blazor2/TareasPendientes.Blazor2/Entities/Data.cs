using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TareasPendientes.Blazor2.Extension;
using System.Text.Json;
using Gabriel.Cat.S.Extension;
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
            
           Data aux=JsonSerialitzer.Deserialitzer<Data>(json);

           listasJson=aux.listasJson;
           categoriasJson=aux.categoriasJson;
           FinishToLoad();

        }
[JsonIgnore]
        public SortedList<long,Lista> Listas { get; set; }
[JsonName("Listas")]
public Lista[] IListas{


get=>Listas.GetValues();
set=>listasJson=value;

}
[JsonIgnore]
        public SortedList<long, Categoria> Categorias { get; set; }
[JsonName("Categorias")]
public Categoria[] ICategorias{


get=>Categorias.GetValues();
set=>categoriasJson=value;

}
        public string ToJson()
        {
            return JsonSerializer.Serialize<Data>(this);
        }
        private void FinishToLoad()
        {
          
            SortedList<long, Tarea> tareas = new SortedList<long, Tarea>();

            if (categoriasJson!=null)
            {
                Listas.Clear();
                Categorias.Clear();

                for (int i = 0; i < listasJson.Length; i++)
                {
                    Listas.Add(listasJson[i].Id, listasJson[i]);
                }
                for (int i = 0; i < listasJson.Length; i++)
                {
                    tareas.AddRange(listasJson[i].Tareas.Values);
                }
                for (int i = 0; i < listasJson.Length; i++)
                {
                    foreach (var item in listasJson[i].Tareas)
                        listasJson[i].TareasOcultas[item.Key] = tareas[item.Key];
                    foreach (var item in listasJson[i].ListasHerencia)
                        listasJson[i].ListasHerencia[item.Key] = Listas[item.Key];
                }
                for (int i = 0; i < categoriasJson.Length; i++)
                {
                    Categorias.Add(categoriasJson[i].Id, categoriasJson[i]);
                    foreach (var lst in categoriasJson[i].Listas)
                        categoriasJson[i].Listas[lst.Key] = Listas[lst.Key];
                }
categoriasJson=null;
listasJson=null;
                Console.WriteLine("Se ha acabado de cargar correctamente :)");
            }
        }
    }
}
