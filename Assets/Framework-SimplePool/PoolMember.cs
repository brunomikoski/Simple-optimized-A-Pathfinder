using System.Collections;
using UnityEngine;

namespace Pooling
{
    /// <summary>
    ///     Added to freshly instantiated objects, so we can link back
    ///     to the correct pool on despawn.
    /// </summary>
    public class PoolMember : MonoBehaviour
    {
        public GameObject OriginalPrefab;

        private bool isSpawned;
        public bool IsSpawned { get { return isSpawned; } }

        private IOnDespawn[] onDespawnComponents;
        private IOnPool[] onPoolComponents;
        private IOnSpawn[] onSpawnComponents;

        private Coroutine autoDespawnRoutine = null;

        internal void Initialize()
        {
            onPoolComponents = GetComponentsInChildren<IOnPool>();
            onSpawnComponents = GetComponentsInChildren<IOnSpawn>();
            onDespawnComponents = GetComponentsInChildren<IOnDespawn>();

            for (int i = 0; i < onPoolComponents.Length; i++)
                if (onPoolComponents[i] != null)
                    onPoolComponents[i].OnPool();
        }

        internal void OnSpawn()
        {
            isSpawned = true;
            for (int i = 0; i < onSpawnComponents.Length; i++)
                if (onSpawnComponents[i] != null)
                    onSpawnComponents[i].OnSpawn();
        }

        internal void OnDespawn()
        {
            for (int i = 0; i < onDespawnComponents.Length; i++)
                if (onDespawnComponents[i] != null)
                    onDespawnComponents[i].OnDespawn();
            isSpawned = false;
        }

        internal void Despawn()
        {
            SimplePool.Despawn(gameObject);

            if (autoDespawnRoutine != null)
            {
                StopCoroutine(autoDespawnRoutine);
            }
        }

        internal void DespawnAfter(float seconds)
        {
            autoDespawnRoutine = StartCoroutine(DespawnEnumerator(seconds));
        }

        private IEnumerator DespawnEnumerator(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Despawn();
        }
    }
}
