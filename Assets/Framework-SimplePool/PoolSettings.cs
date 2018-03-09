using UnityEngine;

namespace Pooling
{
    public class PoolSettings : MonoBehaviour
    {
        [SerializeField]
        private int poolSize = 1;
        public int PoolSize
        {
            get { return poolSize; }
        }
    }
}
