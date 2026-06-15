using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reactor.Utilities;

/// <summary>
/// Dispatches actions on Unity main thread.
/// </summary>
public sealed class Dispatcher : MonoBehaviour
{
    /// <summary>
    /// Gets the instance.
    /// </summary>
    public static Dispatcher Instance { get; private set; } = null!;

    private readonly Queue<Action> _queue = new();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        lock (_queue)
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue().Invoke();
            }
        }
    }

    /// <summary>
    /// Enqueues an <paramref name="action"/>.
    /// </summary>
    /// <param name="action">The action to enqueue.</param>
    
    public void Enqueue(Action action)
    {
        lock (_queue)
        {
            _queue.Enqueue(action);
        }
    }
}
