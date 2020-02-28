using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TareasPendientes.Blazor.Helpers
{
    public interface IDataSave
    {
        Task Save(IJSRuntime js,string dataXML);
        Task<string> Load(IJSRuntime js);
    }
}
