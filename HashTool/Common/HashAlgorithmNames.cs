// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;

namespace HashTool.Common
{
    public class HashAlgorithmNames : IEnumerable<string>
    {
        // CRC
        public const string CRC32 = "CRC32";

        // MD
        public const string MD2 = "MD2";
        public const string MD4 = "MD4";
        public const string MD5 = "MD5";

        // SHA1
        public const string SHA1 = "SHA1";

        // SHA2
        public const string SHA224 = "SHA224";
        public const string SHA256 = "SHA256";
        public const string SHA384 = "SHA384";
        public const string SHA512 = "SHA512";

        // SHA3
        public const string SHA3_224 = "SHA3-224";
        public const string SHA3_256 = "SHA3-256";
        public const string SHA3_384 = "SHA3-384";
        public const string SHA3_512 = "SHA3-512";

        // Blake2B
        public const string Blake2B_160 = "Blake2B-160";
        public const string Blake2B_256 = "Blake2B-256";
        public const string Blake2B_384 = "Blake2B-384";
        public const string Blake2B_512 = "Blake2B-512";

        // Blake2S
        public const string Blake2S_128 = "Blake2S-128";
        public const string Blake2S_160 = "Blake2S-160";
        public const string Blake2S_224 = "Blake2S-224";
        public const string Blake2S_256 = "Blake2S-256";

        // Keccak
        public const string Keccak_224 = "Keccak-224";
        public const string Keccak_256 = "Keccak-256";
        public const string Keccak_288 = "Keccak-288";
        public const string Keccak_384 = "Keccak-384";
        public const string Keccak_512 = "Keccak-512";

        // QuickXor
        public const string QuickXor = "QuickXor";

        public IEnumerator<string> GetEnumerator()
        {
            yield return CRC32;
            // 禁用 MD2，由于 MD2 需要计算两遍，因此多线程计算开始时会出现卡死问题。
            //yield return MD2;
            yield return MD4;
            yield return MD5;
            yield return SHA1;
            yield return SHA224;
            yield return SHA256;
            yield return SHA384;
            yield return SHA512;
            yield return SHA3_224;
            yield return SHA3_256;
            yield return SHA3_384;
            yield return SHA3_512;
            yield return Blake2B_160;
            yield return Blake2B_256;
            yield return Blake2B_384;
            yield return Blake2B_512;
            yield return Blake2S_128;
            yield return Blake2S_160;
            yield return Blake2S_224;
            yield return Blake2S_256;
            yield return Keccak_224;
            yield return Keccak_256;
            yield return Keccak_288;
            yield return Keccak_384;
            yield return Keccak_512;
            yield return QuickXor;
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
