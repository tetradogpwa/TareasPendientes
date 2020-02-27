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
        [Inject] static IJSRuntime JS { get; set; }

        public static void MostrarMensaje(this object obj,string mensaje)
        {
            JS.InvokeVoidAsync("alert", mensaje).Wait();
        }
        public static void DownloadFile(this object obj,string fileName,object data,string metaType)
        {
             JS.InvokeVoidAsync("FileSaveAs", fileName, data, metaType).Wait();
            //da problemas pone que no se ha podido descargar...
        }
        public static async Task SaveLocalStorageAsync(this object obj, string id, string data)
        {
            await JS.InvokeVoidAsync("SaveLocalStorage", id, data);
        }

        public static async Task<string> LoadLocalStorageAsync(this object obj, string id)
        {
            return await JS.InvokeAsync<string>("LoadLocalStorage", id);
        }
        public static bool Pregunta(this object obj, string mensaje)
        {
            ValueTask<bool> respuesta = JS.InvokeAsync<bool>("confirm", mensaje);

            return respuesta.GetResult();
        }


        public static async Task<string> LoadStringAsync(this IFileListEntry file,System.Text.Encoding encoding= null)
        {
            if (encoding == null)
                encoding = System.Text.ASCIIEncoding.ASCII;

           return encoding.GetString( (await file.ReadAllAsync()).ToArray());
        }
        public static string LoadString(this IFileListEntry file, System.Text.Encoding encoding = null)
        {
            return file.LoadStringAsync(encoding).GetResult();
        }

  

        public static void Wait<T>(this ValueTask<T> task)
        {
            task.AsTask().Wait();
        }
        public static void Wait(this ValueTask task)
        {
            task.AsTask().Wait();
        }
        public static T GetResult<T>(this ValueTask<T> task)
        {
            task.Wait();
            return task.Result;
        }
        public static T GetResult<T>(this Task<T> task)
        {
            task.Wait();
            return task.Result;
        }
    }
}
