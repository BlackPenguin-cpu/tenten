using System;
using System.Collections.Generic;
using UnityEngine.Pool;

public class DisposableList<T> : List<T>, IDisposable
{
    public static DisposableList<T> Get()
    {
        return CollectionPool<DisposableList<T>, T>.Get();
    }

    public void Dispose()
    {
        Clear();
        CollectionPool<DisposableList<T>, T>.Release(this);
    }
}