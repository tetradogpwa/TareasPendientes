using Blazor.FileReader;
using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
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
               for(int i=0;i<values.Count;i++)
                    dic.Add(values[i].Id, values[i]);
        }
        public static void AddRange<T>(this SortedList<long, T> dic, IList<long> ids) where T : Base
        {
            if (ids != null)
                for (int i = 0; i < ids.Count; i++)
                    dic.Add(ids[i], default);
        }
        public static void RemoveRange<T>(this SortedList<long, T> dic, IList<T> values) where T : Base
        {
            if (values != null)
                for (int i = 0; i < values.Count; i++)
                    dic.Remove(values[i].Id);
        }
        public static void SetValues<T>(this SortedList<long,T> dic,SortedList<long,T> values)
        {
            long[] ids;
            ids = dic.GetKeys();
            for (int i = 0; i < ids.Length; i++)
                if(values.ContainsKey(ids[i]))
                   dic[ids[i]] = values[ids[i]];
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
        public static async Task DownloadFileStringAsync(this IJSRuntime js, string fileName, string data,string fileType,string charset="utf-8")
        {
        
            await js.InvokeVoidAsync("StringSaveAsFile", fileName, data,fileType,charset);
        }
        public static async Task DownloadFileBinaryAsync(this IJSRuntime js, string fileName, IElementoBinarioComplejo data)
        {
            await DownloadFileBinaryAsync(js, fileName, data.Serialitzer.GetBytes(data));
        }
        public static async Task DownloadFileBinaryAsync(this IJSRuntime js,string fileName,byte[] data)
        {//no funciona
            await js.InvokeVoidAsync("SaveAsFile", fileName, data);
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

        public static async Task<byte[]> Read(this IFileReference fileReader, int buffer = 4 * 1024)
        {
            MemoryStream ms = null;
            byte[] bytesFile = null;
            try
            {
                ms = await fileReader.CreateMemoryStreamAsync(buffer);

                bytesFile = new byte[ms.Length];
                ms.Read(bytesFile, 0, (int)ms.Length);
            }
            finally
            {
                if (ms != null)
                    ms.Close();

            }
            return bytesFile;
        }

    }
}
