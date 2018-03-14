using System.Collections.Generic;
using UnityEngine;

public static class SimplePool
{
    // You can avoid resizing of the Stack's internal data by
    // setting this to a number equal to or greater to what you
    // expect most of your pool sizes to be.
    // Note, you can also use Preload() to set the initial size
    // of a pool -- this can be handy if only some of your pools
    // are going to be exceptionally large (for example, your bullets.)
    private const int DEFAULT_POOL_SIZE = 3;

	/// <summary>
	///     The Pool class represents the pool for a particular prefab.
	/// </summary>
	private class Pool
    {
        // We append an id to the name of anything we instantiate.
        // This is purely cosmetic.
        private int nextId = 1;

        // The structure containing our inactive objects.
        // Why a Stack and not a List? Because we'll never need to
        // pluck an object from the start or middle of the array.
        // We'll always just grab the last one, which eliminates
        // any need to shuffle the objects around in memory.
        private Stack<GameObject> inactive;

        // The prefab that we are pooling
        private GameObject prefab;

        // Constructor
        public Pool( GameObject prefab, int initialQty )
        {
            this.prefab = prefab;

            // If Stack uses a linked list internally, then this
            // whole initialQty thing is a placebo that we could
            // strip out for more minimal code. But it can't *hurt*.
            inactive = new Stack<GameObject>( initialQty );
        }

        // Spawn an object from our pool
        public GameObject Spawn( Vector3 pos, Quaternion rot )
        {
            GameObject obj;
            if ( inactive.Count == 0 )
            {
                // We don't have an object in our pool, so we
                // instantiate a whole new object.
                obj = Object.Instantiate( prefab, pos, rot );
                obj.name = prefab.name + " (" + nextId++ + ")";

                // Add a PoolMember component so we know what pool
                // we belong to.
                obj.AddComponent<PoolMember>().myPool = this;
            }
            else
            {
                // Grab the last object in the inactive array
                obj = inactive.Pop();

                if ( obj == null )
                {
                    // The inactive object we expected to find no longer exists.
                    // The most likely causes are:
                    //   - Someone calling Destroy() on our object
                    //   - A scene change (which will destroy all our objects).
                    //     NOTE: This could be prevented with a DontDestroyOnLoad
                    //	   if you really don't want this.
                    // No worries -- we'll just try the next one in our sequence.

                    return Spawn( pos, rot );
                }
            }

            obj.transform.position = pos;
            obj.transform.rotation = rot;
            obj.SetActive( true );
            return obj;
        }

        // Return an object to the inactive pool.
        public void Despawn( GameObject obj )
        {
            obj.SetActive( false );

            // Since Stack doesn't have a Capacity member, we can't control
            // the growth factor if it does have to expand an internal array.
            // On the other hand, it might simply be using a linked list 
            // internally.  But then, why does it allow us to specify a size
            // in the constructor? Maybe it's a placebo? Stack is weird.
            inactive.Push( obj );
        }
    }

	/// <summary>
	///     Added to freshly instantiated objects, so we can link back
	///     to the correct pool on despawn.
	/// </summary>
	private class PoolMember : MonoBehaviour
    {
        public Pool myPool;
    }

    // All of our pools
    private static Dictionary<GameObject, Pool> pools;

	/// <summary>
	///     Initialize our dictionary.
	/// </summary>
	private static void Init( GameObject prefab = null, int qty = DEFAULT_POOL_SIZE )
    {
        if ( pools == null )
            pools = new Dictionary<GameObject, Pool>();
        if ( prefab != null && pools.ContainsKey( prefab ) == false )
            pools[prefab] = new Pool( prefab, qty );
    }

	/// <summary>
	///     If you want to preload a few copies of an object at the start
	///     of a scene, you can use this. Really not needed unless you're
	///     going to go from zero instances to 100+ very quickly.
	///     Could technically be optimized more, but in practice the
	///     Spawn/Despawn sequence is going to be pretty darn quick and
	///     this avoids code duplication.
	/// </summary>
	public static void Preload( GameObject prefab, int qty = 1 )
    {
        Init( prefab, qty );

        // Make an array to grab the objects we're about to pre-spawn.
        GameObject[] obs = new GameObject[qty];
        for ( int i = 0; i < qty; i++ )
            obs[i] = Spawn( prefab, Vector3.zero, Quaternion.identity );

        // Now despawn them all.
        for ( int i = 0; i < qty; i++ )
            Despawn( obs[i] );
    }

	/// <summary>
	///     Spawns a copy of the specified prefab (instantiating one if required).
	///     NOTE: Remember that Awake() or Start() will only run on the very first
	///     spawn and that member variables won't get reset.  OnEnable will run
	///     after spawning -- but remember that toggling IsActive will also
	///     call that function.
	/// </summary>
	public static GameObject Spawn( GameObject prefab, Vector3 pos, Quaternion rot )
    {
        Init( prefab );

        return pools[prefab].Spawn( pos, rot );
    }

    public static GameObject Spawn( GameObject prefab, Transform parent )
    {
        GameObject instance = Spawn( prefab, Vector3.zero, Quaternion.identity );
        instance.transform.SetParent( parent, false );
        return instance;
    }

	/// <summary>
	///     Despawn the specified gameobject back into its pool.
	/// </summary>
	public static void Despawn( GameObject obj )
    {
        PoolMember pm = obj.GetComponent<PoolMember>();
        if ( pm == null )
        {
            Debug.Log( "Object '" + obj.name + "' wasn't spawned from a pool. Destroying it instead." );
            Object.Destroy( obj );
        }
        else
            pm.myPool.Despawn( obj );
    }
}
