using System;

namespace HashTool.Models.Enums
{
    [Flags]
    public enum AlgorithmEnum
    {
        Null        = 0b_0000_0000,
        MD5         = 0b_0000_0001,
        CRC32       = 0b_0000_0010,
        SHA1        = 0b_0000_0100,
        SHA256      = 0b_0000_1000,
        SHA384      = 0b_0001_0000,
        SHA512      = 0b_0010_0000,
        QuickXor    = 0b_0100_0000,
    }
}
