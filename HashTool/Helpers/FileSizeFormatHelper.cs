// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System;
using System.IO;

namespace HashTool.Helpers
{
    public class FileSizeFormatHelper
    {
        public static long GetFileSize(string path)
        {
            long size = 0;
            if (File.Exists(path))
            {
                size = new FileInfo(path).Length;
            }
            return size;
        }

        private static readonly string[] dataCapacityUnit = new[] { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

        public static string Format(long size)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), $"File size cannot be negative.");
            }
            if (size == 0)
            {
                return "0";
            }
            var index = (int)Math.Log(size, 1024.0);
            var newSize = size / Math.Pow(1024.0, index);
            return $"{newSize:F2} {dataCapacityUnit[index]}";
        }
    }
}
