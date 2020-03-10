using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TareasPendientes.Blazor2.Entities
{
    public class Base
    {
    

        public Base(string nombre, long id)
        {
            Name = nombre;
            if (id < 0)
                id = DateTime.Now.Ticks;
            Id = id;
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public bool HasName => !string.IsNullOrEmpty(Name) && !string.IsNullOrWhiteSpace(Name);

        public override string ToString()
        {
            return HasName? Name:$"{base.ToString().Split('.').Last()} {Id}";
        }
    }
}
