// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using static SharpHash.Base.HashFactory;

namespace HashTool.Helpers.Hashs
{
    internal static class CSharpHash
    {
        // MD
        public static CSharpHashBase CreateMD2() => new(Crypto.CreateMD2());
        public static CSharpHashBase CreateMD4() => new(Crypto.CreateMD4());

        // SHA2
        public static CSharpHashBase CreateSHA2_224() => new(Crypto.CreateSHA2_224());

        // SHA3
        public static CSharpHashBase CreateSHA3_224() => new(Crypto.CreateSHA3_224());
        public static CSharpHashBase CreateSHA3_256() => new(Crypto.CreateSHA3_256());
        public static CSharpHashBase CreateSHA3_384() => new(Crypto.CreateSHA3_384());
        public static CSharpHashBase CreateSHA3_512() => new(Crypto.CreateSHA3_512());

        // Blake2B
        public static CSharpHashBase CreateBlake2B_160() => new(Crypto.CreateBlake2B_160());
        public static CSharpHashBase CreateBlake2B_256() => new(Crypto.CreateBlake2B_256());
        public static CSharpHashBase CreateBlake2B_384() => new(Crypto.CreateBlake2B_384());
        public static CSharpHashBase CreateBlake2B_512() => new(Crypto.CreateBlake2B_512());

        // Blake2S
        public static CSharpHashBase CreateBlake2S_128() => new(Crypto.CreateBlake2S_128());
        public static CSharpHashBase CreateBlake2S_160() => new(Crypto.CreateBlake2S_160());
        public static CSharpHashBase CreateBlake2S_224() => new(Crypto.CreateBlake2S_224());
        public static CSharpHashBase CreateBlake2S_256() => new(Crypto.CreateBlake2S_256());

        // Keccak
        public static CSharpHashBase CreateKeccak_224() => new(Crypto.CreateKeccak_224());
        public static CSharpHashBase CreateKeccak_256() => new(Crypto.CreateKeccak_256());
        public static CSharpHashBase CreateKeccak_288() => new(Crypto.CreateKeccak_288());
        public static CSharpHashBase CreateKeccak_384() => new(Crypto.CreateKeccak_384());
        public static CSharpHashBase CreateKeccak_512() => new(Crypto.CreateKeccak_512());
    }
}
