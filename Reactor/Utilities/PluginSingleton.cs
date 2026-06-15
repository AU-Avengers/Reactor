using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;

namespace Reactor.Utilities;

/// <summary>
/// Provides singleton access to plugins instance.
/// </summary>
/// <typeparam name="T">The type of the plugin.</typeparam>
public static class PluginSingleton<T> where T : BaseUnityPlugin
{
    private static T? _instance;

    /// <summary>
    /// Gets or sets an instance of <typeparamref name="T"/> plugin.
    /// </summary>
    public static T Instance
    {
        get => _instance ??= Chainloader.PluginInfos.Values.Select(x => x.Instance).OfType<T>().Single();
        set
        {
            if (_instance == value) return;
            if (_instance != null) throw new InvalidOperationException($"Instance for {typeof(T)} is already set");
            _instance = value;
        }
    }

    internal static void Initialize()
    {
        PluginLoadHooks.PluginLoaded += (_, plugin) =>
        {
            typeof(PluginSingleton<>).MakeGenericType(plugin.GetType())
                .GetField(nameof(_instance), BindingFlags.Static | BindingFlags.NonPublic)!
                .SetValue(null, plugin);
        };
    }
}
