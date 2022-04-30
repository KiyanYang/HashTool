// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Security.Cryptography;

using SharpHash.Interfaces;

namespace HashTool.Helpers.Hashs
{
    internal class CSharpHashBase : HashAlgorithm
    {
        private readonly IHash _algorithm;

        public CSharpHashBase(IHash algorithm)
        {
            _algorithm = algorithm;
        }
        public override void Initialize() =>
            _algorithm.Initialize();
        protected override void HashCore(byte[] array, int ibStart, int cbSize) =>
            _algorithm.TransformBytes(array, ibStart, cbSize);
        protected override byte[] HashFinal() =>
            _algorithm.TransformFinal().GetBytes();
    }
}
