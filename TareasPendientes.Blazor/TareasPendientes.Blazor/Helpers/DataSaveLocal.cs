using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TareasPendientes.Blazor.Helpers
{
    public class DataSaveLocal : IDataSave
    {
        [Inject] IJSRuntime JS { get; set; }
        const string ID = "BD_TareasPendientes";

        string IDataSave.Load()
        {
            Task<string> load = JS.LoadLocalStorageAsync(ID);
            if(load.Status==TaskStatus.WaitingToRun)
             load.Start();
            load.Wait();
            return load.Result;
        }

        void IDataSave.Save(string dataXML)
        {
            Task save = JS.SaveLocalStorageAsync(ID,dataXML);
            if (save.Status == TaskStatus.WaitingToRun)
                save.Start();
            save.Wait();

        }
    }
}
