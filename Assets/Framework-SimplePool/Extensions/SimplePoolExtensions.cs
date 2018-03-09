using UnityEngine;

namespace Pooling
{
    public static class SimplePoolExtensions
    {
        public static T GetFromPoolAsSibling<T>(this T prefab)
            where T : Component
        {
            T item = SimplePool.Spawn(prefab.gameObject).GetComponent<T>();
            item.transform.SetParent(prefab.transform.parent, false);
            item.gameObject.SetActive(true);
            return item;
        }
    }
}
