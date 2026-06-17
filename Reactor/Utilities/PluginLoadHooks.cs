using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using UnityEngine;

namespace Reactor.Utilities;

internal static class PluginLoadHooks
{
    internal static event Action<PluginInfo, BaseUnityPlugin>? PluginLoaded;

    internal static void Notify(BaseUnityPlugin plugin) => PluginLoaded?.Invoke(plugin.Info, plugin);

    [HarmonyPatch]
    internal static class AddComponentPatch
    {
        [HarmonyPatch(typeof(GameObject), nameof(GameObject.AddComponent), typeof(Type))]
        [HarmonyPostfix]
        internal static void AddComponentPostfix(Component __result)
        {
            if (__result is BaseUnityPlugin plugin)
                Notify(plugin);
        }
    }
}
