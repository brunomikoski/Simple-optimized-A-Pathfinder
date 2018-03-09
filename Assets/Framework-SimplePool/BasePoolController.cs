using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pooling
{
    public abstract class BasePoolController<TData, TEnum> : MonoBehaviour, IPoolController
        where TData : BasePoolData<TEnum>
        where TEnum : struct, IConvertible

    {
        [SerializeField]
        private List<TData> data;

        private Dictionary<TEnum, TData> dataDictionary;

        private void Awake()
        {
            SimplePool.RegisterPoolController<TEnum>(this);

            dataDictionary = new Dictionary<TEnum, TData>();
            foreach (var item in data)
            {
                if (item.Type.ToString().ToLower() != "none"
                    && !dataDictionary.ContainsKey(item.Type))
                {
                    dataDictionary.Add(item.Type, item);
                }

                SimplePool.AddObjectsToPool(item.Prefab, item.PreloadAmount);
            }
        }

        private void OnDestroy()
        {
            SimplePool.UnregisterPoolController<TEnum>();
        }

        public int Count
        {
            get { return dataDictionary.Count; }
        }

        public GameObject GetPrefab<T>(T type)
            where T : struct, IConvertible
        {
            TData item;
            if (dataDictionary.TryGetValue((TEnum) (object) type, out item))
                return item.Prefab;

            return null;
        }

        public GameObject GetPrefab(int index)
        {
            return dataDictionary.ElementAt(index).Value.Prefab;
        }
    }
}
