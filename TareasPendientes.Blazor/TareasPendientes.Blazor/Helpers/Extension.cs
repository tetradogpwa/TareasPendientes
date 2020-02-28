using BlazorInputFile;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//incluir LocalStorage.js,ImportExport.js en wwwroot
namespace TareasPendientes.Blazor.Helpers
{
    public static class ExtensionIJS
    {
        
        public static async Task MostrarMensajeAsync(this IJSRuntime js, string mensaje)
        {
          await  js.InvokeVoidAsync("alert", mensaje);
        }
        public static async Task DownloadFileAsync(this  IJSRuntime js, string fileName,object data,string metaType)
        {
           await js.InvokeVoidAsync("FileSaveAs", fileName, data, metaType);
            //da problemas pone que no se ha podido descargar...
        }
        public static async Task SaveLocalStorageAsync(this IJSRuntime js, string id, string data)
        {
            await js.InvokeVoidAsync("SaveLocalStorage", id, data);
        }

        public static async Task<string> LoadLocalStorageAsync(this IJSRuntime js, string id)
        {
            return await js.InvokeAsync<string>("LoadLocalStorage", id);
        }
        public static async ValueTask<bool> PreguntaAsync(this IJSRuntime js, string mensaje)
        {
            return await js.InvokeAsync<bool>("confirm", mensaje);

        }


        public static async Task<string> LoadStringAsync(this IFileListEntry file,System.Text.Encoding encoding= null)
        {
            if (encoding == null)
                encoding = System.Text.ASCIIEncoding.ASCII;

           return encoding.GetString( (await file.ReadAllAsync()).ToArray());
        }
    
 
    }
}
