using System.Collections;
using UnityEngine;

namespace Pooling
{
    public class ReturnToPoolAfterSeconds : MonoBehaviour, IOnSpawn
    {
        [SerializeField]
        private float seconds;

        void IOnSpawn.OnSpawn()
        {
            StartCoroutine(ReturnToPoolEnumerator());
        }

        private IEnumerator ReturnToPoolEnumerator()
        {
            yield return new WaitForSeconds(seconds);
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            SimplePool.Despawn(this);
        }
    }
}
