using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Model
{
    public class HashResultModel : ObservableObject
    {
        // 输入模式
        private string inputMode = string.Empty;
        // 计算模式：文件或文本
        private string mode = string.Empty;
        // 计算内容：文件路径或文本
        private string content = string.Empty;
        // 以文件计算时
        private string fileSize = string.Empty;
        private string lastWriteTime = string.Empty;

        private string computeTime = string.Empty;
        private string computeCost = string.Empty;
        private string md5 = string.Empty;
        private string crc32 = string.Empty;
        private string sha1 = string.Empty;
        private string sha256 = string.Empty;
        private string sha384 = string.Empty;
        private string sha512 = string.Empty;

        public string InputMode
        {
            get => inputMode;
            set => SetProperty(ref inputMode, value);
        }
        public string Mode
        {
            get => mode;
            set => SetProperty(ref mode, value);
        }
        public string Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }
        public string FileSize
        {
            get => fileSize;
            set => SetProperty(ref fileSize, value);
        }
        public string LastWriteTime
        {
            get => lastWriteTime;
            set => SetProperty(ref lastWriteTime, value);
        }
        public string ComputeTime
        {
            get => computeTime;
            set => SetProperty(ref computeTime, value);
        }
        public string ComputeCost
        {
            get => computeCost;
            set => SetProperty(ref computeCost, value);
        }
        public string MD5
        {
            get => md5;
            set => SetProperty(ref md5, value);
        }
        public string CRC32
        {
            get => crc32;
            set => SetProperty(ref crc32, value);
        }
        public string SHA1
        {
            get => sha1;
            set => SetProperty(ref sha1, value);
        }
        public string SHA256
        {
            get => sha256;
            set => SetProperty(ref sha256, value);
        }
        public string SHA384
        {
            get => sha384;
            set => SetProperty(ref sha384, value);
        }
        public string SHA512
        {
            get => sha512;
            set => SetProperty(ref sha512, value);
        }
    }
}
