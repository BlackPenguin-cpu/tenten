using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

/// <summary>
/// using사용으로 지정된 영역을 벗어났을때 자동으로 리스트풀에 Release시킬 수 있다
/// </summary>
public class DisposableList<T> : List<T>, IDisposable
{
    public static DisposableList<T> Get()
    {
        return CollectionPool<DisposableList<T>, T>.Get();
    }

    public void Dispose()
    {
        CollectionPool<DisposableList<T>, T>.Release(this);
    }
}
