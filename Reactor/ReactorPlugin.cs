global using static Reactor.Utilities.Logger<Reactor.ReactorPlugin>;
using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using QRCoder;
using Reactor.Localization;
using Reactor.Localization.Providers;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Patches;
using Reactor.Patches.Miscellaneous;
using Reactor.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using xCloud;

namespace Reactor;

/// <summary>
/// Reactor's main class.
/// </summary>
[BepInAutoPlugin("gg.reactor.api")]
[BepInProcess("Among Us.exe")]
public partial class ReactorPlugin : BaseUnityPlugin
{
    /// <summary>
    /// Gets harmony instance.
    /// </summary>
    public Harmony Harmony { get; } = new(Id);

    internal static ManualLogSource LogSource { get; private set; } = null!;

    /// <summary>
    /// Gets custom rpc manager.
    /// </summary>
    public CustomRpcManager CustomRpcManager { get; } = new();

    internal RegionInfoWatcher RegionInfoWatcher { get; } = new();
    internal static string BepInExVersion => typeof(Chainloader).Assembly.GetName().Version.ToString();

    /// <inheritdoc />
    private void Awake()
    {
        LogSource = Logger;
        Logger.LogMessage($"Among Us {Application.version} {Application.platform}");

        PluginSingleton<ReactorPlugin>.Instance = this;
        PluginSingleton<BaseUnityPlugin>.Initialize();

        ModList.Initialize();

        RegisterCustomRpcAttribute.Initialize();
        MessageConverterAttribute.Initialize();
        MethodRpcAttribute.Initialize();
        Harmony.PatchAll();
        PluginLoadHooks.Notify(this);
        LocalizationManager.Register(new HardCodedLocalizationProvider());
        ReactorConfig.Bind(Config);

        this.gameObject.AddComponent<ReactorComponent>().Plugin = this;
        this.gameObject.AddComponent<Coroutines.Component>();
        this.gameObject.AddComponent<Dispatcher>();

        ReactorVersionShower.Initialize();
        FreeNamePatch.Initialize();
        DefaultBundle.Load();

        SceneManager.sceneLoaded += (scene, _) =>
        {
            if (scene.name == "MainMenu")
            {
                ModManager.Instance.ShowModStamp();
            }
        };
    }

    private void Start()
    {
        Harmony.Unpatch(
            Harmony.GetPatchedMethods().First(method => method.DeclaringType == typeof(GameObject) && method.Name.Equals("AddComponent", StringComparison.Ordinal)),
            AccessTools.Method(typeof(PluginLoadHooks.AddComponentPatch), nameof(PluginLoadHooks.AddComponentPatch.AddComponentPostfix))
            );
    }

    internal void OnDestroy()
    {
        Harmony.UnpatchSelf();
        RegionInfoWatcher.Dispose();
    }

    private sealed class ReactorComponent : MonoBehaviour
    {
        public ReactorPlugin? Plugin { get; internal set; }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Plugin!.Logger.LogInfo("Reloading all configs");

                foreach (var pluginInfo in Chainloader.PluginInfos.Values)
                {
                    var config = pluginInfo.Instance.Config;
                    if (config.Count == 0)
                    {
                        continue;
                    }

                    try
                    {
                        config.Reload();
                    }
                    catch (Exception e)
                    {
                        Plugin!.Logger.LogWarning($"Exception occured during reload of {pluginInfo.Metadata.Name}: {e}");
                    }
                }
            }
        }
    }
}
