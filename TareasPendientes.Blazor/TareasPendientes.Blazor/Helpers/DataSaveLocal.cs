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
        [Inject] IJSRuntime JS { get; set; }
        public async Task<string> LoadAsync()
        {
            return await JS.InvokeAsync<string>("LoadLocalStorage", ID);
        }

        public async Task SaveAsync(string dataXML)
        {
            await JS.InvokeVoidAsync("SaveLocalStorage", ID, dataXML);
        }

        string IDataSave.Load()
        {
            Task<string> load = LoadAsync();
            if(load.Status==TaskStatus.WaitingToRun)
             load.Start();
            load.Wait();
            return load.Result;
        }

        void IDataSave.Save(string dataXML)
        {
            Task save = SaveAsync(dataXML);
            if (save.Status == TaskStatus.WaitingToRun)
                save.Start();
            save.Wait();

        }
    }
}
