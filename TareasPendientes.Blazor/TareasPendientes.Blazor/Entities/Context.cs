using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TareasPendientes.Blazor.Entities
{
    public class Context
    {
        public class ContextMain
        {
            private int indexCategoria;

            public int IndexCategoria { get => indexCategoria; set { indexCategoria = value<0?0:value>=Context.Categorias.Count? Context.Categorias.Count -1: value; } }
            Context Context { get; set; }
            public Categoria Categoria
            {

                get
                {
                    return Context.Categorias[IndexCategoria];
                }
            }
            public List<Lista> Listas
            {
                get
                {
                    return Categoria.GetListas();
                }
            }
            public Lista ListaActual { get; set; }
            public bool HayLista => ListaActual != null;
            public string NameList { get; set; } = null;

            public bool EditMode { get; set; } = false;
            public ContextMain(Context context)
            {
                Context = context;
                indexCategoria = 0;
            }
            public void Clear()
            {
                IndexCategoria = 0;
                ListaActual = null;

            }
        }
        public class ContextSearch
        {
           public string TextoToSearch { get; set; }
           public  Lista ListaTemporal { get; set; }
           public  List<Lista> ListasTarea { get; set; }
            Context Context { get; set; }
            public ContextSearch(Context context)
            {
                ListaTemporal = new Lista("temporal", 0);
                ListasTarea = new List<Lista>();
                Context = context;
            }
            public void Clear()
            {
                ListasTarea.Clear();
                ListaTemporal = new Lista("temporal", 0);
            }
        }
        public class ContextOrganizar
        {
           public Lista ListaSeleccionada
            {
                get; set;
            }
            public List<Lista> NoHeredadas
            {
                get
                {
                    return ListaSeleccionada.GetListasNoHeredadas();
                }
            }
            public List<Lista> Heredadas
            {
                get
                {
                    return ListaSeleccionada.GetListasHeredadas();
                }
            }
            public Lista TareasOcultas
            {
                get
                {
                    return new Lista(ListaSeleccionada.IdTareasHeredadasOcultas.GetValues());
                }
            }
            public void Clear()
            {
                ListaSeleccionada = null;
            }
        }



        public List<Categoria> Categorias { get; set; }
        public List<Lista> Listas { get; set; }
        public ContextMain Main { get; private set; }
        public ContextSearch Search { get; private set; }
        public ContextOrganizar Organizar { get; private set; }

        Context()
        {
            Categorias = new List<Categoria>();
            Listas = new List<Lista>();
            Main = new ContextMain(this);
            Search = new ContextSearch(this);
            Organizar = new ContextOrganizar();
        }
        public Context(XmlDocument xmlDocument = null):this()
        {
            LoadXML(xmlDocument);

       
        }
        public Context(string xmlDocument = null) : this()
        {
            LoadXML(xmlDocument);


        }
        public void LoadXML(string strXML)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                if (strXML != null)
                    xml.LoadXml(strXML);

            }
            catch { }
            finally { LoadXML(xml); }
        }
        public void LoadXML(XmlDocument xmlDocument)
        {
    

            Lista.ListasCargadas.Clear();
            Tarea.TareasCargadas.Clear();
            Search.Clear();
            Main.Clear();
            Organizar.Clear();

            try
            {
                if (xmlDocument != null && xmlDocument.HasChildNodes)
                {
                    Categorias = Categoria.LoadCategorias(xmlDocument.ChildNodes[0]);
                    Listas = Lista.LoadListas(xmlDocument.ChildNodes[1]);

                }
            }
            catch { }

            if (Categorias.Count == 0)
            {
                Categorias.Add(new Categoria("Todas", 0));

            }
            if (Listas.Count == 0)
            {
                Listas.Add(new Lista("Mi primera lista", 0));
            }
        }
   
        public XmlDocument ToXmlDocument()
        {
            XmlDocument xml = new XmlDocument();
            StringBuilder strNode = new StringBuilder("<Context>");
            strNode.Append(Categoria.SaveCategorias(Categorias).OuterXml);
            strNode.Append(Lista.SaveListas(Listas).OuterXml);
            strNode.Append("</Context>");
            xml.LoadXml(strNode.ToString());
            xml.Normalize();
            return xml;
        }

    }
}
