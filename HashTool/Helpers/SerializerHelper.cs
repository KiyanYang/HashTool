using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

using HashTool.Models;

using YamlDotNet.Serialization;

namespace HashTool.Helpers
{
    internal class SerializerHelper
    {
        public static void Json<T>(string path, List<T> list)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true,
            };
            string jsonString = JsonSerializer.Serialize(list, options);
            File.WriteAllTextAsync(path, jsonString);
        }

        public static void Yaml<T>(string path, List<T> list)
        {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(list);
            File.WriteAllTextAsync(path, yaml);
        }

        public static void Text<T>(string path, List<T> list)
        {
            var serializer = new SerializerBuilder().Build();
            StringBuilder stringBulider = new StringBuilder();
            foreach (T item in list)
            {
                if (item != null)
                {
                    stringBulider.AppendLine(serializer.Serialize(item));
                }
            }
            File.WriteAllTextAsync(path, stringBulider.ToString());
        }

        private static readonly (string Key, string HumanReadableKey)[] hashResultFormatter =
        {
            // 下面的顺序即为储存时的顺序
            ("InputMode", "输入模式"),
            ("Mode", "计算模式"),
            ("Content", "计算内容"),
            ("FileSize", "文件大小"),
            ("LastWriteTime", "文件修改时间"),
            ("ComputeTime", "计算开始时间"),
            ("ComputeCost", "计算用时"),
            ("MD5", "MD5"),
            ("CRC32", "CRC32"),
            ("SHA1", "SHA1"),
            ("SHA256", "SHA256"),
            ("SHA384", "SHA384"),
            ("SHA512", "SHA512"),
        };

        public static Dictionary<string, string> BuildHashResult(HashResultModel hashResult)
        {
            string? tmp;
            Dictionary<string, string> newDict = new();

            foreach (var i in hashResultFormatter)
            {
                CommonHelper.GetProperty(hashResult, i.Key, out tmp);
                if (tmp != null)
                {
                    newDict.Add(i.HumanReadableKey, tmp);
                }
            }

            return newDict;
        }

        public static List<Dictionary<string, string>> BuildHashResult(List<HashResultModel> hashResults)
        {
            List<Dictionary<string, string>> list = new();

            foreach (var i in hashResults)
            {
                list.Add(BuildHashResult(i));
            }

            return list;
        }
    }
}
