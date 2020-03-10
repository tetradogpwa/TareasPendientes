using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TareasPendientes.Blazor2.Extension;
using Gabriel.Cat.S.Extension;

namespace TareasPendientes.Blazor2.Entities
{
    public class Lista:Base, IEnumerable<Tarea>
    {
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

        public SortedList<long, Tarea> Tareas { get; set; }
        public SortedList<long, Lista> ListasHerencia { get; set; }
        public SortedList<long, Tarea> TareasOcultas { get; set; }
        public SortedList<long, DateTime> TareasHechas { get; set; }

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
