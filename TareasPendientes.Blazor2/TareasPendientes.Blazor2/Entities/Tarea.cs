using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TareasPendientes.Blazor2.Entities
{
    public class Tarea:Base
    {
        public Tarea() : this("") { }
        public Tarea(string nombre,long id = -1) : base(nombre, id) { }
    }
}
