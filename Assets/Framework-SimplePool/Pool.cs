using System.Collections.Generic;
using UnityEngine;

namespace Pooling
{
    public class Pool
    {
        // For statistics it's interesting to know how many were created during the game,
        // if this is a large number, you could consider increasing the initial quantity
        private int createdAfterInitialSetup;

        private List<PoolMember> inactive;

        private int initialQuantity;
        // We append an id to the name of anything we instantiate. This is purely cosmetic.
        private int nextId = 1;

        private Transform parent;

        // The prefab that we are pooling
        private GameObject prefab;

        // Constructor
        public Pool(GameObject prefab, int initialQty)
        {
            this.prefab = prefab;

            PoolSettings poolSettings = prefab.GetComponent<PoolSettings>();
            if (poolSettings != null)
                initialQty = poolSettings.PoolSize;

            initialQuantity = initialQty;

            inactive = new List<PoolMember>(initialQuantity);

            parent = new GameObject().transform;
            parent.SetParent(SimplePool.parent);

            AddObjectsToPool(initialQuantity);
        }

        private void SetPoolDisplayName()
        {
            parent.name = string.Format("{0} Pool - Size: {1} - [{2} -> {3}]", prefab.name, nextId,
                initialQuantity, createdAfterInitialSetup);
        }

        internal void AddObjectsToPool(int amount)
        {
            for (int i = 0; i < amount; i++)
                AddObjectToPool();
        }

        internal PoolMember AddObjectToPool()
        {
            GameObject obj = (GameObject)Object.Instantiate(prefab, parent, false);
            obj.SetActive(false);
            obj.name = prefab.name + " (" + (nextId++) + ")";

            SetPoolDisplayName();

            PoolMember poolMember = obj.AddComponent<PoolMember>();
            poolMember.OriginalPrefab = prefab;
            poolMember.Initialize();

            inactive.Add(poolMember);

            return poolMember;
        }

        // Spawn an object from our pool
        internal PoolMember Spawn(Vector3 pos, Quaternion rot, float activeDuration)
        {
            PoolMember poolMember = GetPoolMember();

            poolMember.transform.position = pos;
            poolMember.transform.rotation = rot;

            ReadyPoolMember(poolMember, activeDuration);
            return poolMember;
        }

        internal PoolMember Spawn(Transform parent, float activeDuration)
        {
            PoolMember poolMember = GetPoolMember();

            poolMember.transform.SetParent(parent, false);

            ReadyPoolMember(poolMember, activeDuration);
            return poolMember;
        }

        private PoolMember GetPoolMember()
        {
            EnsurePoolIsNotEmpty();
            PoolMember poolMember = inactive[0];
            inactive.RemoveAt(0);

            if (poolMember == null)
            {
                Debug.LogWarning("Retrieved pool member was empty, getting new!");
                return GetPoolMember();
            }

            return poolMember;
        }

        private void EnsurePoolIsNotEmpty()
        {
            if (inactive.Count == 0)
            {
                createdAfterInitialSetup += 1;
                AddObjectToPool();
            }
        }

        private void ReadyPoolMember(PoolMember poolMember, float activeDuration)
        {
            poolMember.gameObject.SetActive(true);
            poolMember.OnSpawn();

            if (activeDuration > 0)
                poolMember.DespawnAfter(activeDuration);
        }

        // Return an object to the inactive pool.
        internal void Despawn(PoolMember poolMember)
        {
#if DEBUG  // to save performance on release builds, only do this check in debug mode
            if (inactive.Contains(poolMember))
            {
                Debug.LogErrorFormat(poolMember, "Pool member '{0}' was already despawned", poolMember.name);
                return;
            }
#endif

            poolMember.gameObject.SetActive(false);

            poolMember.transform.SetParent(parent, false);

            poolMember.OnDespawn();

            inactive.Add(poolMember);
        }

        public bool Clear()
        {
            bool anyRemainActive = false;
            for (int i = inactive.Count-1; i >= 0; i--)
            {
                PoolMember poolMember = inactive[i];
                if (poolMember.gameObject.activeSelf && poolMember.gameObject.activeInHierarchy)
                {
                    anyRemainActive = true;
                    continue;
                }
                inactive.RemoveAt(i);
                Object.Destroy(poolMember.gameObject);
            }
            nextId = inactive.Count+1;

            if (!anyRemainActive)
                Object.Destroy(parent.gameObject);

            return !anyRemainActive;
        }
    }
}
