using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TareasPendientes.Blazor2.Extension;
using Gabriel.Cat.S.Extension;
using System.Text.Json.Serialization;

namespace TareasPendientes.Blazor2.Entities
{
    public class Lista:Base, IEnumerable<Tarea>
    {
        public Lista() : this("") { }
        public Lista(string nombre,long id=-1):base(nombre,id)
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
                for(int i = 0; i < value.Length; i++)
                {
                    Tareas.Add(value[i]);
                }
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
                for (int i = 0; i < value.Length; i++)
                {
                    ListasHerencia.Add(value[i],null);
                }
            }
        }
        [JsonIgnore]
        public SortedList<long, Tarea> TareasOcultas { get; set; }
        public long[] ITareasOcultas
        {
            get { return TareasOcultas.GetKeys(); }
            set
            {
                TareasOcultas.Clear();
                for (int i = 0; i < value.Length; i++)
                {
                    TareasOcultas.Add(value[i], null);
                }
            }
        }
        [JsonIgnore]
        public SortedList<long, DateTime> TareasHechas { get; set; }

        public long[] ITareasHechasID
        {
            get { return TareasHechas.GetKeys(); }
            set
            {
                TareasHechas.Clear();
                for (int i = 0; i < value.Length; i++)
                {
                    TareasHechas.Add(value[i], default);
                }
            }
        }
        public DateTime[] ITareasHechasFecha
        {
            get { return TareasHechas.GetValues(); }
            set
            {
                int pos = 0;
                TareasHechas.Clear();
                foreach(var fechas in TareasHechas)
                {
                    TareasHechas[fechas.Key] = value[pos++];
                }
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

      public static IEnumerable<Lista> NoHereda(SortedList<long,Lista> dic,Lista lista)
        {
            //de todas las que hay, las listas que no se hereda
            SortedList<long, Lista> dicNoHereda = dic.Clone();
            //hago el diccionario con recursividad
            IQuitarAncestros(dicNoHereda, lista);
            //envio los que no estan :)
            //quitar ancestros puedan heredar de sucesores
           dicNoHereda.RemoveRange(IQuitarSucesores(dicNoHereda, lista));

            foreach(var item in dicNoHereda)
            {
                yield return item.Value;
            }
                   
                
        }
        static List<Lista> IQuitarSucesores(SortedList<long, Lista> dic, Lista lista)
        {
            List<Lista> porQuitar = new List<Lista>();
            //los que hereden//y sus descendencias los quito
            foreach(var lst in dic)
            {
                if(lst.Value.ListasHerencia.Contains(lista))
                {
                    porQuitar.Add(lst.Value);
                    porQuitar.AddRange(IQuitarSucesores(dic, lst.Value));
                }
            }
            return porQuitar;
        }

        public static void IQuitarAncestros(SortedList<long,Lista> dic,Lista lista)
        {
            dic.Remove(lista);
            foreach(var item in lista.ListasHerencia)
            {
                IQuitarAncestros(dic, item.Value);
            }


        }
    }
}
