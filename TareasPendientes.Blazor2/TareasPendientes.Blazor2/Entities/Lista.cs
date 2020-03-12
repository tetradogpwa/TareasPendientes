using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TareasPendientes.Blazor2.Extension;
using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Binaris;
using System.IO;

namespace TareasPendientes.Blazor2.Entities
{
    public class Lista : Base, IEnumerable<Tarea>,IElementoBinarioComplejo
    {
       public class ListaBinaria : ElementoComplejoBinario
        {
            public ListaBinaria() : base(new ElementoBinario[] {

                ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long),
                ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.String),

                new ElementoIListBinario<Tarea>(Tarea.Serializador),

                ElementoIListBinario<long>.ElementosTipoAceptado<long>(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long),
                ElementoIListBinario<DateTime>.ElementosTipoAceptado<DateTime>(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.DateTime),

                ElementoIListBinario<long>.ElementosTipoAceptado<long>(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long),

                ElementoIListBinario<long>.ElementosTipoAceptado<long>(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long)
            })
            {
            }

            protected override IList IGetPartsObject(object obj)
            {
                Lista lista = obj as Lista;
                if (lista == null)
                    throw new Exception("El tipo de objeto valido es Lista");
                return new object[] { 
                    lista.Id,lista.Name,
                    lista.Tareas.GetValues(),
                    lista.TareasHechas.GetKeys(),lista.TareasHechas.GetValues(),
                    lista.TareasOcultas.GetKeys(),
                    lista.ListasHerencia.GetKeys() 
                };
            }

            protected override object JGetObject(MemoryStream bytes)
            {
                object[] partes = base.GetPartsObject(bytes);
                Lista lista = new Lista(partes[1] as string, (long)partes[0]);
                Tarea[] tareas =(Tarea[]) partes[2];
                long[] idTareasHechas = (long[])partes[3];
                DateTime[] fechasTareasHechas = (DateTime[])partes[4];
                long[] idTareasOcultas = (long[])partes[5];
                long[] idListasHeredadas = (long[])partes[6];

                lista.Tareas.AddRange(tareas);
                lista.TareasOcultas.AddRange(idTareasOcultas);
                lista.ListasHerencia.AddRange(idListasHeredadas);
                for (int i = 0; i < idTareasHechas.Length; i++)
                    lista.TareasHechas.Add(idTareasHechas[i], fechasTareasHechas[i]);

                return lista;
            }
        }
        public static readonly ElementoBinario Serializador = new ListaBinaria();
        public Lista() : this("") {  }
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

        public SortedList<long, Tarea> Tareas { get; set; }


        public SortedList<long, Lista> ListasHerencia { get; set; }
  
   
        public SortedList<long, Tarea> TareasOcultas { get; set; }

     
        public SortedList<long, DateTime> TareasHechas { get; set; }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;

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
