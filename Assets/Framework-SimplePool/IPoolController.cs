using System;
using UnityEngine;

namespace Pooling
{
    public interface IPoolController
    {
        int Count { get; }
        GameObject GetPrefab<T>(T type) where T : struct, IConvertible;

        GameObject GetPrefab(int index);
    }
}
