using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using YamlDotNet.Serialization;

namespace HashTool.ViewModel
{
    internal class Serializer
    {
        public static void Json<T>(string path, List<T> hashResults)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            string jsonString = JsonSerializer.Serialize(hashResults, options);
            File.WriteAllTextAsync(path, jsonString);
        }

        public static void Yaml<T>(string path, List<T> hashResults)
        {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(hashResults);
            File.WriteAllTextAsync(path, yaml);
        }

        public static void Text<T>(string path, List<T> hashResults)
        {
            var serializer = new SerializerBuilder().Build();
            StringBuilder sb = new StringBuilder();
            foreach (T item in hashResults)
            {
                if (item != null)
                {
                    sb.AppendLine(serializer.Serialize(item));
                }
            }
            File.WriteAllTextAsync(path, sb.ToString());
        }
    }
}
