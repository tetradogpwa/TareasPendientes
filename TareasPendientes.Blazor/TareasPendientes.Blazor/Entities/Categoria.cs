using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TareasPendientes.Blazor.Entities
{
	public class Categoria

	{
		public LlistaOrdenada<long> Listas { get; set; }
		public string Nombre { get; set; }
		public long Id { get; set; }

		public List<Lista> GetListas() {

			List<Lista> lista = new List<Lista>();
			if (Id != 0)
			{
				for (int i = 0; i < Listas.Count; i++)
					lista.Add(Lista.ListasCargadas[Listas.GetValueAt(i)]);
			}
			else
			{
				lista.AddRange(Lista.ListasCargadas.GetValues());
			}
			return lista;
		
		}
		public Lista GetListaAt(int index = 0)
		{
			Lista lst;
			if (Id != 0)
			{
				lst = Lista.ListasCargadas[Listas.GetValueAt(index)];

			}
			else
			{
				lst = Lista.ListasCargadas.GetValueAt(index);
			}
			return lst;
		}
		public Categoria(string nombre,long id = -1)
		{
			if (id < 0)
				Id = DateTime.Now.Ticks;
			else Id = id;
			Nombre = nombre;
			Listas = new LlistaOrdenada<long>();
		}
		public Categoria(XmlNode nodeCategoria) : this(nodeCategoria.ChildNodes[0].InnerText.DescaparCaracteresXML(), long.Parse(nodeCategoria.ChildNodes[1].InnerText))
		{
			for(int i=0;i<nodeCategoria.ChildNodes[2].ChildNodes.Count;i++)
			{
				Listas.Add(long.Parse(nodeCategoria.ChildNodes[2].ChildNodes[i].InnerText));
			}
		}
		public XmlNode ToXml()
		{
			XmlDocument xml = new XmlDocument();
			StringBuilder strNode = new StringBuilder("<Categoria>");
			strNode.Append($"<Nombre>{Nombre.EscaparCaracteresXML()}</Nombre>");
			strNode.Append($"<Id>{Id.ToString()}</Id>");
			strNode.Append("<Listas>");
			for(int i=0;i<Listas.Count;i++)
				strNode.Append($"<IdLista>{Listas.GetValueAt(i).ToString()}</IdLista>");
			strNode.Append("</Listas>");
			strNode.Append("</Categoria>");
			xml.LoadXml(strNode.ToString());
			xml.Normalize();
			return xml.FirstChild;

		}

		public static XmlNode SaveCategorias(IList<Categoria> categoria)
		{
			XmlDocument xml = new XmlDocument();
			StringBuilder strNode = new StringBuilder("<Categorias>");

			for (int i = 0; i < categoria.Count; i++)
				strNode.Append(categoria[i].ToXml().OuterXml);

			strNode.Append("</Categorias>");
			xml.LoadXml(strNode.ToString());
			xml.Normalize();
			return xml.FirstChild;
		}
		public static List<Categoria> LoadCategorias(XmlNode nodeCategorias)
		{
			List<Categoria> categorias = new List<Categoria>();
			if (nodeCategorias != null && nodeCategorias.HasChildNodes)
			{
				for (int i = 0; i < nodeCategorias.ChildNodes.Count; i++)
					categorias.Add(new Categoria(nodeCategorias.ChildNodes[i]));
			}
			return categorias;
		}

	}

}