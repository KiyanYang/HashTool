// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System.Windows.Controls;

using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace HashTool.Models
{
    internal class PageModel : ObservableObject
    {
        private string name = string.Empty;
        private Page? page;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public Page? Page
        {
            get => page;
            set => SetProperty(ref page, value);
        }
    }
}
