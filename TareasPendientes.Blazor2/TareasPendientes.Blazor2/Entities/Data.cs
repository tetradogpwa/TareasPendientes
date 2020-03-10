using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TareasPendientes.Blazor2.Extension;

namespace TareasPendientes.Blazor2.Entities
{
    public class Data
    {
        public Data()
        {
            Listas = new SortedList<long, Lista>();
            Categorias = new SortedList<long, Categoria>();
            Listas.Add(new Lista("Mi primera lista", 0));
            Categorias.Add(new Categoria("Todas", 0) { Listas = Listas });
        }

        public SortedList<long,Lista> Listas { get; set; }
        public SortedList<long, Categoria> Categorias { get; set; }
    }
}
