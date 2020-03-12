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
        public class CategoriaBinaria : ElementoComplejoBinario
        {
            public  CategoriaBinaria():base(new ElementoBinario[] {
                ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long),
                ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.String),
                ElementoIListBinario<long>.ElementosTipoAceptado<long>(Gabriel.Cat.S.Utilitats.Serializar.TiposAceptados.Long)

            }){}
            protected override IList IGetPartsObject(object obj)
            {
                Categoria categoria = obj as Categoria;
                if (categoria == null)
                    throw new Exception("El tipo de objeto valido es Categoria");
                return new object[]{categoria.Id,categoria.Name,categoria.Listas.GetKeys() };
            }

            protected override object JGetObject(MemoryStream bytes)
            {
                object[] partes = base.GetPartsObject(bytes);
                long[] ids = (long[])partes[2];
                Categoria categoria = new Categoria(partes[1] as string, (long)partes[0]);
                categoria.Listas.AddRange(ids);
                return categoria;

            }
        }
        public static readonly ElementoBinario Serializador = new CategoriaBinaria();
        public Categoria() : this("") { }
        public Categoria(string nombre, long id = -1) : base(nombre, id)
        {
            Listas = new SortedList<long, Lista>();
        }

        public SortedList<long, Lista> Listas { get; set; }

        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;
    }
}
