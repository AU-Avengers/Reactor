using System;
using System.Collections.Generic;
using System.Reflection;
using BepInEx.Unity.Mono;
using HarmonyLib;
using UnityInterop.Runtime.Injection;

namespace Reactor.Utilities.Attributes;

/// <summary>
/// Automatically registers an Unity type using <see cref="ClassInjector.RegisterTypeInUnity{T}()"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RegisterInUnityAttribute : Attribute
{
    private static readonly HashSet<Assembly> _registeredAssemblies = new();

    /// <summary>
    /// Gets Unity interfaces to be injected with this type.
    /// </summary>
    public Type[] Interfaces { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterInUnityAttribute"/> class without any interfaces.
    /// </summary>
    public RegisterInUnityAttribute()
    {
        Interfaces = Type.EmptyTypes;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterInUnityAttribute"/> class with interfaces.
    /// </summary>
    /// <param name="interfaces">Unity interfaces to be injected with this type.</param>
    public RegisterInUnityAttribute(params Type[] interfaces)
    {
        Interfaces = interfaces;
    }

    private static void RegisterType(Type type, Type[] interfaces)
    {
        var baseTypeAttribute = type.BaseType?.GetCustomAttribute<RegisterInUnityAttribute>();
        if (baseTypeAttribute != null)
        {
            RegisterType(type.BaseType!, baseTypeAttribute.Interfaces);
        }

        if (ClassInjector.IsTypeRegisteredInUnity(type))
        {
            return;
        }

        try
        {
            ClassInjector.RegisterTypeInUnity(type, new RegisterTypeOptions { Interfaces = interfaces });
        }
        catch (Exception e)
        {
            Warning($"Failed to register {type.FullDescription()}: {e}");
        }
    }

    /// <summary>
    /// Registers all Unity types annotated with <see cref="RegisterInUnityAttribute"/> in the specified <paramref name="assembly"/>.
    /// </summary>
    /// <remarks>This is called automatically on plugin assemblies so you probably don't need to call this.</remarks>
    /// <param name="assembly">The assembly to search.</param>
    public static void Register(Assembly assembly)
    {
        if (_registeredAssemblies.Contains(assembly)) return;
        _registeredAssemblies.Add(assembly);

        foreach (var type in assembly.GetTypes())
        {
            var attribute = type.GetCustomAttribute<RegisterInUnityAttribute>();
            if (attribute != null)
            {
                RegisterType(type, attribute.Interfaces);
            }
        }
    }

    internal static void Initialize()
    {
        UnityChainloader.Instance.PluginLoad += (_, assembly, _) => Register(assembly);
    }
}
