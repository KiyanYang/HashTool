using System.IO;

namespace HashTool.Model
{
    internal class FileAndFolder
    {
        //得到文件大小
        public static string GetFileSize(string fullName)
        {
            long size = 0;
            if (File.Exists(fullName))
            {
                size = new FileInfo(fullName).Length;
            }
            string fileSize = FileSizeFormatter(size);
            return fileSize;
        }
        //格式化文件大小
        public static string FileSizeFormatter(long size)
        {
            if (size < 1024)
            {
                return $"{size} B";
            }
            if (size < 1048576)
            {
                return $"{size / 1024.0:0.00} KB";
            }
            if (size < 1073741824)
            {
                return $"{size / 1048576.0:0.00} MB";
            }
            if (size < 1099511627776)
            {
                return $"{size / 1073741824.0:0.00} GB";
            }
            if (size < 1125899906842624)
            {
                return $"{size / 1099511627776.0:0.00} TB";
            }
            if (size < 1152921504606847000)
            {
                return $"{size / 1125899906842624.0:0.00} PB";
            }
            return string.Empty;
        }
    }
}
