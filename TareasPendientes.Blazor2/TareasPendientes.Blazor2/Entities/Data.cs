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
        public Data()
        {
            Listas = new SortedList<long, Lista>();
            Categorias = new SortedList<long, Categoria>();
            Listas.Add(new Lista("Mi primera lista", 0));
            Categorias.Add(new Categoria("Todas", 0) { Listas = Listas });
        }
        public Data(string json) : this()
        {
            
            Load(json);

        }

        public SortedList<long,Lista> Listas { get; set; }
        public SortedList<long, Categoria> Categorias { get; set; }

        public string ToJson()
        {
            string[] json=new string[2];
            json[0] = JsonSerializer.Serialize<Categoria[]>(Categorias.GetValues());
            json[1] = JsonSerializer.Serialize<Lista[]>(Listas.GetValues());
            return JsonSerializer.Serialize<string[]>(json);
        }
        public void Load(string json)
        {
            Categoria[] categorias;
            Lista[] listas;
            string[] dataJson;
            SortedList<long, Tarea> tareas = new SortedList<long, Tarea>();

            if (!string.IsNullOrEmpty(json))
            {
                Listas.Clear();
                Categorias.Clear();

                dataJson = JsonSerializer.Deserialize<string[]>(json);
                Console.WriteLine(dataJson[0]);
                Console.WriteLine(dataJson[1]);
                categorias = JsonSerializer.Deserialize<Categoria[]>(dataJson[0]);
                Console.WriteLine($"Desserializa bien Categorias {categorias.Length}");
                listas = JsonSerializer.Deserialize<Lista[]>(dataJson[1]);
                Console.WriteLine($"Desserializa bien Listas {listas.Length}");
                for (int i = 0; i < listas.Length; i++)
                {
                    Listas.Add(listas[i].Id, listas[i]);
                }
                for (int i = 0; i < listas.Length; i++)
                {
                    tareas.AddRange(listas[i].Tareas.Values);
                }
                for (int i = 0; i < listas.Length; i++)
                {
                    foreach (var item in listas[i].Tareas)
                        listas[i].TareasOcultas[item.Key] = tareas[item.Key];
                    foreach (var item in listas[i].ListasHerencia)
                        listas[i].ListasHerencia[item.Key] = Listas[item.Key];
                }
                for (int i = 0; i < categorias.Length; i++)
                {
                    Categorias.Add(categorias[i].Id, categorias[i]);
                    foreach (var lst in categorias[i].Listas)
                        categorias[i].Listas[lst.Key] = Listas[lst.Key];
                }
                Console.WriteLine("Se ha acabado de cargar correctamente :)");
            }
        }
    }
}
