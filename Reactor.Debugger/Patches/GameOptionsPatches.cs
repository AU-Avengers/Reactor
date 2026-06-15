using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;
using HarmonyLib;
using UnityEngine;

namespace Reactor.Debugger.Patches;

[HarmonyPatch]
internal static class GameOptionsPatches
{
    public static void Initialize()
    {
        var maxImpostors = Enumerable.Repeat((int) byte.MaxValue, byte.MaxValue).ToArray();
        AccessTools.Field(typeof(NormalGameOptionsV09), nameof(NormalGameOptionsV09.MaxImpostors)).SetValue(null, maxImpostors);

        var minPlayers = Enumerable.Repeat(1, byte.MaxValue).ToArray();
        AccessTools.Field(typeof(NormalGameOptionsV09), nameof(NormalGameOptionsV09.MinPlayers)).SetValue(null, minPlayers);
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    [HarmonyPrefix]
    public static void UnlockAllOptions(GameSettingMenu __instance)
    {
        __instance.GameSettingsTab.HideForOnline = System.Array.Empty<Transform>();
    }

    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.SetUpFromData))]
    [HarmonyPostfix]
    public static void UnlockOptionRange(NumberOption __instance)
    {
        __instance.ValidRange.min = float.MinValue;
        __instance.ValidRange.max = float.MaxValue;
    }

    [HarmonyPatch(typeof(CreateOptionsPicker), nameof(CreateOptionsPicker.SetImpostorButtons))]
    public static class DisableImpostorCountReset
    {
        private static readonly MethodInfo _refreshMethod = typeof(CreateOptionsPicker).GetMethod("Refresh", BindingFlags.Public | BindingFlags.Instance)!;

        public static bool Prefix()
        {
            foreach (var stackFrame in new StackTrace().GetFrames())
            {
                if (_refreshMethod.Equals(stackFrame.GetMethod()))
                {
                    return false;
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(CrewVisualizer), nameof(CrewVisualizer.SetCrewSize))]
    [HarmonyPrefix]
    public static void SetCrewSizePatch(int numPlayers, ref int numImpostors)
    {
        if (numImpostors >= numPlayers)
        {
            numImpostors = 0;
        }
    }
}
