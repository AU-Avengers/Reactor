global using static Reactor.Utilities.Logger<Reactor.Debugger.DebuggerPlugin>;
using BepInEx;
using BepInEx.Unity.Mono;
using HarmonyLib;
using Reactor.Debugger.AutoJoin;
using Reactor.Debugger.Patches;
using Reactor.Debugger.Window;

namespace Reactor.Debugger;

[BepInAutoPlugin("gg.reactor.debugger")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class DebuggerPlugin : BaseUnityPlugin
{
    public Harmony Harmony { get; } = new(Id);

    private void Awake()
    {
        DebuggerConfig.Bind(Config);

        gameObject.AddComponent<DebuggerWindow>();

        GameOptionsPatches.Initialize();

        Harmony.PatchAll();

        AutoJoinConnectionManager.StartOrConnect();
    }
}
