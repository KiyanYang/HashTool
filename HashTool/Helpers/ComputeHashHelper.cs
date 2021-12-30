using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using HashTool.Models;

namespace HashTool.Helpers
{
    public class ComputeHashHelper
    {
        private static readonly HashAlgorithm crc32 = new CRC("CRC-32", 32, 0x04C11DB7, 0xFFFFFFFF, 0xFFFFFFFF, true, true);
        private static readonly HashAlgorithm md5 = MD5.Create();
        private static readonly HashAlgorithm sha1 = SHA1.Create();
        private static readonly HashAlgorithm sha256 = SHA256.Create();
        private static readonly HashAlgorithm sha384 = SHA384.Create();
        private static readonly HashAlgorithm sha512 = SHA512.Create();
        private static Dictionary<string, HashAlgorithm> hashAlgorithmDict = new();

        private static string HashFormat(byte[] data)
        {
            StringBuilder sBuilder = new();

            foreach (byte b in data)
            {
                sBuilder.Append($"{b:X2}");
            }

            return sBuilder.ToString();
        }

        private static void SetHashAlgorithmDict(HashInputModel mainInput)
        {
            hashAlgorithmDict.Clear();

            if (mainInput.MD5 == true) hashAlgorithmDict.Add("MD5", md5);
            if (mainInput.CRC32 == true) hashAlgorithmDict.Add("CRC32", crc32);
            if (mainInput.SHA1 == true) hashAlgorithmDict.Add("SHA1", sha1);
            if (mainInput.SHA256 == true) hashAlgorithmDict.Add("SHA256", sha256);
            if (mainInput.SHA384 == true) hashAlgorithmDict.Add("SHA384", sha384);
            if (mainInput.SHA512 == true) hashAlgorithmDict.Add("SHA512", sha512);
        }

        public static HashResultModel HashString(HashInputModel hashInput)
        {
            return HashString(hashInput, null, null);
        }

        public static HashResultModel HashString(HashInputModel hashInput, BackgroundWorker? worker, double? maximum)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();

            SetHashAlgorithmDict(hashInput);
            HashResultModel hashResult = new();
            hashResult.InputMode = hashInput.Mode;
            hashResult.Mode = "文本";
            hashResult.Content = hashInput.Input;
            hashResult.ComputeTime = DateTime.Now.ToString("yyyy/M/d HH:mm:ss.fff");
            
            byte[] hashValue;
            foreach (string hashName in hashAlgorithmDict.Keys)
            {
                hashValue = hashAlgorithmDict[hashName].ComputeHash(Encoding.UTF8.GetBytes(hashInput.Input));
                hashResult.Items.Add(new HashResultItemModel(hashName, HashFormat(hashValue)));
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
            SetHashAlgorithmDict(mainInput);
            HashResultModel hashResult = new();
            hashResult.InputMode = mainInput.Mode;
            hashResult.Mode = "文件";
            hashResult.Content = fileInfo.FullName;
            hashResult.FileSize = CommonHelper.FileSizeFormatter(fileInfo.Length);
            hashResult.LastWriteTime = fileInfo.LastWriteTime.ToString();
            hashResult.ComputeTime = DateTime.Now.ToString("yyyy/M/d HH:mm:ss.fff");
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
            Action<HashAlgorithm> action = (hashAlgorithm) =>
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
                foreach (KeyValuePair<string, HashAlgorithm> kvp in hashAlgorithmDict)
                {
                    kvp.Value.TransformFinalBlock(buffer, 0, 0);
                    var hashValue = kvp.Value.Hash;
                    if (hashValue != null)
                    {
                        hashResult.Items.Add(new HashResultItemModel(kvp.Key, HashFormat(hashValue)));
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
            return HashStream(resetEvent, worker, e, new FileInfo(mainInput.Input), mainInput, maximum, 0);
        }

        public static List<HashResultModel> HashFolder(ManualResetEvent resetEvent, BackgroundWorker worker, DoWorkEventArgs e, HashInputModel mainInput, double maximum)
        {
            FileInfo[] fileInfos = new DirectoryInfo(mainInput.Input).GetFiles();
            List<HashResultModel> hashResults = new();
            for (int i = 0; i < fileInfos.Length; i++)
            {
                hashResults.Add(HashStream(resetEvent, worker, e, fileInfos[i], mainInput, maximum, i));
            }
            return hashResults;
        }
    }

    internal class CRC : HashAlgorithm
    {
        #region CRC 算法参数模型

        /// <summary>
        ///     算法名称。
        /// </summary>
        public readonly string Name;

        /// <summary>
        ///     宽度，即 CRC 的比特数。
        /// </summary>
        public readonly int Width;

        /// <summary>
        ///     用于 CRC 计算的多项式，忽略最高位。
        /// </summary>
        public readonly ulong Polynomial;

        /// <summary>
        ///     算法开始时寄存器的初始值。
        /// </summary>
        public readonly ulong Initial;

        /// <summary>
        ///     计算结果与此参数异或后得到最终的 CRC 值。
        /// </summary>
        public readonly ulong OutputXor;

        /// <summary>
        ///     输入是否反转。
        /// </summary>
        public readonly bool IsInputReflected;

        /// <summary>
        ///     输出是否反转。
        /// </summary>
        public readonly bool IsOutputReflected;

        #endregion

        #region 辅助参数

        /// <summary>
        ///     查表法所使用的计算表。
        /// </summary>
        private readonly ulong[] PrecomputationTable = new ulong[256];

        /// <summary>
        ///     用于隐藏 64 位工作寄存器中不需要的数据的掩码。
        /// </summary>
        private readonly ulong Mask;

        /// <summary>
        ///     输出反转处理时所使用的参数。
        /// </summary>
        private readonly int ToRight;

        /// <summary>
        ///     截至目前，处理的所有缓冲区的累积 CRC 值。
        /// </summary>
        private ulong Current;

        #endregion

        public CRC(string name, int width, ulong polynomial, ulong initial, ulong outputXor, bool isInputReflected, bool isOutputReflected)
        {
            if (width < 8 || width > 64)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Must be a multiple of 8 and between 8 and 64.");
            }

            Name = name;
            Width = width;
            Polynomial = polynomial;
            Initial = initial;
            IsInputReflected = isInputReflected;
            IsOutputReflected = isOutputReflected;
            OutputXor = outputXor;
            Mask = ulong.MaxValue >> (64 - width);

            CreateLookupTable();

            if (IsOutputReflected == false)
            {
                ToRight = Width < 8 ? 0 : Width - 8;
            }

            Initialize();
        }

        public override void Initialize()
        {
            Current = IsOutputReflected ? ReverseBits(Initial, Width) : Initial;
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (ibStart < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(ibStart));
            }

            if (cbSize < 0 || ibStart + cbSize > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(cbSize));
            }

            if (IsOutputReflected)
            {
                for (var i = ibStart; i < ibStart + cbSize; i++)
                {
                    Current = PrecomputationTable[(Current ^ array[i]) & 0xFF] ^ (Current >> 8);
                }
            }
            else
            {
                for (var i = ibStart; i < ibStart + cbSize; i++)
                {
                    Current = PrecomputationTable[((Current >> ToRight) ^ array[i]) & 0xFF] ^ (Current << 8);
                }
            }
        }

        protected override byte[] HashFinal()
        {
            var output = (Current ^ OutputXor) & Mask;

            var result = BitConverter.GetBytes(output);

            if (BitConverter.IsLittleEndian == false)
            {
                Array.Reverse(result);
            }

            Array.Resize(ref result, Width / 8);

            Array.Reverse(result);

            return result;
        }

        private void CreateLookupTable()
        {
            for (var i = 0; i < PrecomputationTable.Length; i++)
            {
                var r = (ulong)i;

                if (IsInputReflected)
                {
                    r = ReverseBits(r, Width);
                }
                else if (Width > 8)
                {
                    r <<= Width - 8;
                }

                var lastBit = 1UL << (Width - 1);

                for (var j = 0; j < 8; j++)
                {
                    if ((r & lastBit) != 0)
                    {
                        r = (r << 1) ^ Polynomial;
                    }
                    else
                    {
                        r <<= 1;
                    }
                }

                if (IsInputReflected)
                {
                    r = ReverseBits(r, Width);
                }

                PrecomputationTable[i] = r;
            }
        }

        private static ulong ReverseBits(ulong value, int valueLength)
        {
            ulong output = 0;

            for (var i = valueLength - 1; i >= 0; i--)
            {
                output |= (value & 1) << i;
                value >>= 1;
            }

            return output;
        }

    }
}
