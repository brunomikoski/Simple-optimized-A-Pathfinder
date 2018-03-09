using System;
using UnityEngine;

namespace Pooling
{
    [Serializable]
    public abstract class BasePoolData<T>
        where T : struct, IConvertible
    {
        [SerializeField]
        private GameObject prefab;

        [SerializeField]
        private T type;

        [SerializeField]
        private int preloadAmount;
        public GameObject Prefab
        {
            get { return prefab; }
        }
        public T Type
        {
            get { return type; }
        }
        public int PreloadAmount
        {
            get { return preloadAmount; }
        }
    }
}
