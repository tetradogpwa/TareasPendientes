using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TareasPendientes.Blazor2.Extension;
using System.Text.Json;
using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Binaris;
using System.IO;
using System.Collections;

namespace TareasPendientes.Blazor2.Entities
{
    public class Data : IElementoBinarioComplejo
    {
        public class DataBinaria : ElementoComplejoBinario
        {
            public DataBinaria() : base(new ElementoBinario[] { new ElementoIListBinario<Categoria>(Categoria.Serializador), new ElementoIListBinario<Lista>(Lista.Serializador) })
            {

            }

            protected override IList IGetPartsObject(object obj)
            {
                Data data = obj as Data;
                if (data == null)
                    throw new Exception("El tipo de objeto valido es Data");
                return new object[] { data.Categorias.GetValues(), data.Listas.GetValues() };
            }

            protected override object JGetObject(MemoryStream bytes)
            {
                object[] partes = base.GetPartsObject(bytes);
                Data data = new Data();
                SortedList<long, Tarea> tareas = new SortedList<long, Tarea>();
                Categoria[] categorias = (Categoria[])partes[0];
                Lista[] listas = (Lista[])partes[1];

                data.Listas.Clear();
                data.Categorias.Clear();

                for (int i = 0; i < listas.Length; i++)
                {
                    data.Listas.Add(listas[i]);
                }

                for (int i = 0; i < listas.Length; i++)
                {
                    tareas.AddRange(listas[i].Tareas.Values);
                }

                for (int i = 0; i < listas.Length; i++)
                {

                    foreach (var item in listas[i].TareasOcultas)
                        listas[i].TareasOcultas[item.Key] = tareas[item.Key];

                    foreach (var item in listas[i].ListasHerencia)
                        listas[i].ListasHerencia[item.Key] = data.Listas[item.Key];

                }

                for (int i = 0; i < categorias.Length; i++)
                {

                    data.Categorias.Add(categorias[i]);

                    foreach (var lst in categorias[i].Listas)
                        categorias[i].Listas[lst.Key] = data.Listas[lst.Key];

                }

                return data;
            }
        }
        public static readonly ElementoBinario Serializador = new DataBinaria();

        public Data()
        {
            Listas = new SortedList<long, Lista>();
            Categorias = new SortedList<long, Categoria>();
            Listas.Add(new Lista("Mi primera lista", 0));
            Categorias.Add(new Categoria("Todas", 0) { Listas = Listas });
        }
        public Data(string dataStringBase64) : this()
        {
            Load(dataStringBase64);
        }


        public SortedList<long, Lista> Listas { get; set; }

        public SortedList<long, Categoria> Categorias { get; set; }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;

        public void Load(byte[] data)
        {
            Data aux;
            
            aux=(Data) Serializador.GetObject(data);
            Listas = aux.Listas;
            Categorias = aux.Categorias;
        }
        public void Load(string dataBase64)
        {
            byte[] data;
            if (!string.IsNullOrEmpty(dataBase64))
            {
                data = Convert.FromBase64String(dataBase64);
                Load(data);
            }

        }
        public string SaveStringBase64()
        {
            byte[] data = Serializador.GetBytes(this);
            return Convert.ToBase64String(data);
        }
    }
}
