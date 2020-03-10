using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TareasPendientes.Blazor2.Entities;

namespace TareasPendientes.Blazor2.Extension
{
    public static class Extension
    {

        public static T GetValue<T>(this SortedList<long, T> dic, Base value) 
        {
            return dic.ContainsKey(value.Id)?dic[value.Id]:default;
        }
        public static bool Contains<T>(this SortedList<long, T> dic, T value) where T : Base
        {
            bool contains=value!=default(T);
            if (contains)
                contains = dic.ContainsKey(value.Id);
            return contains;
        }
        public static void Add<T>(this SortedList<long, T> dic, T value) where T : Base
        {
            if (value != default(T))
                dic.Add(value.Id, value);
        }
        public static void Remove<T>(this SortedList<long, T> dic, T value) where T : Base
        {
            if (value != default(T))
                dic.Remove(value.Id);
        }
        public static void AddRange<T>(this SortedList<long, T> dic, IList<T> values) where T : Base
        {
            if (values != null)
                foreach (T value in values)
                    dic.Add(value.Id, value);
        }
        public static void RemoveRange<T>(this SortedList<long, T> dic, IList<T> values) where T : Base
        {
            if (values != null)
                foreach (T value in values)
                    dic.Remove(value.Id);
        }
        public static SortedList<TKey,TValue> Clone<TKey,TValue>(this SortedList<TKey,TValue> dic)
        {
            SortedList<TKey,TValue> clon = new SortedList<TKey,TValue>();
            foreach (var item in dic)
                clon.Add(item.Key, item.Value);
            return clon;
        }

        public static async Task MostrarMensajeAsync(this IJSRuntime js, string mensaje)
        {
            await js.InvokeVoidAsync("alert", mensaje);
        }
        public static async Task DownloadFileAsync(this IJSRuntime js, string fileName, object data, string metaType)
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


        public static T Last<T>(this IList<T> lst)
        {
            return lst.Count>0? lst[lst.Count - 1]:default;
        }
    }
}
