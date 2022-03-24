using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HashTool.Helpers
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
            return Helpers.DeepCopy<T, T>.Trans(obj);
        }
        public static TOut DeepCopy<TIn, TOut>(TIn obj)
        {
            return Helpers.DeepCopy<TIn, TOut>.Trans(obj);
        }

        public static long GetFileSize(string fullName)
        {
            long size = 0;
            if (File.Exists(fullName))
            {
                size = new FileInfo(fullName).Length;
            }
            return size;
        }

        private static readonly string[] DataCapacityUnit = new string[] { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

        public static string FileSizeFormatter(long size)
        {
            var index = (int)Math.Log(size, 1024.0);
            var newSize = size / Math.Pow(1024.0, index);
            return $"{newSize:F2} {DataCapacityUnit[index]}";
        }
    }
}
