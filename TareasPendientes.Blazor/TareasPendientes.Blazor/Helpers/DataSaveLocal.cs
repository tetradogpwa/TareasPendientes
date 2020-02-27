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
        const string ID = "BD_TareasPendientes";

        string IDataSave.Load()
        {
            Task<string> load =this.LoadLocalStorageAsync(ID);
            if(load.Status==TaskStatus.WaitingToRun)
             load.Start();
            load.Wait();
            return load.Result;
        }

        void IDataSave.Save(string dataXML)
        {
            Task save = this.SaveLocalStorageAsync(ID,dataXML);
            if (save.Status == TaskStatus.WaitingToRun)
                save.Start();
            save.Wait();

        }
    }
}
