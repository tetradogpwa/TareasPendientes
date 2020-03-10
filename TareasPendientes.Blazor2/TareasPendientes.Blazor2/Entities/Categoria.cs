using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TareasPendientes.Blazor2.Entities
{
    public class Categoria:Base
    {
        public Categoria() : this("") { }
        public Categoria(string nombre,long id = -1) : base(nombre, id)
        {
            Listas = new SortedList<long, Lista>();
        }
        [JsonIgnore]
        public SortedList<long,Lista> Listas { get; set; }
        [JsonPropertyName("ListasCategoria")]
        public long[] IListas
        {
            get
            {
                return Listas.GetKeys();
            }
            set
            {
                Listas.Clear();
                for (int i = 0;value!=null && i < value.Length; i++)
                    Listas.Add(value[i], null);

            }
        }
       
    }
}
