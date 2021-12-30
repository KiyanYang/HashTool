using System.Collections.Generic;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    public class HashInputModel : ObservableObject
    {
        private string mode = string.Empty;
        private string input = string.Empty;
        private bool? md5 = true;
        private bool? crc32 = false;
        private bool? sha1 = false;
        private bool? sha256 = true;
        private bool? sha384 = false;
        private bool? sha512 = false;

        public static IReadOnlyList<string> ModeItem { get; } = new[]
        {
            "文件",
            "文件夹",
            "文本",
        };
        public string Mode
        {
            get
            {
                if (mode == string.Empty)
                    mode = ModeItem[0];
                return mode;
            }
            set => SetProperty(ref mode, value);
        }
        public string Input
        {
            get => input;
            set => SetProperty(ref input, value);
        }
        public bool? MD5
        {
            get => md5;
            set => SetProperty(ref md5, value);
        }
        public bool? CRC32
        {
            get => crc32;
            set => SetProperty(ref crc32, value);
        }
        public bool? SHA1
        {
            get => sha1;
            set => SetProperty(ref sha1, value);
        }
        public bool? SHA256
        {
            get => sha256;
            set => SetProperty(ref sha256, value);
        }
        public bool? SHA384
        {
            get => sha384;
            set => SetProperty(ref sha384, value);
        }
        public bool? SHA512
        {
            get => sha512;
            set => SetProperty(ref sha512, value);
        }
    }
}
