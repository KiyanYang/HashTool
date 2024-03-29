// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System;

namespace HashTool.Helpers;

public sealed class FileSizeFormatHelper
{
    private static readonly string[] s_dataCapacityUnit = new[] { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

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
        int index = (int)Math.Log(size, 1024.0);
        double newSize = size / Math.Pow(1024.0, index);
        return $"{newSize:F2} {s_dataCapacityUnit[index]}";
    }
}
