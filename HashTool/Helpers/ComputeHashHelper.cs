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
using HashTool.Models.Enums;

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
        private static readonly HashAlgorithm quickXor = new QuickXor();
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

    /// <summary>
    /// QuickXorHash 算法实现。
    /// </summary>
    /// <remarks>
    /// 来自【MicroSoft.Docs】<see href="https://docs.microsoft.com/en-us/onedrive/developer/code-snippets/quickxorhash">Code Snippets: QuickXorHash Algorithm</see>，有删改。
    /// </remarks>
    internal class QuickXor : HashAlgorithm
    {
        private const int BitsInLastCell = 32;
        private const byte Shift = 11;
        private const byte WidthInBits = 160;

        private ulong[] _data = new ulong[0];
        private long _lengthSoFar;
        private int _shiftSoFar;

        public QuickXor()
        {
            this.Initialize();
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            unchecked
            {
                int currentShift = this._shiftSoFar;

                // The bitvector where we'll start xoring
                int vectorArrayIndex = currentShift / 64;

                // The position within the bit vector at which we begin xoring
                int vectorOffset = currentShift % 64;
                int iterations = Math.Min(cbSize, WidthInBits);

                for (int i = 0; i < iterations; i++)
                {
                    bool isLastCell = vectorArrayIndex == this._data.Length - 1;
                    int bitsInVectorCell = isLastCell ? BitsInLastCell : 64;

                    // There's at least 2 bitvectors before we reach the end of the array
                    if (vectorOffset <= bitsInVectorCell - 8)
                    {
                        for (int j = ibStart + i; j < cbSize + ibStart; j += WidthInBits)
                        {
                            this._data[vectorArrayIndex] ^= (ulong)array[j] << vectorOffset;
                        }
                    }
                    else
                    {
                        int index1 = vectorArrayIndex;
                        int index2 = isLastCell ? 0 : vectorArrayIndex + 1;
                        byte low = (byte)(bitsInVectorCell - vectorOffset);

                        byte xoredByte = 0;
                        for (int j = ibStart + i; j < cbSize + ibStart; j += WidthInBits)
                        {
                            xoredByte ^= array[j];
                        }
                        this._data[index1] ^= (ulong)xoredByte << vectorOffset;
                        this._data[index2] ^= (ulong)xoredByte >> low;
                    }
                    vectorOffset += Shift;
                    while (vectorOffset >= bitsInVectorCell)
                    {
                        vectorArrayIndex = isLastCell ? 0 : vectorArrayIndex + 1;
                        vectorOffset -= bitsInVectorCell;
                    }
                }

                // Update the starting position in a circular shift pattern
                this._shiftSoFar = (this._shiftSoFar + Shift * (cbSize % WidthInBits)) % WidthInBits;
            }

            this._lengthSoFar += cbSize;
        }

        protected override byte[] HashFinal()
        {
            // Create a byte array big enough to hold all our data
            byte[] rgb = new byte[(WidthInBits - 1) / 8 + 1];

            // Block copy all our bitvectors to this byte array
            for (int i = 0; i < this._data.Length - 1; i++)
            {
                Buffer.BlockCopy(
                    BitConverter.GetBytes(this._data[i]), 0,
                    rgb, i * 8,
                    8);
            }

            Buffer.BlockCopy(
                BitConverter.GetBytes(this._data[this._data.Length - 1]), 0,
                rgb, (this._data.Length - 1) * 8,
                rgb.Length - (this._data.Length - 1) * 8);

            // XOR the file length with the least significant bits
            // Note that GetBytes is architecture-dependent, so care should
            // be taken with porting. The expected value is 8-bytes in length in little-endian format
            var lengthBytes = BitConverter.GetBytes(this._lengthSoFar);
            Debug.Assert(lengthBytes.Length == 8);
            for (int i = 0; i < lengthBytes.Length; i++)
            {
                rgb[(WidthInBits / 8) - lengthBytes.Length + i] ^= lengthBytes[i];
            }

            return rgb;
        }

        public override sealed void Initialize()
        {
            this._data = new ulong[(WidthInBits - 1) / 64 + 1];
            this._shiftSoFar = 0;
            this._lengthSoFar = 0;
        }

        public override int HashSize
        {
            get => WidthInBits;
        }
    }
}
