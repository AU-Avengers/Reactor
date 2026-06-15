using System.Collections.Generic;
using UnitySystem.Runtime.CompilerServices;

namespace Reactor.Utilities;

/// <inheritdoc />
public sealed class UnityEqualityComparer<T> : IEqualityComparer<T> where T : UnitySystem.Object
{
    private static UnityEqualityComparer<T>? _instance;

    /// <summary>
    /// Gets the instance.
    /// </summary>
    public static UnityEqualityComparer<T> Instance
    {
        get
        {
            _instance ??= new UnityEqualityComparer<T>();
            return _instance;
        }
    }

    private UnityEqualityComparer()
    {
    }

    /// <inheritdoc/>
    public int GetHashCode(T obj)
    {
        return RuntimeHelpers.GetHashCode(obj);
    }

    /// <inheritdoc/>
    public bool Equals(T? x, T? y)
    {
        if (x == null || y == null)
        {
            return x == null && y == null;
        }

        return x.Equals(y);
    }
}
