using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HashTool.Helper
{
    internal class CommonHelper
    {
        public static void SetProperty<T>(object obj, string key, T value)
        {
            var property = obj.GetType().GetProperty(key);
            if (property != null)
            {
                property.SetValue(obj, value);
            }
        }

        public static void SetProperty<T>(object obj, Dictionary<string, T> dict)
        {
            foreach (KeyValuePair<string, T> kvp in dict)
            {
                SetProperty(obj, kvp.Key, kvp.Value);
            }
        }

        public static void GetProperty<T>(object obj, string key, out T? value)
        {
            if (obj.GetType().GetProperty(key) is PropertyInfo property)
            {
                if (property.GetValue(obj) is T val)
                {
                    value = val;
                    return;
                }
            }
            value = default(T);
        }

        public static T DeepCopy<T>(T obj)
        {
            return Helper.DeepCopy<T, T>.Trans(obj);
        }
        public static TOut DeepCopy<TIn, TOut>(TIn obj)
        {
            return Helper.DeepCopy<TIn, TOut>.Trans(obj);
        }

        public static string GetFileSize(string fullName)
        {
            long size = 0;
            if (File.Exists(fullName))
            {
                size = new FileInfo(fullName).Length;
            }
            string fileSize = FileSizeFormatter(size);
            return fileSize;
        }

        private static readonly (double Max, double Min, string Unit)[] fileSizeFormatter =
        {
            (Math.Pow(2.0, 10.0), Math.Pow(2.0, 0.0), "B"),
            (Math.Pow(2.0, 20.0), Math.Pow(2.0, 10.0), "KB"),
            (Math.Pow(2.0, 30.0), Math.Pow(2.0, 20.0), "MB"),
            (Math.Pow(2.0, 40.0), Math.Pow(2.0, 30.0), "GB"),
            (Math.Pow(2.0, 50.0), Math.Pow(2.0, 40.0), "TB"),
            (Math.Pow(2.0, 60.0), Math.Pow(2.0, 50.0), "PB"),
            (Math.Pow(2.0, 70.0), Math.Pow(2.0, 60.0), "EB"),
        };

        public static string FileSizeFormatter(long size)
        {
            foreach (var i in fileSizeFormatter)
            {
                if (size < i.Max)
                {
                    return $"{(size / i.Min):N2} {i.Unit}";
                }
            }
            return $"{(size / fileSizeFormatter[^1].Min):N2} EB";
        }
    }
}
