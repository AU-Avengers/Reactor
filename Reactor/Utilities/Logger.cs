using BepInEx.Logging;
using BepInEx.Unity.Mono;

namespace Reactor.Utilities;

/// <summary>
/// Provides singleton access to plugins logger.
/// </summary>
/// <typeparam name="T">The type of the plugin.</typeparam>
public static class Logger<T> where T : BaseUnityPlugin
{
    private static readonly ManualLogSource Fallback = BepInEx.Logging.Logger.CreateLogSource(typeof(T).FullName);

    /// <summary>
    /// Gets the instance.
    /// </summary>
    public static ManualLogSource Instance =>
        typeof(T) == typeof(ReactorPlugin) && ReactorPlugin.LogSource != null
            ? ReactorPlugin.LogSource
            : Fallback;

    /// <inheritdoc cref="ManualLogSource.Log(BepInEx.Logging.LogLevel,object)"/>
    public static void Log(LogLevel level, object data) => Instance.Log(level, data);

    /// <inheritdoc cref="ManualLogSource.LogFatal(object)"/>
    public static void Fatal(object data) => Instance.LogFatal(data);

    /// <inheritdoc cref="ManualLogSource.LogError(object)"/>
    public static void Error(object data) => Instance.LogError(data);

    /// <inheritdoc cref="ManualLogSource.LogWarning(object)"/>
    public static void Warning(object data) => Instance.LogWarning(data);

    /// <inheritdoc cref="ManualLogSource.LogMessage(object)"/>
    public static void Message(object data) => Instance.LogMessage(data);

    /// <inheritdoc cref="ManualLogSource.LogInfo(object)"/>
    public static void Info(object data) => Instance.LogInfo(data);

    /// <inheritdoc cref="ManualLogSource.LogDebug(object)"/>
    public static void Debug(object data) => Instance.LogDebug(data);
}
