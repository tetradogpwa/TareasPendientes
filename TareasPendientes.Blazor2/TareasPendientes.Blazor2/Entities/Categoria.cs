using Gabriel.Cat.S.Extension;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TareasPendientes.Blazor2.Entities
{
    public class Categoria : Base
    {
        public Categoria() : this("") { }
        public Categoria(string nombre, long id = -1) : base(nombre, id)
        {
            Listas = new SortedList<long, Lista>();
        }
        [JsonIgnore]
        public SortedList<long, Lista> Listas { get; set; }
        [JsonProperty("ListasCategoria")]
        public IList<long> IListas
        {
            get
            {
                return Listas.GetKeys();
            }
            set
            {
                Listas.Clear();
                if (value != null)
                    for (int i = 0; i < value.Count; i++)
                        Listas.Add(value[i], null);

            }
        }

    }
}
