using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TareasPendientes.Blazor.Helpers
{
    public interface IDataSave
    {
        void Save(string dataXML);
        string Load();
    }
}
