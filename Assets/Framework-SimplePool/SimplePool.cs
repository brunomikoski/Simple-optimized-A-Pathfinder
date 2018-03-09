using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Pooling
{
    /// <summary>
    /// Simple pooling for Unity.
    /// Author: Martin "quill18" Glaude (quill18@quill18.com) Latest Version: https://gist.github.com/quill18/5a7cfffae68892621267
    /// License: CC0 (http://creativecommons.org/publicdomain/zero/1.0/) UPDATES: 2015-04-16: Changed
    ///          Pool to use a Stack generic. Usage: There's no need to do any special setup of any
    ///          kind. Instead of call Instantiate(), use this: SimplePool.Spawn(somePrefab,
    ///          somePosition, someRotation); Instead of destroying an object, use this:
    ///          SimplePool.Despawn(myGameObject); If desired, you can preload the pool with a number
    ///          of instances: SimplePool.Preload(somePrefab, 20); Remember that Awake and Start will
    ///          only ever be called on the first instantiatio and that member variables won't be
    ///          reset automatically. You should reset your object yourself after calling Spawn().
    ///          (i.e. You'll have to do things like set the object's HPs to max, reset animation
    ///          states, etc...)
    /// </summary>
    public static class SimplePool
    {
        // You can avoid resizing of the Stack's internal array by setting this to a number equal to
        // or greater to what you expect most of your pool sizes to be. Note, you can also use
        // Preload() to set the initial size of a pool -- this can be handy if only some of your
        // pools are going to be exceptionally large (for example, your bullets.)
        private const int DEFAULT_POOL_SIZE = 3;

        internal static Transform parent;

        /// <summary>
        /// The Pool class represents the pool for a particular prefab.
        /// </summary>

        // All of our pools
        private static Dictionary<GameObject, Pool> pools;

        private static Dictionary<Type, IPoolController> poolControllers =
            new Dictionary<Type, IPoolController>();

        /// <summary>
        /// Init our dictionary.
        /// </summary>
        private static void Init(GameObject prefab = null, int qty = DEFAULT_POOL_SIZE)
        {
            if (pools == null)
            {
                pools = new Dictionary<GameObject, Pool>();
                parent = new GameObject("SimplePool Objects").transform;
                Object.DontDestroyOnLoad(parent);
            }

            if (prefab != null && pools.ContainsKey(prefab) == false)
            {
                pools[prefab] = new Pool(prefab, qty);
            }
        }

        public static void AddObjectsToPool(Component component, int qty = 1)
        {
            AddObjectsToPool(component.gameObject, qty);
        }

        public static void AddObjectsToPool(GameObject prefab, int qty = 1)
        {
            if (pools == null || !pools.ContainsKey(prefab))
            {
                Init(prefab, qty);
            }
            else
            {
                for (int i = 0; i < qty; i++)
                    pools[prefab].AddObjectToPool();
            }
        }

        public static void ClearPool(Component component)
        {
            ClearPool(component.gameObject);
        }

        public static void ClearPool(GameObject prefab)
        {
            if(!pools.ContainsKey(prefab))
                return;

            if(pools[prefab].Clear())
                pools.Remove(prefab);
        }

        public static PoolMember SpawnType<T>(T type, Vector3 pos = default(Vector3),
            Quaternion rot = default(Quaternion), float activeDuration = 0)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            IPoolController poolController;
            if (poolControllers.TryGetValue(typeof(T), out poolController))
            {
                GameObject prefab = poolController.GetPrefab(type);

                if (prefab == null)
                    return null;

                Init(prefab);

                return pools[prefab].Spawn(pos, rot, activeDuration);
            }

            return null;
        }

        public static PoolMember SpawnRandomType<T>(Vector3 pos = default(Vector3),
            Quaternion rot = default(Quaternion), float activeDuration = 0)
            where T : struct, IComparable, IConvertible, IFormattable
        {
            IPoolController poolController;
            if (poolControllers.TryGetValue(typeof(T), out poolController))
            {
                int randomIndex = UnityEngine.Random.Range(0, poolController.Count);

                GameObject prefab = poolController.GetPrefab(randomIndex);

                if (prefab == null)
                    return null;

                Init(prefab);

                return pools[prefab].Spawn(pos, rot, activeDuration);
            }

            return null;
        }

        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// </summary>
        public static GameObject Spawn(GameObject prefab)
        {
            Init(prefab);
            return pools[prefab].Spawn(default(Vector3), default(Quaternion), 0).gameObject;
        }

        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// </summary>
        public static GameObject Spawn(GameObject prefab, float activeDuration)
        {
            Init(prefab);
            return pools[prefab].Spawn(default(Vector3), default(Quaternion), activeDuration).gameObject;
        }

        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// </summary>
        public static GameObject Spawn(GameObject prefab, Vector3 pos,
            Quaternion rot)
        {
            Init(prefab);
            return pools[prefab].Spawn(pos, rot, 0).gameObject;
        }

        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// </summary>
        public static GameObject Spawn(GameObject prefab, Vector3 pos,
            Quaternion rot, float activeDuration)
        {
            Init(prefab);
            return pools[prefab].Spawn(pos, rot, activeDuration).gameObject;
        }

        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// </summary>
        public static GameObject Spawn(GameObject prefab, Transform parent)
        {
            Init(prefab);
            return pools[prefab].Spawn(parent, 0).gameObject;
        }

        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// </summary>
        public static GameObject Spawn(GameObject prefab, Transform parent, float activeDuration)
        {
            Init(prefab);
            return pools[prefab].Spawn(parent, activeDuration).gameObject;
        }


        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// </summary>
        public static T Spawn<T>(T prefab)
            where T : Component
        {
            Init(prefab.gameObject);
            return pools[prefab.gameObject].Spawn(default(Vector3), default(Quaternion), 0).GetComponent<T>();
        }

        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// </summary>
        public static T Spawn<T>(T prefab, float activeDuration)
            where T : Component
        {
            Init(prefab.gameObject);
            return pools[prefab.gameObject].Spawn(default(Vector3), default(Quaternion), activeDuration).GetComponent<T>();
        }

        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// </summary>
        public static T Spawn<T>(T prefab, Vector3 pos, Quaternion rot)
            where T : Component
        {
            Init(prefab.gameObject);
            return pools[prefab.gameObject].Spawn(pos, rot, 0).GetComponent<T>();
        }

        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// </summary>
        public static T Spawn<T>(T prefab, Vector3 pos, Quaternion rot, float activeDuration)
            where T : Component
        {
            Init(prefab.gameObject);
            return pools[prefab.gameObject].Spawn(pos, rot, activeDuration).GetComponent<T>();
        }

        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// </summary>
        public static T Spawn<T>(T prefab, Transform parent)
            where T : Component
        {
            Init(prefab.gameObject);
            return pools[prefab.gameObject].Spawn(parent, 0).GetComponent<T>();
        }

        /// <summary>
        /// Spawns a copy of the specified prefab (instantiating one if required).
        /// </summary>
        public static T Spawn<T>(T prefab, Transform parent, float activeDuration)
            where T : Component
        {
            Init(prefab.gameObject);
            return pools[prefab.gameObject].Spawn(parent, activeDuration).GetComponent<T>();
        }

        public static void Despawn<T>(T prefab) where T : MonoBehaviour
        {
            Despawn(prefab.gameObject);
        }

        /// <summary>
        /// Despawn the specified gameobject back into its pool.
        /// </summary>
        public static void Despawn(GameObject obj)
        {
            PoolMember pm = obj.GetComponent<PoolMember>();
            if (pm == null)
            {
                Debug.LogWarning(
                    "Object '" + obj.name + "' wasn't spawned from a pool. Destroying it instead.");
                Object.Destroy(obj);
            }
            else
            {
                if (!pools.ContainsKey(pm.OriginalPrefab))
                {
                    Debug.LogWarning(
                        "Object's '" + obj.name + "' pool was destroyed. Destroying it instead of despawning.");
                    Object.Destroy(obj);
                }
                else
                   pools[pm.OriginalPrefab].Despawn(pm);
            }
        }

        internal static void RegisterPoolController<TEnum>(IPoolController poolController)
        {
            Type enumType = typeof(TEnum);
            if (!poolControllers.ContainsKey(enumType))
                poolControllers.Add(enumType, poolController);
        }

        internal static void UnregisterPoolController<TEnum>()
        {
            Type enumType = typeof(TEnum);
            if (poolControllers.ContainsKey(enumType))
                poolControllers.Remove(enumType);
        }

    }
}
