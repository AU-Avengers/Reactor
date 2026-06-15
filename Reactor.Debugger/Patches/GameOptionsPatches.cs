using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using UnityInterop.Runtime;
using UnitySystem.Reflection;
using UnityEngine;

namespace Reactor.Debugger.Patches;

[HarmonyPatch]
internal static class GameOptionsPatches
{
    public static void Initialize()
    {
        var maxImpostors = (UnityStructArray<int>) Enumerable.Repeat((int) byte.MaxValue, byte.MaxValue).ToArray();
        NormalGameOptionsV09.MaxImpostors = maxImpostors;
        NormalGameOptionsV09.MaxImpostors = maxImpostors;

        var minPlayers = (UnityStructArray<int>) Enumerable.Repeat(1, byte.MaxValue).ToArray();
        NormalGameOptionsV09.MinPlayers = minPlayers;
        NormalGameOptionsV09.MinPlayers = minPlayers;
    }

    [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.Start))]
    [HarmonyPrefix]
    public static void UnlockAllOptions(GameSettingMenu __instance)
    {
        __instance.GameSettingsTab.HideForOnline = new UnityReferenceArray<Transform>(0);
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
        private static readonly MethodInfo _refreshMethod = UnityType.Of<CreateOptionsPicker>().GetMethod("Refresh", BindingFlags.Public | BindingFlags.Instance);

        public static bool Prefix()
        {
            foreach (var stackFrame in new UnitySystem.Diagnostics.StackTrace().GetFrames())
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
