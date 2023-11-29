using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static JsonHelp.JsonHelper;
using Debug = UnityEngine.Debug;

namespace JsonHelp
{
    internal static class JsonHelper
    {
        static JsonHelper() { }


        /// <summary>
        /// 写入到文件
        /// </summary>
        /// <param name="path"></param>
        public static void WriteJsonToFile(object jsonObj,  string filePath)
        {
            JsonSerializer SERIALIZER = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                using(JsonWriter w = new JsonTextWriter(sw))
                {
                    SERIALIZER.Serialize(w, jsonObj);
                }
            }
        }

        public abstract class JsonCreateConverter<T> : JsonConverter
        {
            protected abstract T Create(Type objectType, JObject jsonObject);
            public override bool CanConvert(Type objectType)
            {
                return typeof(T).IsAssignableFrom(objectType);
            }
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jsonObject = JObject.Load(reader);
                var target = Create(objectType, jsonObject);
                serializer.Populate(jsonObject.CreateReader(), target);
                return target;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        private static void ReadJson<T>(string file,out T obj, params JsonConverter[] converters)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sr = new StreamReader(file))
            {
                obj = JsonConvert.DeserializeObject<T>(sr.ReadToEnd(), converters);
            }
        }



        

        /// <summary>
        /// 打开对话框选择json文件并返回c#类对象
        /// </summary>
        public static T ReadTreeInfo<T>(params JsonConverter[] converters)
        {
            string path;
            string filename;
            path = openFileDialog(out filename);
            T ret = default(T);
            if (path == null)
            {
                return ret;
            }
            if (!File.Exists(path))
            {
                Debug.LogError("不存在路径" + path);
                return ret;
            }
            string jsonStr = File.ReadAllText(path);
            
            try
            {
                ReadJson<T>(path, out ret, converters);
                //ret = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonStr);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message +"\n"+ e.StackTrace);
                return ret;
            }
            if (ret == null)
            {
                Debug.LogError("解析json返回null!");
                return ret;
            }
            return ret;
        }

        /// <summary>
        /// 打开一个对话框，返回选择文件的地址
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public static string openFileDialog(out string filename)
        {
            FileOpenDialog dialog = new FileOpenDialog();
            dialog.structSize = Marshal.SizeOf(dialog);
            dialog.filter = "json files\0*.json\0All Files\0*.*\0\0";
            dialog.file = new string(new char[256]);
            dialog.maxFile = dialog.file.Length;
            dialog.fileTitle = new string(new char[64]);
            dialog.maxFileTitle = dialog.fileTitle.Length;
            dialog.initialDir = UnityEngine.Application.dataPath;  //默认路径
            dialog.title = "Open File Dialog";
            dialog.defExt = "json";//显示文件的类型
            dialog.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;  //OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
            if (DialogShow.GetOpenFileName(dialog))
            {
                filename = dialog.fileTitle.Split('.')[0];
                return dialog.file;
            }
            filename = null;
            return null;
        }
    }



}
