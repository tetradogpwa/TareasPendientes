using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TareasPendientes.Blazor2.Extension;
using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Binaris;
using System.IO;
using Gabriel.Cat.S.Utilitats;

namespace TareasPendientes.Blazor2.Entities
{
    public class Lista : Base, IEnumerable<Tarea>,IElementoBinarioComplejo
    {
        class ListaBinaria : ElementoComplejoBinario
        {
            const int ID = 0, NAME = ID + 1, TAREAS = NAME + 1, IDTAREASHECHAS = TAREAS + 1, FECHATAREASHECHAS = IDTAREASHECHAS + 1, IDTAREASOCULTAS = FECHATAREASHECHAS + 1, IDLISTASHEREDADAS = IDTAREASOCULTAS + 1;
            const int TOTAL = IDLISTASHEREDADAS + 1;

            public ListaBinaria() : base(new ElementoBinario[] {
                //ID
            ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long),
            //Name
            ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.String),
            //Tareas
            new ElementoIListBinario<Tarea>(Tarea.Serializador),
            //Id Tareas Hechas
            ElementoIListBinario<long>.ElementosTipoAceptado<long>(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long),
            //Fechas Tareas Hechas
            ElementoIListBinario<DateTime>.ElementosTipoAceptado<DateTime>(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.DateTime),
            //Id Tareas ocultas
            ElementoIListBinario<long>.ElementosTipoAceptado<long>(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long),
            //Id Listas heredadas
            ElementoIListBinario<long>.ElementosTipoAceptado<long>(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long),



            }) { }
            protected override IList IGetPartsObject(object obj)
            {
                Console.WriteLine("Inicio Serializar Lista");
                object[] partes;
                Lista lista = obj as Lista;

                if (lista == null)
                    throw new Exception("El tipo de objeto valido es Lista");

                partes = new object[TOTAL];
                partes[ID]=lista.Id;
                partes[NAME] = lista.Name;
                partes[TAREAS] = lista.Tareas.GetValues();
                partes[IDTAREASHECHAS] = lista.TareasHechas.GetKeys();
                partes[FECHATAREASHECHAS] = lista.TareasHechas.GetValues();
                partes[IDTAREASOCULTAS] = lista.TareasOcultas.GetKeys();
                partes[IDLISTASHEREDADAS] = lista.ListasHerencia.GetKeys();
                Console.WriteLine("Fin Serializar Lista");
                return partes;
                
            }

            protected override object JGetObject(MemoryStream bytes)
            {
                Console.WriteLine("Inicio Deserializar Lista");
                object[] partes = base.GetPartsObject(bytes);
                Lista lista = new Lista((string)partes[NAME], (long)partes[ID]);
                Tarea[] tareas =(Tarea[]) partes[TAREAS];
                long[] idTareasHechas = (long[])partes[IDTAREASHECHAS];
                DateTime[] fechasTareasHechas = (DateTime[])partes[FECHATAREASHECHAS];
                long[] idTareasOcultas = (long[])partes[IDTAREASOCULTAS];
                long[] idListasHeredadas = (long[])partes[IDLISTASHEREDADAS];

                lista.Tareas.AddRange(tareas);
                lista.TareasOcultas.AddRange(idTareasOcultas);
                lista.ListasHerencia.AddRange(idListasHeredadas);
                for (int i = 0; i < idTareasHechas.Length; i++)
                    lista.TareasHechas.Add(idTareasHechas[i], fechasTareasHechas[i]);
                Console.WriteLine("Fin Deserializar Lista");
                return lista;
            }
        }
        internal static readonly ElementoBinario Serializador = new ListaBinaria();
        public Lista(string nombre, long id = -1) : base(nombre, id)
        {
            Tareas = new LlistaOrdenada<long, Tarea>();
            ListasHerencia = new LlistaOrdenada<long, Lista>();
            TareasOcultas = new LlistaOrdenada<long, Tarea>();
            TareasHechas = new LlistaOrdenada<long, DateTime>();
        }
        public Lista(IList<Tarea> listaTemporal) : this("Temporal", 0)
        {
            Tareas.AddRange(listaTemporal);
        }

        public LlistaOrdenada<long, Tarea> Tareas { get; set; }


        public LlistaOrdenada<long, Lista> ListasHerencia { get; set; }
  
   
        public LlistaOrdenada<long, Tarea> TareasOcultas { get; set; }

     
        public LlistaOrdenada<long, DateTime> TareasHechas { get; set; }

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
            for (int i=0;i<ListasHerencia.Count;i++)
                foreach (Tarea tarea in (IEnumerable<Tarea>)ListasHerencia.GetValueAt(i))
                    if (!TareasOcultas.Contains(tarea))
                        yield return tarea;
            for(int i=0;i<Tareas.Count;i++)
                yield return Tareas.GetValueAt(i);
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

        public static IEnumerable NoHereda(IDictionary<long, Lista> dic, Lista lista)
        {
            //de todas las que hay, las listas que no se hereda
            LlistaOrdenada<long, Lista> dicNoHereda = dic.Clone();
            //hago el diccionario con recursividad
            IQuitarAncestros(dicNoHereda, lista);
            //envio los que no estan :)
            //quitar ancestros puedan heredar de sucesores
            dicNoHereda.RemoveRange(IQuitarSucesores(dicNoHereda, lista));

            return dic.GetValues(); 


        }
        static List<Lista> IQuitarSucesores(LlistaOrdenada<long, Lista> dic, Lista lista)
        {   
            Lista lst;
            List<Lista> porQuitar = new List<Lista>();
            
            //los que hereden//y sus descendencias los quito
            for(int i=0;i<dic.Count;i++)
            {
                lst = dic.GetValueAt(i);
                if (lst.ListasHerencia.Contains(lista))
                {
                    porQuitar.Add(lst);
                    porQuitar.AddRange(IQuitarSucesores(dic, lst));
                }
            }
            return porQuitar;
        }

        public static void IQuitarAncestros(LlistaOrdenada<long, Lista> dic, Lista lista)
        {
            dic.Remove(lista);
            for(int i=0;i< lista.ListasHerencia.Count;i++)
            {
                IQuitarAncestros(dic, lista.ListasHerencia.GetValueAt(i));
            }


        }
    }
}
