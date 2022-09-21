// Copyright (c) Kiyan Yang. All rights reserved.
// Licensed under the GNU General Public License v3.0.
// See LICENSE file in the project root for full license information.

namespace HashTool.Helpers;

/// <summary>
/// 获取类型 T 的单例
/// </summary>
/// <typeparam name="T">具有公共无参构造函数的类</typeparam>
internal static class SingletonHelper<T> where T : class, new()
{
    public static T Instance { get; } = new();
}

/// <summary>
/// 获取类型 T 的单例的快捷类
/// </summary>
internal static class SingletonHelper
{
    /// <summary>
    /// 获取类型 T 的单例的快捷方法
    /// </summary>
    /// <typeparam name="T">类型 T</typeparam>
    /// <returns>类型 T 的单例</returns>
    /// <example>
    /// 这显示了这个快捷方式的正确用法。
    /// <code>
    /// using static HashTool.Helpers.SingletonHelper;
    /// var Instance = GetInstance<T>();
    /// </code>
    /// </example>
    public static T GetInstance<T>() where T : class, new() => SingletonHelper<T>.Instance;
}
