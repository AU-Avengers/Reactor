using System.Diagnostics;
using BenchmarkDotNet.Running;
using BepInEx;

namespace Reactor.Benchmarks;

[BepInAutoPlugin("gg.reactor.benchmarks")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class BenchmarksPlugin : BaseUnityPlugin
{
    private void Awake()
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
