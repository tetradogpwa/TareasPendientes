﻿using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TareasPendientes.Blazor.Entities
{

    public class Lista : IEnumerable<Tarea>
    {
        public static LlistaOrdenada<long, Lista> ListasCargadas = new LlistaOrdenada<long, Lista>();
        public string Nombre { get; set; }
        public long Id { get; set; }
        LlistaOrdenada<long, Tarea> Tareas { get; set; }

        public LlistaOrdenada<long,Tarea> TareasLista { get; private set; }
        public LlistaOrdenada<long> ListasHeredadas { get; private set; }
        public LlistaOrdenada<long> IdTareasHeredadasOcultas { get; private set; }
        public LlistaOrdenada<long, long> FechaTareaHecha { get; private set; }//IdTarea,Ticks
        public Lista(IList<Tarea> tareas) : this("temporal", 0)
        {
            for (int i = 0; i < tareas.Count; i++)
                AddTarea(tareas[i]);
        }
        public Lista(IList<long> tareas) : this("temporal", 0)
        {
            for (int i = 0; i < tareas.Count; i++)
                AddTarea(Tarea.TareasCargadas[tareas[i]]);
        }
        public Lista(string nombre, long id = -1)
        {
            if (id < 0)
                Id = DateTime.Now.Ticks;
            else Id = id;
            Tareas = new LlistaOrdenada<long, Tarea>();
            Nombre = nombre;
            TareasLista = new LlistaOrdenada<long, Tarea>();
            ListasHeredadas = new LlistaOrdenada<long>();
            IdTareasHeredadasOcultas = new LlistaOrdenada<long>();
            FechaTareaHecha = new LlistaOrdenada<long, long>();
            if (!ListasCargadas.ContainsKey(Id))
                ListasCargadas.Add(Id, this);
            ListasHeredadas.Updated += (s, e) => Tareas.Clear();
          
        }
        public Lista(XmlNode nodeList) : this(nodeList.ChildNodes[0].InnerText.DescaparCaracteresXML(), long.Parse(nodeList.ChildNodes[1].InnerText))
        {
            Tarea tarea;
            for (int i = 0; i < nodeList.ChildNodes[2].ChildNodes.Count; i++)
            {
                tarea = new Tarea(nodeList.ChildNodes[2].ChildNodes[i]);
                TareasLista.Add(tarea.Id,tarea);
            }
            for (int i = 0; i < nodeList.ChildNodes[3].ChildNodes.Count; i++)
            {
                ListasHeredadas.Add(long.Parse(nodeList.ChildNodes[3].ChildNodes[i].InnerText));
            }
            for (int i = 0; i < nodeList.ChildNodes[4].ChildNodes.Count; i++)
            {
                IdTareasHeredadasOcultas.Add(long.Parse(nodeList.ChildNodes[4].ChildNodes[i].InnerText));
            }
            for (int i = 0; i < nodeList.ChildNodes[5].ChildNodes.Count; i++)
            {
                FechaTareaHecha.Add(long.Parse(nodeList.ChildNodes[5].ChildNodes[i].ChildNodes[0].InnerText), long.Parse(nodeList.ChildNodes[5].ChildNodes[i].ChildNodes[1].InnerText));
            }
        }

        public SortedList<long, long> GetIdListas()
        {
            SortedList<long, long> ids = new SortedList<long, long>();

            for (int i = 0; i < ListasHeredadas.Count; i++)
            {
                foreach (var lst in Lista.ListasCargadas[ListasHeredadas[ListasHeredadas.GetValueAt(i)]].GetIdListas())
                {
                    if (!ids.ContainsKey(lst.Value))ids.Add(lst.Key,lst.Key) ;
                }
            }
            ids.Add(Id, Id);
            return ids;
        }

        public bool EstaHecha(Tarea tarea) => FechaTareaHecha.ContainsKey(tarea.Id);
        public bool AddList(Lista listaAHeredar)
        {
            bool noSeHereda = Id != listaAHeredar.Id && NoHereda(listaAHeredar)&&listaAHeredar.NoHereda(this);
            if (noSeHereda)//mirar que se sea padre de estas...si se añade el hijo al padre
            {
                ListasHeredadas.Add(listaAHeredar.Id);
                Tareas.Clear();
            }
            return noSeHereda;
        }
        public bool RemoveList(Lista listaAEliminar)
        {
            Tareas.Clear();
            return ListasHeredadas.Remove(listaAEliminar.Id);
        }
        public Tarea AddTarea(string textTarea = "")
        {
            Tarea tarea = new Tarea(textTarea);
            AddTarea(tarea);
            return tarea;
        }
        public void AddTarea(Tarea tarea)
        {
            TareasLista.Add(tarea.Id, tarea);
            Tareas.Add(tarea.Id, tarea);
        }
        public bool RemoveTarea(Tarea tarea)
        {
            bool removed= TareasLista.ContainsKey(tarea.Id);
            if (removed)
            {
                TareasLista.Remove(tarea.Id);
                Tareas.Remove(tarea.Id);
            }
            else if (!IdTareasHeredadasOcultas.ContainsKey(tarea.Id) && EstaTarea(tarea))
            {
                IdTareasHeredadasOcultas.Add(tarea.Id);
                Tareas.Remove(tarea.Id);
            }
    
            
            return removed;
        }
        public bool RemoveTarea(long idTarea)
        {
            bool hecho = false;
            RefreshTareas();
            if(Tareas.ContainsKey(idTarea)&& !IdTareasHeredadasOcultas.ContainsKey(idTarea))
              hecho= RemoveTarea(Tareas[idTarea]);
            return hecho;
        }
        public void TareaHecha(Tarea tarea)
        {
            if (!FechaTareaHecha.ContainsKey(tarea.Id))
                FechaTareaHecha.Add(tarea.Id, DateTime.Now.Ticks);
        }
        public void TareaNoHecha(Tarea tarea)
        {
            if(FechaTareaHecha.ContainsKey(tarea.Id))
             FechaTareaHecha.Remove(tarea.Id);
        }
        private bool EstaTarea(Tarea tarea)
        {
            RefreshTareas();
            return  Tareas.ContainsKey(tarea.Id);
        }
        public Tarea Get(long idTarea)
        {
            Tarea tarea = null;
            RefreshTareas();
            if (Tareas.ContainsKey(idTarea))
                tarea = Tareas[idTarea];
            return tarea;

        }

        private bool NoHereda(Lista listaAHeredar)
        {
            bool noHereda = true;
            for (int i = 0; i < ListasHeredadas.Count && noHereda; i++)
            {
                noHereda = ListasHeredadas.GetKey(i).CompareTo(listaAHeredar.Id) != 0;
                if (noHereda)
                {
                    noHereda = !ListasCargadas[ListasHeredadas.GetValueAt(i)].ListasHeredadas.ContainsKey(listaAHeredar.Id);

                    if (noHereda)
                        noHereda = ListasCargadas[ListasHeredadas.GetValueAt(i)].NoHereda(listaAHeredar);
                }
            }
            return noHereda;
        }
        public List<Lista> GetListasNoHeredadas()
        {
            SortedList<long, long> dicHerencia = GetIdListas();
            return Lista.ListasCargadas.Values.ToArray().Filtra((lst) => !dicHerencia.ContainsKey(lst.Id) && lst.NoHereda(this));
        }
        public List<Lista> GetListasHeredadas()
        {
            SortedList<long, long> dicHerencia = GetIdListas();
            dicHerencia.Remove(Id);
            return Lista.ListasCargadas.Values.ToArray().Filtra((lst)=>dicHerencia.ContainsKey(lst.Id));
        }
        public XmlNode ToXml()
        {
            XmlDocument xml = new XmlDocument();
            StringBuilder strNode = new StringBuilder($"<List><Nombre>{Nombre.EscaparCaracteresXML()}</Nombre><Id>{Id.ToString()}</Id><Tareas>");
            //Nombre
            //IdLista
            //tareasLista
            for (int i = 0; i < TareasLista.Count; i++)
                strNode.Append(TareasLista.GetValueAt(i).ToXml().OuterXml);
            strNode.Append("</Tareas>");
            //ListasHeredadas
            strNode.Append("<ListasHeredadas>");
            for (int i = 0; i < ListasHeredadas.Count; i++)
                strNode.Append($"<IdLista>{ListasHeredadas.GetValueAt(i).ToString()}</IdLista>");
            strNode.Append("</ListasHeredadas>");
            //TareasHeredadasOcultas
            strNode.Append("<TareasHeredadasOcultas>");
            for (int i = 0; i < IdTareasHeredadasOcultas.Count; i++)
                strNode.Append($"<IdTarea>{IdTareasHeredadasOcultas.GetValueAt(i).ToString()}</IdTarea>");
            strNode.Append("</TareasHeredadasOcultas>");
            //TareasHechas
            strNode.Append("<TareasHechas>");
            for (int i = 0; i < IdTareasHeredadasOcultas.Count; i++)
                strNode.Append($"<Tarea><IdTarea>{IdTareasHeredadasOcultas.GetKey(i).ToString()}</IdTarea><Hora>{IdTareasHeredadasOcultas.GetValueAt(i).ToString()}</Hora></Tarea>");
            strNode.Append("</TareasHechas>");
            strNode.Append("</List>");

            xml.LoadXml(strNode.ToString());
            xml.Normalize();
            return xml.FirstChild;
        }

        public IEnumerator<Tarea> GetEnumerator()
        {
            RefreshTareas();
            return Tareas.Values.GetEnumerator();

        }

        private void RefreshTareas()
        {
            if (Tareas.Count == 0)
            {
                for (int i = 0; i < ListasHeredadas.Count; i++)
                    foreach (Tarea tarea in ListasCargadas[ListasHeredadas.GetValueAt(i)])
                        if (!IdTareasHeredadasOcultas.ContainsKey(tarea.Id) && !Tareas.ContainsKey(tarea.Id))
                            Tareas.Add(tarea.Id, tarea);
                for (int i = 0; i < TareasLista.Count; i++)
                    if (!Tareas.ContainsKey(TareasLista.GetValueAt(i).Id))
                        Tareas.Add(TareasLista.GetValueAt(i).Id, TareasLista.GetValueAt(i));
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static XmlNode SaveListas(IList<Lista> listas)
        {
            XmlDocument xml = new XmlDocument();
            StringBuilder strNode = new StringBuilder("<Listas>");

            for (int i = 0; i < listas.Count; i++)
                strNode.Append(listas[i].ToXml().OuterXml);

            strNode.Append("</Listas>");
            xml.LoadXml(strNode.ToString());
            xml.Normalize();
            return xml.FirstChild;
        }
        public static List<Lista> LoadListas(XmlNode nodeListas)
        {
            List<Lista> listas = new List<Lista>();
            for (int i = 0; i < nodeListas.ChildNodes.Count; i++)
                listas.Add(new Lista(nodeListas.ChildNodes[i]));
            return listas;
        }
        public static List<Lista> TienenTarea(Tarea tarea)
        {
            return ListasCargadas.Filtra((lst) => lst.Value.EstaTarea(tarea)).ConvertAll<Lista>((lst) => lst.Value);
        }
    }

}

