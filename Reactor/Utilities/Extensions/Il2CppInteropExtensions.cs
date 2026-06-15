using System;

namespace Reactor.Utilities.Extensions;

/// <summary>
/// Provides extension methods for UnityInterop.
/// </summary>
public static class UnityInteropExtensions
{
    /// <summary>
    /// Creates a span over a <see cref="UnityStructArray{T}"/>.
    /// </summary>
    /// <param name="array">The array to create a span over.</param>
    /// <typeparam name="T">The type of items in the <see cref="UnityStructArray{T}"/>.</typeparam>
    /// <returns>A span.</returns>
    public static unsafe Span<T> ToSpan<T>(this UnityStructArray<T> array) where T : unmanaged
    {
        return new Span<T>(IntPtr.Add(array.Pointer, IntPtr.Size * 4).ToPointer(), array.Length);
    }
}
