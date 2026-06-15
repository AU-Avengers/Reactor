using System.Diagnostics;
using BenchmarkDotNet.Running;
using BepInEx;
using BepInEx.Unity.Mono;

namespace Reactor.Benchmarks;

[BepInAutoPlugin("gg.reactor.benchmarks")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class BenchmarksPlugin : BaseUnityPlugin
{
    public override void Load()
    {
        try
        {
            BenchmarkRunner.Run<AssetBundleBenchmarks>();
        }
        finally
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
