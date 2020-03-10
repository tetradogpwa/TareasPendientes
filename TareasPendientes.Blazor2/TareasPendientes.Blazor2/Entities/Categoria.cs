using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TareasPendientes.Blazor2.Entities
{
    public class Categoria:Base
    {
        public Categoria(string nombre,long id = -1) : base(nombre, id)
        {
            Listas = new SortedList<long, Lista>();
        }
        public SortedList<long,Lista> Listas { get; set; }

       
    }
}
