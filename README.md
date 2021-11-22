# HashTool

Hash 校验工具

# 依赖环境

- `.NET 6` [点击下载](https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-desktop-6.0.0-windows-x64-installer)

# 界面介绍

![主界面-文件](https://cdn.jsdelivr.net/gh/KiyanYang/HashTool@main/HashTool/Resource/MainWindowFile.png)

- **模式**：在左上角的下拉框中选择模式，一共有“文件”、“文件夹”、“文本”三种模式。其中“文件夹”模式下只会计算此文件夹所含的文件，不会计算其子文件夹内的文件。
- **输入**：在上面的输入框输入“文件路径”、“文件夹路径”或“文本”，并选择相应模式进行计算。可以拖放“文件”或“文件夹”到此自动获取路径，也可以点击右上角“选择文件”按钮自动获取“文件”路径。
- **算法**：提供了 6 种算法，按需自动选择。默认为“MD5”和“SHA256”。
- **保存**：保存结果有 3 种格式：`yaml`、`json`、`txt`。
- **对比**。在最下方的两个文本框输入哈希值自动对比。
- **进度条**：在“文件”和“文本”模式下只有主进度条，“文件夹”模式下还有一个副进度条用来显示文件进度，如下图所示。
  ![主界面-文件夹](https://cdn.jsdelivr.net/gh/KiyanYang/HashTool@main/HashTool/Resource/MainWindowFolder.png)

# 使用

在 [`Releases`](https://github.com/KiyanYang/HashTool/releases) 下载 `HashTool.zip` 解压后点击 `HashTool.exe` 运行。

# 感谢

- [HandyControl](https://github.com/HandyOrg/HandyControl)
- [Visual Studio 2022](https://visualstudio.microsoft.com/zh-hans/vs/)
- [.NET 6](https://docs.microsoft.com/zh-cn/dotnet/api/?view=net-6.0)
- MyHash

# 开发者的话

本软件是练手作，在没有系统学习过 C# 相关知识下，从[教程：使用 C# 创建简单应用](https://docs.microsoft.com/zh-cn/visualstudio/get-started/csharp/tutorial-wpf?view=vs-2022)起步，依靠 `HandyControl` [中文文档](https://handyorg.github.io/handycontrol/)和[英文文档](https://hosseini.ninja/handycontrol/)、`.NET 文档`等，在百度、必应、谷歌的帮助下大约 2 周完成，所有没有多线程计算也没有其他骚操作，不过我在开发时参考另一款哈希计算软件 `MyHash` 的功能和性能，对于大文件（大于 500MB）在选择 3 种哈希算法的情况下计算速度相当（`Intel i5-8250U`），但是在选择更多的算法下本软件速度相对较低。

记录一下这次查资料最烦的一件事，由于本软件采用 `.NET 6` 没有 `System.Windows.Forms` ，但是在查资料的时候好多都用了这个，最后发现这里面的内容在 `.NET 6` 里一部分换位置并进行一些修改，还有的我根本就没找到，这还不说了，最神奇的是在微软的 `.NET 6` 文档里有 [`System.Windows.Forms`](https://docs.microsoft.com/zh-cn/dotnet/api/system.windows.Forms?view=net-6.0) 的文档，这就非常离谱，把给我整不会了。
