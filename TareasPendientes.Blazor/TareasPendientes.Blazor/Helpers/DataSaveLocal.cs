using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TareasPendientes.Helpers
{
    public class DataSaveLocal : IDataSave
    {
        const string ID = "BD_TareasPendientes";

      public async Task<string> Load(IJSRuntime js)
        {
           return (await  js.LoadLocalStorageAsync(ID));
           
        }

       public async Task Save(IJSRuntime js,string dataXML)
        {
            await js.SaveLocalStorageAsync(ID,dataXML);
        }
    }
}
