// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using HashTool.Common;
using HashTool.Helpers.Hashs;
using HashTool.Models;
using HashTool.Models.Controls;

namespace HashTool.Helpers
{
    public class ComputeHashHelper
    {
        private static readonly HashAlgorithm s_crc32 = CRC.CreateCRC32();
        private static readonly HashAlgorithm s_md2 = CSharpHash.CreateMD2();
        private static readonly HashAlgorithm s_md4 = CSharpHash.CreateMD4();
        private static readonly HashAlgorithm s_md5 = MD5.Create();
        private static readonly HashAlgorithm s_sha1 = SHA1.Create();
        private static readonly HashAlgorithm s_sha224 = CSharpHash.CreateSHA2_224();
        private static readonly HashAlgorithm s_sha256 = SHA256.Create();
        private static readonly HashAlgorithm s_sha384 = SHA384.Create();
        private static readonly HashAlgorithm s_sha512 = SHA512.Create();
        private static readonly HashAlgorithm s_sha3_224 = CSharpHash.CreateSHA3_224();
        private static readonly HashAlgorithm s_sha3_256 = CSharpHash.CreateSHA3_256();
        private static readonly HashAlgorithm s_sha3_384 = CSharpHash.CreateSHA3_384();
        private static readonly HashAlgorithm s_sha3_512 = CSharpHash.CreateSHA3_512();
        private static readonly HashAlgorithm s_blake2B_160 = CSharpHash.CreateBlake2B_160();
        private static readonly HashAlgorithm s_blake2B_256 = CSharpHash.CreateBlake2B_256();
        private static readonly HashAlgorithm s_blake2B_384 = CSharpHash.CreateBlake2B_384();
        private static readonly HashAlgorithm s_blake2B_512 = CSharpHash.CreateBlake2B_512();
        private static readonly HashAlgorithm s_blake2S_128 = CSharpHash.CreateBlake2S_128();
        private static readonly HashAlgorithm s_blake2S_160 = CSharpHash.CreateBlake2S_160();
        private static readonly HashAlgorithm s_blake2S_224 = CSharpHash.CreateBlake2S_224();
        private static readonly HashAlgorithm s_blake2S_256 = CSharpHash.CreateBlake2S_256();
        private static readonly HashAlgorithm s_keccak_224 = CSharpHash.CreateKeccak_224();
        private static readonly HashAlgorithm s_keccak_256 = CSharpHash.CreateKeccak_256();
        private static readonly HashAlgorithm s_keccak_288 = CSharpHash.CreateKeccak_288();
        private static readonly HashAlgorithm s_keccak_384 = CSharpHash.CreateKeccak_384();
        private static readonly HashAlgorithm s_keccak_512 = CSharpHash.CreateKeccak_512();
        private static readonly HashAlgorithm s_quickXor = new QuickXorHash();
        private static readonly Dictionary<string, HashAlgorithm> s_hashAlgorithmDict = new();

        /// <summary>
        /// 设置计算时需要使用的哈希算法字典。
        /// </summary>
        /// <remarks>
        /// 该方法应该对某一任务只进行一次调用，以避免在计算时由于界面参数的变化导致后续计算所使用的参数发生改变。
        /// </remarks>
        /// <param name="hashInput">哈希计算所需的参数</param>
        /// <exception cref="ArgumentOutOfRangeException">调用的哈希算法不在范围之内。</exception>
        private static void SetHashAlgorithmDict(HashInputModel hashInput)
        {
            s_hashAlgorithmDict.Clear();
            HashAlgorithm algorithm;
            foreach (CheckBoxModel i in hashInput.CheckBoxItems)
            {
                if (i.IsChecked != true)
                    continue;
                algorithm = i.Content switch
                {
                    HashAlgorithmNames.CRC32 => s_crc32,
                    HashAlgorithmNames.MD2 => s_md2,
                    HashAlgorithmNames.MD4 => s_md4,
                    HashAlgorithmNames.MD5 => s_md5,
                    HashAlgorithmNames.SHA1 => s_sha1,
                    HashAlgorithmNames.SHA224 => s_sha224,
                    HashAlgorithmNames.SHA256 => s_sha256,
                    HashAlgorithmNames.SHA384 => s_sha384,
                    HashAlgorithmNames.SHA512 => s_sha512,
                    HashAlgorithmNames.SHA3_224 => s_sha3_224,
                    HashAlgorithmNames.SHA3_256 => s_sha3_256,
                    HashAlgorithmNames.SHA3_384 => s_sha3_384,
                    HashAlgorithmNames.SHA3_512 => s_sha3_512,
                    HashAlgorithmNames.Blake2B_160 => s_blake2B_160,
                    HashAlgorithmNames.Blake2B_256 => s_blake2B_256,
                    HashAlgorithmNames.Blake2B_384 => s_blake2B_384,
                    HashAlgorithmNames.Blake2B_512 => s_blake2B_512,
                    HashAlgorithmNames.Blake2S_128 => s_blake2S_128,
                    HashAlgorithmNames.Blake2S_160 => s_blake2S_160,
                    HashAlgorithmNames.Blake2S_224 => s_blake2S_224,
                    HashAlgorithmNames.Blake2S_256 => s_blake2S_256,
                    HashAlgorithmNames.Keccak_224 => s_keccak_224,
                    HashAlgorithmNames.Keccak_256 => s_keccak_256,
                    HashAlgorithmNames.Keccak_288 => s_keccak_288,
                    HashAlgorithmNames.Keccak_384 => s_keccak_384,
                    HashAlgorithmNames.Keccak_512 => s_keccak_512,
                    HashAlgorithmNames.QuickXor => s_quickXor,
                    _ => throw new ArgumentOutOfRangeException(nameof(hashInput), "The input.CheckBoxItems'EnumContent out of range."),
                };
                s_hashAlgorithmDict.Add(i.Content, algorithm);
            }
        }

        private static HashResultItemModel BuildHashResultItem(string name, byte[] hashValue)
        {
            string hash = name switch
            {
                HashAlgorithmNames.QuickXor => HashFormatBase64(hashValue),
                _ => HashFormatHex(hashValue),
            };
            return new HashResultItemModel(name, hash);
        }

        public static HashResultModel HashString(HashInputModel hashInput) =>
            HashString(hashInput, null, null);

        public static HashResultModel HashString(HashInputModel hashInput, BackgroundWorker? worker, double? maximum)
        {
            SetHashAlgorithmDict(hashInput);
            Stopwatch stopWatch = Stopwatch.StartNew();

            HashResultModel hashResult = new();
            hashResult.InputMode = hashInput.Mode;
            hashResult.Mode = "字符串";
            hashResult.Content = hashInput.Input;
            hashResult.EncodingName = hashInput.EncodingName;
            hashResult.ComputeTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");

            byte[] hashValue;
            var encoding = Encoding.GetEncoding(hashInput.EncodingName);
            foreach (string name in s_hashAlgorithmDict.Keys)
            {
                hashValue = s_hashAlgorithmDict[name].ComputeHash(encoding.GetBytes(hashInput.Input));
                hashResult.Items.Add(BuildHashResultItem(name, hashValue));
            }
            if (worker != null && maximum != null)
            {
                worker.ReportProgress((int)maximum);
            }

            stopWatch.Stop();
            hashResult.ComputeCost = $"{stopWatch.Elapsed.TotalSeconds:N3} 秒";
            return hashResult;
        }

        private static HashResultModel HashStream(ManualResetEventSlim mres, BackgroundWorker worker, DoWorkEventArgs e, FileInfo fileInfo, HashInputModel mainInput, double maximum, int offset)
        {
            #region 初始化文件流，哈希算法实例字典

            using FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            fileStream.Position = 0;
            HashResultModel hashResult = new();
            hashResult.InputMode = mainInput.Mode;
            hashResult.Mode = "文件流";
            hashResult.Content = fileInfo.FullName;
            hashResult.FileSize = FileSizeFormatHelper.Format(fileInfo.Length);
            hashResult.LastWriteTime = fileInfo.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
            hashResult.ComputeTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            Stopwatch stopWatch = Stopwatch.StartNew();

            #endregion

            #region 定义文件流读取参数及变量

            // 自定义缓冲区大小，分为 4 档：文件大小，16 KB，1 MB，4 MB
            int bufferSize = fileInfo.Length switch
            {
                < 1024 * 32 => (int)fileInfo.Length,
                < 1024 * 1024 => 1024 * 16,
                < 1024 * 1024 * 256 => 1024 * 1024,
                _ => 1024 * 1024 * 4,
            };
            byte[] buffer = new byte[bufferSize];
            // 每次实际读取长度，此初值仅为启动作用，真正的赋值在屏障内完成
            int readLength = bufferSize;

            #endregion

            #region 使用屏障完成多算法的并行计算

            using Barrier barrier = new(s_hashAlgorithmDict.Count, (b) =>
            {
                readLength = fileStream.Read(buffer, 0, bufferSize);

                mres.Wait();

                // 设置进度条为 maximum 份, 并且对多文件流设置偏移量
                worker.ReportProgress((int)(maximum * ((double)fileStream.Position / fileStream.Length + offset)));
            });

            // 定义本地函数，先屏障同步并完成读取文件、报告进度等操作，再并行计算
            void action(HashAlgorithm hashAlgorithm)
            {
                while (readLength > 0)
                {
                    if (worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        barrier.RemoveParticipant();
                        break;
                    }
                    barrier.SignalAndWait();
                    hashAlgorithm.TransformBlock(buffer, 0, readLength, null, 0);
                }
            }

            // 开启并行动作
            Parallel.ForEach(s_hashAlgorithmDict.Values, action);

            #endregion

            // 判断是否进行最后一次哈希计算
            if (!worker.CancellationPending)
            {
                foreach (KeyValuePair<string, HashAlgorithm> kvp in s_hashAlgorithmDict)
                {
                    kvp.Value.TransformFinalBlock(buffer, 0, 0);
                    if (kvp.Value.Hash is byte[] hashValue)
                    {
                        HashResultItemModel s = BuildHashResultItem(kvp.Key, hashValue);
                        hashResult.Items.Add(s);
                    }
                }
                stopWatch.Stop();
                hashResult.ComputeCost = $"{stopWatch.Elapsed.TotalSeconds:N3} 秒";
                worker.ReportProgress((int)(maximum * (1 + offset)));
            }

            return hashResult;
        }

        public static HashResultModel HashFile(ManualResetEventSlim mres, BackgroundWorker worker, DoWorkEventArgs e, HashInputModel mainInput, double maximum)
        {
            SetHashAlgorithmDict(mainInput);
            return HashStream(mres, worker, e, new FileInfo(mainInput.Input), mainInput, maximum, 0);
        }

        public static List<HashResultModel> HashFolder(ManualResetEventSlim mres, BackgroundWorker worker, DoWorkEventArgs e, HashInputModel mainInput, double maximum)
        {
            SetHashAlgorithmDict(mainInput);
            FileInfo[] fileInfos = new DirectoryInfo(mainInput.Input).GetFiles();
            List<HashResultModel> hashResults = new();
            for (int i = 0; i < fileInfos.Length; i++)
            {
                // 判断任务是否取消，若取消则不继续进行后续文件的计算
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                hashResults.Add(HashStream(mres, worker, e, fileInfos[i], mainInput, maximum, i));
            }
            return hashResults;
        }

        #region 格式化哈希值 bytes -> string

        private static string HashFormatHex(byte[] data)
        {
            StringBuilder sBuilder = new(data.Length << 1);

            foreach (byte b in data)
            {
                sBuilder.Append($"{b:X2}");
            }

            return sBuilder.ToString();
        }
        private static string HashFormatBase64(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        #endregion
    }
}
