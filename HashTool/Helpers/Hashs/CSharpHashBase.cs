// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Security.Cryptography;

using SharpHash.Interfaces;

namespace HashTool.Helpers.Hashs
{
    internal class CSharpHashBase : HashAlgorithm
    {
        private readonly IHash algorithm;

        public CSharpHashBase(IHash algorithm)
        {
            this.algorithm = algorithm;
        }
        public override void Initialize() =>
            algorithm.Initialize();
        protected override void HashCore(byte[] array, int ibStart, int cbSize) =>
            algorithm.TransformBytes(array, ibStart, cbSize);
        protected override byte[] HashFinal() =>
            algorithm.TransformFinal().GetBytes();
    }
}
