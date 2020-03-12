using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TareasPendientes.Helpers
{
    public interface IDataSave
    {
        Task Save(IJSRuntime js,string data);
        Task<string> Load(IJSRuntime js);
    }
}
