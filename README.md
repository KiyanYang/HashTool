# HashTool

<div align=center>
 <img src="https://cdn.jsdelivr.net/gh/KiyanYang/HashTool@main/HashTool/Resource/HashTool.ico" width = "64" height = "64" alt="图标"/>
</div>

Hash 校验工具，提供 3 种模式（文件、文件夹、文本）和 6 种算法（`MD5`、`CRC32`、`SHA1`、`SHA256`、`SHA384`、`SHA512`）的哈希计算，对于文件采用并行计算（在选择多种算法时优势明显），同时也提供了对比框便于对比校验。

# 依赖环境

- `.NET 6` [点击下载](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-desktop-6.0.0-windows-x64-installer)

# 使用

在 [`Releases`](https://github.com/KiyanYang/HashTool/releases) 下载 `HashTool.zip` 或 `HashTool.7z` 解压后点击 `HashTool.exe` 运行。

# 界面介绍

![主界面-文件](https://cdn.jsdelivr.net/gh/KiyanYang/HashTool@main/Image/MainWindowFile.png)

- **模式**：在左上角的下拉框中选择模式，一共有 3 种模式（文件、文件夹、文本）。其中“文件夹”模式下只会计算此文件夹所含的文件，不会计算其子文件夹内的文件。
- **输入**：在上面的输入框输入“文件路径”、“文件夹路径”或“文本”，并选择相应模式进行计算。可以拖放“文件”或“文件夹”到此自动获取路径，也可以点击右上角“选择文件”按钮自动获取“文件”路径。
- **算法**：提供了 6 种算法（`MD5`、`CRC32`、`SHA1`、`SHA256`、`SHA384`、`SHA512`），可以按需选择。默认为“MD5”和“SHA256”。
- **保存**：保存结果有 3 种格式：`yaml`、`json`、`txt`。
- **对比**。在最下方的两个文本框输入哈希值自动对比。
- **进度条**：在“文件”和“文本”模式下只有主进度条，“文件夹”模式下还有一个副进度条用来显示文件进度，如下图所示。
  ![主界面-文件夹](https://cdn.jsdelivr.net/gh/KiyanYang/HashTool@main/Image/MainWindowFolder.png)

# 感谢

- [HandyControl](https://github.com/HandyOrg/HandyControl)
- [Visual Studio 2022](https://visualstudio.microsoft.com/zh-hans/vs/)
- [.NET 6](https://docs.microsoft.com/zh-cn/dotnet/api/?view=net-6.0)
- MyHash

# 开发者的话

本软件是练手作，在没有系统学习过 C# 相关知识下，从[教程：使用 C# 创建简单应用](https://docs.microsoft.com/zh-cn/visualstudio/get-started/csharp/tutorial-wpf?view=vs-2022)起步，依靠 `HandyControl` [中文文档](https://handyorg.github.io/handycontrol/)和[英文文档](https://hosseini.ninja/handycontrol/)、`.NET 文档`等，在百度、必应、谷歌的帮助下大约 2 周完成。

记录一下这次查资料较烦的一件事，由于本软件采用 `.NET 6` 没有 `System.Windows.Forms` ，但是在查资料的时候好多都用了这个，最后发现这里面的内容在 `.NET 6` 里一部分换位置并进行一些修改，还有的我根本就没找到，这还不说了，最神奇的是在微软的 `.NET 6` 文档里有 [`System.Windows.Forms`](https://docs.microsoft.com/zh-cn/dotnet/api/system.windows.Forms?view=net-6.0) 的文档，这就非常离谱，把给我整不会了。
