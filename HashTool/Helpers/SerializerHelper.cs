// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml;
using System.Xml.Serialization;

using HashTool.Models;

using YamlDotNet.Serialization;

namespace HashTool.Helpers
{
    internal static class SerializerHelper
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
            StringBuilder stringBulider = new();
            foreach (T item in list)
            {
                if (item != null)
                {
                    stringBulider.AppendLine(serializer.Serialize(item));
                }
            }
            File.WriteAllTextAsync(path, stringBulider.ToString());
        }

        public static void Xml(string path, List<Dictionary<string, string>> list)
        {
            XmlWriterSettings settings = new();
            settings.Indent = true;
            settings.IndentChars = "    ";

            using var writer = XmlWriter.Create(path, settings);
            var xmlResult = new HashResultRoot()
            {
                HashResultItems = list.Select(dict => new HashResultItem()
                {
                    Items = dict.Select(kv => new Item() { id = kv.Key, value = kv.Value }).ToArray()
                }).ToArray()
            };

            XmlSerializer serializer = new(typeof(HashResultRoot));
            serializer.Serialize(writer, xmlResult);
        }

        public static Dictionary<string, string> BuildHashResult(HashResultModel hashResult)
        {
            Dictionary<string, string> dict = new();

            // 下面的顺序即为储存时的顺序
            dict.Add("输入模式", hashResult.InputMode);
            dict.Add("计算模式", hashResult.Mode);
            dict.Add("计算内容", hashResult.Content);
            dict.Add("文件大小", hashResult.FileSize);
            dict.Add("文件修改时间", hashResult.LastWriteTime);
            dict.Add("计算开始时间", hashResult.ComputeTime);
            dict.Add("计算用时", hashResult.ComputeCost);

            hashResult.Items.ForEach(i => dict.Add(i.Name, i.Value));

            return dict;
        }

        public static List<Dictionary<string, string>> BuildHashResult(List<HashResultModel> hashResults)
        {
            List<Dictionary<string, string>> list = new();

            hashResults.ForEach(i => list.Add(BuildHashResult(i)));

            return list;
        }
    }

    #region XmlSerializer Helper

    [XmlRoot("HashTool")]
    public class HashResultRoot
    {
        [XmlElement("Result")]
        public HashResultItem[]? HashResultItems;
    }

    public class HashResultItem
    {
        [XmlElement("Item")]
        public Item[]? Items;
    }

    public class Item
    {
        [XmlAttribute]
        public string? id;
        [XmlText]
        public string? value;
    }

    #endregion  XmlSerializer Helper
}
