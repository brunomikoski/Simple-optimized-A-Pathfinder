namespace Pooling
{
    public interface IOnSpawn
    {
        /// <summary>
        ///     Called everytime the object is Spawned from the pool
        /// </summary>
        void OnSpawn();
    }

    public interface IOnPool
    {
        /// <summary>
        ///     Called when the object is created inside the POOL,
        ///     and this will only happen once like the Awake(); method
        /// </summary>
        void OnPool();
    }

    public interface IOnDespawn
    {
        /// <summary>
        ///     Called everytime the object will return to the pool
        /// </summary>
        void OnDespawn();
    }
}
