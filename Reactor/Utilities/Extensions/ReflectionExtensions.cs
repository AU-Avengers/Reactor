using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MethodInfo = System.Reflection.MethodInfo;

namespace Reactor.Utilities.Extensions;

/// <summary>
/// Provides extension methods for reflection.
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Gets MoveNext <see cref="MethodInfo"/> for specified enumerator method.
    /// </summary>
    /// <param name="type">The enclosing type of the method.</param>
    /// <param name="methodName">The name of the method.</param>
    /// <returns>A <see cref="MethodInfo"/>.</returns>
    public static MethodInfo EnumeratorMoveNext(Type type, string methodName)
    {
        return AccessTools.EnumeratorMoveNext(AccessTools.Method(type, methodName));
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
