// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

namespace HashTool.Helpers.Hashs
{
    internal class CRC
    {
        public static CRCAlgorithm CreateCRC32()
            => new("CRC-32", 32, 0x04C11DB7, 0xFFFFFFFF, true, true, 0xFFFFFFFF);
    }
}
