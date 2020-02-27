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
        public List<Categoria> Categorias { get; set; }
        public List<Lista> Listas { get; set; }

        public Context(XmlDocument xmlDocument = null)
        {
            if (xmlDocument == null)
            {
                Categorias = new List<Categoria>();
                Listas = new List<Lista>();
 
            }
            else
            {
                Categorias = Categoria.LoadCategorias(xmlDocument.ChildNodes[0]);
                Listas = Lista.LoadListas(xmlDocument.ChildNodes[1]);
            }

            if (Categorias.Count == 0)
            {
                Categorias.Add(new Categoria("Todas", 0));
             
            }
            if(Listas.Count==0)
            {
                Listas.Add(new Lista("Mi primera lista",0));
            }
        }
        public void LoadXML(string strXML)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(strXML);
            LoadXML(xml);
        }
        public void LoadXML(XmlDocument xml)
        {
            Context context;
            Lista.ListasCargadas.Clear();
            context= new Context(xml);
            Categorias = context.Categorias;
            Listas = context.Listas;
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
        public static Context Load(string strXmlDocument)
        {
            XmlDocument xml=null;
            if (!string.IsNullOrEmpty(strXmlDocument))
            {
                xml = new XmlDocument();
                xml.LoadXml(strXmlDocument);
                
            }
            
            return new Context(xml);
        }
    }
}
