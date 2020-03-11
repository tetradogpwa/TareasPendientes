using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TareasPendientes.Blazor2.Extension;
using Gabriel.Cat.S.Extension;
using Newtonsoft.Json;

namespace TareasPendientes.Blazor2.Entities
{
    public class Lista : Base, IEnumerable<Tarea>
    {
        public Lista() : this("") { Console.WriteLine("Lista creada"); }
        public Lista(string nombre, long id = -1) : base(nombre, id)
        {
            Tareas = new SortedList<long, Tarea>();
            ListasHerencia = new SortedList<long, Lista>();
            TareasOcultas = new SortedList<long, Tarea>();
            TareasHechas = new SortedList<long, DateTime>();
        }
        public Lista(IList<Tarea> listaTemporal) : this("Temporal", 0)
        {
            Tareas.AddRange(listaTemporal);
        }
        [JsonIgnore]
        public SortedList<long, Tarea> Tareas { get; set; }
        public Tarea[] ITareas
        {
            get { return Tareas.GetValues(); }
            set
            {
                Tareas.Clear();
                if (value != null)
                    Tareas.AddRange(value);
                Console.WriteLine("Cargadas tareas lista");
            }
        }
        [JsonIgnore]
        public SortedList<long, Lista> ListasHerencia { get; set; }
        public long[] IListasHerencia
        {
            get { return ListasHerencia.GetKeys(); }
            set
            {
                ListasHerencia.Clear();
                if (value != null)
                    for (int i = 0; i < value.Length; i++)
                    {
                        ListasHerencia.Add(value[i], null);
                    }
                Console.WriteLine("Cargadas listas herencia");
            }
        }
        [JsonIgnore]
        public SortedList<long, Tarea> TareasOcultas { get; set; }
        public IList<long> ITareasOcultas
        {
            get { return TareasOcultas.GetKeys(); }
            set
            {
                TareasOcultas.Clear();
                if (value != null)
                    for (int i = 0; i < value.Count; i++)
                    {
                        TareasOcultas.Add(value[i], null);
                    }
                Console.WriteLine("Cargadas tareas ocultas");
            }
        }
        [JsonIgnore]
        public SortedList<long, DateTime> TareasHechas { get; set; }

        public IList<long> ITareasHechasID
        {
            get { return TareasHechas.GetKeys(); }
            set
            {
                TareasHechas.Clear();
                if (value != null)
                    for (int i = 0; i < value.Count; i++)
                    {
                        TareasHechas.Add(value[i], default);
                    }
                Console.WriteLine("Cargadas tareas hechas long");
            }
        }
        public IList<DateTime> ITareasHechasFecha
        {
            get { return TareasHechas.GetValues(); }
            set
            {
                int pos = 0;
                TareasHechas.Clear();
                if (value != null)
                    foreach (var fechas in TareasHechas)
                    {
                        TareasHechas[fechas.Key] = value[pos];
                        pos++;
                    }
                Console.WriteLine("Cargadas tareas hechas datetime");
            }
        }

        public void Clear()
        {
            Tareas.Clear();
            ListasHerencia.Clear();
            TareasOcultas.Clear();
            TareasHechas.Clear();
        }

        #region Enumerator
        private IEnumerator<Tarea> GetEnumerator()
        {
            foreach (var lista in ListasHerencia)
                foreach (var tarea in ((IEnumerable<Tarea>)lista.Value))
                    if (!TareasOcultas.Contains(tarea))
                        yield return tarea;
            foreach (var tarea in Tareas)
                yield return tarea.Value;
        }

        IEnumerator<Tarea> IEnumerable<Tarea>.GetEnumerator()
        {
            return GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        public static IEnumerable<Lista> NoHereda(SortedList<long, Lista> dic, Lista lista)
        {
            //de todas las que hay, las listas que no se hereda
            SortedList<long, Lista> dicNoHereda = dic.Clone();
            //hago el diccionario con recursividad
            IQuitarAncestros(dicNoHereda, lista);
            //envio los que no estan :)
            //quitar ancestros puedan heredar de sucesores
            dicNoHereda.RemoveRange(IQuitarSucesores(dicNoHereda, lista));

            foreach (var item in dicNoHereda)
            {
                yield return item.Value;
            }


        }
        static List<Lista> IQuitarSucesores(SortedList<long, Lista> dic, Lista lista)
        {
            List<Lista> porQuitar = new List<Lista>();
            //los que hereden//y sus descendencias los quito
            foreach (var lst in dic)
            {
                if (lst.Value.ListasHerencia.Contains(lista))
                {
                    porQuitar.Add(lst.Value);
                    porQuitar.AddRange(IQuitarSucesores(dic, lst.Value));
                }
            }
            return porQuitar;
        }

        public static void IQuitarAncestros(SortedList<long, Lista> dic, Lista lista)
        {
            dic.Remove(lista);
            foreach (var item in lista.ListasHerencia)
            {
                IQuitarAncestros(dic, item.Value);
            }


        }
    }
}
