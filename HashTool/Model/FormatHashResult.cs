using System.Collections.Generic;

namespace HashTool.Model
{
    internal class FormatHashResult
    {
        public static Dictionary<string, string> BuildHashResult(Dictionary<string, string> hashResultDict)
        {
            string[] hash = { "MD5", "CRC32", "SHA1", "SHA256", "SHA384", "SHA512" };
            string? tmp;
            Dictionary<string, string> newDict = new();

            // 下面的顺序即为储存时的顺序
            if (hashResultDict.TryGetValue("InputMode", out tmp))
            {
                newDict.Add("模式", tmp);
            }
            if (hashResultDict.TryGetValue("Input", out tmp))
            {
                newDict.Add("输入", tmp);
            }
            if (hashResultDict.TryGetValue("FileSize", out tmp))
            {
                newDict.Add("文件大小", FileAndFolder.FileSizeFormatter(long.Parse(tmp)));
            }
            if (hashResultDict.TryGetValue("LastWriteTime", out tmp))
            {
                newDict.Add("文件修改时间", tmp);
            }
            // 添加其他属性
            foreach (string h in hash)
            {
                if (hashResultDict.TryGetValue(h, out tmp))
                {
                    newDict.Add(h, tmp);
                }
            }

            return newDict;
        }

        public static List<Dictionary<string, string>> BuildHashResult(List<Dictionary<string, string>> hashResultDict)
        {
            List<Dictionary<string, string>> hashResultList = new();
            foreach (Dictionary<string, string> item in hashResultDict)
            {
                hashResultList.Add(BuildHashResult(item));
            }
            return hashResultList;
        }

        public static List<Dictionary<string, string>> BuildHashResult(Dictionary<string, Dictionary<string, string>> hashResultDict)
        {
            List<Dictionary<string, string>> hashResultList = new();
            foreach (Dictionary<string, string> item in hashResultDict.Values)
            {
                hashResultList.Add(BuildHashResult(item));
            }
            return hashResultList;
        }

    }
}
