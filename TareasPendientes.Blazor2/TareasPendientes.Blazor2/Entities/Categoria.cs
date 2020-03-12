using Gabriel.Cat.S.Extension;
using Gabriel.Cat.S.Binaris;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using TareasPendientes.Blazor2.Extension;

namespace TareasPendientes.Blazor2.Entities
{
    public class Categoria : Base,IElementoBinarioComplejo
    {
         class CategoriaBinaria : ElementoComplejoBinario
        {
            const int ID = 0, NAME = ID + 1, IDLISTAS = NAME + 1,TOTAL=IDLISTAS+1;
            public  CategoriaBinaria():base(new ElementoBinario[] {
                ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long),
                ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.String),
                ElementoIListBinario<long>.ElementosTipoAceptado<long>(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long)

            }){}
            protected override IList IGetPartsObject(object obj)
            {
                Console.WriteLine("Inicio Serializar Categoria");
                object[] partes;
                Categoria categoria = obj as Categoria;
                if (categoria == null)
                    throw new Exception("El tipo de objeto valido es Categoria");
                partes = new object[TOTAL];
                partes[ID] = categoria.Id;
                partes[NAME] = categoria.Name;
                partes[IDLISTAS] = categoria.Listas.GetKeys();
                Console.WriteLine("Fin Serializar Categoria");
                return partes;
            }

            protected override object JGetObject(MemoryStream bytes)
            {
                Console.WriteLine("Inicio deserializar Categoria");
                object[] partes = base.GetPartsObject(bytes);
                long[] ids = (long[])partes[IDLISTAS];
                Categoria categoria = new Categoria((string)partes[NAME], (long)partes[ID]);
                categoria.Listas.AddRange(ids);
                Console.WriteLine("Fin deserializar Categoria");
                return categoria;

            }
        }
       internal static readonly ElementoBinario Serializador = new CategoriaBinaria();
    
        public Categoria(string nombre, long id = -1) : base(nombre, id)
        {
            Listas = new SortedList<long, Lista>();
        }

        public SortedList<long, Lista> Listas { get; set; }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
    }
}
