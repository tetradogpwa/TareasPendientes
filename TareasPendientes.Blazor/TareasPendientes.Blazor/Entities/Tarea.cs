using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Utilitats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TareasPendientes.Entities
{
    public class Tarea {

        public static LlistaOrdenada<long, Tarea> TareasCargadas { get; private set; } = new LlistaOrdenada<long, Tarea>();
        public long Id { get; set; }
        public string Texto { get; set; }

        public Tarea(string texto = "",long id=-1)
        {
            Texto = texto;
            if (id < 0)
                Id = DateTime.Now.Ticks;
            else Id = id;
            if(!TareasCargadas.ContainsKey(Id))
                 TareasCargadas.Add(Id, this);
        }
        public Tarea(XmlNode nodeTarea):this(nodeTarea.ChildNodes[0].InnerText.DescaparCaracteresXML(),long.Parse(nodeTarea.ChildNodes[1].InnerText))
        { }
        public XmlNode ToXml()
        {
            XmlDocument xml = new XmlDocument();
            StringBuilder strNode = new StringBuilder("<Tarea>");
            strNode.Append($"<Texto>{Texto.EscaparCaracteresXML()}</Texto>");
            strNode.Append($"<Id>{Id.ToString()}</Id>");
            strNode.Append("</Tarea>");
            xml.LoadXml(strNode.ToString());
            xml.Normalize();
            return xml.FirstChild;
        }

        public static List<Tarea> HaveToContain(string textoAContener)
        {
            return TareasCargadas.Filtra((tarea) => tarea.Value.Texto.Contains(textoAContener)).ConvertAll<Tarea>((tarea)=>tarea.Value);
        }

    }

}