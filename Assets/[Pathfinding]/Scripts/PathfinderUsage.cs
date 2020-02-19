using BrunoMikoski.Pathfinder;
using Unity.Mathematics;
using UnityEngine;

public class PathfinderUsage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SimplePathfinder.Setup(new int2(100, 100));
        
        SimplePathfinder.GetPath(new int2(0,0), new int2(99,99), OnPathResolved );
    }

    private void OnPathResolved(SimplePathfinder.PathResultData pathResultData)
    {
        for (var i = 0; i < pathResultData.PathPositions.Length; i++)
        {
            int2 position = pathResultData.PathPositions[i];
            Debug.Log(position);
        }
    }
}
