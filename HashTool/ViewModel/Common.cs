using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows.Data;

namespace HashTool.ViewModel
{
    internal class Common
    {
        public static void SetProperties<T>(object obj, Dictionary<string, T> dict)
        {
            foreach (KeyValuePair<string, T> kvp in dict)
            {
                var property = obj.GetType().GetProperty(kvp.Key);
                if (property != null)
                {
                    property.SetValue(obj, kvp.Value);
                }
            }
        }
        public static Binding GetBinding(string path)
        {
            var bd = new Binding(path);
            return bd;
        }
        public static Binding GetBinding(string path, string elementName, BindingMode bindingMode)
        {
            var bd = new Binding(path);
            bd.ElementName = elementName;
            bd.Mode = bindingMode;
            return bd;
        }
    }

    public class InputValue
    {
        public string inputMode { get; set; } = string.Empty;
        public string input { get; set; } = string.Empty;
        public Dictionary<string, bool?> isCheckedDict { get; set; } = new();
    }

    public static class HashResultAllHistory
    {
        public static List<Dictionary<string, string>> allHistory = new();
    }

    public class HashResultCore : INotifyPropertyChanged
    {
        // 是否为取消操作后的结果, 是 => "Yes", 否 => "No".
        private string isCanceled = string.Empty;
        private string inputMode = string.Empty;
        private string input = string.Empty;
        private string fileSize = string.Empty;
        private string lastWriteTime = string.Empty;
        private string md5 = string.Empty;
        private string crc32 = string.Empty;
        private string sha1 = string.Empty;
        private string sha256 = string.Empty;
        private string sha384 = string.Empty;
        private string sha512 = string.Empty;
        public event PropertyChangedEventHandler? PropertyChanged;

        [JsonIgnore]
        public string IsCanceled
        {
            get { return isCanceled; }
            set { UpdateProper(ref isCanceled, value); }
        }

        [JsonPropertyOrder(-3)]
        public string InputMode
        {
            get { return inputMode; }
            set { UpdateProper(ref inputMode, value); }
        }

        [JsonPropertyOrder(-2)]
        public string Input
        {
            get { return input; }
            set { UpdateProper(ref input, value); }
        }

        public string FileSize
        {
            get { return fileSize; }
            set { UpdateProper(ref fileSize, value); }
        }

        [JsonPropertyOrder(-1)]
        public string LastWriteTime
        {
            get { return lastWriteTime; }
            set { UpdateProper(ref lastWriteTime, value); }
        }

        public string MD5
        {
            get { return md5; }
            set { UpdateProper(ref md5, value); }
        }

        public string CRC32
        {
            get { return crc32; }
            set { UpdateProper(ref crc32, value); }
        }

        public string SHA1
        {
            get { return sha1; }
            set { UpdateProper(ref sha1, value); }
        }

        public string SHA256
        {
            get { return sha256; }
            set { UpdateProper(ref sha256, value); }
        }

        public string SHA384
        {
            get { return sha384; }
            set { UpdateProper(ref sha384, value); }
        }
        
        public string SHA512
        {
            get { return sha512; }
            set { UpdateProper(ref sha512, value); }
        }

        protected void UpdateProper<T>(ref T properValue, T newValue, [CallerMemberName] string properName = "")
        {
            if (Equals(properValue, newValue))
                return;

            properValue = newValue;
            OnPropertyChanged(properName);
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class HashResultFolderInfo : INotifyPropertyChanged
    {
        private string folderPath = string.Empty;
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
        public string FolderPath
        {
            get { return folderPath; }
            set { UpdateProper(ref folderPath, value); }
        }

        protected void UpdateProper<T>(ref T properValue, T newValue, [CallerMemberName] string properName = "")
        {
            if (Equals(properValue, newValue))
                return;

            properValue = newValue;
            OnPropertyChanged(properName);
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class HashResultFileName : INotifyPropertyChanged
    {
        private string fileName = string.Empty;
        private string fullName = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string FileName
        {
            get { return fileName; }
            set { UpdateProper(ref fileName, value); }
        }

        public string FullName
        {
            get { return fullName; }
            set { UpdateProper(ref fullName, value); }
        }

        protected void UpdateProper<T>(ref T properValue, T newValue, [CallerMemberName] string properName = "")
        {
            if (Equals(properValue, newValue))
                return;

            properValue = newValue;
            OnPropertyChanged(properName);
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
