using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Reflection;

namespace jsonReadModule
{
    public static class jsonRead
    {
        static public Dictionary<string, T> ReadFromJson<T>(string fileName)
        {
            string json;
            using (StreamReader sr = new StreamReader(fileName))
            {
                json = sr.ReadToEnd();
            }
            Dictionary<string, T> ret = JsonSerializer.Deserialize<Dictionary<string, T>>(json);
            return ret;
        }
    }
}
