using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace Reactor.Utilities;

/// <summary>
/// A safe wrapper around IL2CPP compiler generated classes like DisplayClass or IEnumerator state machines.
/// This is useful for gracefully handling changes across different game versions and updates, which may affect
/// compiler generated names.
/// To use this class, pass the instance object of the compiler generated class into the constructor.
/// Then you can use the GetField and SetField methods to access the fields of the compiler generated class
/// by their original names.
/// </summary>
public class Il2CppCompilerGeneratedObjectWrapper
{
    /// <summary>
    /// Gets a reference to the compiler generated object.
    /// </summary>
    public object GeneratedObject { get; }

    /// <summary>
    /// Gets the type of the compiler generated object.
    /// </summary>
    protected Type GeneratedType { get; }

    /// <summary>
    /// Gets the property info cache for faster lookups.
    /// </summary>
    protected Dictionary<string, FieldInfo> FieldCache { get; }

    /// <summary>
    /// Gets the getter cache for faster property access.
    /// </summary>
    /// <summary>
    /// Initializes a new instance of the <see cref="Il2CppCompilerGeneratedObjectWrapper"/> class.
    /// </summary>
    /// <param name="generatedObject">An instance of the compiler generated object.</param>
    public Il2CppCompilerGeneratedObjectWrapper(object generatedObject)
    {
        GeneratedObject = generatedObject;
        GeneratedType = generatedObject.GetType();

        FieldCache = [];
    }

    /// <summary>
    /// Caches the property info, getter, and setter for a given field name and type.
    /// </summary>
    /// <param name="fieldName">The name of the field to cache.</param>
    /// <typeparam name="T">The expected type of the field.</typeparam>
    /// <returns>The cached <see cref="PropertyInfo"/> for the specified field.</returns>
    /// <exception cref="MissingMemberException">Thrown if the field does not exist in the compiler generated type.</exception>
    /// <exception cref="InvalidCastException">Thrown if the field exists but is not of the expected type.</exception>
    public FieldInfo CacheField<T>(string fieldName)
    {
        var fieldInfo = AccessTools.Field(GeneratedType, fieldName)
                        ?? throw new MissingMemberException(
                            $"Could not find field '{fieldName}' in type '{GeneratedType}'.");

        if (fieldInfo.FieldType != typeof(T))
        {
            throw new InvalidCastException(
                $"Field '{fieldName}' is of type '{fieldInfo.FieldType}', not '{typeof(T)}'.");
        }

        FieldCache[fieldName] = fieldInfo;
        return fieldInfo;
    }

    /// <summary>
    /// Gets the value of a field in the compiler generated object.
    /// </summary>
    /// <param name="fieldName">The name of the field to get.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>>The value of the field.</returns>
    /// <exception cref="MissingMemberException">Thrown if the field does not exist.</exception>
    public TField GetField<TField>(string fieldName)
    {
        if (!FieldCache.TryGetValue(fieldName, out var fieldInfo))
        {
            fieldInfo = CacheField<TField>(fieldName);
        }

        return (TField) fieldInfo.GetValue(GeneratedObject)!;
    }

    /// <summary>
    /// Sets the value of a field in the compiler generated object.
    /// </summary>
    /// <param name="fieldName">The name of the field to set.</param>
    /// <param name="value">The value to set.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <exception cref="MissingMemberException">Thrown if the field does not exist.</exception>
    public void SetField<TField>(string fieldName, TField value)
    {
        if (!FieldCache.TryGetValue(fieldName, out var fieldInfo))
        {
            fieldInfo = CacheField<TField>(fieldName);
        }

        fieldInfo.SetValue(GeneratedObject, value);
    }
}
