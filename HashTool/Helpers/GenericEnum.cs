// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

namespace HashTool.Helpers;

public class GenericEnum<T>
{
    protected readonly T _value;

    protected GenericEnum(T value)
    {
        _value = value;
    }

    public override string ToString() => _value?.ToString() ?? string.Empty;
}
