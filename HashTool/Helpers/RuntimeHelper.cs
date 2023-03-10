// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace HashTool.Helpers;

internal class RuntimeHelper
{
    private static Version? s_appVersion;

    public static Version AppVersion => s_appVersion ??= GetAppVersion();

    private static Version GetAppVersion()
    {
        Assembly assem = Assembly.GetExecutingAssembly();
        AssemblyName assemName = assem.GetName();
        return assemName.Version ?? new Version(1, 0, 0, 0);
    }
}
