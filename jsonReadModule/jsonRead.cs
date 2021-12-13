using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace jsonReadModule
{
    public static class JsonRead
    {
        static public Dictionary<string, T> ReadFromJson<T>(string fileName)
        {
            string json;
            using (StreamReader sr = new StreamReader(fileName))
            {
                json = sr.ReadToEnd();
            }

            Dictionary<string, T> ret = JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
            return ret;
        }
    }
}
