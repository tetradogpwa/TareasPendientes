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
        class TareaBinaria : ElementoComplejoBinario
        {
            const int ID = 0, NAME = ID + 1, TOTAL = NAME + 1;
            public TareaBinaria() : base(new ElementoBinario[] {

                ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long),

                ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.String)})

            { }

            protected override IList IGetPartsObject(object obj)
            {
                Console.WriteLine("Inicio Serializar Tarea");
                object[] partes;
                Tarea tarea = obj as Tarea;

                if (tarea == null)
                    throw new Exception("El tipo valido es Tarea");

                partes = new object[TOTAL];
                partes[ID] = tarea.Id;
                partes[NAME] = tarea.Name;
                Console.WriteLine("Fin Serializar Tarea");
                return partes;

            }



            protected override object JGetObject(MemoryStream bytes)
            {
                Console.WriteLine("Inicio Deserializar Tarea");
                object[] partes = base.GetPartsObject(bytes);

                Tarea tarea= new Tarea((string)partes[NAME], (long)partes[ID]);

                Console.WriteLine("Fin Deserializar Tarea");

                return tarea;

            }

        }
        internal static readonly ElementoBinario Serializador = new TareaBinaria();
        public Tarea(string nombre,long id = -1) : base(nombre, id) { }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
    }
}
