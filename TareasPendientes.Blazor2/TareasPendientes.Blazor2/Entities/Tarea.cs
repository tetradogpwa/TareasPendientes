using Gabriel.Cat.S.Binaris;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TareasPendientes.Blazor2.Entities
{
    public class Tarea:Base,IElementoBinarioComplejo
    {
        public class TareaBinaria : ElementoComplejoBinario
        {
            public TareaBinaria():base(new ElementoBinario[] { 
                ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long),
                ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.String)}) 
            { }
            protected override IList IGetPartsObject(object obj)
            {
                Tarea tarea = obj as Tarea;
                if (tarea == null)
                    throw new Exception("El tipo valido es Tarea");

                return new object[] {tarea.Id,tarea.Name };
            }

            protected override object JGetObject(MemoryStream bytes)
            {
                object[] partes = base.GetPartsObject(bytes);
                return new Tarea(partes[1] as string,(long)partes[1]);
            }
        }
        public static readonly ElementoBinario Serializador = new TareaBinaria();
        public Tarea() : this("") { }
        public Tarea(string nombre,long id = -1) : base(nombre, id) { }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
    }
}
