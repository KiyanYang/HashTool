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

using HashTool.Helpers.Hashs;
using HashTool.Models;
using HashTool.Models.Enums;

namespace HashTool.Helpers
{
    public class ComputeHashHelper
    {
        private static readonly HashAlgorithm crc32 = CRC.CreateCRC32();
        private static readonly HashAlgorithm md5 = MD5.Create();
        private static readonly HashAlgorithm sha1 = SHA1.Create();
        private static readonly HashAlgorithm sha256 = SHA256.Create();
        private static readonly HashAlgorithm sha384 = SHA384.Create();
        private static readonly HashAlgorithm sha512 = SHA512.Create();
        private static readonly HashAlgorithm quickXor = new QuickXorHash();
        private static readonly Dictionary<AlgorithmEnum, HashAlgorithm> hashAlgorithmDict = new();

        private static string HashFormatHex(byte[] data)
        {
            StringBuilder sBuilder = new();

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
            hashAlgorithmDict.Clear();
            HashAlgorithm algorithm;
            foreach (var i in hashInput.CheckBoxItems)
            {
                if (i.IsChecked != true)
                    continue;
                algorithm = i.EnumContent switch
                {
                    AlgorithmEnum.MD5 => md5,
                    AlgorithmEnum.CRC32 => crc32,
                    AlgorithmEnum.SHA1 => sha1,
                    AlgorithmEnum.SHA256 => sha256,
                    AlgorithmEnum.SHA384 => sha384,
                    AlgorithmEnum.SHA512 => sha512,
                    AlgorithmEnum.QuickXor => quickXor,
                    _ => throw new ArgumentOutOfRangeException(),
                };
                hashAlgorithmDict.Add(i.EnumContent, algorithm);
            }
        }

        private static HashResultItemModel BuildHashResultItem(AlgorithmEnum id, byte[] data)
        {
            string hash = id switch
            {
                AlgorithmEnum.QuickXor => HashFormatBase64(data),
                _ => HashFormatHex(data),
            };
            return new HashResultItemModel(id, hash);
        }

        public static HashResultModel HashString(HashInputModel hashInput) => HashString(hashInput, null, null);

        public static HashResultModel HashString(HashInputModel hashInput, BackgroundWorker? worker, double? maximum)
        {
            SetHashAlgorithmDict(hashInput);
            Stopwatch stopWatch = Stopwatch.StartNew();

            HashResultModel hashResult = new();
            hashResult.InputMode = hashInput.Mode;
            hashResult.Mode = "字符串_UTF-8";
            hashResult.Content = hashInput.Input;
            hashResult.ComputeTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");

            byte[] hashValue;
            foreach (var hashId in hashAlgorithmDict.Keys)
            {
                hashValue = hashAlgorithmDict[hashId].ComputeHash(Encoding.UTF8.GetBytes(hashInput.Input));
                hashResult.Items.Add(BuildHashResultItem(hashId, hashValue));
            }
            if (worker != null && maximum != null)
            {
                worker.ReportProgress((int)maximum);
            }

            stopWatch.Stop();
            hashResult.ComputeCost = $"{stopWatch.Elapsed.TotalSeconds:N3} 秒";
            return hashResult;
        }

        private static HashResultModel HashStream(ManualResetEvent resetEvent, BackgroundWorker worker, DoWorkEventArgs e, FileInfo fileInfo, HashInputModel mainInput, double maximum, int offset)
        {
            #region 初始化文件流，哈希算法实例字典

            using FileStream fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            fileStream.Position = 0;
            HashResultModel hashResult = new();
            hashResult.InputMode = mainInput.Mode;
            hashResult.Mode = "文件流";
            hashResult.Content = fileInfo.FullName;
            hashResult.FileSize = CommonHelper.FileSizeFormatter(fileInfo.Length);
            hashResult.LastWriteTime = fileInfo.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss");
            hashResult.ComputeTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
            Stopwatch stopWatch = Stopwatch.StartNew();

            #endregion

            #region 定义文件流读取参数及变量

            // 自定义缓冲区大小 1024 KB
            int bufferSize = 1024 * 1024;
            byte[] buffer = new byte[bufferSize];
            // 每次实际读取长度，此初值仅为启动作用，真正的赋值在屏障内完成
            int readLength = fileStream.Length > 0 ? bufferSize : 0;

            #endregion

            #region 使用屏障完成多算法的并行计算

            using Barrier barrier = new Barrier(hashAlgorithmDict.Count, (b) =>
            {
                readLength = fileStream.Read(buffer, 0, bufferSize);

                resetEvent.WaitOne();

                // 设置进度条为 maximum 份, 并且对多文件流设置偏移量
                worker.ReportProgress((int)(maximum * ((double)fileStream.Position / fileStream.Length + offset)));
            });
            // 定义动作，先屏障同步并完成读取文件、报告进度等操作，再并行计算
            Action<HashAlgorithm> action = hashAlgorithm =>
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
            };
            // 开启并行动作
            Parallel.ForEach(hashAlgorithmDict.Values, action);

            #endregion

            // 判断是否进行最后一次哈希计算
            if (!worker.CancellationPending)
            {
                foreach (var kvp in hashAlgorithmDict)
                {
                    kvp.Value.TransformFinalBlock(buffer, 0, 0);
                    var hashValue = kvp.Value.Hash;
                    if (hashValue != null)
                    {
                        var s = BuildHashResultItem(kvp.Key, hashValue);
                        hashResult.Items.Add(s);
                    }
                }
                stopWatch.Stop();
                hashResult.ComputeCost = $"{stopWatch.Elapsed.TotalSeconds:N3} 秒";
                worker.ReportProgress((int)(maximum * (1 + offset)));
            }

            return hashResult;
        }

        public static HashResultModel HashFile(ManualResetEvent resetEvent, BackgroundWorker worker, DoWorkEventArgs e, HashInputModel mainInput, double maximum)
        {
            SetHashAlgorithmDict(mainInput);
            return HashStream(resetEvent, worker, e, new FileInfo(mainInput.Input), mainInput, maximum, 0);
        }

        public static List<HashResultModel> HashFolder(ManualResetEvent resetEvent, BackgroundWorker worker, DoWorkEventArgs e, HashInputModel mainInput, double maximum)
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
                hashResults.Add(HashStream(resetEvent, worker, e, fileInfos[i], mainInput, maximum, i));
            }
            return hashResults;
        }
    }
}
