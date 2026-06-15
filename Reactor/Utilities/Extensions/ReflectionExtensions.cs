using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityInterop.Common;
using UnityInterop.Common.Attributes;
using UnityInterop.Runtime;
using UnitySystem.Runtime.CompilerServices;
using UnityCustomAttributeExtensions = UnitySystem.Reflection.CustomAttributeExtensions;
using UnityMethodInfo = UnitySystem.Reflection.MethodInfo;
using UnitySystemType = UnitySystem.Type;
using MethodInfo = System.Reflection.MethodInfo;

namespace Reactor.Utilities.Extensions;

/// <summary>
/// Provides extension methods for reflection.
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Gets a <see cref="UnityMethodInfo"/> for the specified <see cref="MethodInfo"/>.
    /// </summary>
    /// <param name="methodInfo">The <see cref="MethodInfo"/>.</param>
    /// <returns>A <see cref="UnityMethodInfo"/>.</returns>
    public static UnityMethodInfo ToUnityMethodInfo(this MethodInfo methodInfo)
    {
        var UnityMethodField = UnityInteropUtils.GetUnityMethodInfoPointerFieldForGeneratedMethod(methodInfo);
        if (UnityMethodField == null) throw new ArgumentException($"'{methodInfo.Name}' is not an Unity method", nameof(methodInfo));
        var UnityMethod = (IntPtr) UnityMethodField.GetValue(null)!;

        return new UnityMethodInfo(Unity.Unity_method_get_object(UnityMethod, IntPtr.Zero));
    }

    /// <summary>
    /// Gets enumerator's MoveNext type for the specified <see cref="UnityMethodInfo"/>.
    /// </summary>
    /// <param name="methodInfo">The enumerator <see cref="UnityMethodInfo"/>.</param>
    /// <returns>A <see cref="UnitySystemType"/> for enumerator's MoveNext type.</returns>
    public static UnitySystemType GetEnumeratorMoveNextType(this UnityMethodInfo methodInfo)
    {
        var customAttribute = UnityCustomAttributeExtensions.GetCustomAttribute(methodInfo, UnityType.Of<IteratorStateMachineAttribute>()).TryCast<IteratorStateMachineAttribute>();
        if (customAttribute == null) throw new ArgumentException($"'{methodInfo.Name}' is not an enumerator method", nameof(methodInfo));

        return customAttribute._StateMachineType_k__BackingField;
    }

    /// <summary>
    /// Gets a <see cref="Type"/> for the specified <see cref="UnitySystemType"/>.
    /// </summary>
    /// <param name="type">The <see cref="UnitySystemType"/>.</param>
    /// <returns>A <see cref="Type"/>.</returns>
    public static Type ToSystemType(this UnitySystemType type)
    {
        var result = Type.GetType(type.AssemblyQualifiedName);
        if (result != null) return result;

        foreach (var t in AccessTools.GetTypesFromAssembly(Assembly.Load(type.Assembly.FullName)))
        {
            if (t.Namespace != type.Namespace) continue;

            var name = t.GetCustomAttribute<ObfuscatedNameAttribute>()?.ObfuscatedName?.Replace('/', '+') ?? t.FullName;
            if (name != type.FullName) continue;

            return t;
        }

        throw new TypeLoadException("Failed to find a system type for " + type.AssemblyQualifiedName);
    }

    /// <summary>
    /// Gets MoveNext <see cref="MethodInfo"/> for specified enumerator method.
    /// </summary>
    /// <param name="type">The enclosing type of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <returns>A <see cref="MethodInfo"/>.</returns>
    public static MethodInfo EnumeratorMoveNext(Type type, string methodName)
    {
        return AccessTools.Method(AccessTools.Method(type, methodName).ToUnityMethodInfo().GetEnumeratorMoveNextType().ToSystemType(), "MoveNext");
    }

    /// <summary>
    /// Gets all method from the <paramref name="type"/> with the specified <paramref name="bindingFlags"/>, <paramref name="returnType"/> and <paramref name="parameterTypes"/>.
    /// </summary>
    /// <param name="type">The type to search the methods in.</param>
    /// <param name="bindingFlags">The <see cref="BindingFlags"/>.</param>
    /// <param name="returnType">The return type.</param>
    /// <param name="parameterTypes">The parameter types.</param>
    /// <returns>An array of <see cref="MethodInfo"/> objects representing all methods defined for the current <see cref="Type"/> that match the specified binding constraints.</returns>
    public static IEnumerable<MethodBase> GetMethods(this Type type, BindingFlags bindingFlags, Type returnType, params Type[] parameterTypes)
    {
        return type.GetMethods(bindingFlags).Where(x => x.ReturnType == returnType && x.GetParameters().Select(x => x.ParameterType).SequenceEqual(parameterTypes));
    }

    /// <summary>
    /// Gets all method from the <paramref name="type"/> with the specified <paramref name="returnType"/> and <paramref name="parameterTypes"/>.
    /// </summary>
    /// <param name="type">The type to search the methods in.</param>
    /// <param name="returnType">The return type.</param>
    /// <param name="parameterTypes">The parameter types.</param>
    /// <returns>An array of <see cref="MethodInfo"/> objects representing all methods defined for the current <see cref="Type"/> that match the specified binding constraints.</returns>
    public static IEnumerable<MethodBase> GetMethods(this Type type, Type returnType, params Type[] parameterTypes)
    {
        return type.GetMethods(AccessTools.all, returnType, parameterTypes);
    }
}
