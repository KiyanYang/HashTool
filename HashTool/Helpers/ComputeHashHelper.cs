// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using HashTool.Helpers.Hashs;
using HashTool.Models;
using HashTool.Models.Controls;

namespace HashTool.Helpers;

public sealed class ComputeHashHelper
{
    private static readonly List<Hash> s_hashs = new();

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
        s_hashs.Clear();
        Hash hash;
        foreach (CheckBoxModel i in hashInput.CheckBoxItems)
        {
            if (i.IsChecked != true)
                continue;

            hash = Hash.GetHashs().Where(hash => hash.Name == i.Content).FirstOrDefault() ??
                throw new ArgumentOutOfRangeException(nameof(hashInput), "The input.CheckBoxItems'EnumContent out of range.");
            s_hashs.Add(hash);
        }
    }

    private static HashResultItemModel BuildHashResultItem(string name, byte[] hashValue)
    {
        string hash = name switch
        {
            _ when name == Hash.QuickXor.Name => HashFormatBase64(hashValue),
            _ => HashFormatHex(hashValue),
        };
        return new HashResultItemModel() { Name = name, Value = hash };
    }

    public static HashResultModel HashString(HashInputModel hashInput) =>
        HashString(hashInput, null, null);

    public static HashResultModel HashString(HashInputModel hashInput, BackgroundWorker? worker, double? maximum)
    {
        SetHashAlgorithmDict(hashInput);
        Stopwatch stopWatch = Stopwatch.StartNew();

        HashResultModel hashResult = new()
        {
            InputMode = hashInput.Mode,
            Mode = "字符串",
            Content = hashInput.Input,
            EncodingName = hashInput.EncodingName,
            ComputeTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")
        };

        byte[] hashValue;
        var encoding = Encoding.GetEncoding(hashInput.EncodingName);
        foreach (Hash hash in s_hashs)
        {
            hashValue = hash.Algorithm.ComputeHash(encoding.GetBytes(hashInput.Input));
            hashResult.Items.Add(BuildHashResultItem(hash.Name, hashValue));
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
        HashResultModel hashResult = new()
        {
            InputMode = mainInput.Mode,
            Mode = "文件流",
            Content = fileInfo.FullName,
            FileSize = FileSizeFormatHelper.Format(fileInfo.Length),
            LastWriteTime = fileInfo.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss.fff"),
            ComputeTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff")
        };
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

        using Barrier barrier = new(s_hashs.Count, (b) =>
        {
            readLength = fileStream.Read(buffer, 0, bufferSize);

            mres.Wait();

            // 设置进度条为 maximum 份, 并且对多文件流设置偏移量
            worker.ReportProgress((int)(maximum * ((double)fileStream.Position / fileStream.Length + offset)));
        });

        // 定义本地函数，先屏障同步并完成读取文件、报告进度等操作，再并行计算
        void action(Hash hash)
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
                hash.Algorithm.TransformBlock(buffer, 0, readLength, null, 0);
            }
        }

        // 开启并行动作
        Parallel.ForEach(s_hashs, action);

        #endregion

        // 判断是否进行最后一次哈希计算
        if (!worker.CancellationPending)
        {
            foreach (Hash hash in s_hashs)
            {
                hash.Algorithm.TransformFinalBlock(buffer, 0, 0);
                if (hash.Algorithm.Hash is byte[] hashValue)
                {
                    HashResultItemModel s = BuildHashResultItem(hash.Name, hashValue);
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
