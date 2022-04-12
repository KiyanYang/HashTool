// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Windows;

using HashTool.Models;
using HashTool.ViewModels;

namespace HashTool.Views
{
    /// <summary>
    /// HashResultWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HashResultWindow : Window
    {
        public HashResultWindow(List<HashResultModel> hashResultModels)
        {
            InitializeComponent();

            HashResultViewModel vm = new(hashResultModels);
            DataContext = vm;
        }
    }
}
