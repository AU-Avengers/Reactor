using System;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;

namespace Reactor.Utilities;

internal static class PluginLoadHooks
{
    internal static event Action<PluginInfo, BaseUnityPlugin>? PluginLoaded;

    internal static void Notify(BaseUnityPlugin plugin)
    {
        PluginLoaded?.Invoke(plugin.Info, plugin);
    }

    [HarmonyPatch]
    private static class UnityChainloaderLoadPluginPatch
    {
        private static MethodBase TargetMethod()
        {
            return AccessTools.DeclaredMethod(
                typeof(Chainloader),
                "LoadPlugin",
                new[] { typeof(PluginInfo), typeof(Assembly) });
        }

        private static void Postfix(BaseUnityPlugin __result)
        {
            if (__result != null)
            {
                Notify(__result);
            }
        }
    }
}
