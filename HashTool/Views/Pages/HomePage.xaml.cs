// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using System.Windows.Controls;

using HashTool.Models.Controls;
using HashTool.ViewModels;

namespace HashTool.Views.Pages;

/// <summary>
/// HomePage.xaml 的交互逻辑
/// </summary>
public sealed partial class HomePage : Page
{
    public HomePage()
    {
        InitializeComponent();

        _homeViewModel = new();
        DataContext = _homeViewModel;
    }

    private readonly HomePageViewModel _homeViewModel;

    public ProgressBarModel TaskbarProgress
    {
        get => _homeViewModel.TaskbarProgress;
    }

    private void TextBoxInput_PreviewDragOver(object sender, DragEventArgs e)  //不能使用PreviewDragEnter, 否则在TextBox内无法捕获数据
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;  //不清楚此语句是如何工作的, 但是如果没有则不会调用Drop, 可能对拖放事件级别和高级别的输入事件争用处理
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private void TextBoxInput_PreviewDrop(object sender, DragEventArgs e)
    {
        try
        {
            if (e.Data.GetData(DataFormats.FileDrop) is string[] paths)
            {
                _homeViewModel.HashInput.Input = paths[0];
            }
        }
        catch (Exception ex)
        {
            HandyControl.Controls.MessageBox.Show(ex.Message);
        }
    }
}
